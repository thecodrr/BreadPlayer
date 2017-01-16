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
        public override async void CreateDB()
        {
            System.IO.Directory.CreateDirectory(ApplicationData.Current.LocalFolder.Path + @"\playlists\");
            var disk = new FileDiskService(ApplicationData.Current.LocalFolder.Path + @"\playlists\" + Name + ".db", new FileOptions() { FileMode = FileMode.Exclusive, Journal = true });
            db = new LiteDatabase(disk);
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
        public PlaylistService(string name)
        {
            Name = name;
            CreateDB();
        }

        public new void Dispose()
        {
            db.Dispose();
        }
    }
}
