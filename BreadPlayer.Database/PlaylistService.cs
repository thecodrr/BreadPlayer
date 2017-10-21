using BreadPlayer.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BreadPlayer.Database
{
    public class PlaylistService : ChildSongsService, IDisposable
    {
        private IDatabaseService Database
        {
            get; set;
        }

        public PlaylistService(IDatabaseService database) : base(database, "PlaylistSongs")
        {
            Database = database;
        }

        public async Task AddPlaylistAsync(Playlist pList)
        {
            Database.ChangeContext("Playlists");
            await Database.InsertRecord(pList);
        }

        public Task<IEnumerable<Playlist>> GetPlaylistsAsync()
        {
            Database.ChangeContext("Playlists");
            return Database.GetRecords<Playlist>();
        }

        public Task<Playlist> GetPlaylistAsync(string query)
        {
            Database.ChangeContext("Playlists");
            return Database.GetRecordByQueryAsync<Playlist>(query);
        }
        public Playlist GetPlaylistByIdAsync(long Id)
        {
            Database.ChangeContext("Playlists");
            return Database.GetRecordById<Playlist>(Id);
        }
        public bool PlaylistExists(string query)
        {
            Database.ChangeContext("Playlists");
            return Database.CheckExists(query);
        }

        public async Task RemovePlaylistAsync(Playlist list)
        {
            Database.ChangeContext("Playlists");
            await Database.RemoveRecord(list);
        }

        public async Task UpdatePlaylistAsync(Playlist list)
        {
            Database.ChangeContext("Playlists");
            await Database.UpdateRecordAsync(list, list.Id);
        }

        //PlaylistSongs Methods

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}