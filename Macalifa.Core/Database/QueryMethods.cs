using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using LiteDB.Platform;
using Windows.Storage;
using Macalifa.Models;
namespace Macalifa.Database
{
    public class QueryMethods : IDisposable
    {
        LiteDatabase db;
        LiteCollection<Mediafile> tracks;
        public QueryMethods()
        {
            LitePlatform.Initialize(new LitePlatformWindowsStore());
            CreateDB();
        }

        public  void CreateDB()
        {
            db = new LiteDatabase(ApplicationData.Current.LocalFolder.Path + @"\library.db");
            tracks = db.GetCollection<Mediafile>("tracks");
        }
        public void Insert(ObservableRangeCollection<Mediafile> fileCol)
        {           
            tracks.Insert(fileCol);                
        }
        public void Insert(Mediafile file)
        {            
            tracks.Insert(file);
        }
        public IEnumerable<Mediafile> GetTracks()
        {
            return tracks.FindAll();
        }
        public IEnumerable<Mediafile> PlaylistSort(string PlaylistName)
        {
            var found =tracks.Find(t => t.playlists != null);           
            var newFound = found.Where(a => a.playlists.All(t => t.Name == PlaylistName) && a.playlists.Count == 1);
            return newFound ;
        }
        public void Update(Mediafile file)
        {
            tracks.Update(file);
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}
