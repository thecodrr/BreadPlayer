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
        public void AddPlaylist(Playlist pList)
        {
            Database.ChangeTable("Playlists", "PlaylistsText");
            Database.InsertRecord(pList);
        }
        public async Task<IEnumerable<Playlist>> GetPlaylists()
        {
            Database.ChangeTable("Playlists", "PlaylistsText");
            return await Database.GetRecords<Playlist>();
        }
        public async Task<Playlist> GetPlaylistAsync(string query)
        {
            Database.ChangeTable("Playlists", "PlaylistsText");
            return (Playlist)await Database.GetRecordByQueryAsync(query);
        }        
        public async Task RemovePlaylistAsync(Playlist List)
        {
            Database.ChangeTable("Playlists", "PlaylistsText");
            await Database.RemoveRecord(List);
        }
        public async Task UpdatePlaylistAsync(Playlist list)
        {
            Database.ChangeTable("Playlists", "PlaylistsText");
            await Database.UpdateRecordAsync(list);
        }

        //PlaylistSongs Methods
       
        
        public void Dispose()
        {
            Database.Dispose();
        }
    }   
}
