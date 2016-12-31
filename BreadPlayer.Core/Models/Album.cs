using System.Collections.ObjectModel;

namespace BreadPlayer.Models
{
    public class Album
    {
        public string AlbumName { get; set; }
        public string Artist { get; set; }
        public string AlbumArt { get; set; }
        public ObservableCollection<Mediafile> AlbumSongs { get; set; } = new ObservableCollection<Mediafile>();
    }
}
