using DBreeze;
using DBreeze.Transactions;
using DBreeze.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
            if (dbPath == null)
                return;
            if (StaticKeyValueDatabase.IsDisposed || engine == null || engine?.IsDatabaseOperable == false || engine?.Disposed == true)
            {
                BLogger.I("Database is not operable or it was disposed. Reinitializing. Message: {message}", engine.DatabaseNotOperableReason ?? "");
                _dbPath = dbPath;
                StaticKeyValueDatabase.DisposeDatabaseEngine();
                engine = StaticKeyValueDatabase.GetDatabaseEngine(dbPath);
                BLogger.I("Engine reintialized. Path: {path}", dbPath);
            }
        }
        public static Transaction GetSafeTransaction(this DBreezeEngine engine)
        {
            try
            {
                BLogger.I("Getting transaction.");
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
                BLogger.I("Comitting changes to transaction. Time created: {timecreated}.", FromUDT(transaction.CreatedUdt).ToString());
                transaction.Commit();
            }
            catch (Exception ex)
            {
                BLogger.E("Error while committing transaction. Time created: {timecreated}.", ex, FromUDT(transaction.CreatedUdt).ToString());
                transaction.Rollback();
            }
        }
        private static DateTime FromUDT(long unixDateTime)
        {
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return start.AddTicks(unixDateTime).ToLocalTime();
        }
    }
}
