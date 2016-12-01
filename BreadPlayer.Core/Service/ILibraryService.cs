using System.Collections.Generic;
using BreadPlayer.Models;
using System.Threading.Tasks;
using BreadPlayer.Common;
using System;

namespace BreadPlayer.Service
{
    public interface ILibraryService : IDisposable
    {
        /// <summary>
        /// Return a list of Customers' List Data filtered by State
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Mediafile>> GetAllMediafiles();
        Task<IEnumerable<Mediafile>> Query(string field, object term);
        /// <summary>
        /// Update a customer in the data store
        /// </summary>
        /// <param name="?"></param>
        void UpdateMediafile(Mediafile data);
        void UpdateMediafiles(IEnumerable<Mediafile> data);
        void AddMediafile(Mediafile data);
        void AddMediafiles(IEnumerable<Mediafile> data);
        void RemoveFolder(string folderPath);
        void RemoveMediafile(Mediafile data);
        void GetMediafile(string path);
        void RemovePlaylist(Playlist List);
        bool CheckExists<T>(LiteDB.Query query, ICollection collection) where T : new();
        void AddPlaylist(Playlist pList);
        IEnumerable<Playlist> GetPlaylists();
        int SongCount { get;}
    }
}