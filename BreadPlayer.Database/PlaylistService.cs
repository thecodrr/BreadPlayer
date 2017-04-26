using BreadPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BreadPlayer.Database
{
	public class PlaylistService : ChildSongsService, IDisposable
    {
        private IDatabaseService Database
        {
            get; set;
        }
        public PlaylistService(IDatabaseService database) : base(database, "PlaylistSongs", "PlaylistSongsText")
        {
            Database = database;
        }
        public async Task AddPlaylistAsync(Playlist pList)
        {
            Database.ChangeTable("Playlists", "PlaylistsText");
            await Database.InsertRecord(pList);
        }
        public async Task<IEnumerable<Playlist>> GetPlaylistsAsync()
        {
            Database.ChangeTable("Playlists", "PlaylistsText");
            return await Database.GetRecords<Playlist>();
        }
        public async Task<Playlist> GetPlaylistAsync(string query)
        {
            Database.ChangeTable("Playlists", "PlaylistsText");
            return await Database.GetRecordByQueryAsync<Playlist>(query);
        }  
        public bool PlaylistExists(string query)
        {
            return Database.CheckExists(query);
        }
        public async Task RemovePlaylistAsync(Playlist List)
        {
            Database.ChangeTable("Playlists", "PlaylistsText");
            await Database.RemoveRecord(List);
        }
        public async Task UpdatePlaylistAsync(Playlist list)
        {
            Database.ChangeTable("Playlists", "PlaylistsText");
            await Database.UpdateRecordAsync(list, list.Id);
        }

        //PlaylistSongs Methods
       
        
        public void Dispose()
        {
            Database.Dispose();
        }
    }   
}
