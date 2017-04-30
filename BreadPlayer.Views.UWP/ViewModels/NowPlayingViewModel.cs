using BreadPlayer.Web.BaiduLyricsAPI;
using BreadPlayer.Web.Lastfm;
using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Api.Helpers;
using IF.Lastfm.Core.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking.Connectivity;

namespace BreadPlayer.ViewModels
{
    public class NowPlayingViewModel : ViewModelBase
    {
        #region Loading Properties
        bool isLoading;
        public bool IsLoading { get => isLoading; set => Set(ref isLoading, value); }
        bool isFailed;
        public bool IsFailed { get => isFailed; set => Set(ref isFailed, value); }
        double progress;
        public double Progress { get => progress; set => Set(ref progress, value); }
        string progressText;
        public string ProgressText { get => progressText; set => Set(ref progressText, value); }
        #endregion

        public string CorrectArtist { get; set; }
        public string CorrectAlbum { get; set; }
        string artistBio;
        public string ArtistBio
        {
            get => artistBio;
            set => Set(ref artistBio, value);
        }
        ThreadSafeObservableCollection<LastTrack> albumTracks;
        public ThreadSafeObservableCollection<LastTrack> AlbumTracks
        {
            get => albumTracks;
            set => Set(ref albumTracks, value);
        }
        ThreadSafeObservableCollection<LastArtist> similarArtists;
        public ThreadSafeObservableCollection<LastArtist> SimilarArtists
        {
            get => similarArtists;
            set => Set(ref similarArtists, value);
        }
        public ICommand RetryCommand { get; set; }
        private LastfmClient LastfmClient
        {
            get => new Lastfm().LastfmClient;
        }
        public NowPlayingViewModel()
        {
            RetryCommand = new DelegateCommand(Retry);
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
            
            //the work around to knowing when the new song has started.
            //the event is needed to update the bio etc.
            Core.SharedLogic.Player.MediaChanging += (sender, e) =>
            {
                Core.SharedLogic.Player.MediaStateChanged += Player_MediaStateChanged;
            };
        }
        private async void Retry()
        {
            if (string.IsNullOrEmpty(CorrectAlbum) || string.IsNullOrEmpty(CorrectArtist))
                return;
            IsFailed = false;
            await GetInfo(CorrectArtist, CorrectAlbum);
        }
        private async void Player_MediaStateChanged(object sender, Events.MediaStateChangedEventArgs e)
        {
            if (e.NewState == Core.PlayerState.Playing)
            {
                Core.SharedLogic.Player.MediaStateChanged -= Player_MediaStateChanged;
                await GetInfo(Core.SharedLogic.Player.CurrentlyPlayingFile.LeadArtist, Core.SharedLogic.Player.CurrentlyPlayingFile.Album);
            }
        }
        Task FetchInfoTask;
        CancellationToken token;
        CancellationTokenSource tokenSource;
        IAsyncOperation<LastResponse<LastArtist>> ArtistInfoOperation;
        IAsyncOperation<LastResponse<LastAlbum>> AlbumInfoOperation;
      
        private async Task GetInfo(string artistName, string albumName)
        {
            await Core.SharedLogic.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                if (FetchInfoTask?.IsCompleted == false)
                    tokenSource.Cancel();
                FetchInfoTask = GetArtistInfo(artistName, albumName, token);
                await FetchInfoTask;
            });
        }
        private async Task GetArtistInfo(string artistName, string albumName, CancellationToken token)
        {
            ConnectionProfile InternetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();

            if (InternetConnectionProfile != null)
            {
                IsLoading = true;
                await GetAlbumInfo(artistName, albumName, token);
                CheckAndCancelOperation(ArtistInfoOperation, token);
                ArtistInfoOperation = LastfmClient.Artist.GetInfoAsync(artistName, "en", true).AsAsyncOperation();

                var artistInfoResponse = await ArtistInfoOperation;
                if (artistInfoResponse.Success)
                {
                    LastArtist artist = artistInfoResponse.Content;
                    ArtistBio = artist.Bio.Content;
                    SimilarArtists = new ThreadSafeObservableCollection<LastArtist>(artist.Similar);
                    
                }
                else
                {
                    IsFailed = true;
                    IsLoading = false;
                }
                if (string.IsNullOrEmpty(ArtistBio) || !AlbumTracks.Any())
                    IsFailed = true;
                IsLoading = false;
            }
        }
        private async Task GetAlbumInfo(string artistName, string albumName, CancellationToken token)
        {
            CheckAndCancelOperation(AlbumInfoOperation, token);
            AlbumInfoOperation = LastfmClient.Album.GetInfoAsync(artistName, albumName, true).AsAsyncOperation();
            var albumInfoResponse = await AlbumInfoOperation;
            if (albumInfoResponse.Success)
            {
                LastAlbum album = albumInfoResponse.Content;
                AlbumTracks = new ThreadSafeObservableCollection<LastTrack>(album.Tracks);
            }
            else
            {
                IsFailed = true;
            }
        }
        private void CheckAndCancelOperation<T>(IAsyncOperation<T> operation, CancellationToken token)
        {
            //check if there is any old operation running.
            if (operation != null && token.IsCancellationRequested && operation.Status != AsyncStatus.Completed)
            {
                //cancel old operiation
                operation.Cancel();
            }
        }
        private async void GetLyrics(string query)
        {
            ApiMethods methods = new ApiMethods();
            var response = await methods.Search(query);
        }
    }
}
