using System.Collections.Generic;
using BreadPlayer.Models;
using System.Threading.Tasks;
using BreadPlayer.Common;
using System;

namespace BreadPlayer.Database
{
    public interface ILibraryService : IDisposable
    {
        /// <summary>
        /// Return a list of Customers' List Data filtered by State
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Mediafile>> GetAllMediafiles();
        Task<IEnumerable<Mediafile>> Query(string term);
        ///<summary>
        ///Update a customer in the data store
        ///</summary>
        ///<param name="?"></param>
        Task<bool> UpdateMediafile(Mediafile data);
        void UpdateMediafiles(IEnumerable<Mediafile> data);
        void AddMediafile(Mediafile data);
        Task AddMediafiles(IEnumerable<Mediafile> data);
        void RemoveFolder(string folderPath);
        Task RemoveMediafile(Mediafile data);
        Mediafile GetMediafile(string path);
        void RemovePlaylist(Playlist List);
        bool CheckExists<T>(string table, string path);
        void AddPlaylist(Playlist pList);
        Task<IEnumerable<Playlist>> GetPlaylists();
        Playlist GetPlaylist(string name);
        int SongCount { get; }
    }
}
