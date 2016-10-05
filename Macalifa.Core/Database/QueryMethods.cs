/* 
	Macalifa. A music player made for Windows 10 store.
    Copyright (C) 2016  theweavrs (Abdullah Atta)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
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
        public LiteCollection<Playlist> playlists;
        public QueryMethods()
        {
            LitePlatform.Initialize(new LitePlatformWindowsStore());
            CreateDB();
        }

        public void CreateDB()
        {
            db = new LiteDatabase(ApplicationData.Current.LocalFolder.Path + @"\library.db");
            tracks = db.GetCollection<Mediafile>("tracks");
            playlists = db.GetCollection<Playlist>("playlists");
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
            var found =tracks.Find(t => t.Playlists != null);           
            var newFound = found.Where(a => a.Playlists.All(t => t.Name == PlaylistName) && a.Playlists.Count == 1);
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
