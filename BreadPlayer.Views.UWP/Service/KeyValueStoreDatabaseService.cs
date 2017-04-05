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
                        new DBreezeIndex(1, record.Path) { PrimaryIndex = true }, //PI Primary Index
                        //One PI must be set, if any secondary index will append it to the end, for uniqueness
                       new DBreezeIndex(2, record.FolderPath), //SI - Secondary Index
                       new DBreezeIndex(3, record.LeadArtist),
                       new DBreezeIndex(4, record.Album),
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

        public IEnumerable<T> QueryRecords<T>(string tableName, string term)
        {
            using (var tran = engine.GetTransaction())
            {
                var records = tran.TextSearch(tableName).BlockAnd(term).GetDocumentIDs(); // tran.SelectForwardStartsWith<string, DbCustomSerializer<T>>(tableName, term);
                var recordList = new List<T>();
                foreach (var doc in records)
                {
                    var obj = tran.Select<byte[], byte[]>(tableName, 1.ToIndex(doc)).ObjectGet<T>();
                    recordList.Add(obj.Entity);
                }
                return recordList;
            }
        }              

        public void UpdateRecord<T>(string tableName, string primaryKey, T record)
        {
            using (var tran = engine.GetTransaction())
            {
                var ord = tran.Select<byte[], byte[]>(tableName, 1.ToIndex(primaryKey)).ObjectGet<T>();
                ord.Entity = record;
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
        public IEnumerable<T> GetRecords<T>(string tableName)
        {
            using (var tran = engine.GetTransaction())
            {
                var records = tran.SelectForward<byte[], byte[]>(tableName);
                var recordList = new List<T>();
                foreach (var record in records)
                {
                    recordList.Add(record.ObjectGet<T>().Entity);
                }
                return recordList;
            }
        }
    }
}
