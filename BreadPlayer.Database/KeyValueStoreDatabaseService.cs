using BreadPlayer.Models;
using DBreeze;
using DBreeze.Objects;
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
        static DBreezeEngine db;
        public static DBreezeEngine GetDatabaseEngine(string dbPath)
        {
            if (db == null || DbPath != dbPath)
            {
                DbPath = dbPath;
                var dbConfig = new DBreezeConfiguration()
                {
                    DBreezeDataFolderName = dbPath,
                    Storage = DBreezeConfiguration.eStorage.DISK
                };
                db = new DBreezeEngine(dbConfig);
            }
            return db;
        }
        public static void DisposeDatabaseEngine()
        {
            db.Dispose();
            db = null;
        }
    }
    public class KeyValueStoreDatabaseService : IDatabaseService
    {
        private string DbPath;
        DBreezeEngine engine = null;
        private string TextTableName;
        private string TableName;
        public KeyValueStoreDatabaseService(string dbPath, string textTableName, string tableName)
        {
            CreateDB(dbPath);
            TextTableName = textTableName;
            TableName = tableName;
        }

        public void CreateDB(string dbPath)
        {
            DbPath = dbPath;
            engine = StaticKeyValueDatabase.GetDatabaseEngine(dbPath);
            DBreeze.Utils.CustomSerializator.ByteArraySerializator = (object o) => { return JsonConvert.SerializeObject(o).To_UTF8Bytes(); };
            DBreeze.Utils.CustomSerializator.ByteArrayDeSerializator = (byte[] bt, Type t) => { return JsonConvert.DeserializeObject(bt.UTF8_GetString(), t); };
        }
        public void ChangeTable(string tableName, string textTableName)
        {
            TextTableName = textTableName;
            TableName = tableName;
        }
        public bool CheckExists(IDBRecord record)
        {
            using (var tran = engine.GetTransaction())
            {
                var item = tran.Select<byte[], byte[]>(TableName, 1.ToIndex(record.Id));//.ObjectGet<T>().Entity;
                return item.Exists;
            }
        }
        public void Dispose()
        {
            StaticKeyValueDatabase.DisposeDatabaseEngine();
        }

        public IDBRecord GetRecord(long id)
        {
            try
            {
                using (var tran = engine.GetTransaction())
                {
                    return tran.Select<byte[], byte[]>(TableName, 1.ToIndex(id)).ObjectGet<IDBRecord>().Entity;
                }
            }
            catch
            {
                return null;
            }
        }

        public int GetRecordsCount()
        {
            try
            {
                using (var tran = engine.GetTransaction())
                {
                    var count = (int)tran.Count(TableName);
                    return count;
                }
            }
            catch
            {
                return 0;
            }
        }

        public void InsertRecord(IDBRecord record)
        {
            using (var tran = engine.GetTransaction())
            {
                var ir = tran.ObjectInsert(TableName, new DBreezeObject<IDBRecord>
                {
                    Indexes = new List<DBreezeIndex>() { new DBreezeIndex(1, record.Id) { PrimaryIndex = true } },
                    NewEntity = true,
                    //Changes Select-Insert pattern to Insert (speeds up insert process)
                    Entity = record //Entity itself
                },
                        true);
                tran.Commit();
            }
        }
        public async Task InsertRecords(IEnumerable<IDBRecord> records)
        {
            await Task.Run(() =>
            {
                using (var tran = engine.GetTransaction())
                {
                    foreach (var record in records)
                    {
                        record.Id = tran.ObjectGetNewIdentity<long>(TableName);
                        var ir = tran.ObjectInsert(TableName, new DBreezeObject<IDBRecord>
                        {
                            Indexes = new List<DBreezeIndex>
                        {
                        new DBreezeIndex(1, record.Id) {PrimaryIndex = true }, },
                            NewEntity = true,
                            //Changes Select-Insert pattern to Insert (speeds up insert process)
                            Entity = record //Entity itself
                        },
                            true);
                        //Using text-search engine for the free text search
                        tran.TextInsert(TextTableName, record.Id.To_8_bytes_array_BigEndian(), record.GetTextSearchKey());
                    }
                    tran.Commit();
                }
            });
        }

        public async Task<IEnumerable<T>> QueryRecords<T>(string term)
        {
            return await Task.Run(() =>
            {
                using (var tran = engine.GetTransaction())
                {
                    var recordList = new List<T>();
                    foreach (var record in tran.TextSearch(TextTableName).Block(term).GetDocumentIDs())
                    {
                        var o = tran.Select<byte[], byte[]>(TableName, 1.ToIndex(record)).ObjectGet<T>();
                        recordList.Add(o.Entity);
                    }
                    return recordList;
                }
            });
        }

        public async Task<bool> UpdateRecordAsync(IDBRecord record)
        {
            return await Task.Run(() =>
            {
                using (var tran = engine.GetTransaction())
                {
                    var row = tran.Select<byte[], byte[]>(TableName, 1.ToIndex(record.Id));
                    if (row.Exists)
                    {
                        var getRecord = row.ObjectGet<IDBRecord>();
                        getRecord.Entity = record;
                        getRecord.NewEntity = false;
                        getRecord.Indexes = new List<DBreezeIndex> { new DBreezeIndex(1, record.Id) { PrimaryIndex = true } }; //PI Primary Index
                        if (tran.ObjectInsert(TableName, getRecord, true).EntityWasInserted)
                        {
                            tran.Commit();
                            return true;
                        }
                    }
                    return false;
                }
            });
        }

        public void UpdateRecords(IEnumerable<IDBRecord> records)
        {
            using (var tran = engine.GetTransaction())
            {
                foreach (var data in records)
                {
                    var ord = tran.Select<byte[], byte[]>(TableName, 1.ToIndex(data.Id)).ObjectGet<IDBRecord>();
                    ord.Entity = data;
                    ord.NewEntity = false;
                    ord.Indexes = new List<DBreezeIndex> { new DBreezeIndex(1, data.Id) { PrimaryIndex = true } }; //PI Primary Index
                    tran.ObjectInsert(TableName, ord, true);
                }
                tran.Commit();
            }
        }

        public async Task RemoveRecord(IDBRecord record)
        {
            await Task.Run(() =>
            {
                using (var tran = engine.GetTransaction())
                {
                    tran.ObjectRemove(TableName, 1.ToIndex(record.Id));
                    //remove from text engine too
                    tran.TextRemove(TextTableName, record.Id.To_8_bytes_array_BigEndian(), record.GetTextSearchKey());
                    tran.Commit();
                }
            });
        }

        public async Task RemoveRecords(IEnumerable<IDBRecord> records)
        {
            await Task.Run(() =>
            {
                using (var tran = engine.GetTransaction())
                {
                    foreach (var data in records)
                    {
                        tran.ObjectRemove(TableName, 1.ToIndex(data.Id));
                        //remove from text engine too
                        tran.TextRemove(TextTableName, data.Id.To_8_bytes_array_BigEndian(), data.GetTextSearchKey());
                    }
                    tran.Commit();
                }
            });
        }
        public async Task<IEnumerable<T>> GetRecords<T>()
        {
            return await Task.Run(() =>
            {
                var recordList = new List<T>();
                using (var tran = engine.GetTransaction())
                {
                    foreach (var record in tran.SelectForwardFromTo<byte[], byte[]>(TableName, 1.ToIndex(long.MinValue), true, 1.ToIndex(long.MaxValue), true))
                    {
                        recordList.Add(record.ObjectGet<T>().Entity);
                    }
                }
                return recordList;
            });
        }
    }
}
