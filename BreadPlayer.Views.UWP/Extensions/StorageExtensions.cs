using BreadPlayer.Core;
using BreadPlayer.Core.Models;
using BreadPlayer.Database;
using BreadPlayer.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace BreadPlayer.Extensions
{
    public static class StorageExtensions
    {
        public static bool IsItemInLibrary(this StorageLibraryChange change, IEnumerable<Mediafile> Library, out Mediafile file)
        {
            if (Library.Any(t => t.Path == (string.IsNullOrEmpty(change.PreviousPath) ? change.Path : change.PreviousPath)))
            {
                file = Library.First(t => t.Path == (string.IsNullOrEmpty(change.PreviousPath) ? change.Path : change.PreviousPath));
                return true;
            }
            file = null;
            return false;
        }

        public static async Task UpdateChangedItem(this StorageLibraryChange change, IEnumerable<Mediafile> Library, LibraryService LibraryService)
        {
            if (change.IsOfType(StorageItemTypes.File))
            {
                if (IsItemInLibrary(change, Library, out Mediafile createdItem))
                {
                    var id = createdItem.Id;
                    createdItem = await TagReaderHelper.CreateMediafile((StorageFile)await change.GetStorageItemAsync());
                    createdItem.Id = id;
                    if (await LibraryService.UpdateMediafile(createdItem))
                    {
                        await SharedLogic.Instance.NotificationManager.ShowMessageAsync(string.Format("Mediafile Updated. File Path: {0}", createdItem.Path), 5);
                    }
                }
                else
                    await AddNewItem(change);
            }
        }

        public static async Task AddNewItem(this StorageLibraryChange change)
        {
            if (change.IsOfType(StorageItemTypes.File))
            {
                if (await change.GetStorageItemAsync() == null)
                    return;
                if (IsItemPotentialMediafile(await change.GetStorageItemAsync()))
                {
                    var newFile = await TagReaderHelper.CreateMediafile((StorageFile)await change.GetStorageItemAsync());
                    newFile.FolderPath = Path.GetDirectoryName(newFile.Path);
                    if (SharedLogic.Instance.AddMediafile(newFile))
                    {
                        await SharedLogic.Instance.NotificationManager.ShowMessageAsync(string.Format("Mediafile Added. File Path: {0}", newFile.Path), 5);
                    }
                }
            }
        }

        public static async Task RemoveItem(this StorageLibraryChange change, IEnumerable<Mediafile> Library, LibraryService LibraryService)
        {
            if (change.IsOfType(StorageItemTypes.File))
            {
                if (IsItemInLibrary(change, Library, out Mediafile movedItem))
                {
                    if (await SharedLogic.Instance.RemoveMediafile(movedItem))
                    {
                        await SharedLogic.Instance.NotificationManager.ShowMessageAsync(string.Format("Mediafile Removed. File Path: {0}", movedItem.Path), 5);
                    }
                }
            }
            else
            {
                await RemoveFolder(change, (ThreadSafeObservableCollection<Mediafile>)Library, LibraryService);
            }
        }

        public static async Task RemoveFolder(this StorageLibraryChange change, ThreadSafeObservableCollection<Mediafile> Library, LibraryService LibraryService)
        {
            int successCount = 0;
            List<Mediafile> RemovedMediafiles = new List<Mediafile>();

            //iterate all the files in the library that were in
            //the deleted folder.
            foreach (var mediaFile in await LibraryService.Query(string.IsNullOrEmpty(change.PreviousPath) ? change.Path.ToUpperInvariant() : change.PreviousPath.ToUpperInvariant()))
            {
                //verify that the file was deleted because it can be a false call.
                //we do not want to delete a file that exists.
                if (!SharedLogic.Instance.VerifyFileExists(mediaFile.Path, 200))
                {
                    RemovedMediafiles.Add(mediaFile);
                    successCount++;
                }
            }
            if (successCount > 0)
            {
                Library.RemoveRange(RemovedMediafiles);
                await LibraryService.RemoveMediafiles(RemovedMediafiles);
                await SharedLogic.Instance.NotificationManager.ShowMessageAsync(string.Format("{0} Mediafiles Removed. Folder Path: {1}", successCount, change.Path), 5);
            }
        }

        public static async Task RenameFolder(this StorageLibraryChange item, IEnumerable<Mediafile> Library, LibraryService LibraryService)
        {
            int successCount = 0;
            List<Mediafile> ChangedMediafiles = new List<Mediafile>();

            //if it's a folder, get all elements in that folder and change their directory
            foreach (var mediaFile in await LibraryService.Query(item.PreviousPath.ToUpperInvariant()))
            {
                var libraryMediafile = Library.First(t => t.Path == mediaFile.Path);

                //change the folder path.
                //NOTE: this also works for subfolders
                mediaFile.FolderPath = mediaFile.FolderPath.Replace(item.PreviousPath, item.Path);
                mediaFile.Path = mediaFile.Path.Replace(item.PreviousPath, item.Path);

                //verify that the new path exists before updating.
                if (SharedLogic.Instance.VerifyFileExists(mediaFile.Path, 200))
                {
                    successCount++;

                    //add to the list so we can update in bulk (it's faster that way.)
                    ChangedMediafiles.Add(mediaFile);

                    libraryMediafile.Path = mediaFile.Path;
                }
            }
            if (successCount > 0)
            {
                //update in bulk.
                LibraryService.UpdateMediafiles<Mediafile>(ChangedMediafiles);

                await SharedLogic.Instance.NotificationManager.ShowMessageAsync(string.Format("{0} Mediafiles Updated. Folder Path: {1}", successCount, item.Path), 5);
            }
        }
        public static async Task<bool> TryDeleteItemAsync(this IStorageItem item)
        {
            try
            {
                await item.DeleteAsync(StorageDeleteOption.PermanentDelete);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool IsItemPotentialMediafile(this IStorageItem file)
        {
            string[] mediaExtensions =
            {
                ".WAV", ".MID", ".MIDI", ".WMA", ".MP3", ".OGG", ".RMA", ".OGG",
                ".FLAC", ".M4A", ".AIF", ".AAC"
            };
            return -1 != Array.IndexOf(mediaExtensions, Path.GetExtension(file.Path).ToUpperInvariant());
        }
    }
}