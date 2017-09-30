using BreadPlayer.Core;
using BreadPlayer.Core.Common;
using BreadPlayer.Core.Extensions;
using BreadPlayer.Database;
using BreadPlayer.Dispatcher;
using BreadPlayer.Extensions;
using BreadPlayer.Helpers;
using BreadPlayer.Parsers.LRCParser;
using BreadPlayer.Parsers.TagParser;
using BreadPlayer.Web.Lastfm;
using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BreadPlayer.ViewModels
{
    public class NowPlayingViewModel : ViewModelBase
    {
        #region Loading Properties

        private bool _albumFetchFailed;
        private bool _albumInfoLoading;
        private bool _artistFetchFailed;
        private bool _artistInfoLoading;
        private bool _lyricsLoading;
        public bool AlbumFetchFailed { get => _albumFetchFailed; set => Set(ref _albumFetchFailed, value); }
        public bool AlbumInfoLoading { get => _albumInfoLoading; set => Set(ref _albumInfoLoading, value); }
        public bool ArtistFetchFailed { get => _artistFetchFailed; set => Set(ref _artistFetchFailed, value); }
        public bool ArtistInfoLoading { get => _artistInfoLoading; set => Set(ref _artistInfoLoading, value); }
        public bool LyricsLoading { get => _lyricsLoading; set => Set(ref _lyricsLoading, value); }
        #endregion Loading Properties

        private ThreadSafeObservableCollection<LastTrack> _albumTracks;
        private IOneLineLyric _currentLyric;
        private int _retries;
        private LibraryService _service = new LibraryService(new DocumentStoreDatabaseService(SharedLogic.DatabasePath, "Tracks"));
        private ThreadSafeObservableCollection<LastArtist> _similarArtists;
        private ThreadSafeObservableCollection<LastArtist> artists;
        private ThreadSafeObservableCollection<IOneLineLyric> lyrics;
        private List<Task> TaskList = new List<Task>();
        private BreadPlayer.Core.PortableAPIs.DispatcherTimer timer;
        public NowPlayingViewModel()
        {
            RetryCommand = new RelayCommand(Retry);

            //the work around to knowing when the new song has started.
            //the event is needed to update the bio etc.
            SharedLogic.Player.MediaChanged += OnMediaChanged;
        }

        public event EventHandler LyricActivated;

        public ThreadSafeObservableCollection<LastTrack> AlbumTracks
        {
            get => _albumTracks;
            set => Set(ref _albumTracks, value);
        }

        public ThreadSafeObservableCollection<LastArtist> Artists
        {
            get => artists;
            set => Set(ref artists, value);
        }

        public string CorrectAlbum { get; set; }
        public string CorrectArtist { get; set; }
        public IOneLineLyric CurrentLyric
        {
            get => _currentLyric;
            set => Set(ref _currentLyric, value);
        }
        private LastfmClient LastfmClient => new Lastfm().LastfmClient;

        public ThreadSafeObservableCollection<IOneLineLyric> Lyrics
        {
            get => lyrics;
            set => Set(ref lyrics, value);
        }
        public ICommand RetryCommand { get; set; }

        public ThreadSafeObservableCollection<LastArtist> SimilarArtists
        {
            get => _similarArtists;
            set => Set(ref _similarArtists, value);
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

        private async Task GetInfo(string artistName, string albumName)
        {
            try
            {
                TaskList.Clear();
                if (InternetConnectivityHelper.IsInternetConnected || !string.IsNullOrEmpty(Player.CurrentlyPlayingFile.SynchronizedLyric))
                {
                    //cancel any previous requests
                    LastfmClient.HttpClient.CancelPendingRequests();
                    TaskList.Add(GetLyrics());
                    if(InternetConnectivityHelper.IsInternetConnected)
                        TaskList.Add(GetArtistInfo(artistName.GetTag()));
                }
                else
                {
                    AlbumFetchFailed = true;
                    ArtistFetchFailed = true;
                } 
                //start all tasks   
                if (TaskList.Any())
                    await Task.WhenAll(TaskList).ConfigureAwait(false);
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
                    Player.CurrentlyPlayingFile.SynchronizedLyric = await list[0].ZipAsync();
                    await _service.UpdateMediafile(Player.CurrentlyPlayingFile);
                }
                else
                {
                    lyricsText = await Player.CurrentlyPlayingFile?.SynchronizedLyric?.UnzipAsync();
                }
                if (!string.IsNullOrEmpty(lyricsText))
                {
                    var parser = LrcParser.FromText(lyricsText);
                    if (parser.Lyrics.Any())
                    {
                        Lyrics = new ThreadSafeObservableCollection<IOneLineLyric>(parser.Lyrics);
                        timer.Start();
                    }
                }
                LyricsLoading = false;
                timer.Tick += (s, e) =>
                {
                    var currentPosition = TimeSpan.FromSeconds(Player.Position);
                    if (Lyrics?.Any(t => t.Timestamp.Minutes == currentPosition.Minutes && t.Timestamp.Seconds == currentPosition.Seconds && (t.Timestamp.Milliseconds - currentPosition.Milliseconds) < 50) == true)
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
    }
}