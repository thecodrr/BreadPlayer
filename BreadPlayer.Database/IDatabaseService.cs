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
        void InsertRecord(IDBRecord record);
        Task InsertRecords(IEnumerable<IDBRecord> records);
        Task<IEnumerable<T>> GetRecords<T>();
        IDBRecord GetRecord(long id);
        Task RemoveRecords(IEnumerable<IDBRecord> records);
        Task RemoveRecord(IDBRecord record);
        void UpdateRecords(IEnumerable<IDBRecord> records);
        Task<bool> UpdateRecordAsync(IDBRecord record);
        Task<IEnumerable<T>> QueryRecords<T>(string term);
        int GetRecordsCount();
        bool CheckExists(IDBRecord record);
    }
}
