using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Common
{
    public interface ICollection
    {
        string Name { get;}
    }
    public class PlaylistCollection : ICollection
    {
        public string Name { get { return "playlists"; } }
    }
    public class TracksCollection : ICollection
    {
        public string Name { get { return "tracks"; } }
    }
    public class RecentCollection : ICollection
    {
        public string Name { get { return "recent"; } }
    }
}
