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
        public static async Task<IEnumerable<Mediafile>> GetSongsFromFolderAsync(StorageFolder folder, bool useIndexer = true, uint stepSize = 20)
        {
            StorageFileQueryResult queryResult = folder.CreateFileQueryWithOptions(DirectoryWalker.GetQueryOptions(null, useIndexer));

            uint index = 0;
            IReadOnlyList<StorageFile> files = await queryResult.GetFilesAsync(index, stepSize);
            index += stepSize;
            if (!files.Any())
                return null;
            var count = await queryResult.GetItemCountAsync();
            if (count <= 0)
            {
                BLogger.I("No songs found.");
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
                        if (files[i]?.IsAvailable == true)
                        {
                            progress++;
                            Messenger.Instance.NotifyColleagues(MessageTypes.MsgUpdateSongCount, progress);
                            Mediafile mp3File = await TagReaderHelper.CreateMediafile(files[i], false).ConfigureAwait(false); //the core of the whole method.
                            if (mp3File != null)
                            {
                                if (files[i].Path.Length < 260)
                                {
                                    mp3File.FolderPath = Path.GetDirectoryName(files[i].Path);
                                }
                                else
                                {
                                    await SharedLogic.Instance.NotificationManager.ShowMessageAsync("The path of the file is longer than 260 characters. File: " + files[i].Path);
                                }
                                await SaveSingleFileAlbumArtAsync(mp3File, files[i]).ConfigureAwait(false);
                                SharedLogic.Instance.NotificationManager.ShowStaticMessage(progress + "\\" + count + " Song(s) Loaded");
                                tempList.Add(mp3File);
                            }
                        }
                    }
                    files = await fileTask;
                    index += stepSize;
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message + "||" + ex.InnerException;
                BLogger.E("Error while importing folder.", ex);
                await SharedLogic.Instance.NotificationManager.ShowMessageAsync(message);
            }
            return tempList.DistinctBy(f => f.OrginalFilename);
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
                        songs = await GetSongsFromFolderAsync(folder, false, 400).ConfigureAwait(false);
                    }
                    else
                        return;
                }

                Messenger.Instance.NotifyColleagues(MessageTypes.MsgImportFolder, songs);
                Messenger.Instance.NotifyColleagues(MessageTypes.MsgAddAlbums, songs);

                SharedLogic.Instance.NotificationManager.HideStaticMessage();

                string message = string.Format("Songs successfully imported!");
                BLogger.I(message);
                await SharedLogic.Instance.NotificationManager.ShowMessageAsync(message, 3);
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
                BLogger.E("Failed to save albumart.", ex);
                await SharedLogic.Instance.NotificationManager.ShowMessageAsync("Failed to save album art of " + mp3File.OrginalFilename);
            }
        }
    }
}
