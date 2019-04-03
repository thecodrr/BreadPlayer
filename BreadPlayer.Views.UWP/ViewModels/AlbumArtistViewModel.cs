using BreadPlayer.Core;
using BreadPlayer.Core.Common;
using BreadPlayer.Core.Models;
using BreadPlayer.Database;
using BreadPlayer.DataSources;
using BreadPlayer.Dispatcher;
using BreadPlayer.Extensions;
using BreadPlayer.Helpers;
using BreadPlayer.Interfaces;
using BreadPlayer.Messengers;
using BreadPlayer.Parsers.TagParser;
using BreadPlayer.Web.Lastfm;
using IF.Lastfm.Core.Api;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace BreadPlayer.ViewModels
{
    public class AlbumArtistViewModel : ObservableObject
    {
        #region Database Methods

        private AlbumArtistService AlbumArtistService { get; set; }

        public void InitDb()
        {
            AlbumArtistService = new AlbumArtistService(new KeyValueStoreDatabaseService(SharedLogic.Instance.DatabasePath, "Albums"));
        }

        #endregion Database Methods

        private IncrementalLoadingObservableCollection<AlbumDataSource, Album> _albumcollection;

        private IncrementalLoadingObservableCollection<ArtistDataSource, Artist> _artistscollection;

        private bool _recordsLoading = true;

        /// <summary>
        /// The Constructor.
        /// </summary>
        public AlbumArtistViewModel()
        {
            InitDb();
            LoadAlbums();
            LoadArtists();
            Messenger.Instance.Register(MessageTypes.MsgAddAlbums, new Action<Message>(HandleAddAlbumMessage));
        }

        /// <summary>
        /// Collection containing all albums.
        /// </summary>
        public IncrementalLoadingObservableCollection<AlbumDataSource, Album> AlbumCollection
        {
            get { if (_albumcollection == null) { _albumcollection = new IncrementalLoadingObservableCollection<AlbumDataSource, Album>(); } return _albumcollection; }
            set => Set(ref _albumcollection, value);
        }

        /// <summary>
        /// Collection containing all albums.
        /// </summary>
        public IncrementalLoadingObservableCollection<ArtistDataSource, Artist> ArtistsCollection
        {
            get { if (_artistscollection == null) { _artistscollection = new IncrementalLoadingObservableCollection<ArtistDataSource, Artist>(); } return _artistscollection; }
            set => Set(ref _artistscollection, value);
        }

        private LastfmClient LastfmClient => new Lastfm().LastfmClient;

        /// <summary>
        /// Collection containing all albums.
        /// </summary>
        public bool RecordsLoading
        {
            get => _recordsLoading;
            set => Set(ref _recordsLoading, value);
        }
        private RelayCommand _deleteCommand;
        /// <summary>
        /// Gets Play command. This calls the <see cref="Delete(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand DeleteCommand
        {
            get
            { if (_deleteCommand == null) { _deleteCommand = new RelayCommand(param => Delete(param)); } return _deleteCommand; }
        }
        private async void Delete(object para)
        {
            if (para is Album album)
            {
                AlbumCollection.Remove(album);
                await AlbumArtistService.DeleteAlbumAsync(album).ConfigureAwait(false);
            }
        }
        /// <summary>
        /// Adds all albums to <see cref="AlbumCollection"/>.
        /// </summary>
        public async Task AddAlbumsAndArtists(IEnumerable<Mediafile> mediafiles)
        {
            if (mediafiles == null)
                return;

            List<Album> albums = new List<Album>();
            List<Artist> artists = new List<Artist>();
            await Task.Run(() =>
            {
                foreach (var mediafile in mediafiles.ToList())
                {
                    if (albums.All(t => t.AlbumName != mediafile.Album))
                    {
                        Album album = new Album
                        {
                            Artist = mediafile.LeadArtist,
                            AlbumName = mediafile.Album,
                            AlbumArt = string.IsNullOrEmpty(mediafile?.AttachedPicture) ? null : mediafile?.AttachedPicture
                        };
                        albums.Add(album);
                    }
                    if (artists.All(t => t.Name != mediafile.LeadArtist))
                    {
                        Artist artist = new Artist
                        {
                            Name = mediafile?.LeadArtist
                        };

                        artists.Add(artist);
                    }
                }
            }).ContinueWith(async (task) =>
            {
                await AlbumArtistService.InsertAlbums(albums);
                await AlbumArtistService.InsertArtists(artists);
                ArtistsCollection.CollectionChanged += ArtistsCollection_CollectionChanged;
            });
        }

        public void LoadAlbums()
        {
            AlbumCollection.OnStartLoading = () => RecordsLoading = true;
            AlbumCollection.OnEndLoading = () => RecordsLoading = false;
            AlbumCollection.OnError = (ex) => RecordsLoading = false;
        }

        public void LoadArtists()
        {
            ArtistsCollection.CollectionChanged += ArtistsCollection_CollectionChanged;
            ArtistsCollection.OnStartLoading = () => RecordsLoading = true;
            ArtistsCollection.OnEndLoading = () =>
            {
                RecordsLoading = false;
            };
            ArtistsCollection.OnError = (ex) => RecordsLoading = false;
        }

        private async void ArtistsCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                await CacheArtists(e.NewItems.Cast<Artist>()).ConfigureAwait(false);
            }
            if (ArtistsCollection.Count == AlbumArtistService.ArtistsCount)
            {
                ArtistsCollection.CollectionChanged -= ArtistsCollection_CollectionChanged;
            }
        }

        private async Task CacheArtists(IEnumerable<Artist> artists)
        {
            BLogger.I("Starting artist art caching...");
            if (!InternetConnectivityHelper.IsInternetConnected || !artists.Any(t => t.HasFetchedInfo == false))
            {
                //No internet or no artists to fetch info for. Stop!
                BLogger.I("Stopping artist art caching. Internet state: {state}", InternetConnectivityHelper.IsInternetConnected);
                return;
            }
            foreach (var artist in artists.Where(t => t.HasFetchedInfo == false))
            {
                if (!InternetConnectivityHelper.IsInternetConnected)
                {
                    //if internet is disconnected in the middle of this process, stop!
                    BLogger.I("Stopping artist art caching. No internet connection.");
                    break;
                }
                var collectionArtist = ArtistsCollection.FirstOrDefault(t => t.Name == artist.Name);
                if (collectionArtist == null)
                {
                    BLogger.I("No artist found for: {name}", artist.Name);
                    continue;
                }

                var artistName = TagParser.ParseArtists(artist.Name.ToString())[0];
                BLogger.I("Artist name parsed: {name}", artistName);

                if (string.IsNullOrEmpty(artistName))
                {
                    BLogger.I("Invalid parsed artist name: {original}. Using original.", artist.Name);
                    artistName = artist.Name;
                }

                try
                {
                    var artistInfo = (await LastfmClient.Artist.GetInfoAsync(artistName, "en", true).ConfigureAwait(false))?.Content;

                    BLogger.I("Info fetched for artist {artist}. Info: {info}", artistName, artistInfo);

                    if (artistInfo?.MainImage?.Large != null)
                    {
                        var cached = await TagReaderHelper.CacheArtistArt(artistInfo.MainImage.Large.AbsoluteUri, artist).ConfigureAwait(false);

                        if (cached.artistArtPath == null)
                        {
                            BLogger.I("Artist art for {artist} failed to cache.", artist.Name);
                            throw new TaskCanceledException();
                        }
                        BLogger.I("Artist art for {artist} cached.", artist.Name);

                        collectionArtist.Picture = cached.artistArtPath;
                        collectionArtist.PictureColor = cached.dominantColor.ToHexString();
                    }
                    if (!string.IsNullOrEmpty(artistInfo?.Bio?.Content))
                    {
                        string bio = await artistInfo?.Bio?.Content.ZipAsync();
                        collectionArtist.Bio = bio ?? "";
                    }
                    collectionArtist.HasFetchedInfo = true;

                    BLogger.I("Saving changes for {artist} into database.", artist.Name);
                    await AlbumArtistService.UpdateArtistAsync(collectionArtist).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    BLogger.I("Task was cancelled during artist caching. Will try next time.");
                }
            }
            BLogger.I("Artist art caching completed.");
        }

        private async void HandleAddAlbumMessage(Message message)
        {
            if (message != null)
            {
                message.HandledStatus = MessageHandledStatus.HandledCompleted;
                await AddAlbumsAndArtists(message.Payload as IEnumerable<Mediafile>);
            }
        }
    }
}