using BreadPlayer.Core.Common;
using DBreeze;
using DBreeze.Objects;
using DBreeze.Utils;
using Newtonsoft.Json;
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
            if (_db == null || DbPath != dbPath)
            {
                DbPath = dbPath;
                var dbConfig = new DBreezeConfiguration
                {
                    DBreezeDataFolderName = dbPath,
                    Storage = DBreezeConfiguration.eStorage.DISK
                };
                _db = new DBreezeEngine(dbConfig);
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

    public class KeyValueStoreDatabaseService : IDatabaseService
    {
        private string _dbPath;
        private DBreezeEngine _engine;
        private string _textTableName;
        private string _tableName;

        public KeyValueStoreDatabaseService(string dbPath, string tableName)
        {
            _dbPath = dbPath;
            _engine = StaticKeyValueDatabase.GetDatabaseEngine(dbPath);
            CustomSerializator.ByteArraySerializator = o => JsonConvert.SerializeObject(o).To_UTF8Bytes();
            CustomSerializator.ByteArrayDeSerializator = (bt, t) => JsonConvert.DeserializeObject(bt.UTF8_GetString(), t);
            _textTableName = tableName + "Text";
            _tableName = tableName;
        } 

        public void ChangeContext(string context)
        {
            _textTableName = context + "Text";
            _tableName = context;
        }

        public bool CheckExists(long id)
        {
            using (var tran = _engine.GetTransaction())
            {
                var item = tran.Select<byte[], byte[]>(_tableName, 1.ToIndex(id));//.ObjectGet<T>().Entity;
                return item.Exists;
            }
        }

        public bool CheckExists(string query)
        {
            using (var tran = _engine.GetTransaction())
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
            StaticKeyValueDatabase.IsDisposed = true;
            StaticKeyValueDatabase.DisposeDatabaseEngine();
            _engine = null;
        }

        public T GetRecordById<T>(long id)
        {
            try
            {
                using (var tran = _engine.GetTransaction())
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
                using (var tran = _engine.GetTransaction())
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
                using (var tran = _engine.GetTransaction())
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
                ReinitEngine();
                using (var tran = _engine.GetTransaction())
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
                    tran.Commit();
                }
            });
        }

        private void ReinitEngine()
        {
            if (StaticKeyValueDatabase.IsDisposed || _engine == null)
            {
                _engine = StaticKeyValueDatabase.GetDatabaseEngine(_dbPath);
            }
        }

        public async Task InsertRecords(IEnumerable<IDbRecord> records)
        {
            await Task.Run(() =>
            {
                //ReinitEngine();
                using (var tran = _engine.GetTransaction())
                {
                    if (records.Any())
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
                                Entity = record //Entity itself
                            },
                                true);
                            //Using text-search engine for the free text search
                            tran.TextInsert(_textTableName, record.Id.To_8_bytes_array_BigEndian(), record.TextSearchKey);
                        }

                        tran.Commit();
                    }
                }
            });
        }

        public async Task<IEnumerable<T>> QueryRecords<T>(string term, int limit = int.MaxValue)
        {
            return await Task.Run(() =>
            {
                using (var tran = _engine.GetTransaction())
                {
                    var recordList = new List<T>();
                    foreach (var record in tran.TextSearch(_textTableName).Block(term.ToLower()).GetDocumentIDs())
                    {
                        var o = tran.Select<byte[], byte[]>(_tableName, 1.ToIndex(record)).ObjectGet<T>();
                        recordList.Add(o.Entity);
                    }
                    return recordList;
                }
            });
        }

        public Task<bool> UpdateRecordAsync<T>(T record, long id)
        {
            return Task.Run(() =>
            {
                using (var tran = _engine.GetTransaction())
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
                            tran.Commit();
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
                using (var tran = _engine.GetTransaction())
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
                    tran.Commit();
                }
            });
        }

        public async Task RemoveRecord(IDbRecord record)
        {
            await Task.Run(() =>
            {
                using (var tran = _engine.GetTransaction())
                {
                    tran.ObjectRemove(_tableName, 1.ToIndex(record.Id));
                    //remove from text engine too
                    tran.TextRemove(_textTableName, record.Id.To_8_bytes_array_BigEndian(), record.GetTextSearchKey());
                    tran.Commit();
                }
            });
        }

        public async Task RemoveRecords(IEnumerable<IDbRecord> records)
        {
            await Task.Run(() =>
            {
                using (var tran = _engine.GetTransaction())
                {
                    foreach (var data in records)
                    {
                        tran.ObjectRemove(_tableName, 1.ToIndex(data.Id));
                        //remove from text engine too
                        tran.TextRemove(_textTableName, data.Id.To_8_bytes_array_BigEndian(), data.GetTextSearchKey());
                    }
                    tran.Commit();
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
                using (var tran = _engine.GetTransaction())
                {
                    recordList = tran.SelectForwardFromTo<byte[], byte[]>(_tableName, 1.ToIndex(fromId), true, 1.ToIndex(toId), false)
                    .Select(x => x.ObjectGet<T>().Entity).ToList();
                }
                return recordList;
            });
        }
    }
}