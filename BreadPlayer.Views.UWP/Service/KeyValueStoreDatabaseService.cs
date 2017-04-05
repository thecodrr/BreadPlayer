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
            DBreeze.Utils.CustomSerializator.Serializator =  JsonConvert.SerializeObject;
            DBreeze.Utils.CustomSerializator.Deserializator = JsonConvert.DeserializeObject;          
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
                return tran.Select<string, DbCustomSerializer<T>>(table, path).Value.Get;
            }
        } 
        public int GetRecordsCount(string tableName)
        {
            using (var tran = engine.GetTransaction())
            {
                return (int)tran.Count(tableName);
            }
        }
       
        public void InsertRecord<T>(string tableName, string key, T record)
        {
            using (var tran = engine.GetTransaction())
            {
                tran.Insert<string, DbCustomSerializer<T>>(tableName, key, record);
                tran.Commit();
            }
        }

        public void InsertTracks(IEnumerable<Mediafile> records)
        {
            using (var tran = engine.GetTransaction())
            {
                foreach (var record in records)
                {
                    tran.Insert<string, DbCustomSerializer<Mediafile>>("Tracks", record.Path, record);
                }
                tran.Commit();
            }
        }

        public IEnumerable<T> QueryRecords<T>(string tableName, string term)
        {
            using (var tran = engine.GetTransaction())
            {
                var records = tran.SelectForwardStartsWith<string, DbCustomSerializer<T>>(tableName, term);
                var recordList = new List<T>();
                foreach (var record in records)
                {
                    recordList.Add(record.Value.Get);
                }
                return recordList;
            }
        }

        public void RemoveRecords<T>(string tableName, string key, IEnumerable<T> records)
        {
            using (var tran = engine.GetTransaction())
            {
                foreach (var record in records)
                {
                    tran.RemoveKey(tableName, key);
                }
                tran.Commit();
            }
        }

        public void UpdateRecord<T>(string tableName, string key, T record)
        {
            var getEntry = GetRecord<T>(tableName, key);
            getEntry = record;
            InsertRecord<T>(tableName, key, getEntry);
        }

        public void UpdateTracks(IEnumerable<Mediafile> records)
        {
            using (var tran = engine.GetTransaction())
            {
                foreach (var record in records)
                {
                    var getEntry = tran.Select<string, DbCustomSerializer<Mediafile>>("Tracks", record.Path).Value.Get;
                    getEntry = record;
                    tran.Insert<string, DbCustomSerializer<Mediafile>>("Tracks", record.Path, getEntry);
                }
                tran.Commit();
            }
        }

        public void RemoveRecord(string tableName, string key)
        {
            using (var tran = engine.GetTransaction())
            {
                tran.RemoveKey(tableName, key);
                tran.Commit();
            }
        }
        public IEnumerable<T> GetRecords<T>(string tableName)
        {
            using (var tran = engine.GetTransaction())
            {
                var records = tran.SelectForward<string, DbCustomSerializer<T>>(tableName);
                var recordList = new List<T>();
                foreach (var record in records)
                {
                    recordList.Add(record.Value.Get);
                }
                return recordList;
            }
        }
    }
}
