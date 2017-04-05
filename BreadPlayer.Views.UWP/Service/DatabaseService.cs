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
using System.Threading.Tasks;
using LiteDB;
using Windows.Storage;
using BreadPlayer.Models;
using System.Diagnostics;

namespace BreadPlayer.Service
{
    public class DatabaseService : IDatabaseService
    {
        protected LiteCollection<Mediafile> tracks;
        LiteCollection<Playlist> playlists;
        LiteCollection<Mediafile> recent;
        public DatabaseService()
        {
            CreateDB();
        }
        bool isValid;
        public bool IsValid { get { return isValid; } set { isValid = value; } }

        public virtual void CreateDB()
        {
            try
            {
                IsValid = StaticDatabase.DB.Engine != null;
                if (IsValid)
                {
                    tracks = StaticDatabase.DB.GetCollection<Mediafile>("tracks");
                    playlists = StaticDatabase.DB.GetCollection<Playlist>("playlists");
                    recent = StaticDatabase.DB.GetCollection<Mediafile>("recent");
                    tracks.EnsureIndex(t => t.Title);
                    tracks.EnsureIndex(t => t.LeadArtist);
                }              
                GetTrackCount();
            }
            catch (Exception)
            {
            }
        }
        public LiteCollection<T> GetCollection<T>(string colName) where T : new()
        {
            return StaticDatabase.DB.GetCollection<T>(colName);
        }
        public void Insert(IEnumerable<Mediafile> fileCol)
        {
            try
            {
                tracks.Insert(fileCol);
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message + "|" + fileCol.Count()); }
        }
        public int GetTrackCount()
        {
            try
            {
                if (StaticDatabase.DB != null && this != null && tracks != null)
                    return tracks.Count();
            }
            catch { }
            return 0;
        }
        public Mediafile FindOne(string path)
        {
           return tracks.FindOne(t => t.Path == path);
        }
        public void Remove(Mediafile file)
        {
            tracks.Delete(file._id);
        }
        public void Insert(Mediafile file)
        {
            tracks.Insert(file);
        }
        public void RemoveTracks(Query query)
        {
            tracks.Delete(query);
        }
        public IEnumerable<Mediafile> GetTracks()
        {
            IEnumerable<Mediafile> collection = null;
            collection = tracks.Find(LiteDB.Query.All());
            return collection;
        }

        public async Task<IEnumerable<Mediafile>> GetRangeOfTracks(int skip, int limit)
        {
            IEnumerable<Mediafile> collection = null;
            await Core.SharedLogic.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                collection = tracks.Find(LiteDB.Query.All(), skip, limit);
            });
            return collection;
        }
        public void UpdateTrack(Mediafile file)
        {
            if (file != null && tracks.Exists(t => t.Path == file.Path))
            {
                tracks.Update(file);
            }
        }
        public void UpdateTracks(IEnumerable<Mediafile> files)
        {
            if (files != null)
            {
                tracks.Update(files);
            }
        }
        public IEnumerable<Mediafile> Query(string field, object term)
        {
            IEnumerable<Mediafile> collection = null;
            collection = tracks.Find(LiteDB.Query.Contains(field, term.ToString()));//tracks.Find(x => x.Title.Contains(term) || x.LeadArtist.Contains(term));
            return collection;
        }
        public void Dispose()
        {
            StaticDatabase.DB.Dispose();
            StaticDatabase.DB = null;
        }
    }
    public static class StaticDatabase
    {
        static LiteDatabase db;
        static IDiskService service = new FileDiskService(ApplicationData.Current.LocalFolder.Path + @"\breadplayer.db");
        public static LiteDatabase DB
        {
            get
            {
                if (db == null)
                {
                    db = new LiteDatabase(service);
                }
                return db;
            }
            set
            {
                db = value;
            }
        }
    }
}
