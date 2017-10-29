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
                Database.ChangeContext("Artists");
                return Database.GetRecordsCount();
            }
        }

        public AlbumArtistService(IDatabaseService database)
        {
            Database = database;
        }

        public async Task InsertAlbums(IEnumerable<Album> albums)
        {
            Database.ChangeContext("Albums");
            await Database.InsertRecords(albums);
        }

        public async Task InsertArtists(IEnumerable<Artist> artists)
        {
            Database.ChangeContext("Artists");
            await Database.InsertRecords(artists);
        }

        public Task<Artist> GetArtistAsync(string query)
        {
            Database.ChangeContext("Artists");
            return Database.GetRecordByQueryAsync<Artist>("artist=" + query);
        }

        public Task<Album> GetAlbumAsync(string query)
        {
            Database.ChangeContext("Albums");
            return Database.GetRecordByQueryAsync<Album>("album=" + query);
        }
        public Artist GetArtistByIdAsync(long id)
        {
            Database.ChangeContext("Artists");
            return Database.GetRecordById<Artist>(id);
        }

        public Album GetAlbumByIdAsync(long id)
        {
            Database.ChangeContext("Albums");
            return Database.GetRecordById<Album>(id);
        }
        public Task DeleteAlbumAsync(Album album)
        {
            Database.ChangeContext("Albums");
            return Database.RemoveRecord(album);
        }
        public Task<IEnumerable<Artist>> GetArtistsAsync()
        {
            Database.ChangeContext("Artists");
            return Database.GetRecords<Artist>();
        }
        public Task<IEnumerable<Artist>> GetRangeOfArtistsAsync(int index, int limit)
        {
            Database.ChangeContext("Artists");
            return Database.GetRangeOfRecords<Artist>(index, limit);
        }
        public Task<IEnumerable<Album>> GetRangeOfAlbumsAsync(int index, int limit)
        {
            Database.ChangeContext("Albums");
            return Database.GetRangeOfRecords<Album>(index, limit);
        }
        public Task<IEnumerable<Album>> GetAlbumsAsync()
        {
            Database.ChangeContext("Albums");
            return Database.GetRecords<Album>();
        }

        public Task<IEnumerable<Artist>> QueryArtistsAsync(string term, int limit = int.MaxValue)
        {
            Database.ChangeContext("Artists");
            return Database.QueryRecords<Artist>(term, limit);
        }

        public Task<IEnumerable<Album>> QueryAlbumsAsync(string term, int limit = int.MaxValue)
        {
            Database.ChangeContext("Albums");
            return Database.QueryRecords<Album>(term, limit);
        }

        public Task UpdateArtistAsync(Artist artist)
        {
            Database.ChangeContext("Artists");
            return Database.UpdateRecordAsync(artist, artist.Id);
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}