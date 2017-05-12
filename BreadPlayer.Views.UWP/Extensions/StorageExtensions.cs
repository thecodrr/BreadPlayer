using BreadPlayer.Core;
using BreadPlayer.Core.Models;
using BreadPlayer.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
                    createdItem = await SharedLogic.CreateMediafile((StorageFile)await change.GetStorageItemAsync());
                    createdItem.Id = id;
                    if (await LibraryService.UpdateMediafile(createdItem))
                    {
                        await SharedLogic.NotificationManager.ShowMessageAsync(string.Format("Mediafile Updated. File Path: {0}", createdItem.Path), 5);
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
                if (IsItemPotentialMediafile(await change.GetStorageItemAsync()))
                {
                    var newFile = await SharedLogic.CreateMediafile((StorageFile)await change.GetStorageItemAsync());
                    if (SharedLogic.AddMediafile(newFile))
                    {
                        await SharedLogic.NotificationManager.ShowMessageAsync(string.Format("Mediafile Added. File Path: {0}", newFile.Path), 5);
                    }
                }
            }
        }
        public static async Task RemoveItem(this StorageLibraryChange change, IEnumerable<Mediafile> Library)
        {
            if (change.IsOfType(StorageItemTypes.File))
            {
                if (IsItemInLibrary(change, Library, out Mediafile movedItem))
                {
                    if (await SharedLogic.RemoveMediafile(movedItem))
                    {
                        await SharedLogic.NotificationManager.ShowMessageAsync(string.Format("Mediafile Removed. File Path: {0}", movedItem.Path), 5);
                    }
                }
            }
        }
        public static bool IsItemPotentialMediafile(this IStorageItem file)
        {
            string[] mediaExtensions =
            {
                ".WAV", ".MID", ".MIDI", ".WMA", ".MP3", ".OGG", ".RMA", ".OGG",
                ".FLAC", ".M4A", ".AIF"
            };
            return -1 != Array.IndexOf(mediaExtensions, Path.GetExtension(file.Path).ToUpperInvariant());
        }
    }
}
