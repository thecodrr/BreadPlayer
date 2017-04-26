using BreadPlayer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BreadPlayer.Database
{
	public interface IDatabaseService : IDisposable
    {
        void ChangeTable(string tableName, string textTableName);
        void CreateDB(string dbPath);
        Task InsertRecord(IDBRecord record);
        Task InsertRecords(IEnumerable<IDBRecord> records);
        Task<IEnumerable<T>> GetRecords<T>();
        Task<IEnumerable<T>> GetRecords<T>(long fromID, long toID);
        Task<T> GetRecordByQueryAsync<T>(string query);
        T GetRecordById<T>(long id);
        Task RemoveRecords(IEnumerable<IDBRecord> records);
        Task RemoveRecord(IDBRecord record);
        void UpdateRecords(IEnumerable<IDBRecord> records);
        Task<bool> UpdateRecordAsync<T>(T record, long id);
        Task<IEnumerable<T>> QueryRecords<T>(string term);
        int GetRecordsCount();
        bool CheckExists(long id);
        bool CheckExists(string query);
    }
}
