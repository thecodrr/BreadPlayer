using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using BreadPlayer.Core;
using BreadPlayer.Core.Models;
using BreadPlayer.Database;
using BreadPlayer.Messengers;
using IF.Lastfm.Core.Api;
using BreadPlayer.Web.Lastfm;

namespace BreadPlayer.ViewModels
{
    public class AlbumArtistViewModel : ViewModelBase
    {
        #region Database Methods

        private AlbumArtistService AlbumArtistService { get; set; }
        public void InitDb()
        {
            AlbumArtistService = new AlbumArtistService(new DocumentStoreDatabaseService(SharedLogic.DatabasePath, "Albums"));
        }       
        #endregion

        private async void HandleAddAlbumMessage(Message message)
        {
            if (message != null)
            {
                message.HandledStatus = MessageHandledStatus.HandledCompleted;
                await AddAlbums(message.Payload as IEnumerable<Mediafile>);
                await AddArtists(message.Payload as IEnumerable<Mediafile>);
            }
        }
        /// <summary>
        /// The Constructor.
        /// </summary>
        public AlbumArtistViewModel()
        {
            InitDb();
            Messenger.Instance.Register(MessageTypes.MsgAddAlbums, new Action<Message>(HandleAddAlbumMessage));
        }

      
        public async Task LoadAlbums()
        {
            if (AlbumCollection?.Count <= 0)
            {
                AlbumCollection.AddRange(await AlbumArtistService.GetAlbumsAsync().ConfigureAwait(false));//.Add(album);
                AlbumCollection.CollectionChanged += AlbumCollection_CollectionChanged;
                if (AlbumCollection.Count <= 0)
                {
                    RecordsLoaded = false;
                }
            }
        }

        public async Task LoadArtists()
        {
            if (ArtistsCollection?.Count <= 0)
            {
                ArtistsCollection.AddRange(await AlbumArtistService.GetArtistsAsync().ConfigureAwait(false));//.Add(album);
                ArtistsCollection.CollectionChanged += AlbumCollection_CollectionChanged;
                if (ArtistsCollection.Count <= 0)
                {
                    RecordsLoaded = false;
                }
            }
        }

        private void AlbumCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //Albums are loaded, we can now hide the progress ring.
            if (AlbumCollection.Count > 0 || ArtistsCollection.Count > 0)
            {
                RecordsLoaded = false;
            }
            else
            {
                RecordsLoaded = true;
            }
        }

        private bool _recordsLoaded = true;
        /// <summary>
        /// Collection containing all albums.
        /// </summary>
        public bool RecordsLoaded
        {
            get => _recordsLoaded;
            set => Set(ref _recordsLoaded, value);
        }

        private ThreadSafeObservableCollection<Album> _albumcollection;
        /// <summary>
        /// Collection containing all albums.
        /// </summary>
        public ThreadSafeObservableCollection<Album> AlbumCollection
        {
            get { if (_albumcollection == null) { _albumcollection = new ThreadSafeObservableCollection<Album>(); } return _albumcollection; }
            set => Set(ref _albumcollection, value);
        }

        private ThreadSafeObservableCollection<Artist> _artistscollection;
        /// <summary>
        /// Collection containing all albums.
        /// </summary>
        public ThreadSafeObservableCollection<Artist> ArtistsCollection
        {
            get { if (_artistscollection == null) { _artistscollection = new ThreadSafeObservableCollection<Artist>(); } return _artistscollection; }
            set => Set(ref _artistscollection, value);
        }
        /// <summary>
        /// Adds all albums to <see cref="AlbumCollection"/>.
        /// </summary>
        public async Task AddAlbums(IEnumerable<Mediafile> mediafiles)
        {
            List<Album> albums = new List<Album>();
            //List<ChildSong> childsongs = new List<ChildSong>();
            await Task.Run(() =>
            {
                //Random albumRandom = new Random();
                foreach (var albumGroup in mediafiles.GroupBy(t => t.Album))
                {
                    var firstSong = albumGroup.First() ?? new Mediafile();
                    Album album = new Album
                    {
                        Artist = firstSong?.LeadArtist,
                        AlbumName = albumGroup.Key,
                        AlbumArt = string.IsNullOrEmpty(firstSong?.AttachedPicture) ? null : firstSong?.AttachedPicture
                    };                           
                    albums.Add(album);
                }
            }).ContinueWith(async (task) =>
            {
                await AlbumArtistService.InsertAlbums(albums);
            });           
        }
        private LastfmClient LastfmClient => new Lastfm().LastfmClient;

        /// <summary>
        /// Adds all albums to <see cref="AlbumCollection"/>.
        /// </summary>
        public async Task AddArtists(IEnumerable<Mediafile> mediafiles)
        {
            List<Artist> artists = new List<Artist>();
            //List<ChildSong> childsongs = new List<ChildSong>();
            await Task.Run(async () =>
            {
                //Random albumRandom = new Random();
                foreach (var artistGroup in mediafiles.GroupBy(t => t.LeadArtist))
                {
                    var firstSong = artistGroup.First() ?? new Mediafile();
                     var artistInfo = await LastfmClient.Artist.GetInfoAsync(firstSong?.LeadArtist, "en", true);
                    Artist artist = new Artist
                    {
                        Name = firstSong?.LeadArtist,
                        Picture = artistInfo?.Content?.MainImage?.Large.AbsoluteUri,
                        Bio = artistInfo?.Content?.Bio?.Content
                    };
                    artists.Add(artist);
                }
            }).ContinueWith(async (task) =>
            {
                await AlbumArtistService.InsertArtists(artists);
            });
        }
    }
}
