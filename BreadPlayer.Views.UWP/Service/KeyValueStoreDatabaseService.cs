using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadPlayer.Models;
using DBreeze;
using Newtonsoft.Json;
using DBreeze.Utils;
using DBreeze.DataTypes;
using Windows.Storage;
using LiteDB;
using DBreeze.Objects;
using DBreeze.Utils.Async;

namespace BreadPlayer.Service
{
    public class StaticKeyValueDatabase
    {
        static DBreezeEngine db;
        //static IDiskService service = new FileDiskService(ApplicationData.Current.LocalFolder.Path + @"\breadplayer.db");
        public static DBreezeEngine DB
        {
            get
            {
                if (db == null)
                {
                    var dbPath = ApplicationData.Current.LocalFolder.Path + @"\breadplayerDB";
                    db = new DBreezeEngine(dbPath);
                }
                return db;
            }
            set
            {
                db = value;
            }
        }
    }
    public class KeyValueStoreDatabaseService : IDatabaseService
    {
        DBreezeEngine engine = null;
        public KeyValueStoreDatabaseService()
        {
            CreateDB();
        }
       
        public void CreateDB()
        {
            engine = StaticKeyValueDatabase.DB;
            DBreeze.Utils.CustomSerializator.ByteArraySerializator = (object o) => { return JsonConvert.SerializeObject(o).To_UTF8Bytes(); };
           
            DBreeze.Utils.CustomSerializator.ByteArrayDeSerializator = (byte[] bt, Type t) => { return JsonConvert.DeserializeObject(bt.UTF8_GetString(), t); };          
        }
        public bool CheckExists<T>(string table, string path)
        {
            return GetRecord<T>(table, path) != null;
        }
        
        public void Dispose()
        {
            if (engine != null)
                engine.Dispose();
            StaticKeyValueDatabase.DB = null;
            engine = StaticKeyValueDatabase.DB;
        }

        public T GetRecord<T>(string table, string path)
        {
            using (var tran = engine.GetTransaction())
            {
                return tran.Select<byte[], byte[]>(table, path.ToBytes()).ObjectGet<T>().Entity;
            }
        } 
        public int GetRecordsCount(string tableName)
        {
            using (var tran = engine.GetTransaction())
            {
                return (int)tran.Count(tableName);
            }
        }
       
        public void InsertRecord<T>(string tableName, List<DBreezeIndex> indexes, T record)
        {
            using (var tran = engine.GetTransaction())
            {
                var ir = tran.ObjectInsert<T>(tableName, new DBreezeObject<T>
                {
                    Indexes = indexes,
                    NewEntity = true,
                    //Changes Select-Insert pattern to Insert (speeds up insert process)
                    Entity = record //Entity itself
                },
                        true);
                tran.Commit();
            }
        }

        public void InsertTracks(IEnumerable<Mediafile> records)
        {
            using (var tran = engine.GetTransaction())
            {
                foreach (var record in records)
                {
                    var ir = tran.ObjectInsert<Mediafile>("Tracks", new DBreezeObject<Mediafile>
                    {
                        Indexes = new List<DBreezeIndex>
                        {
                        new DBreezeIndex(1, record.Path, record.FolderPath, record.LeadArtist, record.Album, record.Title) { PrimaryIndex = true }, //PI Primary Index
                        //One PI must be set, if any secondary index will append it to the end, for uniqueness
                       //new DBreezeIndex(2, record.FolderPath) { AddPrimaryToTheEnd = false }, //SI - Secondary Index
                       //new DBreezeIndex(3, record.LeadArtist){ AddPrimaryToTheEnd = false },
                       //new DBreezeIndex(4, record.Album){ AddPrimaryToTheEnd = false },
                       //new DBreezeIndex(5, record.Title){ AddPrimaryToTheEnd = false },
                       //new DBreezeIndex(3,p.Salary) //SI
                        //new DBreezeIndex(4,p.Id) { AddPrimaryToTheEnd = false } //SI
                        },

                        NewEntity = true,
                        //Changes Select-Insert pattern to Insert (speeds up insert process)
                        Entity = record //Entity itself
                    },
                        true);
                }
                tran.Commit();
            }
        }

        public async Task<IEnumerable<T>> QueryRecords<T>(string tableName, string term)
        {
            return await Task.Run(() =>
            {
                using (var tran = engine.GetTransaction())
                {
                    var records = tran.SelectDictionary<byte[], byte[]>(tableName);
                    var recordList = new List<T>();
                    foreach (var doc in records)
                    {
                        if (doc.Key.ToUTF8String().Contains(term))
                        {
                            var key = doc.Key.ToUTF8String();
                            var val = tran.Select<byte[], byte[]>(tableName, doc.Key).ObjectGet<T>().Entity;
                            recordList.Add(val);
                        }
                    }
                    return recordList;
                }
            });
        }              

        public void UpdateRecord<T>(string tableName, string primaryKey, T record)
        {
            using (var tran = engine.GetTransaction())
            {
                var ord = tran.Select<byte[], byte[]>(tableName, 1.ToIndex(primaryKey)).ObjectGet<T>();
                ord.Entity = record;
                ord.NewEntity = false;
                tran.ObjectInsert(tableName, ord, true);
                tran.Commit();
            }
        }

        public void UpdateTracks(IEnumerable<Mediafile> records)
        {
            using (var tran = engine.GetTransaction())
            {
                foreach (var record in records)
                {
                    var ord = tran.Select<byte[], byte[]>("Tracks", 1.ToIndex(record.Path)).ObjectGet<Mediafile>();
                    ord.Entity = record;
                    tran.ObjectInsert("Tracks", ord, true);
                }
                tran.Commit();
            }
        }

        public void RemoveRecord(string tableName, string key)
        {
            using (var tran = engine.GetTransaction())
            {
                tran.ObjectRemove(tableName, key.ToBytes());
                tran.Commit();
            }
        }
       
        public async Task<IEnumerable<T>> GetRecords<T>(string tableName)
        {
            return await Task.Run(() =>
            {
                var recordList = new List<T>();
                using (var tran = engine.GetTransaction())
                {
                    var records = tran.SelectForward<byte[], byte[]>(tableName);

                    foreach (var record in records)
                    {
                        recordList.Add(record.ObjectGet<T>().Entity);
                    }
                }
                return recordList;
            });            
        }
    }
}
