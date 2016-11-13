/* 
	BreadPlayer. A music player made for Windows 10 store.
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
using BreadPlayer.Models;
using System.Diagnostics;

namespace BreadPlayer.Database
{
    public class DatabaseQueryMethods : IDisposable
    {
        LiteDatabase db;
        public LiteCollection<Mediafile> tracks;
        public LiteCollection<Playlist> playlists;
        public LiteCollection<Mediafile> recent;
        public DatabaseQueryMethods()
        {
            LitePlatform.Initialize(new LitePlatformWindowsStore());
            CreateDB();
        }

        public void CreateDB()
        {
            db = new LiteDatabase("filename=" + ApplicationData.Current.LocalFolder.Path + @"\breadplayer.db;journal=false;");            
            tracks = db.GetCollection<Mediafile>("tracks");
            playlists = db.GetCollection<Playlist>("playlists");
            recent = db.GetCollection<Mediafile>("recent");         
            tracks.EnsureIndex(t => t.Title);
            tracks.EnsureIndex(t => t.LeadArtist);
        }
        public void CreatePlaylistDB(string name)
        {
            System.IO.Directory.CreateDirectory(ApplicationData.Current.LocalFolder.Path + @"\playlists\");
            db = new LiteDatabase("filename=" + ApplicationData.Current.LocalFolder.Path + @"\playlists\" + name + ".db;journal=false;");
            tracks = db.GetCollection<Mediafile>("songs");
            tracks.EnsureIndex(t => t.Title);
            tracks.EnsureIndex(t => t.LeadArtist);
        }
        public void Insert(IEnumerable<Mediafile> fileCol)
        {
            try
            {
                tracks.Insert(fileCol);
            }  
            catch(Exception ex) { Debug.WriteLine(ex.Message + "|" + fileCol.Count()); }        
        }
        public void Remove(Mediafile file)
        {
            tracks.Delete(file._id);
        }
        public void Insert(Mediafile file)
        {            
            tracks.Insert(file);
        }
        public void RemoveFolder(string folderPath)
        {
            tracks.Delete(LiteDB.Query.EQ("FolderPath", folderPath));
            Core.CoreMethods.LibVM.TracksCollection.Elements.RemoveRange(Core.CoreMethods.LibVM.TracksCollection.Elements.Where(t => t.FolderPath == folderPath));
        }
        public async Task<IEnumerable<Mediafile>> GetTracks()
        {     
            IEnumerable<Mediafile> collection = null;
            await Core.CoreMethods.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Core.CoreMethods.LibVM.SongCount = tracks.Count();
                collection = tracks.Find(LiteDB.Query.All());
            });

            return collection;
        }
        public void Update(Mediafile file, bool updateWithMatch = false)
        {
            if (file != null && tracks.Exists(t => t.Path == file.Path))
            {
                if (updateWithMatch == false)
                    tracks.Update(file);
                else
                {
                    Mediafile mp3 = tracks.FindOne(t => t.Path == file.Path);
                    mp3.Playlists.Clear();
                    mp3.Playlists.AddRange(file.Playlists);
                    tracks.Update(mp3);
                }
            }
        }
        public async Task<IEnumerable<Mediafile>> Query(string term)
        {
            IEnumerable<Mediafile> collection = null;
            await Core.CoreMethods.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                collection = tracks.Find(LiteDB.Query.Contains("Title", term));//tracks.Find(x => x.Title.Contains(term) || x.LeadArtist.Contains(term));
            });
            return collection;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}
