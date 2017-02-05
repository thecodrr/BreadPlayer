using System.Collections.Generic;
using BreadPlayer.Models;
using System.Threading.Tasks;
using BreadPlayer.Common;

namespace BreadPlayer.Service
{
	/// <summary>
	/// Provide services for retrieving and storing Customer information
	/// </summary>
	public class LibraryService : ILibraryService
    {

        private IDatabaseService Database
        {
            get;
            set;
        }

        public LibraryService(IDatabaseService service)
        {
            Database = service;
        }

        #region ILibraryService 
        public async Task<IEnumerable<Mediafile>> Query(string field, object term)
        {
            return await Database.Query(field, term);
        }
        public async Task<IEnumerable<Mediafile>> GetAllMediafiles()
        {
            return await Database.GetTracks().ConfigureAwait(false);
        }
        public async Task<IEnumerable<Mediafile>> GetRangeOfMediafiles(int skip, int limit)
        {
            return await Database.GetRangeOfTracks(skip, limit).ConfigureAwait(false);
        }
        public void AddMediafile(Mediafile data)
        {
            Database.Insert(data);
        }
        public void AddMediafiles(IEnumerable<Mediafile> data)
        {
            Database.Insert(data);
        }
        public void UpdateMediafile(Mediafile data)
        {
            Database.UpdateTrack(data);
        }
        public void UpdateMediafiles(IEnumerable<Mediafile> data)
        {
            Database.UpdateTracks(data);
        }
        public void RemoveFolder(string folderPath)
        {
            Database.RemoveTracks(LiteDB.Query.EQ("FolderPath", folderPath));
            // Core.CoreMethods.LibVM.TracksCollection.Elements.RemoveRange(Core.CoreMethods.LibVM.TracksCollection.Elements.Where(t => t.FolderPath == folderPath));
        }
        public void RemoveMediafile(Mediafile data)
        {
            Database.Remove(data);
        }
        public void GetMediafile(string path)
        {
            Database.FindOne(path);
        }
        public void AddPlaylist(Playlist pList)
        {
            Database.GetCollection<Playlist>("playlists").Insert(pList);
        }
        public LiteDB.LiteCollection<Mediafile> GetRecentCollection()
        {
            return Database.GetCollection<Mediafile>("recents");
        }
        public IEnumerable<Playlist> GetPlaylists()
        {
            return Database.GetCollection<Playlist>("playlists").FindAll();
        }
        public bool CheckExists<T>(LiteDB.Query query, ICollection collection) where T : new()
        {
            return Database.GetCollection<T>(collection.Name).Exists(query);
        }
        public void RemovePlaylist(Playlist List)
        {
            Database.GetCollection<Playlist>("playlists").Delete(t => t.Name == List.Name);
        }
        public int SongCount
        {
            get { return Database.GetTrackCount(); }
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            Database.Dispose();
        }
        #endregion
    }
}