using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macalifa.Models;
namespace Macalifa.ViewModels
{
    public class PlaylistViewModel : ViewModelBase
    {
        public ObservableRangeCollection<Mediafile> Playlist { get; set; }

        public PlaylistViewModel(ObservableRangeCollection<Mediafile> playlist)
        {
            Playlist = playlist;
        }
    }
}
