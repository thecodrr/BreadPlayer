using BreadPlayer.Core.Models;
using Microsoft.Toolkit.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using BreadPlayer.Core;

namespace BreadPlayer.DataSources
{
    public class ArtistDataSource : IIncrementalSource<Artist>
    {
        public Task<IEnumerable<Artist>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            return SharedLogic.Instance.AlbumArtistService.GetRangeOfArtistsAsync(pageIndex * pageSize, pageSize);
        }
    }
    public class AlbumDataSource : IIncrementalSource<Album>
    {
        public Task<IEnumerable<Album>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            return SharedLogic.Instance.AlbumArtistService.GetRangeOfAlbumsAsync(pageIndex * pageSize, (pageIndex * pageSize) + pageSize);
        }
    }
}
