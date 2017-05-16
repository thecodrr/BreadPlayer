using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using BreadPlayer.Core.Models;

namespace BreadPlayer.PlaylistBus
{
    internal interface IPlaylist
    {
        Task<IEnumerable<Mediafile>> LoadPlaylist(StorageFile file);
        Task<bool> SavePlaylist(IEnumerable<Mediafile> songs);
    }
}
