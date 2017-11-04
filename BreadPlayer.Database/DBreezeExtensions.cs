using DBreeze;
using DBreeze.Transactions;
using DBreeze.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BreadPlayer.Database
{
    public static class DBreezeExtensions
    {
        static string _dbPath;
        public static DBreezeEngine Initialize(this KeyValueStoreDatabaseService service, string dbPath)
        {
            _dbPath = dbPath;
            CustomSerializator.ByteArraySerializator = o => JsonConvert.SerializeObject(o).To_UTF8Bytes();
            CustomSerializator.ByteArrayDeSerializator = (bt, t) => JsonConvert.DeserializeObject(bt.UTF8_GetString(), t);
            return StaticKeyValueDatabase.GetDatabaseEngine(dbPath);
        }
        public static void Reinitialize(this DBreeze.DBreezeEngine engine, string dbPath)
        {
            if (StaticKeyValueDatabase.IsDisposed || engine == null)
            {
                _dbPath = dbPath;
                engine = StaticKeyValueDatabase.GetDatabaseEngine(dbPath);
            }
        }
        public static Transaction GetSafeTransaction(this DBreezeEngine engine)
        {
            try
            {
                engine.Reinitialize(_dbPath);
                return engine.GetTransaction();
            }
            catch(Exception ex)
            {
                BLogger.E("Error while getting transaction.", ex);
                return null;
            }
        }
        public static void TryCommit(this Transaction transaction)
        {
            try
            {
                transaction.Commit();
            }
            catch (Exception ex)
            {
                BLogger.E("Error while committing transaction.", ex);
                transaction.Rollback();
            }
        }

    }
}
