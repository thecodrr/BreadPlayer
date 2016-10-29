using BreadPlayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadPlayer.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Windows.Input;

namespace BreadPlayer.ViewModels
{
    public class AlbumArtistViewModel : ViewModelBase
    {
        /// <summary>
        /// The Constructor.
        /// </summary>
        public AlbumArtistViewModel()
        {
        }
      

        /// <summary>
        /// Collection containing all albums.
        /// </summary>
        public ThreadSafeObservableCollection<Album> AlbumCollection { get; set; } = new ThreadSafeObservableCollection<Album>();
        /// <summary>
        /// Adds all albums to <see cref="AlbumCollection"/>.
        /// </summary>
        /// <remarks>This is still experimental, a lot of performance improvements are needed. 
        /// For instance, for each loop needs to be removed.
        /// Maybe we can use direct database queries and fill the AlbumCollection with it?
        /// </remarks>
        public async Task AddAlbums()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async() =>
            {
                List<Album> albums = new List<Album>();
                int index = 0;
                foreach (var song in await LibVM.db.GetTracks().ConfigureAwait(false))
                {
                    index++;
                    Album alb = null;
                    if (!albums.Any(t => t.AlbumName == song.Album && t.Artist == song.LeadArtist))
                    {
                        alb = new Album();
                        alb.AlbumName = song.Album;
                        alb.Artist = song.LeadArtist;
                        alb.AlbumArt = song.AttachedPicture;
                        albums.Add(alb);
                    }
                    if (albums.Any()) albums.FirstOrDefault(t => t.AlbumName == song.Album && t.Artist == song.LeadArtist).AlbumSongs.Add(song);
                    if(index == 50)
                    {
                        index = 0;
                        AlbumCollection.AddRange(albums);
                        albums.Clear();
                    }
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
                Dictionary<Playlist, IEnumerable<Mediafile>> albumDict = new Dictionary<Playlist, IEnumerable<Mediafile>>();
                albumDict.Add(new Playlist() { Name = album.AlbumName, Description=album.Artist }, album.AlbumSongs);
                PlaylistVM.IsMenuVisible = false;
                SplitViewMenu.SplitViewMenu.UnSelectAll();
                SplitViewMenu.SplitViewMenu.NavService.Frame.Navigate(typeof(PlaylistView), albumDict);
            }
        }
    }
}
