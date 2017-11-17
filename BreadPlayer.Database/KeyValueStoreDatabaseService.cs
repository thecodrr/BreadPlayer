using BreadPlayer.Core.Common;
using BreadPlayer.Interfaces;
using DBreeze;
using DBreeze.Objects;
using DBreeze.TextSearch;
using DBreeze.Transactions;
using DBreeze.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BreadPlayer.Database
{
    public class StaticKeyValueDatabase
    {
        private static string DbPath { get; set; }
        private static DBreezeEngine _db;
        public static bool IsDisposed { get; set; }

        public static DBreezeEngine GetDatabaseEngine(string dbPath)
        {
            if (_db == null || DbPath != dbPath || _db.Disposed || !_db.IsDatabaseOperable)
            {
                BLogger.I("Initializing db engine. Path: {path}", dbPath);
                DbPath = dbPath;
                var dbConfig = new DBreezeConfiguration
                {
                    DBreezeDataFolderName = dbPath,
                    Storage = DBreezeConfiguration.eStorage.DISK
                };
                _db = new DBreezeEngine(dbConfig);
                IsDisposed = false;
                BLogger.I("Db engine initialized. Path: {path}", dbPath);
            }
            return _db;
        }

        public static void DisposeDatabaseEngine()
        {
            BLogger.I("Disposing db engine.");
            DbPath = null;
            _db.Dispose();
            _db = null;
            IsDisposed = true;
            BLogger.I("Db engine disposed.");
        }
    }

    public class KeyValueStoreDatabaseService : IDatabaseService
    {
        private DBreezeEngine _engine;
        private string _textTableName;
        private string _tableName;

        public KeyValueStoreDatabaseService(string dbPath, string tableName)
        {
            _engine = this.Initialize(dbPath);
            ChangeContext(tableName);
        }
        public void ChangeContext(string context)
        {
            _textTableName = context + "Text";
            _tableName = context;
            BLogger.I("Tables changed. Old table: {oldtable}; New table: {newtable}", _tableName, context);
        }

        public bool CheckExists(long id)
        {
            using (var tran = _engine.GetSafeTransaction())
            {
                var item = tran.Select<byte[], byte[]>(_tableName, 1.ToIndex(id));//.ObjectGet<T>().Entity;
                return item.Exists;
            }
        }

        public bool CheckExists(string query)
        {
            using (var tran = _engine.GetSafeTransaction())
            {
                var records = tran.TextSearch(_textTableName).Block(query).GetDocumentIDs();
                if (records.Any())
                {
                    return true;
                }
            }
            return false;
        }

        public void Dispose()
        {
            StaticKeyValueDatabase.DisposeDatabaseEngine();
            _engine = null;
        }

        public T GetRecordById<T>(long id)
        {
            try
            {
                using (var tran = _engine.GetSafeTransaction())
                {
                    var entity = tran.Select<byte[], byte[]>(_tableName, 1.ToIndex(id)).ObjectGet<T>();
                    if (entity != null)
                    {
                        return entity.Entity;
                    }

                    return default(T);
                }
            }
            catch
            {
                return default(T);
            }
        }

        public Task<T> GetRecordByQueryAsync<T>(string query)
        {
            return Task.Run(() =>
            {
                using (var tran = _engine.GetSafeTransaction())
                {
                    var records = tran.TextSearch(_textTableName).Block(query).GetDocumentIDs();
                    var enumerable = records as IList<byte[]> ?? records.ToList();
                    if (enumerable.Any())
                    {
                        var o = tran.Select<byte[], byte[]>(_tableName, 1.ToIndex(enumerable.First())).ObjectGet<T>();
                        return o.Entity;
                    }
                    return default(T);
                }
            });
        }

        public int GetRecordsCount()
        {
            try
            {
                using (var tran = _engine.GetSafeTransaction())
                {
                    var count = (int)tran.Count(_tableName);
                    return count - 1;
                }
            }
            catch
            {
                return 0;
            }
        }

        public async Task InsertRecord(IDbRecord record)
        {
            await Task.Run(() =>
            {
                using (var tran = _engine.GetSafeTransaction())
                {
                    tran.Technical_SetTable_OverwriteIsNotAllowed(_tableName);

                    record.Id = tran.ObjectGetNewIdentity<long>(_tableName);
                    tran.ObjectInsert(_tableName, new DBreezeObject<IDbRecord>
                    {
                        Indexes = new List<DBreezeIndex> { new DBreezeIndex(1, record.Id) { PrimaryIndex = true } },
                        NewEntity = true,
                        //Changes Select-Insert pattern to Insert (speeds up insert process)
                        Entity = record //Entity itself
                    },
                        true);
                    tran.TextInsert(_textTableName, record.Id.To_8_bytes_array_BigEndian(), record.GetTextSearchKey());
                    tran.TryCommit();
                }
            });
        }
       

        public async Task InsertRecords(IEnumerable<IDbRecord> records)
        {
            await Task.Run(() =>
            {
                using (var tran = _engine.GetSafeTransaction())
                {
                    if (tran != null && records?.Any() == true)
                    {
                        //tran.Technical_SetTable_OverwriteIsNotAllowed(_tableName);
                        foreach (var record in records.ToList())
                        {
                            record.Id = tran.ObjectGetNewIdentity<long>(_tableName);
                            var ir = tran.ObjectInsert(_tableName, new DBreezeObject<IDbRecord>
                            {
                                Indexes = new List<DBreezeIndex>
                                        {
                                            new DBreezeIndex(1, record.Id) {PrimaryIndex = true }
                                        },
                                NewEntity = true,
                                //Changes Select-Insert pattern to Insert (speeds up insert process)
                                Entity = record, //Entity itself
                            });
                            //Using text-search engine for the free text search
                            tran.TextInsert(_textTableName, record.Id.To_8_bytes_array_BigEndian(), record.TextSearchKey);
                        }

                        tran.TryCommit();
                    }
                }
            });
        }

        public async Task<IEnumerable<T>> QueryRecords<T>(string term, int limit = int.MaxValue)
        {
            return await Task.Run(() =>
            {
                using (var tran = _engine.GetSafeTransaction())
                {
                    var terms = term.Split('&');
                    var recordList = new List<T>();
                    var search = tran.TextSearch(_textTableName).Block(term.ToLower(), ignoreOnEmptyParameters: true);
                    for(int a = 0; a < terms.Length; a++)
                    {
                        search = search.Or(terms[a]);
                    }
                    var ids = search.GetDocumentIDs();
                    int index = -1;
                    foreach(var id in ids)
                    {
                        index++;
                        if (index > limit)
                            break;
                        var o = tran.Select<byte[], byte[]>(_tableName, 1.ToIndex(id)).ObjectGet<T>();
                        recordList.Add(o.Entity);
                    }                  
                    return recordList.Take(limit);
                }
            });
        }

        public Task<bool> UpdateRecordAsync<T>(T record, long id)
        {
            return Task.Run(() =>
            {
                using (var tran = _engine.GetSafeTransaction())
                {
                    var row = tran.Select<byte[], byte[]>(_tableName, 1.ToIndex(id));
                    if (row.Exists)
                    {
                        var getRecord = row.ObjectGet<T>();
                        getRecord.Entity = record;
                        getRecord.NewEntity = false;
                        getRecord.Indexes = new List<DBreezeIndex> { new DBreezeIndex(1, id) { PrimaryIndex = true } }; //PI Primary Index
                        if (tran.ObjectInsert(_tableName, getRecord, true).EntityWasInserted)
                        {
                            tran.TryCommit();
                            return true;
                        }
                    }
                    return false;
                }
            });
        }

        public Task UpdateRecords<T>(IEnumerable<IDbRecord> records)
        {
            return Task.Run(() =>
            {
                using (var tran = _engine.GetSafeTransaction())
                {
                    foreach (var data in records)
                    {
                        var row = tran.Select<byte[], byte[]>(_tableName, 1.ToIndex(data.Id));
                        if (row.Exists)
                        {
                            var getRecord = row.ObjectGet<T>();
                            getRecord.Entity = (T)data;
                            getRecord.NewEntity = false;
                            getRecord.Indexes = new List<DBreezeIndex> { new DBreezeIndex(1, data.Id) { PrimaryIndex = true } }; //PI Primary Index
                            tran.ObjectInsert(_tableName, getRecord, true);
                        }
                    }
                    tran.TryCommit();
                }
            });
        }

        public async Task RemoveRecord(IDbRecord record)
        {
            await Task.Run(() =>
            {
                using (var tran = _engine.GetSafeTransaction())
                {
                    tran.ObjectRemove(_tableName, 1.ToIndex(record.Id));
                    //remove from text engine too
                    tran.TextRemove(_textTableName, record.Id.To_8_bytes_array_BigEndian(), record.GetTextSearchKey());
                    tran.TryCommit();
                }
            });
        }

        public async Task RemoveRecords(IEnumerable<IDbRecord> records)
        {
            await Task.Run(() =>
            {
                using (var tran = _engine.GetSafeTransaction())
                {
                    foreach (var data in records)
                    {
                        tran.ObjectRemove(_tableName, 1.ToIndex(data.Id));
                        //remove from text engine too
                        tran.TextRemove(_textTableName, data.Id.To_8_bytes_array_BigEndian(), data.GetTextSearchKey());
                    }
                    tran.TryCommit();
                }
            });
        }

        public Task<IEnumerable<T>> GetRecords<T>()
        {
            return GetRangeOfRecords<T>(int.MinValue, int.MaxValue);
        }

        public Task<IEnumerable<T>> GetRangeOfRecords<T>(long fromId, long toId)
        {
            return Task.Run(() =>
            {
                IEnumerable<T> recordList;
                using (var tran = _engine.GetSafeTransaction())
                {
                    recordList = tran.SelectForwardFromTo<byte[], byte[]>(_tableName, 1.ToIndex(fromId), true, 1.ToIndex(toId), false)
                    .Select(x => x.ObjectGet<T>().Entity).ToList();
                }
                return recordList;
            });
        }
    }
}