using BreadPlayer.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Service
{
    public interface IDatabaseService : IDisposable
    {
        void CreateDB();
        Task<IEnumerable<Mediafile>> GetTracks();
        void UpdateTrack(Mediafile file);
        Task<IEnumerable<Mediafile>> Query(string field, string term);
        void RemoveTracks(Query query);
        void Insert(Mediafile file);
        void Insert(IEnumerable<Mediafile> files);
        LiteCollection<T> GetCollection<T>(string colName) where T : new();
        void Remove(Mediafile file);
        int GetTrackCount();
    }
}
