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
            AddAlbums();
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
        public void AddAlbums()
        {
            foreach (var song in LibVM.TracksCollection.Elements)
            {
                Album alb = null;
                if (!AlbumCollection.Any(t => t.AlbumName == song.Album && t.Artist == song.LeadArtist))
                {
                    alb = new Album();
                    alb.AlbumName = song.Album;
                    alb.Artist = song.LeadArtist;
                    alb.AlbumArt = song.AttachedPicture;
                    AlbumCollection.Add(alb);
                }
                else
                {
                    AlbumCollection.First(t => t.AlbumName == song.Album && t.Artist == song.LeadArtist).AlbumSongs.Add(song);
                }
            }
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
                SplitViewMenu.SplitViewMenu.NavService.NavigateTo(typeof(PlaylistView), albumDict);
            }
        }
    }
}
