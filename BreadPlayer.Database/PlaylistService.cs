using BreadPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BreadPlayer.Database
{
	public class PlaylistService : IDisposable
    {
        private IDatabaseService Database
        {
            get; set;
        }
        public PlaylistService(IDatabaseService database)
        {
            Database = database;
        }
        public void AddPlaylist(Playlist pList)
        {
            Database.InsertRecord(pList);
        }
        public async Task<IEnumerable<Playlist>> GetPlaylists()
        {
            return await Database.GetRecords<Playlist>();
        }
        public Playlist GetPlaylist(long id)
        {
            return (Playlist)Database.GetRecord(id);
        }        
        public async Task RemovePlaylistAsync(Playlist List)
        {
            await Database.RemoveRecord(List);
        }
        public async Task UpdatePlaylistAsync(Playlist list)
        {
            await Database.UpdateRecordAsync(list);
        }

        //PlaylistSongs Methods
        public async Task Insert(IEnumerable<Mediafile> fileCol, Playlist pList)
        {
            List<PlaylistSong> PlaylistSongs = new List<PlaylistSong>();
            foreach(var file in fileCol)
            {
                PlaylistSongs.Add(new PlaylistSong()
                {
                    SongId = file.Id,
                    PlaylistId = pList.Id
                });
            }
            Database.ChangeTable("PlaylistSongs", "PlaylistSongsText");
            await  Database.InsertRecords(PlaylistSongs);
        }
        public void InsertSong(PlaylistSong file)
        {
            Database.ChangeTable("PlaylistSongs", "PlaylistSongsText");
            Database.InsertRecord(file);
        }
        public async Task RemoveSongAsync(PlaylistSong file)
        {
            Database.ChangeTable("PlaylistSongs", "PlaylistSongsText");
            await Database.RemoveRecord(file);
        }
        public bool Exists(long id)
        {
            Database.ChangeTable("PlaylistSongs", "PlaylistSongsText");
            return Database.CheckExists(id);
        }
        public async Task<IEnumerable<Mediafile>> GetTracksAsync(long playlistID)
        {
            return await Task.Run(async() =>
            {
                Database.ChangeTable("PlaylistSongs", "PlaylistSongsText");
                var trackIds = (await Database.QueryRecords<PlaylistSong>(playlistID.ToString())).Select(t => t.SongId);

                Database.ChangeTable("Tracks", "TracksText");
                List<Mediafile> Tracks = new List<Mediafile>();
                foreach (var id in trackIds)
                {
                    Tracks.Add((Mediafile)Database.GetRecord(id));
                }
                return Tracks;
            });
        }
        
        public void Dispose()
        {
            Database.Dispose();
        }
    }   
}
