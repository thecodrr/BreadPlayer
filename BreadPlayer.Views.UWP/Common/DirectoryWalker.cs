/* 
	BreadPlayer. A music player made for Windows 10 store.
    Copyright (C) 2016  theweavrs (Abdullah Atta)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;
using BreadPlayer.ViewModels;

namespace BreadPlayer.Common
{
    internal class DirectoryWalker
    {
        public static QueryOptions GetQueryOptions(string aqsQuery = null)
        {
            QueryOptions options = new QueryOptions(CommonFileQuery.OrderByName, new[] { ".mp3" });
            options.FileTypeFilter.Add(".wav");
            options.FileTypeFilter.Add(".ogg");
            options.FileTypeFilter.Add(".flac");
            options.FileTypeFilter.Add(".m4a");
            options.FileTypeFilter.Add(".aif");
            options.FileTypeFilter.Add(".wma");
            options.FolderDepth = FolderDepth.Deep;
            options.SetThumbnailPrefetch(ThumbnailMode.MusicView, 300, ThumbnailOptions.UseCurrentScale);
            options.IndexerOption = IndexerOption.UseIndexerWhenAvailable;
            options.SetPropertyPrefetch(PropertyPrefetchOptions.MusicProperties, new[] { "System.Music.AlbumTitle", "System.Music.Artist", "System.Music.Genre" });
            if(aqsQuery != null)
            {
                options.ApplicationSearchFilter += "kind:music " + aqsQuery;
            }

            return options;
        }
        public static async Task<List<StorageFile>> GetModifiedFiles(IEnumerable<StorageFolder> folderCollection, string timeModified)
        {
            List<StorageFile> modifiedFiles = new List<StorageFile>();        
            foreach (var folder in folderCollection)
            {
                StorageFileQueryResult modifiedqueryResult = folder.CreateFileQueryWithOptions(GetQueryOptions("datemodified:> " + timeModified));
                if (await modifiedqueryResult.GetItemCountAsync() > 0)
                {
                    modifiedFiles.AddRange(await modifiedqueryResult.GetFilesAsync());
                }
            }
            return modifiedFiles;
        }
        public async static void SetupDirectoryWatcher(IEnumerable<StorageFolder> folderCollection)
        {
            foreach (var folder in folderCollection)
            {
                StorageFileQueryResult queryResult = folder.CreateFileQueryWithOptions(GetQueryOptions());
                var files = await queryResult.GetItemCountAsync();
                queryResult.ContentsChanged += QueryResult_ContentsChanged; ;
            }
        }

        private async static void QueryResult_ContentsChanged(IStorageQueryResultBase sender, object args)
        {
            await SettingsViewModel.PerformWatcherWorkAsync(sender.Folder).ConfigureAwait(false);
        }
    }
}
