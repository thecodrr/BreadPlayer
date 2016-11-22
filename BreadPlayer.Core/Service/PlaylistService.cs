using BreadPlayer.Models;
using LiteDB;
using LiteDB.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            db = new LiteDatabase("filename=" + ApplicationData.Current.LocalFolder.Path + @"\playlists\" + Name + ".db;journal=false;");
            IsValid = db.DbVersion.ToString() != "";
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
            LitePlatform.Initialize(new LitePlatformWindowsStore());
            Name = name;
            CreateDB();
        }

        public new void Dispose()
        {
            db.Dispose();
        }
    }
}
