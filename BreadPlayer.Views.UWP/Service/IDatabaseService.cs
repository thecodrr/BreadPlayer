using BreadPlayer.Models;
using DBreeze.Objects;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BreadPlayer.Service
{
	public interface IDatabaseService : IDisposable
    {
        void CreateDB();
        void InsertRecord<T>(string tableName, List<DBreezeIndex> indexes, T record);
        void InsertTracks(IEnumerable<Mediafile> records);
        IEnumerable<T> GetRecords<T>(string tableName);
        T GetRecord<T>(string table, string path);
        void RemoveRecord(string tableName, string key);
        void UpdateTracks(IEnumerable<Mediafile> records);
        void UpdateRecord<T>(string tableName, string key, T record);
        IEnumerable<T> QueryRecords<T>(string tableName, string term);
        int GetRecordsCount(string tableName);
        bool CheckExists<T>(string table, string path);        
    }
}
