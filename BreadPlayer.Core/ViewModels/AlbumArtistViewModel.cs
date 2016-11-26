using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BreadPlayer.Models;
using System.Windows.Input;
using LiteDB;
using Windows.Storage;
using BreadPlayer.Service;
using BreadPlayer.Messengers;

namespace BreadPlayer.ViewModels
{
	public class AlbumArtistViewModel : ViewModelBase, IDisposable
    {
        async void HandleAddAlbumMessage(Message message)
        {
            if (message != null)
            {
                message.HandledStatus = MessageHandledStatus.HandledCompleted;
                await AddAlbums().ConfigureAwait(false);
            }
        }
        LiteDatabase db;
        public LiteCollection<Album> albumCollection;
        /// <summary>
        /// The Constructor.
        /// </summary>
        public AlbumArtistViewModel()
        {
            InitDB();
            Messenger.Instance.Register(MessageTypes.MSG_ADD_ALBUMS, new Action<Message>(HandleAddAlbumMessage));
        }
       public async void InitDB()
        {
            try
            {
                db = new LiteDatabase("filename=" + ApplicationData.Current.LocalFolder.Path + @"\albums.db;journal=false;");

                albumCollection = db.GetCollection<Album>("albums");
                albumCollection.EnsureIndex(t => t.AlbumName);
                albumCollection.EnsureIndex(t => t.Artist);
            }
            catch
            {
                await (await StorageFile.GetFileFromPathAsync(ApplicationData.Current.LocalFolder.Path + @"\albums.db")).DeleteAsync();
                InitDB();
            }
        }
        public async Task LoadAlbums()
        {
          //  AlbumCollection.IsObserving = true;
             //AlbumCollection.AddRange();
            foreach(var album in await GetAlbums().ConfigureAwait(false))
            {
                AlbumCollection.Add(album);
            }
        }
        public async Task<IEnumerable<Album>> GetAlbums()
        {
            IEnumerable<Album> collection = null;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                collection = albumCollection.Find(LiteDB.Query.All());
            });
            return collection;
        }
        ThreadSafeObservableCollection<Album> albumcollection;
        /// <summary>
        /// Collection containing all albums.
        /// </summary>
        public ThreadSafeObservableCollection<Album> AlbumCollection
        {
            get {if(albumcollection ==null) albumcollection = new ThreadSafeObservableCollection<Album>(); return albumcollection; }
            set { Set(ref albumcollection, value); }
        }
        /// <summary>
        /// Adds all albums to <see cref="AlbumCollection"/>.
        /// </summary>
        /// <remarks>This is still experimental, a lot of performance improvements are needed. 
        /// For instance, for each loop needs to be removed.
        /// Maybe we can use direct database queries and fill the AlbumCollection with it?
        /// </remarks>
        public async Task AddAlbums()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                using(LibraryService service = new LibraryService(new DatabaseService()))
                {
                    List<Album> albums = new List<Album>();
                    foreach (var song in await service.GetAllMediafiles().ConfigureAwait(false))
                    {
                        Album alb = null;
                        if (!albums.Any(t => t.AlbumName == song.Album && t.Artist == song.LeadArtist))
                        {
                            alb = new Album();
                            alb.AlbumName = song.Album;
                            alb.Artist = song.LeadArtist;
                            alb.AlbumArt = string.IsNullOrEmpty(song.AttachedPicture) ? null : song.AttachedPicture;
                            albums.Add(alb);
                        }
                        if (albums.Any()) albums.FirstOrDefault(t => t.AlbumName == song.Album && t.Artist == song.LeadArtist).AlbumSongs.Add(song);
                    }
                    albumCollection.Insert(albums);
                    AlbumCollection.AddRange(albums);
                }                
            }).AsTask().ConfigureAwait(false);

           
        }
        RelayCommand _navigateCommand;
        public ICommand NavigateToAlbumPageCommand
        {
            get
            { if (_navigateCommand == null) { _navigateCommand = new RelayCommand(param => this.NavigateToAlbumPage(param)); } return _navigateCommand; }
        }
        void NavigateToAlbumPage(object para)
        {
            if(para is Album)
            {
                Album album = para as Album;
               //PlaylistVM.IsMenuVisible = false;
                SplitViewMenu.SplitViewMenu.UnSelectAll();
                SplitViewMenu.SplitViewMenu.NavService.Frame.Navigate(typeof(PlaylistView), album);
            }
        }

        public void Dispose()
        {
            AlbumCollection.Clear();
        }
    }
}
