using BreadPlayer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BreadPlayer.Database
{
    public class AlbumService : ChildSongsService, IDisposable
    {
        private IDatabaseService Database
        {
            get; set;
        }
        public AlbumService(IDatabaseService database) : base(database, "AlbumSongs", "AlbumSongsText")
        {
            Database = database;
        }
        public async Task InsertAlbums(IEnumerable<Album> albums)
        {
            await Database.InsertRecords(albums);
        }
        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
