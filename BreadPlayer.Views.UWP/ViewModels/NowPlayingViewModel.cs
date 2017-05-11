using BreadPlayer.Database;
using BreadPlayer.Extensions;
using BreadPlayer.Web.Lastfm;
using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Objects;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using BreadPlayer.Core.Common;
using BreadPlayer.Core.Events;

namespace BreadPlayer.ViewModels
{
    public class NowPlayingViewModel : ViewModelBase
    {
        #region Loading Properties

        private bool artistInfoLoading;
        public bool ArtistInfoLoading { get => artistInfoLoading; set => Set(ref artistInfoLoading, value); }
        private bool albumInfoLoading;
        public bool AlbumInfoLoading { get => albumInfoLoading; set => Set(ref albumInfoLoading, value); }
        private bool artistFetchFailed;
        public bool ArtistFetchFailed { get => artistFetchFailed; set => Set(ref artistFetchFailed, value); }
        private bool albumFetchFailed;
        public bool AlbumFetchFailed { get => albumFetchFailed; set => Set(ref albumFetchFailed, value); }
        #endregion

        private LibraryService service = new LibraryService(new KeyValueStoreDatabaseService(Core.SharedLogic.DatabasePath, "Tracks", "TracksText"));
        public string CorrectArtist { get; set; }
        public string CorrectAlbum { get; set; }
        private string artistBio;
        public string ArtistBio
        {
            get => artistBio;
            set => Set(ref artistBio, value);
        }

        private ThreadSafeObservableCollection<LastTrack> albumTracks;
        public ThreadSafeObservableCollection<LastTrack> AlbumTracks
        {
            get => albumTracks;
            set => Set(ref albumTracks, value);
        }

        private ThreadSafeObservableCollection<LastArtist> similarArtists;
        public ThreadSafeObservableCollection<LastArtist> SimilarArtists
        {
            get => similarArtists;
            set => Set(ref similarArtists, value);
        }
        public ICommand RetryCommand { get; set; }
        private LastfmClient LastfmClient => new Lastfm().LastfmClient;

        public NowPlayingViewModel()
        {
            RetryCommand = new RelayCommand(Retry);

            //the work around to knowing when the new song has started.
            //the event is needed to update the bio etc.
            Core.SharedLogic.Player.MediaChanging += (sender, e) =>
            {
                Core.SharedLogic.Player.MediaStateChanged += Player_MediaStateChanged;
            };
        }
        private async void Retry(object para)
        {
            if(para.ToString() == "Artist")
            {
                if (string.IsNullOrEmpty(CorrectArtist))
                    return;
                await GetArtistInfo(CorrectArtist);
                Core.SharedLogic.Player.CurrentlyPlayingFile.LeadArtist = CorrectArtist;
            }
            else if(para.ToString() == "Album")
            {
                if (string.IsNullOrEmpty(CorrectAlbum) || string.IsNullOrEmpty(CorrectArtist))
                    return;
                await GetAlbumInfo(CorrectArtist, CorrectAlbum);
                Core.SharedLogic.Player.CurrentlyPlayingFile.LeadArtist = CorrectArtist;
                Core.SharedLogic.Player.CurrentlyPlayingFile.Album = CorrectAlbum;
            }
            await service.UpdateMediafile(Core.SharedLogic.Player.CurrentlyPlayingFile);
        }
        private async void Player_MediaStateChanged(object sender, MediaStateChangedEventArgs e)
        {
            if (e.NewState == PlayerState.Playing)
            {
                Core.SharedLogic.Player.MediaStateChanged -= Player_MediaStateChanged;
                await GetInfo(Core.SharedLogic.Player.CurrentlyPlayingFile.LeadArtist, Core.SharedLogic.Player.CurrentlyPlayingFile.Album);
            }
        }

        private int retries = 0;
        private async Task GetInfo(string artistName, string albumName)
        {
            try
            {
                //start the tasks on another thread so that the UI doesn't hang.

                ConnectionProfile InternetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();
                
                if (InternetConnectionProfile != null)
                {
                    //cancel any previous requests
                    LastfmClient.HttpClient.CancelPendingRequests();
                    //start both tasks
                    await GetArtistInfo(artistName.ScrubGarbage().GetTag()).ConfigureAwait(false);
                    await GetAlbumInfo(artistName.ScrubGarbage().GetTag(), albumName.ScrubGarbage().GetTag()).ConfigureAwait(false);
                }
                else
                {
                    AlbumFetchFailed = true;
                    ArtistFetchFailed = true;
                }
            }
            catch (Exception)
            {
                //we use this simple logic to avoid too many retries.
                //MAX_RETRIES = 10;
                if (retries == 10)
                {
                    //increase retry count
                    retries++;

                    //retry
                    await GetInfo(artistName, albumName);
                }
            }
        }
        private async Task GetArtistInfo(string artistName)
        {
            await Core.SharedLogic.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                LastfmClient.Artist.HttpClient.CancelPendingRequests();
                //CheckAndCancelOperation(ArtistInfoOperation, token);
                ArtistInfoLoading = true;
                var artistInfoResponse = await LastfmClient.Artist.GetInfoAsync(artistName, "en", true).ConfigureAwait(false);
                ArtistBio = "";
                ArtistFetchFailed = false;
                if (artistInfoResponse.Success)
                {
                    LastArtist artist = artistInfoResponse.Content;
                    ArtistBio = artist.Bio.Content.ScrubHtml();
                    SimilarArtists = new ThreadSafeObservableCollection<LastArtist>(artist.Similar);
                }
                else
                {
                    ArtistFetchFailed = true;
                    ArtistInfoLoading = false;
                }
                //if it is empty or it starts with [unknown],
                //which is the identifier for unknown artists;
                //just fail.
                if (string.IsNullOrEmpty(ArtistBio) || ArtistBio.StartsWith("[unknown]") || ArtistBio.StartsWith("This is not an artist"))
                    ArtistFetchFailed = true;
                ArtistInfoLoading = false;
            });
        }
        private async Task GetAlbumInfo(string artistName, string albumName)
        {
            await Core.SharedLogic.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                LastfmClient.Album.HttpClient.CancelPendingRequests();
                //CheckAndCancelOperation(AlbumInfoOperation, token);
                AlbumInfoLoading = true;
                AlbumTracks?.Clear();
                AlbumFetchFailed = false;
                var albumInfoResponse = await LastfmClient.Album.GetInfoAsync(artistName, albumName, true).ConfigureAwait(false);
                if (albumInfoResponse.Success)
                {
                    LastAlbum album = albumInfoResponse.Content;
                    AlbumTracks = new ThreadSafeObservableCollection<LastTrack>(album.Tracks);
                }
                else
                {
                    AlbumFetchFailed = true;
                    AlbumInfoLoading = false;
                }
                if (AlbumTracks?.Any() == false)
                    AlbumFetchFailed = true;

                AlbumInfoLoading = false;
            });
        }     
    }
}
