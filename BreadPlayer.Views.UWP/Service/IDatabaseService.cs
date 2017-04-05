using BreadPlayer.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BreadPlayer.Service
{
	public interface IDatabaseService : IDisposable
    {
        void CreateDB();
        void InsertRecord<T>(string tableName, string key, T Value);
        void InsertTracks(IEnumerable<Mediafile> records);
        IEnumerable<T> GetRecords<T>(string tableName);
        T GetRecord<T>(string table, string path);
        void RemoveRecord(string tableName, string key);
        void RemoveRecords<T>(string tableName, string key, IEnumerable<T> records);
        void UpdateTracks(IEnumerable<Mediafile> records);
        void UpdateRecord<T>(string tableName, string key, T record);
        IEnumerable<T> QueryRecords<T>(string tableName, string term);
        int GetRecordsCount(string tableName);
        bool CheckExists<T>(string table, string path);        
    }
}
