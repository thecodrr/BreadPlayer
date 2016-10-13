using Macalifa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadPlayer.Models;

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
                if (!AlbumCollection.Any(t => t.AlbumName == song.Album && t.Artist == song.LeadArtist))
                {
                    Album alb = new Album();
                    alb.AlbumName = song.Album;
                    alb.Artist = song.LeadArtist;
                    alb.AlbumArt = song.AttachedPicture;
                    AlbumCollection.Add(alb);
                }
            }

        }
    }
}
