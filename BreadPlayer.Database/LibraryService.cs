using System.Collections.Generic;
using BreadPlayer.Models;
using System.Threading.Tasks;
using BreadPlayer.Common;
using DBreeze.Objects;
using BreadPlayer.Database;

namespace BreadPlayer.Database
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
        public async Task<IEnumerable<Mediafile>> Query(string term)
        {
            return await Database.QueryRecords<Mediafile>(term);
        }
        public async Task<IEnumerable<Mediafile>> GetAllMediafiles()
        {
            return await Database.GetRecords<Mediafile>();
        }
        public void AddMediafile(Mediafile data)
        {
            Database.InsertRecord(data);
        }
        public async Task AddMediafiles(IEnumerable<Mediafile> data)
        {
            await Database.InsertRecords(data);
        }
        public async Task<bool> UpdateMediafile(Mediafile data)
        {
            return await Database.UpdateRecordAsync(data);
        }
        public void UpdateMediafiles(IEnumerable<Mediafile> data)
        {
            Database.UpdateRecords(data);
        }
        public void RemoveFolder(string folderPath)
        {
            //Database.RemoveTracks(LiteDB.Query.EQ("FolderPath", folderPath));
            // Core.CoreMethods.LibVM.TracksCollection.Elements.RemoveRange(Core.CoreMethods.LibVM.TracksCollection.Elements.Where(t => t.FolderPath == folderPath));
        }
        public async Task RemoveMediafile(Mediafile data)
        {
            await Database.RemoveRecord(data);
        }
        public async Task RemoveMediafiles(IEnumerable<Mediafile> data)
        {
            await Database.RemoveRecords(data);
        }
        public Mediafile GetMediafile(long id)
        {
            return (Mediafile)Database.GetRecord(id);
        }
      
        public int SongCount
        {
            get { return Database.GetRecordsCount("Tracks"); }
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
