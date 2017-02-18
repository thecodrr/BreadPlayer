using BreadPlayer.Models;
using LiteDB;
using System;
using Windows.Storage;

namespace BreadPlayer.Service
{
	public class PlaylistService : DatabaseService, IDatabaseService
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public PlaylistDatabase PDatabase
        {
            get;set;
        }
        public override async void CreateDB()
        {
            if (PDatabase != null)
            {
                using (PDatabase.DB)
                {
                    System.IO.Directory.CreateDirectory(ApplicationData.Current.LocalFolder.Path + @"\playlists\");
                    IsValid = PDatabase.DB.Engine != null;
                    if (IsValid)
                    {
                        tracks = PDatabase.DB.GetCollection<Mediafile>("songs");
                        tracks.EnsureIndex(t => t.Title);
                        tracks.EnsureIndex(t => t.LeadArtist);
                    }
                    else
                    {
                        await (await StorageFile.GetFileFromPathAsync(ApplicationData.Current.LocalFolder.Path + @"\playlists\" + Name + ".db")).DeleteAsync(StorageDeleteOption.PermanentDelete);
                        CreateDB();
                    }
                }
            }
        }
        public PlaylistService(string name, bool isPrivate, string password)
        {
            Name = name;
            Password = password;
            PDatabase = new PlaylistDatabase("filename=" + string.Format(ApplicationData.Current.LocalFolder.Path + @"\playlists\{0}.db;{1};",Name, isPrivate ? "password=" + Password : ""));
            CreateDB();
        }

        public new void Dispose()
        {
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
            ConnectionString = connectionString;
            if (db == null && ConnectionString != null)
            {
                db = new LiteDatabase(ConnectionString);
            }
        }
    }
}
