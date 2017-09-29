using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using XLog;
using XLog.Formatters;

namespace BreadPlayer.Targets
{
    internal class AsyncFileTarget : Target, ILogStorage
    {
        private readonly object _syncRoot = new object();
        private readonly string _logFilePath;
        private static StorageFile _file;

        public AsyncFileTarget(string logFilePath)
            : this(null, logFilePath)
        {
        }

        public AsyncFileTarget(IFormatter formatter, string logFilePath, bool autoFlush = false)
            : base(formatter)
        {
            try
            {
                _logFilePath = logFilePath;
                CreateFile(_logFilePath);
            }
            catch (IOException)
            {
            }
        }

        private async void CreateFile(string filename)
        {
            StorageFolder storageFolder = await KnownFolders.MusicLibrary.CreateFolderAsync(".breadplayerLogs", CreationCollisionOption.OpenIfExists);
            _file = await storageFolder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
        }

        public async override void Write(string content)
        {
            try
            {
                if (_file != null)
                {
                    await FileIO.AppendTextAsync(_file, content);
                }
            }
            catch { }
        }

        public string GetLastLogs()
        {
            Flush();
            int numOfRetries = 3;
            do
            {
                try
                {
                    return ReadFileContentsAsync(_file).Result;
                }
                catch (IOException)
                {
                }
            } while (--numOfRetries > 0);

            return string.Empty;
        }

        private async static Task<string> ReadFileContentsAsync(StorageFile f)
        {
            string copyName = f.DisplayName + ".copy";
            var copiedFile = await _file.CopyAsync(ApplicationData.Current.LocalFolder, copyName);
            try
            {
                return await FileIO.ReadTextAsync(copiedFile);
            }
            finally
            {
                await _file.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
        }

        public override void Flush()
        {
            try
            {
                _file = null;
            }
            catch (IOException)
            {
                // If log file cannot be flushed - we shouldn't crash.
                // Supressing finalization to avoid crash in finalizer
                GC.SuppressFinalize(_file);
            }
        }
    }
}