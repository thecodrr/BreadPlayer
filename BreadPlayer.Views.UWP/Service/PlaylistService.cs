using BreadPlayer.Models;
using LiteDB;
using System;
using Windows.Storage;

namespace BreadPlayer.Service
{
	public class PlaylistService : DatabaseService, IDatabaseService
    {
        public string Name { get; set; }
        LiteDatabase db;
        readonly string ConnectionString = ApplicationData.Current.LocalFolder.Path + @"\playlists\{0}.db;";
        IDiskService service;
        LiteDatabase DB
        {
            get
            {
                if (db == null)
                {
                    db = new LiteDatabase(service);
                    CreateDB();
                }
                return db;
            }
            set { db = value; }
        }
        public override async void CreateDB()
        {
            using (DB)
            {
                System.IO.Directory.CreateDirectory(ApplicationData.Current.LocalFolder.Path + @"\playlists\");
                IsValid = db.Engine != null;
                if (IsValid)
                {
                    tracks = db.GetCollection<Mediafile>("songs");
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
        public PlaylistService(string name)
        {
            Name = name;
            service = new FileDiskService(string.Format(ConnectionString, Name), new FileOptions() { FileMode = FileMode.Exclusive, Journal = true });
            CreateDB();
        }

        public new void Dispose()
        {
            db.Dispose();
            DB = null;
        }
    }
}
