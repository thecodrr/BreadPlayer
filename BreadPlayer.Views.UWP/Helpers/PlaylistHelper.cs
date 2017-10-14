using BreadPlayer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using System.IO;
using BreadPlayer.Dispatcher;
using Windows.Storage;
using BreadPlayer.PlaylistBus;
using Windows.Storage.AccessCache;
using Windows.Storage.Search;
using BreadPlayer.Common;
using BreadPlayer.ViewModels;
using BreadPlayer.Core;

namespace BreadPlayer.Helpers
{
    public class PlaylistHelper
    {
        public static async Task<bool> SavePlaylist(Playlist playlist, IEnumerable<Mediafile> songs)
        {
            bool saved = false;
            FileSavePicker picker = new FileSavePicker();
            picker.FileTypeChoices.Add("PLS Playlists", new List<string> { ".pls" });
            picker.FileTypeChoices.Add("M3U Playlists", new List<string> { ".m3u" });
            picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            picker.SuggestedFileName = playlist.Name;

            await BreadDispatcher.InvokeAsync(async () =>
            {
                var file = await picker.PickSaveFileAsync();
                if (file != null)
                {
                    IPlaylist sPlaylist = null;
                    switch (file.FileType.ToLower())
                    {
                        case ".m3u":
                            sPlaylist = new M3U();
                            break;
                        case ".pls":
                            sPlaylist = new Pls();
                            break;
                    }
                    saved = await sPlaylist.SavePlaylist(songs, await file.OpenStreamForWriteAsync().ConfigureAwait(false)).ConfigureAwait(false);
                }
            });
            return saved;
        }

        private static async Task<Dictionary<string, (List<string> songs, StorageFolder folder)>> GetFoldersFromSongsAsync(List<string> songPaths)
        {
            var folderPaths = songPaths.GroupBy(t => Path.GetDirectoryName(t));
            Dictionary<string, (List<string> songs, StorageFolder folder)> folders = new Dictionary<string, (List<string> songs, StorageFolder folder)>();

            foreach (var folderPath in folderPaths)
            {
                try
                {
                    var folder = await StorageFolder.GetFolderFromPathAsync(folderPath.Key);
                    var token = StorageApplicationPermissions.FutureAccessList.Add(folder);
                    folders.Add(token, (folderPath.ToList(), folder));
                }
                catch (UnauthorizedAccessException)
                {
                    await SharedLogic.Instance.NotificationManager.ShowMessageAsync(string.Format("We cannot import songs due to access problems. Please import {0} to Bread Player library.", folderPath.Key), 5);
                }
            }
            
            return folders;
        }
        public static async Task<IEnumerable<Mediafile>> GetSongsInAllFoldersAsync(List<string> songPaths)
        {
            List<Mediafile> songsList = new List<Mediafile>();
            foreach (var folder in await GetFoldersFromSongsAsync(songPaths).ConfigureAwait(false))
            {
                if (folder.Key == null)
                    break;
                songsList.AddRange(await GetSongsInFolderAsync(folder.Value.songs, folder.Value.folder).ConfigureAwait(false));
                StorageApplicationPermissions.FutureAccessList.Remove(folder.Key);
            }
            return songsList;
        }
        private static async Task<IEnumerable<Mediafile>> GetSongsInFolderAsync(List<string> songs, StorageFolder folder)
        {
            StorageFileQueryResult queryResult = folder.CreateFileQueryWithOptions(DirectoryWalker.GetQueryOptions(null, true, FolderDepth.Shallow));
            uint index = 0, stepSize = 40;
            IReadOnlyList<StorageFile> files = await queryResult.GetFilesAsync(index, stepSize);
            index += stepSize;
            var tempList = new List<Mediafile>(songs.Count);

            // Note that I'm paging in the files as described
            while (files.Count != 0 && tempList.Count <= songs.Count)
            {
                var fileTask = queryResult.GetFilesAsync(index, stepSize).AsTask();
                for (int i = 0; i < files.Count; i++)
                {
                    try
                    {
                        if (songs.Contains(files[i].Path))
                        {
                            Mediafile mp3File = await TagReaderHelper.CreateMediafile(files[i], false).ConfigureAwait(false); //the core of the whole method.
                            if (mp3File != null)
                            {
                                mp3File.FolderPath = Path.GetDirectoryName(files[i].Path);
                                await LibraryHelper.SaveSingleFileAlbumArtAsync(mp3File, files[i]).ConfigureAwait(false);
                                tempList.Add(mp3File);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        BLogger.E("Loading of a playlist song in folder failed.", ex);
                    }
                }
                files = await fileTask.ConfigureAwait(false);
                index += stepSize;
            }

            return tempList;
        }
    }
}
