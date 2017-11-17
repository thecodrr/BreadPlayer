using BreadPlayer.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public Task<IEnumerable<Mediafile>> Query(string term, int limit = int.MaxValue)
        {
            return Database.QueryRecords<Mediafile>(string.Format("{0}&{1}&{2}", term, term, term), limit);
        }

        public Task<IEnumerable<Mediafile>> GetAllMediafiles()
        {
            return Database.GetRecords<Mediafile>();
        }

        public Task<IEnumerable<Mediafile>> GetRangeOfMediafiles(int index, int limit)
        {
            return Database.GetRangeOfRecords<Mediafile>(index, limit);
        }

        public void AddMediafile(Mediafile data)
        {
            Database.InsertRecord(data);
        }

        public async Task AddMediafiles(IEnumerable<Mediafile> data)
        {
            await Database.InsertRecords(data);
        }

        public Task<bool> UpdateMediafile(Mediafile data)
        {
            return Database.UpdateRecordAsync(data, data.Id);
        }

        public void UpdateMediafiles<T>(IEnumerable<Mediafile> data)
        {
            Database.UpdateRecords<T>(data);
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
            return Database.GetRecordById<Mediafile>(id);
        }

        public bool CheckExists(long id)
        {
            return Database.CheckExists(id);
        }

        public int SongCount => Database.GetRecordsCount();

        #endregion ILibraryService

        #region IDisposable

        public void Dispose()
        {
            Database.Dispose();
        }

        #endregion IDisposable
    }
}