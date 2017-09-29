using BreadPlayer.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BreadPlayer.Database
{
    public interface ILibraryService : IDisposable
    {
        /// <summary>
        /// Return a list of Customers' List Data filtered by State
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Mediafile>> GetAllMediafiles();

        Task<IEnumerable<Mediafile>> Query(string term, int limit = int.MaxValue);

        ///<summary>
        ///Update a customer in the data store
        ///</summary>
        ///<param name="?"></param>
        Task<bool> UpdateMediafile(Mediafile data);

        void UpdateMediafiles<T>(IEnumerable<Mediafile> data);

        void AddMediafile(Mediafile data);

        Task AddMediafiles(IEnumerable<Mediafile> data);

        void RemoveFolder(string folderPath);

        Task RemoveMediafile(Mediafile data);

        Mediafile GetMediafile(long id);

        bool CheckExists(long id);

        int SongCount { get; }
    }
}