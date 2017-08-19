using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BreadPlayer.Core.Common;
using LiteDB;
using System.Linq;

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
        LiteDatabase DB { get; set; }
        LiteCollection<IDbRecord> currentCollection;
        public DocumentStoreDatabaseService(string dbPath, string collectionName)
        {
            CreateDb(dbPath.ToLower() + ".db");
            currentCollection = DB.GetCollection<IDbRecord>(collectionName);
            currentCollection.EnsureIndex(t => t.Id);
            currentCollection.EnsureIndex(t => t.TextSearchKey);
        }

        public void CreateDb(string dbPath)
        {
            DB = StaticDocumentDatabase.GetDatabase(dbPath);
        }
        public void ChangeTable(string tableName, string textTableName)
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
                var dbRecord = currentCollection.FindOne(t => t.TextSearchKey.Contains(query));
                if (dbRecord != null)
                   return (T)dbRecord;
                return default(T);
            });
        }

        public async Task<IEnumerable<T>> GetRecords<T>()
        {
            return await Task.Run(() =>
            {
                var records = currentCollection.Find(Query.All());
                if (records.Any())
                    return records.Cast<T>();
                else
                    return null;
            }).ConfigureAwait(false);
        }

        public Task<IEnumerable<T>> GetRecords<T>(long fromId, long toId)
        {
            throw new NotImplementedException();
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
                using (var trans = DB.BeginTrans())
                {
                    try
                    {
                        foreach (var record in records.ToList())
                        {
                            record.Id = Guid.NewGuid().GetHashCode();
                            currentCollection.Insert(record);
                        }

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                    }
                }
            });
        }

        public async Task<IEnumerable<T>> QueryRecords<T>(string term, System.Linq.Expressions.Expression<Func<IDbRecord, bool>> filterFunc = null)
        {
            return await Task.Run(() =>
            {
                var records = currentCollection.Find(filterFunc ?? (t => t.TextSearchKey.Contains(term.ToLower())));
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
                using (var trans = DB.BeginTrans())
                {
                    try
                    {
                        foreach (var record in records)
                        {
                            currentCollection.Delete(record.Id);
                        }

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                    }
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

        public void UpdateRecords<T>(IEnumerable<IDbRecord> records)
        {
            throw new NotImplementedException();
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
