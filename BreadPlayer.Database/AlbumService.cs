using BreadPlayer.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BreadPlayer.Database
{
    public class AlbumArtistService : IDisposable
    {
        private IDatabaseService Database
        {
            get;
        }
        public AlbumArtistService(IDatabaseService database)
        {
            Database = database;
        }
        public async Task InsertAlbums(IEnumerable<Album> albums)
        {
            Database.ChangeTable("Albums", "AlbumsText");
            await Database.InsertRecords(albums);
        }
        public async Task InsertArtists(IEnumerable<Artist> artists)
        {
            Database.ChangeTable("Artists", "ArtistsText");
            await Database.InsertRecords(artists);
        }
        public Task<IEnumerable<Artist>> GetArtistsAsync()
        {
            Database.ChangeTable("Artists", "ArtistsText");
            return Database.GetRecords<Artist>();
        }
        public Task<IEnumerable<Album>> GetAlbumsAsync()
        {
            Database.ChangeTable("Albums", "AlbumsText");                  
            return Database.GetRecords<Album>();
        }
        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
