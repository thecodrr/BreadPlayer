using BreadPlayer.Core.Common;
using BreadPlayer.Interfaces;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BreadPlayer.Database
{
    public class StaticDocumentDatabase
    {
        private static string DbPath { get; set; }
        private static LiteDatabase _db;
        public static bool IsDisposed { get; set; }

        public static LiteDatabase GetDatabase(string dbPath)
        {
            if (_db == null || DbPath != dbPath)
            {
                DbPath = dbPath;
                _db = new LiteDatabase($"Filename={dbPath}; Journal=false; Async=true");
                IsDisposed = false;
            }
            return _db;
        }

        public static void DisposeDatabaseEngine()
        {
            _db.Dispose();
            _db = null;
            IsDisposed = true;
        }
    }

    public class DocumentStoreDatabaseService : IDatabaseService
    {
        private LiteDatabase DB { get; set; }
        private LiteCollection<IDbRecord> currentCollection;

        public DocumentStoreDatabaseService(string dbPath, string collectionName)
        {
            DB = StaticDocumentDatabase.GetDatabase(dbPath.ToLower() + ".db");
            currentCollection = DB.GetCollection<IDbRecord>(collectionName);
            currentCollection.EnsureIndex(t => t.Id);
            currentCollection.EnsureIndex(t => t.TextSearchKey);
        }

        public void ChangeContext(string tableName)
        {
            currentCollection = DB.GetCollection<IDbRecord>(tableName);
            currentCollection.EnsureIndex(t => t.Id);
            currentCollection.EnsureIndex(t => t.TextSearchKey);
        }

        public bool CheckExists(long id)
        {
            return currentCollection.Exists(t => t.Id == id);
        }

        public bool CheckExists(string query)
        {
            return currentCollection.Exists(t => t.TextSearchKey.Contains(query));
        }

        public T GetRecordById<T>(long id)
        {
            return (T)currentCollection.FindOne(t => t.Id == id);
        }

        public Task<T> GetRecordByQueryAsync<T>(string query)
        {
            return Task.Run(() =>
            {
                return (T)currentCollection.FindOne(t => t.TextSearchKey.Contains(query.ToLower()));
            });
        }

        public async Task<IEnumerable<T>> GetRecords<T>()
        {
            return await Task.Run(() =>
            {
                var records = currentCollection.FindAll();
                if (records.Any())
                    return records.Cast<T>();
                else
                    return null;
            }).ConfigureAwait(false);
        }

        public Task<IEnumerable<T>> GetRangeOfRecords<T>(long index, long limit)
        {
            return Task.Run(() =>
            {
                var records = currentCollection.Find(Query.All(), (int)index, (int)limit);
                if (records.Any())
                    return records.Cast<T>();
                else
                    return null;
            });
        }

        public int GetRecordsCount()
        {
            return currentCollection.Count();
        }

        public Task InsertRecord(IDbRecord record)
        {
            return Task.Run(() =>
            {
                record.Id = Guid.NewGuid().GetHashCode();
                currentCollection.Insert(record);
            });
        }

        public Task InsertRecords(IEnumerable<IDbRecord> records)
        {
            return Task.Run(() =>
            {
                currentCollection.InsertBulk(records);
            });
        }

        public async Task<IEnumerable<T>> QueryRecords<T>(string term, int limit = int.MaxValue)
        {
            return await Task.Run(() =>
            {
                var records = currentCollection.Find(t => t.TextSearchKey.Contains(term.ToLower()), 0, limit);
                if (records.Any())
                    return records.Cast<T>();
                else
                    return null;
            }).ConfigureAwait(false);
        }

        public Task RemoveRecord(IDbRecord record)
        {
            return Task.Run(() =>
            {
                currentCollection.Delete(record.Id);
            });
        }

        public Task RemoveRecords(IEnumerable<IDbRecord> records)
        {
            return Task.Run(() =>
            {
                foreach (var record in records)
                {
                    currentCollection.Delete(record.Id);
                }
            });
        }

        public Task<bool> UpdateRecordAsync<T>(T record, long id)
        {
            return Task.Run(() =>
            {
                try
                {
                    return currentCollection.Update(id, (IDbRecord)record);
                }
                catch
                {
                    return false;
                }
            });
        }

        public Task UpdateRecords<T>(IEnumerable<IDbRecord> records)
        {
            return Task.Run(() =>
            {
                currentCollection.Update(records);                
            });
        }

        public void Dispose()
        {
            StaticDocumentDatabase.DisposeDatabaseEngine();
            DB.Dispose();
            DB = null;
            currentCollection = null;
        }
    }
}