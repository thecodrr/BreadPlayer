namespace BreadPlayer.Core.Common
{
    public interface ICollection
    {
        string Name { get; }
    }

    public class PlaylistCollection : ICollection
    {
        public string Name => "playlists";
    }

    public class TracksCollection : ICollection
    {
        public string Name => "tracks";
    }

    public class RecentCollection : ICollection
    {
        public string Name => "recent";
    }
}