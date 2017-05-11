using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BreadPlayer.Core.Models;
using BreadPlayer.Messengers;
using BreadPlayer.Database;

namespace BreadPlayer.ViewModels
{
    public class AlbumArtistViewModel : ViewModelBase
    {
        #region Database Methods

        private AlbumService AlbumService { get; set; }
        public void InitDB()
        {
            AlbumService = new AlbumService(new KeyValueStoreDatabaseService(Core.SharedLogic.DatabasePath, "Albums", "AlbumsText"));
        }       
        #endregion

        private async void HandleAddAlbumMessage(Message message)
        {
            if (message != null)
            {
                message.HandledStatus = MessageHandledStatus.HandledCompleted;
                await AddAlbums(message.Payload as IEnumerable<Mediafile>);
            }
        }
        /// <summary>
        /// The Constructor.
        /// </summary>
        public AlbumArtistViewModel()
        {
            InitDB();
            Messenger.Instance.Register(MessageTypes.MSG_ADD_ALBUMS, new Action<Message>(HandleAddAlbumMessage));
        }

      
        public async Task LoadAlbums()
        {
            AlbumCollection.AddRange(await AlbumService.GetAlbumsAsync().ConfigureAwait(false));//.Add(album);
            AlbumCollection.CollectionChanged += AlbumCollection_CollectionChanged;
            if (AlbumCollection.Count <= 0)
                AlbumsLoaded = false;
        }

        private void AlbumCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //Albums are loaded, we can now hide the progress ring.
            if (AlbumCollection.Count > 0)
            {
                AlbumsLoaded = false;
            }
            else
                AlbumsLoaded = true;
        }

        private bool albumsLoaded = true;
        /// <summary>
        /// Collection containing all albums.
        /// </summary>
        public bool AlbumsLoaded
        {
            get => albumsLoaded;
            set => Set(ref albumsLoaded, value);
        }

        private ThreadSafeObservableCollection<Album> albumcollection;
        /// <summary>
        /// Collection containing all albums.
        /// </summary>
        public ThreadSafeObservableCollection<Album> AlbumCollection
        {
            get { if (albumcollection == null) albumcollection = new ThreadSafeObservableCollection<Album>(); return albumcollection; }
            set => Set(ref albumcollection, value);
        }
        /// <summary>
        /// Adds all albums to <see cref="AlbumCollection"/>.
        /// </summary>
        public async Task AddAlbums(IEnumerable<Mediafile> mediafiles)
        {
            List<Album> albums = new List<Album>();
            List<ChildSong> childsongs = new List<ChildSong>();
            await Task.Run(() =>
            {
                Random albumRandom = new Random();
                foreach (var albumGroup in mediafiles.GroupBy(t => t.Album))
                {
                    var firstSong = albumGroup.First() ?? new Mediafile();
                    Album album = new Album()
                    {
                        Artist = firstSong?.LeadArtist,
                        AlbumName = albumGroup.Key,
                        AlbumArt = string.IsNullOrEmpty(firstSong?.AttachedPicture) ? null : firstSong?.AttachedPicture
                    };                           
                    albums.Add(album);
                }
            }).ContinueWith(async(task) =>
            {
                await AlbumService.InsertAlbums(albums);
                AlbumCollection.AddRange(albums);
            });           
        }        
    }
}
