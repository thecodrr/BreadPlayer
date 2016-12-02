namespace BreadPlayer.Models
{
    public class Album : ViewModelBase
    {
        public string AlbumName { get; set; }
        public string Artist { get; set; }
        public string AlbumArt { get; set; }
        public ThreadSafeObservableCollection<Mediafile> AlbumSongs { get; set; } = new ThreadSafeObservableCollection<Mediafile>();
    }
}
