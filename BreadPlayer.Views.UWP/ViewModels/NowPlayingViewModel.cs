using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.UI.Core;
using BreadPlayer.Core;
using BreadPlayer.Core.Common;
using BreadPlayer.Core.Events;
using BreadPlayer.Database;
using BreadPlayer.Extensions;
using BreadPlayer.Web.Lastfm;
using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Objects;
using BreadPlayer.Dispatcher;
using System.Collections.Generic;
using Windows.UI.Xaml;
using BreadPlayer.Parsers.LRCParser;
using BreadPlayer.Parsers.TagParser;
using BreadPlayer.Core.Extensions;
using System.Threading;
using BreadPlayer.Helpers;

namespace BreadPlayer.ViewModels
{
    public class NowPlayingViewModel : ViewModelBase
    {
        #region Loading Properties
        private bool _artistInfoLoading;
        public bool ArtistInfoLoading { get => _artistInfoLoading; set => Set(ref _artistInfoLoading, value); }
        private bool _albumInfoLoading;
        public bool AlbumInfoLoading { get => _albumInfoLoading; set => Set(ref _albumInfoLoading, value); }
        private bool _lyricsLoading;
        public bool LyricsLoading { get => _lyricsLoading; set => Set(ref _lyricsLoading, value); }
        private bool _artistFetchFailed;
        public bool ArtistFetchFailed { get => _artistFetchFailed; set => Set(ref _artistFetchFailed, value); }
        private bool _albumFetchFailed;
        public bool AlbumFetchFailed { get => _albumFetchFailed; set => Set(ref _albumFetchFailed, value); }       
        #endregion

        private LibraryService _service = new LibraryService(new DocumentStoreDatabaseService(SharedLogic.DatabasePath, "Tracks"));
        public string CorrectArtist { get; set; }
        public string CorrectAlbum { get; set; }
        IOneLineLyric _currentLyric;
        public IOneLineLyric CurrentLyric
        {
            get => _currentLyric;
            set => Set(ref _currentLyric, value);
        }
        ThreadSafeObservableCollection<LastArtist> artists;
        public ThreadSafeObservableCollection<LastArtist> Artists
        {
            get => artists;
            set => Set(ref artists, value);
        }
        private ThreadSafeObservableCollection<LastTrack> _albumTracks;
        public ThreadSafeObservableCollection<LastTrack> AlbumTracks
        {
            get => _albumTracks;
            set => Set(ref _albumTracks, value);
        }
        private ThreadSafeObservableCollection<IOneLineLyric> lyrics;
        public ThreadSafeObservableCollection<IOneLineLyric> Lyrics
        {
            get => lyrics;
            set => Set(ref lyrics, value);
        }

        private ThreadSafeObservableCollection<LastArtist> _similarArtists;
        public ThreadSafeObservableCollection<LastArtist> SimilarArtists
        {
            get => _similarArtists;
            set => Set(ref _similarArtists, value);
        }
        public ICommand RetryCommand { get; set; }
        private LastfmClient LastfmClient => new Lastfm().LastfmClient;

        List<Task> TaskList = new List<Task>();
        public NowPlayingViewModel()
        {
            RetryCommand = new RelayCommand(Retry);

            //the work around to knowing when the new song has started.
            //the event is needed to update the bio etc.
            SharedLogic.Player.MediaChanged += OnMediaChanged;
        }
        
        private async void OnMediaChanged(object sender, EventArgs e)
        {
            Lyrics?.Clear();
            CurrentLyric = null;
            await GetInfo(SharedLogic.Player.CurrentlyPlayingFile.LeadArtist, SharedLogic.Player.CurrentlyPlayingFile.Album).ConfigureAwait(false);
        }

        private async void Retry(object para)
        {
            if (string.IsNullOrEmpty(CorrectArtist))
            {
                return;
            }

            if (para.ToString() == "Artist")
            {
                await GetArtistInfo(CorrectArtist);
                SharedLogic.Player.CurrentlyPlayingFile.LeadArtist = CorrectArtist;
            }
            else if (para.ToString() == "Album")
            {
                if (string.IsNullOrEmpty(CorrectAlbum))
                {
                    return;
                }

                await GetAlbumInfo(CorrectArtist, CorrectAlbum);
                SharedLogic.Player.CurrentlyPlayingFile.LeadArtist = CorrectArtist;
                SharedLogic.Player.CurrentlyPlayingFile.Album = CorrectAlbum;
            }
            await _service.UpdateMediafile(SharedLogic.Player.CurrentlyPlayingFile);
        }
       

        private int _retries;
        BreadPlayer.Core.PortableAPIs.DispatcherTimer timer;
        private async Task GetLyrics()
        {
            await BreadDispatcher.InvokeAsync(async () =>
            {
                if (SharedLogic.SettingsVm.AccountSettingsVM.LyricType == "None")
                    return;
                LyricsLoading = true;

                timer = new Core.PortableAPIs.DispatcherTimer(new BreadDispatcher())
                {
                    Interval = TimeSpan.FromMilliseconds(10)
                };
                timer.Start();
                string lyricsText = "";
                if (string.IsNullOrEmpty(Player.CurrentlyPlayingFile?.SynchronizedLyric))
                {
                    var list = await Web.LyricsFetch.LyricsFetcher.FetchLyrics(SharedLogic.Player.CurrentlyPlayingFile).ConfigureAwait(false);

                    if (list == null || list?.Any() == false)
                    {
                        LyricsLoading = false;
                        return;
                    }
                    while (!LrcParser.IsLrc(list[0]))
                        list.RemoveAt(0);
                    lyricsText = list[0];
                    var s = SettingsViewModel.TracksCollection;
                    Player.CurrentlyPlayingFile.SynchronizedLyric = await list[0].ZipAsync();
                    await _service.UpdateMediafile(Player.CurrentlyPlayingFile);
                }
                else
                {
                    lyricsText = await Player.CurrentlyPlayingFile?.SynchronizedLyric?.UnzipAsync();
                }

                var parser = LrcParser.FromText(lyricsText);
                Lyrics = new ThreadSafeObservableCollection<IOneLineLyric>(parser.Lyrics);

                LyricsLoading = false;
                timer.Tick += (s, e) =>
                {
                    var currentPosition = TimeSpan.FromSeconds(Player.Position);
                    if (Lyrics.Any(t => t.Timestamp.Minutes == currentPosition.Minutes && t.Timestamp.Seconds == currentPosition.Seconds && (t.Timestamp.Milliseconds - currentPosition.Milliseconds) < 50))
                    {
                        var currentLyric = Lyrics.First(t => t.Timestamp.Minutes == currentPosition.Minutes && t.Timestamp.Seconds == currentPosition.Seconds);
                        if (currentLyric == null)
                            return;

                        var previousLyric = Lyrics.FirstOrDefault(t => t.IsActive) ?? null;
                        if (previousLyric != null && previousLyric.IsActive == true)
                            previousLyric.IsActive = false;
                        currentLyric.IsActive = true;

                        CurrentLyric = currentLyric;
                        LyricActivated?.Invoke(currentLyric, new EventArgs());
                    }
                };
            });
        }
        
        public event EventHandler LyricActivated;
        
        private async Task GetInfo(string artistName, string albumName)
        {
            try
            {
                if (InternetConnectivityHelper.IsInternetConnected)
                {
                    //cancel any previous requests
                    LastfmClient.HttpClient.CancelPendingRequests();
                    //start both tasks
                    TaskList.Clear();
                    TaskList.Add(GetLyrics());
                    TaskList.Add(GetArtistInfo(artistName.ScrubGarbage().GetTag()));
                    await Task.WhenAll(TaskList).ConfigureAwait(false);
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
                if (_retries == 10)
                {
                    //increase retry count
                    _retries++;

                    //retry
                    await GetInfo(artistName, albumName);
                }
            }
        }
        private async Task GetArtistInfo(string artistName)
        {
            await BreadDispatcher.InvokeAsync(async () =>
            {
                ArtistInfoLoading = true;
                Artists = new ThreadSafeObservableCollection<LastArtist>();
                SimilarArtists = null;

                //Parse and make a list of all artists from title
                //and artist strings
                var artistsList = TagParser.ParseArtists(artistName);
                if (SharedLogic.SettingsVm.AccountSettingsVM.NoOfArtistsToFetchInfoFor == "All artists")
                {
                    var artistsFromTitle = TagParser.ParseArtistsFromTitle(Player.CurrentlyPlayingFile.Title);
                    if (artistsFromTitle != null)
                        artistsList.AddRange(artistsFromTitle);
                    artistsList = artistsList.DistinctBy(t => t.Trim().ToLower()).ToList();
                }
                ArtistFetchFailed = false;
                //var trackInfo = await LastfmClient.Track.GetInfoAsync(TagParser.ParseTitle(SharedLogic.Player.CurrentlyPlayingFile.Title), artistsList[0]);
                //if (trackInfo.Success && SharedLogic.Player.CurrentlyPlayingFile.AttachedPicture == null)
                //{
                //    Player.CurrentlyPlayingFile.AttachedPicture = trackInfo.Content.Images?.Large?.AbsoluteUri;
                //    Player.CurrentlyPlayingFile.Album = trackInfo.Content.AlbumName;
                //    Player.CurrentlyPlayingFile.LeadArtist = trackInfo.Content.ArtistName;
                //    Player.CurrentlyPlayingFile.Title = trackInfo.Content.Name;
                //}
                //begin fetching all artist's info
                foreach (var artist in artistsList)
                {
                    var artistInfoResponse = await LastfmClient.Artist.GetInfoAsync(artist, "en", true).ConfigureAwait(false);
                    if (artistInfoResponse.Success)
                    {
                        var bio = artistInfoResponse.Content.Bio.Content;
                        bio = bio.ScrubHtml();
                        if (bio.Any())
                            bio = bio.Insert(bio.IndexOf("Read more on Last.fm."), "\r\n\r\n");
                        artistInfoResponse.Content.Bio.Content = bio;
                        Artists.Add(artistInfoResponse.Content);

                        if (SimilarArtists == null)
                            SimilarArtists = new ThreadSafeObservableCollection<LastArtist>(artistInfoResponse.Content.Similar);
                    }
                }

                //fail if no artist info is fetched
                if (!Artists.Any())
                {
                    ArtistFetchFailed = true;
                    ArtistInfoLoading = false;
                }
                
                ArtistInfoLoading = false;
            });
        }
        private async Task GetAlbumInfo(string artistName, string albumName)
        {
            await BreadDispatcher.InvokeAsync(async () =>
            {
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
                {
                    AlbumFetchFailed = true;
                }

                AlbumInfoLoading = false;
            });
        }     
    }
}
