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
        IEnumerable<Mediafile> GetAllMediafiles();
        IEnumerable<Mediafile> Query(string term);
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
        Mediafile GetMediafile(string path);
        void RemovePlaylist(Playlist List);
        bool CheckExists<T>(string table, string path);
        void AddPlaylist(Playlist pList);
        IEnumerable<Playlist> GetPlaylists();
        Playlist GetPlaylist(string name);
        int SongCount { get; }
    }
}
