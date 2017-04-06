using BreadPlayer.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using Windows.Storage;

namespace BreadPlayer.Service
{
	public class PlaylistService : IDisposable
    {
        LiteCollection<Mediafile> tracks;
        bool isValid;
        public bool IsValid { get { return isValid; } set { isValid = value; } }
        public string Name { get; set; }
        public string Password { get; set; }
        private PlaylistDatabase PDatabase
        {
            get; set;
        }
        public void CreateDB()
        {
            if (PDatabase != null)
            {
                IsValid = PDatabase.DB.Engine != null;
                if (IsValid)
                {
                    tracks = PDatabase.DB.GetCollection<Mediafile>("songs");
                    tracks.EnsureIndex(t => t.Title);
                    tracks.EnsureIndex(t => t.LeadArtist);
                }
            }
        }
        public void Remove(Mediafile file)
        {
            tracks.Delete(file._id);
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
        public PlaylistService(string name, bool isPrivate, string hash)
        {
            Name = name;
            Password = hash;
            PDatabase = new PlaylistDatabase("filename=" + string.Format(ApplicationData.Current.LocalFolder.Path + @"\playlists\{0}.db;{1}",Name, isPrivate ? "password=" + Password + ";" : ""));
            CreateDB();
        }
        public void Dispose()
        {
            PDatabase.DB.Dispose();
            PDatabase = null;
        }
    }
    public class PlaylistDatabase
    {
        LiteDatabase db;
        string ConnectionString;
        public LiteDatabase DB
        {
            get
            {
                return db;
            }
            set { db = value; }
        }
        public PlaylistDatabase(string connectionString)
        {
            System.IO.Directory.CreateDirectory(ApplicationData.Current.LocalFolder.Path + @"\playlists\");

            ConnectionString = connectionString;
            if (db == null && ConnectionString != null)
            {
                db = new LiteDatabase(ConnectionString);
            }
        }
    }
}
