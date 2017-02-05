using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using XLog;
using XLog.Formatters;

namespace BreadPlayer.Targets
{
    class AsyncFileTarget : Target, ILogStorage
    {
        private readonly object _syncRoot = new object();
        private readonly string _logFilePath;
        static StorageFile File;
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
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            File =  await storageFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            stream = await File.OpenStreamForWriteAsync();
            writer = new StreamWriter(stream);
        }
        Stream stream;
        StreamWriter writer;
        public override void Write(string content)
        {
            if(writer != null)
            {
                writer.Write(content);
                writer.Flush();
            }          
        }

        public string GetLastLogs()
        {
                Flush();            
                int numOfRetries = 3;
                do
                {
                    try
                    {
                        return ReadFileContentsAsync(File).Result;
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
            var copiedFile = await File.CopyAsync(ApplicationData.Current.LocalFolder, copyName);
            try
            {
                
                    return await FileIO.ReadTextAsync(copiedFile);
                
            }
            finally
            {
                await File.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
        }

        public override void Flush()
        {
            try
            {
                File = null;
            }
            catch (IOException)
            {
                // If log file cannot be flushed - we shouldn't crash. 
                // Supressing finalization to avoid crash in finalizer
                GC.SuppressFinalize(File);
            }
        }
    }
}
