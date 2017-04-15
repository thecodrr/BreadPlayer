using BreadPlayer.Models;
using DBreeze;
using DBreeze.Objects;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BreadPlayer.Service
{
	public interface IDatabaseService : IDisposable
    {
        void InsertAlbums(IEnumerable<Album> albums);
        void CreateDB(string dbPath = null, bool createNew = true);
        void InsertRecord<T>(string tableName, List<DBreezeIndex> indexes, T record);
        Task InsertTracks(IEnumerable<Mediafile> records);
        Task<IEnumerable<T>> GetRecords<T>(string tableName);
        T GetRecord<T>(string table, string path);
        void RemoveRecord(string tableName, string key);
        void UpdateTracks(IEnumerable<Mediafile> records);
        Task<bool> UpdateRecordAsync<T>(string tableName, string primaryKey, T record);
        Task<IEnumerable<T>> QueryRecords<T>(string tableName, string term);
        int GetRecordsCount(string tableName);
        bool CheckExists<T>(string table, string path);        
    }
}
