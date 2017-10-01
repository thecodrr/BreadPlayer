using BreadPlayer.Core.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace BreadPlayer.PlaylistBus
{
    internal interface IPlaylist
    {
        Task<IEnumerable<Mediafile>> LoadPlaylist(StorageFile file);

        Task<bool> SavePlaylist(IEnumerable<Mediafile> songs, Stream fileStream);
    }
}