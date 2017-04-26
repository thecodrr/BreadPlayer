using BreadPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BreadPlayer.Database
{
    public class AlbumService : IDisposable
    {
        private IDatabaseService Database
        {
            get; set;
        }
        public AlbumService(IDatabaseService database)
        {
            Database = database;
        }
        public async Task InsertAlbums(IEnumerable<Album> albums)
        {
            Database.ChangeTable("Albums", "AlbumsText");
            await Database.InsertRecords(albums);
        }
        public async Task<IEnumerable<Album>> GetAlbumsAsync()
        {
            Database.ChangeTable("Albums", "AlbumsText");                  
            return await Database.GetRecords<Album>();
        }
        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
