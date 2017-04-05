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
            DBreeze.Utils.CustomSerializator.Serializator = JsonConvert.SerializeObject;
            DBreeze.Utils.CustomSerializator.Deserializator = JsonConvert.DeserializeObject;          
        }

        public void Dispose()
        {
            if (engine != null)
                engine.Dispose();
            StaticKeyValueDatabase.DB = null;
        }

        public Mediafile FindOne(string path)
        {
            using (var t = engine.GetTransaction())
            {
                return t.Select<string, DbCustomSerializer<Mediafile>>("Tracks", path).Value.Get;
            }
        }
        
        public Task<IEnumerable<Mediafile>> GetRangeOfTracks(int skip, int limit)
        {
            throw new NotImplementedException();
        }

        public int GetTrackCount()
        {
            using (var tran = engine.GetTransaction())
            {
                return (int)tran.Count("Tracks");
            }
        }

        public IEnumerable<Mediafile> GetTracks()
        {
            using (var tran = engine.GetTransaction())
            {
                var fil = tran.SelectForward<string, DbCustomSerializer<Mediafile>>("Tracks");
                var mediafiles = new List<Mediafile>();
                foreach(var mp3File in fil)
                {
                    mediafiles.Add(mp3File.Value.Get);
                }
                return mediafiles;
            }
        }

        public void Insert(Mediafile file)
        {
            using (var tran = engine.GetTransaction())
            {
                tran.Insert<string, DbCustomSerializer<Mediafile>>("Tracks", file.Path, file);
                tran.Commit();
            }
        }

        public void Insert(IEnumerable<Mediafile> files)
        {
            using (var tran = engine.GetTransaction())
            {
                foreach (var file in files)
                {
                    try
                    {
                        tran.Insert<string, DbCustomSerializer<Mediafile>>("Tracks", file.Path, file);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                tran.Commit();
            }
        }

        public IEnumerable<Mediafile> Query(string field, object term)
        {
            using (var tran = engine.GetTransaction())
            {
                return tran.SelectForwardStartsWith<string, DbCustomSerializer<Mediafile>>("Tracks", term.ToString()).ToList().Select(t => t.Value.Get);
            }
        }

        public void Remove(Mediafile file)
        {
            throw new NotImplementedException();
        }

        public void RemoveTracks(Query query)
        {
            throw new NotImplementedException();
        }

        public void UpdateTrack(Mediafile file)
        {
            throw new NotImplementedException();
        }

        public void UpdateTracks(IEnumerable<Mediafile> files)
        {
            throw new NotImplementedException();
        }
    }
}
