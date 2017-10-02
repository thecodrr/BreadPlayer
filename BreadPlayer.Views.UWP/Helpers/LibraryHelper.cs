using BreadPlayer.Common;
using BreadPlayer.Core;
using BreadPlayer.Core.Extensions;
using BreadPlayer.Core.Models;
using BreadPlayer.Dispatcher;
using BreadPlayer.Extensions;
using BreadPlayer.Messengers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Popups;

namespace BreadPlayer.Helpers
{
    public class LibraryHelper
    {
        /// <summary>
        /// Add folder to Library asynchronously.
        /// </summary>
        /// <param name="queryResult">The query result after querying in a specific folder.</param>
        /// <returns></returns>
        public static async Task<List<Mediafile>> GetSongsFromFolderAsync(StorageFolder folder, bool useIndexer = true)
        {
            StorageFileQueryResult queryResult = folder.CreateFileQueryWithOptions(DirectoryWalker.GetQueryOptions(null, useIndexer));

            uint index = 0, stepSize = 20;
            IReadOnlyList<StorageFile> files = await queryResult.GetFilesAsync(index, stepSize);
            index += stepSize;
            
            var count = await queryResult.GetItemCountAsync();
            if (count <= 0)
            {
                await SharedLogic.Instance.NotificationManager.ShowMessageAsync("No songs found! Please try again.");
                return null;
            }
            var tempList = new List<Mediafile>((int)count);
            short progress = 0;
            try
            {
                // Note that I'm paging in the files as described
                while (files.Count != 0)
                {
                    var fileTask = queryResult.GetFilesAsync(index, stepSize).AsTask();
                    for (int i = 0; i < files.Count; i++)
                    {
                        progress++;
                        Messenger.Instance.NotifyColleagues(MessageTypes.MsgUpdateSongCount, progress);
                        Mediafile mp3File = await TagReaderHelper.CreateMediafile(files[i], false).ConfigureAwait(false); //the core of the whole method.
                        if (mp3File != null)
                        {
                            mp3File.FolderPath = Path.GetDirectoryName(files[i].Path);
                            await SaveSingleFileAlbumArtAsync(mp3File, files[i]).ConfigureAwait(false);
                            await SharedLogic.Instance.NotificationManager.ShowMessageAsync(progress + "\\" + count + " Song(s) Loaded", 0);
                            tempList.Add(mp3File);
                        }
                    }
                    files = await fileTask;
                    index += stepSize;
                }
            }
            catch (Exception ex)
            {
                string message1 = ex.Message + "||" + ex.InnerException;
                await SharedLogic.Instance.NotificationManager.ShowMessageAsync(message1);
            }        
            return tempList;
        }
        public static async Task ImportFolderIntoLibraryAsync(StorageFolder folder)
        {
            var songs = await GetSongsFromFolderAsync(folder).ConfigureAwait(false);

            await BreadDispatcher.InvokeAsync(async () =>
            {
                if (songs == null)
                {
                    var dialog = new MessageDialog($"There were no songs in the {folder.DisplayName} folder. Do you want to try and search without using the indexer (the process might be a bit slow)?", "No songs were found!");
                    dialog.Commands.Add(new UICommand("Yes", null, "yesCmd"));
                    dialog.Commands.Add(new UICommand("No", null, "noCmd"));
                    var result = await dialog.ShowAsync();
                    if (result.Id.ToString() == "yesCmd")
                    {
                        songs = await GetSongsFromFolderAsync(folder, false).ConfigureAwait(false);
                    }
                }
                var uniqueFiles = songs.DistinctBy(f => f.OrginalFilename);

                Messenger.Instance.NotifyColleagues(MessageTypes.MsgImportFolder, uniqueFiles);
                Messenger.Instance.NotifyColleagues(MessageTypes.MsgAddAlbums, uniqueFiles);

                string message = string.Format("Songs successfully imported!");
                BLogger.Logger.Info(message);
                await SharedLogic.Instance.NotificationManager.ShowMessageAsync(message);
            });
        }
        public static async Task SaveSingleFileAlbumArtAsync(Mediafile mp3File, StorageFile file = null)
        {
            if (mp3File == null || mp3File.Path == null) return;

            try
            {
                if (file == null)
                {
                    file = await StorageFile.GetFileFromPathAsync(mp3File.Path);
                }

                var albumartFolder = ApplicationData.Current.LocalFolder;
                var albumartLocation = albumartFolder.Path + @"\AlbumArts\" + (mp3File.Album + mp3File.LeadArtist).ToLower().ToSha1() + ".jpg";

                if (!SharedLogic.Instance.VerifyFileExists(albumartLocation, 300))
                {
                    bool albumSaved = await TagReaderHelper.SaveAlbumArtsAsync(file, mp3File);
                    mp3File.AttachedPicture = albumSaved ? albumartLocation : null;
                }
                file = null;
            }
            catch (Exception ex)
            {
                BLogger.Logger.Info("Failed to save albumart.", ex);
                await SharedLogic.Instance.NotificationManager.ShowMessageAsync("Failed to save album art of " + mp3File.OrginalFilename);
            }
        }
    }
}
