using BreadPlayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BreadPlayer.Database
{
    public interface IDatabaseService : IDisposable
    {
        void ChangeContext(string context);        

        Task InsertRecord(IDbRecord record);

        Task InsertRecords(IEnumerable<IDbRecord> records);

        Task<IEnumerable<T>> GetRecords<T>();

        //Task<IEnumerable<T>> GetRecords<T>(long fromId, long toId);
        Task<T> GetRecordByQueryAsync<T>(string query);

        T GetRecordById<T>(long id);

        Task RemoveRecords(IEnumerable<IDbRecord> records);

        Task RemoveRecord(IDbRecord record);

        Task<IEnumerable<T>> GetRangeOfRecords<T>(long index, long limit);

        Task UpdateRecords<T>(IEnumerable<IDbRecord> records);

        Task<bool> UpdateRecordAsync<T>(T record, long id);

        Task<IEnumerable<T>> QueryRecords<T>(string term, int limit = int.MaxValue);

        int GetRecordsCount();

        bool CheckExists(long id);

        bool CheckExists(string query);
    }
}