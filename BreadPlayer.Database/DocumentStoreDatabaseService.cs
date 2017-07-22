using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BreadPlayer.Core.Common;

namespace BreadPlayer.Database
{
    public class DocumentStoreDatabaseService : IDatabaseService
    {
        public void ChangeTable(string tableName, string textTableName)
        {
            throw new NotImplementedException();
        }

        public bool CheckExists(long id)
        {
            throw new NotImplementedException();
        }

        public bool CheckExists(string query)
        {
            throw new NotImplementedException();
        }

        public void CreateDb(string dbPath)
        {
            throw new NotImplementedException();
        }

        public T GetRecordById<T>(long id)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetRecordByQueryAsync<T>(string query)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> GetRecords<T>()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> GetRecords<T>(long fromId, long toId)
        {
            throw new NotImplementedException();
        }

        public int GetRecordsCount()
        {
            throw new NotImplementedException();
        }

        public Task InsertRecord(IDbRecord record)
        {
            throw new NotImplementedException();
        }

        public Task InsertRecords(IEnumerable<IDbRecord> records)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> QueryRecords<T>(string term)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRecord(IDbRecord record)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRecords(IEnumerable<IDbRecord> records)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateRecordAsync<T>(T record, long id)
        {
            throw new NotImplementedException();
        }

        public void UpdateRecords<T>(IEnumerable<IDbRecord> records)
        {
            throw new NotImplementedException();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DocumentStoreDatabaseService() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
