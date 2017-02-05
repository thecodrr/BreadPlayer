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
        LiteDatabase db;
        IDiskService service = new FileDiskService(ApplicationData.Current.LocalFolder.Path + @"\breadplayer.db", new FileOptions() { FileMode = FileMode.Exclusive, Journal = true });
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
        }

        public LiteCollection<Mediafile> tracks;
        public LiteCollection<Playlist> playlists;
        public LiteCollection<Mediafile> recent;
        public DatabaseService()
        {
        }
        bool isValid;
        public bool IsValid { get { return isValid; } set { isValid = value; } }

        public virtual async void CreateDB()
        {
            try
            {
                IsValid = DB.Engine != null;
                if (IsValid)
                {
                    tracks = db.GetCollection<Mediafile>("tracks");
                    playlists = db.GetCollection<Playlist>("playlists");
                    recent = db.GetCollection<Mediafile>("recent");
                    tracks.EnsureIndex(t => t.Title);
                    tracks.EnsureIndex(t => t.LeadArtist);
                }
                else
                {
                    await ApplicationData.Current.ClearAsync();
                    CreateDB();
                }
                GetTrackCount();
            }
            catch(Exception)
            {
                await ApplicationData.Current.ClearAsync();
                CreateDB();
            }
        }
        public LiteCollection<T> GetCollection<T>(string colName) where T : new()
        {
            using (DB)
                return DB.GetCollection<T>(colName);
        }
        public void Insert(IEnumerable<Mediafile> fileCol)
        {
            try
            {
                if (DB != null)
                    tracks.Insert(fileCol);
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message + "|" + fileCol.Count()); }        
        }
        public int GetTrackCount()
        {
            try
            {
                if (DB != null && this != null && tracks != null)
                    return tracks.Count();
                else
                    return 0;
            }
            catch
            {
                return 0;
            }
        }
        public void FindOne(string path)
        {
            using (DB)
                tracks.FindOne(t => t.Path == path);
        }
        public void Remove(Mediafile file)
        {
            using (DB)
                tracks.Delete(file._id);
        }
        public void Insert(Mediafile file)
        {
            using (DB)
                tracks.Insert(file);
        }
        public void RemoveTracks(Query query)
        {
            using (DB)
                tracks.Delete(query);
        }
        public async Task<IEnumerable<Mediafile>> GetTracks()
        {
            IEnumerable<Mediafile> collection = null;
            using (DB)
            {
                await Core.SharedLogic.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    collection = tracks.Find(LiteDB.Query.All());
                });
            }
            return collection;
        }

        public async Task<IEnumerable<Mediafile>> GetRangeOfTracks(int skip, int limit)
        {
            IEnumerable<Mediafile> collection = null;
            using (DB)
            {
                await Core.SharedLogic.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    collection = tracks.Find(LiteDB.Query.All(), skip, limit);
                });
            }
            return collection;
        }
        public void UpdateTrack(Mediafile file)
        {
            using (DB)
            {
                if (file != null && tracks.Exists(t => t.Path == file.Path))
                {
                    tracks.Update(file);
                }
            }
        }
        public void UpdateTracks(IEnumerable<Mediafile> files)
        {
            using (DB)
            {
                if (files != null)
                {
                    tracks.Update(files);
                }
            }
        }
        public async Task<IEnumerable<Mediafile>> Query(string field, object term)
        {
            IEnumerable<Mediafile> collection = null;
            using (DB)
            {
                await Core.SharedLogic.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    collection = tracks.Find(LiteDB.Query.Contains(field, term.ToString()));//tracks.Find(x => x.Title.Contains(term) || x.LeadArtist.Contains(term));
                });
            }
            return collection;
        }
        public void Dispose()
        {
            DB?.Dispose();
        }
    }
}
