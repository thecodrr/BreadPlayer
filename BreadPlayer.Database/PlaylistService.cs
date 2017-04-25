using BreadPlayer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BreadPlayer.Database
{
	public class PlaylistService : IDisposable
    {
        bool isValid;
        public bool IsValid { get { return isValid; } set { isValid = value; } }
        public string Name { get; set; }
        public string Password { get; set; }
        private IDatabaseService Database
        {
            get; set;
        }
        public PlaylistService(string name, bool isPrivate, string hash)
        {
            Name = name;
            Password = hash;
            PDatabase = new PlaylistDatabase("filename=" + string.Format(ApplicationData.Current.LocalFolder.Path + @"\playlists\{0}.db;{1}", Name, isPrivate ? "password=" + Password + ";" : ""));
            CreateDB();
        }
        public void AddPlaylist(Playlist pList)
        {
            Database.InsertRecord(pList);
        }
        public async Task<IEnumerable<Playlist>> GetPlaylists()
        {
            return await Database.GetRecords<Playlist>("Playlists");
        }
        public Playlist GetPlaylist(string name)
        {
            return Database.GetRecord<Playlist>("Playlists", name);
        }
        public bool CheckExists<T>(string table, string path)
        {
            return Database.CheckExists<T>(table, path);
        }
        public void RemovePlaylist(Playlist List)
        {
            Database.RemoveRecord("Playlists", List.Name);
        }
        public void Remove(Mediafile file)
        {
            tracks.Delete(file._id);
        }

        public bool Exists(string path)
        {
          return tracks.Exists(t => t.Path == path);
        }
        public IEnumerable<Mediafile> GetTracks()
        {
            IEnumerable<Mediafile> collection = null;
            collection = tracks.Find(LiteDB.Query.All());
            return collection;
        }
        public void Insert(Mediafile file)
        {
            tracks.Insert(file);
        }
        public void Insert(IEnumerable<Mediafile> fileCol)
        {
            tracks.Insert(fileCol);
        }
        public LiteCollection<T> GetCollection<T>(string colName) where T : new()
        {
            return PDatabase.DB.GetCollection<T>(colName);
        }
        
        public void Dispose()
        {
            PDatabase.DB.Dispose();
            PDatabase = null;
        }
    }   
}
