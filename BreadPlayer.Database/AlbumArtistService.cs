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

        public int ArtistsCount
        {
            get
            {
                Database.ChangeTable("Artists", "ArtistsText");
                return Database.GetRecordsCount();
            }
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

        public Task<Artist> GetArtistAsync(string query)
        {
            Database.ChangeTable("Artists", "ArtistsText");
            return Database.GetRecordByQueryAsync<Artist>(query);
        }

        public Task<Album> GetAlbumAsync(string query)
        {
            Database.ChangeTable("Albums", "AlbumsText");
            return Database.GetRecordByQueryAsync<Album>(query);
        }
        public Artist GetArtistByIdAsync(long id)
        {
            Database.ChangeTable("Artists", "ArtistsText");
            return Database.GetRecordById<Artist>(id);
        }

        public Album GetAlbumByIdAsync(long id)
        {
            Database.ChangeTable("Albums", "AlbumsText");
            return Database.GetRecordById<Album>(id);
        }
        public Task DeleteAlbumAsync(Album album)
        {
            Database.ChangeTable("Albums", "AlbumsText");
            return Database.RemoveRecord(album);
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

        public Task<IEnumerable<Artist>> QueryArtistsAsync(string term, int limit = int.MaxValue)
        {
            Database.ChangeTable("Artists", "ArtistsText");
            return Database.QueryRecords<Artist>(term, limit);
        }

        public Task<IEnumerable<Album>> QueryAlbumsAsync(string term, int limit = int.MaxValue)
        {
            Database.ChangeTable("Albums", "AlbumsText");
            return Database.QueryRecords<Album>(term, limit);
        }

        public Task UpdateArtistAsync(Artist artist)
        {
            Database.ChangeTable("Artists", "ArtistsText");
            return Database.UpdateRecordAsync(artist, artist.Id);
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}