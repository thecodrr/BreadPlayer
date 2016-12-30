using BreadPlayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace BreadPlayer.PlaylistBus
{
	interface IPlaylist
    {
        Task LoadPlaylist(StorageFile file);
        Task<bool> SavePlaylist(IEnumerable<Mediafile> songs);
    }
}
