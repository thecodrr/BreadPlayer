using BreadPlayer.Common;
using BreadPlayer.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Xaml;

namespace BreadPlayer.Services
{
    public class StorageLibraryService
    {
        private StorageLibrary MusicLibrary { get; set; }
        private StorageFolder MusicLibraryParentFolder { get; set; }
        private DispatcherTimer _updateTimer;

        public StorageLibraryService()
        {
            Initialize();
        }

        private async void Initialize()
        {
            try
            {
                //MusicLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);
                //MusicLibrary.ChangeTracker.Enable();
                MusicLibraryParentFolder = KnownFolders.MusicLibrary;

                ////run a timer every five seconds to check for updated files
                //_updateTimer = new DispatcherTimer();
                //_updateTimer.Interval = TimeSpan.FromSeconds(5);
                //_updateTimer.Start();
                //_updateTimer.Tick += _updateTimer_Tick;
            }
            catch
            {
                await SharedLogic.Instance.NotificationManager.ShowMessageAsync("Failed to initialize file system watcher.");
                BLogger.I("Failed to initialize file system watcher");
            }
        }

        private async void _updateTimer_Tick(object sender, object e)
        {
            if (MusicLibrary != null && MusicLibraryParentFolder != null)
                StorageItemsUpdated.Invoke(
                     MusicLibrary.ChangeTracker,
                     new StorageItemsUpdatedEventArgs(
                         await MusicLibrary.ChangeTracker.GetChangeReader().ReadBatchAsync()));
        }

        public Task<IEnumerable<StorageFile>> GetStorageFilesInLibraryAsync()
        {
            return GetStorageFilesInFolderAsync(MusicLibraryParentFolder);
        }

        public async Task<IEnumerable<StorageFile>> GetStorageFilesInFolderAsync(StorageFolder folder)
        {
            //Get query options with which we search for files in the specified folder
            var options = DirectoryWalker.GetQueryOptions();
            /*
            options.FileTypeFilter.Add(".mp3");
            options.FileTypeFilter.Add(".wav");
            options.FileTypeFilter.Add(".ogg");
            options.FileTypeFilter.Add(".flac");
            options.FileTypeFilter.Add(".m4a");
            options.FileTypeFilter.Add(".aif");
            options.FileTypeFilter.Add(".wma");*/

            //this is the query result which we recieve after querying in the folder
            StorageFileQueryResult queryResult = folder.CreateFileQueryWithOptions(options);

            //'count' is for total files got after querying.
            uint count = await queryResult.GetItemCountAsync();

            //the event for files changed
            queryResult.ContentsChanged += QueryResult_ContentsChanged;

            if (count == 0)
            {
                string error = "No songs found!";
                BLogger.I("No songs were found!");
                await SharedLogic.Instance.NotificationManager.ShowMessageAsync(error);
                return null;
            }
            await SharedLogic.Instance.NotificationManager.ShowMessageAsync("Getting files...");
            return await queryResult.GetFilesAsync();
        }

        public void SetupDirectoryWatcher(IEnumerable<StorageFolder> folderCollection)
        {
            //await Task.Delay(10000);
            //foreach (var folder in folderCollection)
            //{
            //    StorageFileQueryResult queryResult = folder.CreateFileQueryWithOptions(DirectoryWalker.GetQueryOptions());
            //    var files = await queryResult.GetItemCountAsync();
            //    queryResult.ContentsChanged += QueryResult_ContentsChanged; ;
            //}
        }

        public async Task<StorageFolder> AddFolderToLibraryAsync()
        {
            if (MusicLibrary != null)
                return await MusicLibrary.RequestAddFolderAsync();
            return null;
        }

        private void QueryResult_ContentsChanged(IStorageQueryResultBase sender, object args)
        {
            //if (MusicLibrary != null)
            //    StorageItemsUpdated.Invoke(
            //    MusicLibrary.ChangeTracker,
            //    new StorageItemsUpdatedEventArgs(
            //        await MusicLibrary.ChangeTracker.GetChangeReader().ReadBatchAsync()));
        }

        public event OnStorageItemsUpdatedEventHandler StorageItemsUpdated;
    }

    public delegate void OnStorageItemsUpdatedEventHandler(object sender, StorageItemsUpdatedEventArgs e);

    public class StorageItemsUpdatedEventArgs : EventArgs
    {
        public IReadOnlyList<StorageLibraryChange> UpdatedItems { get; set; }

        public StorageItemsUpdatedEventArgs(IReadOnlyList<StorageLibraryChange> updatedItems)
        {
            UpdatedItems = updatedItems;
        }
    }
}