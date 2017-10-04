using BreadPlayer.Core;
using BreadPlayer.Core.Common;
using BreadPlayer.Core.Models;
using BreadPlayer.Database;
using BreadPlayer.DataSources;
using BreadPlayer.Dispatcher;
using BreadPlayer.Extensions;
using BreadPlayer.Helpers;
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
            AlbumArtistService = new AlbumArtistService(new DocumentStoreDatabaseService(SharedLogic.Instance.DatabasePath, "Albums"));
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
                await AlbumArtistService.InsertAlbums(albums).ConfigureAwait(false);
                await AlbumArtistService.InsertArtists(artists).ConfigureAwait(false);
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
            ArtistsCollection.OnEndLoading = () => RecordsLoading = false;
            ArtistsCollection.OnError = (ex) => RecordsLoading = false;
        }

        private async void ArtistsCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                // delay by 2 seconds so as not to hang up UI
                await Task.Delay(2000);
                
                await CacheArtists(e.NewItems.Cast<Artist>()).ConfigureAwait(false);
            }
            if (ArtistsCollection.Count == AlbumArtistService.ArtistsCount)
            {
                ArtistsCollection.CollectionChanged -= ArtistsCollection_CollectionChanged;
            }
        }

        private async Task CacheArtists(IEnumerable<Artist> artists)
        {
            if (InternetConnectivityHelper.IsInternetConnected && artists.Any(t => t.HasFetchedInfo == false))
            {
                foreach (var artist in artists.Where(t => t.HasFetchedInfo == false))
                {
                    //if internet is disconnected in the middle of this process,
                    //we need to take precautions.
                    if (InternetConnectivityHelper.IsInternetConnected)
                    {
                        var artistName = TagParser.ParseArtists(artist.Name)[0];
                        if (!string.IsNullOrEmpty(artistName) && string.IsNullOrEmpty(artist.Picture))
                        {
                            await BreadDispatcher.InvokeAsync(async () =>
                            {
                                try
                                {
                                    var artistInfo = (await LastfmClient.Artist.GetInfoAsync(artistName, "en", true).ConfigureAwait(false))?.Content;
                                    if (artistInfo?.MainImage != null && artistInfo?.MainImage?.Large != null)
                                    {
                                        var cached = await TagReaderHelper.CacheArtistArt(artistInfo.MainImage.Large.AbsoluteUri, artist).ConfigureAwait(false);
                                        ArtistsCollection.FirstOrDefault(t => t.Name == artist.Name).Picture = cached.artistArtPath;
                                        ArtistsCollection.FirstOrDefault(t => t.Name == artist.Name).PictureColor = cached.dominantColor.ToHexString();
                                    }
                                    if (!string.IsNullOrEmpty(artistInfo?.Bio?.Content))
                                    {
                                        string bio = await artistInfo?.Bio?.Content.ZipAsync();
                                        ArtistsCollection.FirstOrDefault(t => t.Name == artist.Name).Bio = bio ?? "";
                                    }
                                    ArtistsCollection.FirstOrDefault(t => t.Name == artist.Name).HasFetchedInfo = true;
                                    await AlbumArtistService.UpdateArtistAsync(ArtistsCollection.FirstOrDefault(t => t.Name == artist.Name)).ConfigureAwait(false);
                                }
                                catch(TaskCanceledException)
                                {
                                    //there **is** nothing to be done :D
                                }
                            });
                        }
                    }
                }
            }
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