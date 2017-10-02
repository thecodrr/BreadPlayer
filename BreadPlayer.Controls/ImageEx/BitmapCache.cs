using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;

namespace BreadPlayer.Controls
{
    public static class BitmapCache
    {
        public const int Maxresolution = 1920;
        public const int Midresolution = 960;

        private const string FolderName = "ImageCache";

        private static Dictionary<string, Task> _concurrentTasks = new Dictionary<string, Task>();
        private static object _lock = new object();

        static BitmapCache()
        {
            CacheDuration = TimeSpan.FromHours(24);
        }

        public static TimeSpan CacheDuration { get; set; }

        #region ClearCacheAsync

        public static async Task ClearCacheAsync(TimeSpan? duration = null)
        {
            duration = duration ?? TimeSpan.FromSeconds(0);
            DateTime expirationDate = DateTime.Now.Subtract(duration.Value);
            try
            {
                var folder = await EnsureCacheFolderAsync();
                foreach (var file in await folder.GetFilesAsync())
                {
                    try
                    {
                        if (file.DateCreated < expirationDate)
                        {
                            await file.DeleteAsync();
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        #endregion ClearCacheAsync

        public static async Task<Uri> GetImageUriAsync(Uri uri, int maxWidth, int maxHeight)
        {
            Task busy = null;
            string key = BuildKey(uri);

            lock (_lock)
            {
                if (_concurrentTasks.ContainsKey(key))
                {
                    busy = _concurrentTasks[key];
                }
                else
                {
                    busy = EnsureFilesAsync(uri);
                    _concurrentTasks.Add(key, busy);
                }
            }

            try
            {
                await busy;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("GetImageUriAsync. {0}", ex.Message);
            }

            lock (_lock)
            {
                if (_concurrentTasks.ContainsKey(key))
                {
                    _concurrentTasks.Remove(key);
                }
            }

            string fileName = BuildFileName(uri, maxWidth, maxHeight);
            var cacheFolder = await EnsureCacheFolderAsync();
            if (await cacheFolder.TryGetItemAsync(fileName) != null)
            {
                return new Uri($"ms-appdata:///temp/{FolderName}/{fileName}");
            }
            return null;
        }

        private static async Task EnsureFilesAsync(Uri uri)
        {
            DateTime expirationDate = DateTime.Now.Subtract(CacheDuration);

            var cacheFolder = await EnsureCacheFolderAsync();

            string fileName = BuildFileName(uri, Maxresolution, Maxresolution);
            StorageFile mainFile = await cacheFolder.TryGetItemAsync(fileName) as StorageFile;
            if (await IsFileOutOfDate(mainFile, expirationDate))
            {
                mainFile = await cacheFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                if (!await BitmapTools.DownloadImageAsync(mainFile, uri, Maxresolution, Maxresolution))
                {
                    await mainFile.DeleteAsync();
                    return;
                }
            }

            fileName = BuildFileName(uri, Midresolution, Midresolution);
            var resizedFile = await cacheFolder.TryGetItemAsync(fileName) as StorageFile;
            if (await IsFileOutOfDate(resizedFile, expirationDate))
            {
                resizedFile = await cacheFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                try
                {
                    await BitmapTools.ResizeImageUniformAsync(mainFile, resizedFile, Midresolution, Midresolution);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("EnsureFilesAsync. {0}", ex.Message);
                    await resizedFile.DeleteAsync();
                }
            }
        }

        private static async Task<bool> IsFileOutOfDate(StorageFile file, DateTime expirationDate)
        {
            if (file != null)
            {
                var properties = await file.GetBasicPropertiesAsync();
                return properties.DateModified < expirationDate;
            }
            return true;
        }

        #region GetCacheFolder

        private static StorageFolder _cacheFolder;
        private static SemaphoreSlim _cacheFolderSemaphore = new SemaphoreSlim(1);

        internal static async Task<StorageFolder> EnsureCacheFolderAsync()
        {
            if (_cacheFolder == null)
            {
                await _cacheFolderSemaphore.WaitAsync();
                try
                {
                    _cacheFolder = await ApplicationData.Current.TemporaryFolder.TryGetItemAsync(FolderName) as StorageFolder;
                    if (_cacheFolder == null)
                    {
                        _cacheFolder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync(FolderName);
                    }
                }
                catch { }
                finally
                {
                    _cacheFolderSemaphore.Release();
                }
            }
            return _cacheFolder;
        }

        #endregion GetCacheFolder

        #region File Hash

        private static string BuildKey(Uri uri)
        {
            ulong uriHash = CreateHash64(uri);
            return $"{uriHash}";
        }

        private static string BuildFileName(Uri uri, int maxWidth, int maxHeight)
        {
            string prefix = GetPrefixName(maxWidth, maxHeight);
            ulong uriHash = CreateHash64(uri);

            return $"{prefix}.{uriHash}";
        }

        private static UInt64 CreateHash64(Uri uri)
        {
            return CreateHash64(uri.Host + uri.PathAndQuery);
        }

        private static UInt64 CreateHash64(string str)
        {
            byte[] utf8 = Encoding.UTF8.GetBytes(str);

            ulong value = (ulong)utf8.Length;
            for (int n = 0; n < utf8.Length; n++)
            {
                value += (ulong)utf8[n] << ((n * 5) % 56);
            }

            return value;
        }

        private static string GetPrefixName(double width, double height)
        {
            if (width <= Midresolution && height <= Midresolution)
            {
                return "M";
            }
            return "L";
        }

        #endregion File Hash

        #region GetSizeLevel

        public static int GetSizeLevel(Size size)
        {
            double width = size.Width;
            double height = size.Height;
            if (width <= Midresolution && height <= Midresolution)
            {
                return 1;
            }
            return 0;
        }

        #endregion GetSizeLevel
    }
}