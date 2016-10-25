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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;
using Windows.UI.Core;
using Windows.ApplicationModel.Core;
using Windows.Storage.Search;
using Windows.Storage.FileProperties;

namespace BreadPlayer.Common
{
    class DirectoryWalker
    {
        public static async Task<IReadOnlyList<StorageFile>> GetFiles(string dirPath)
        {
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(dirPath);
            QueryOptions options = new QueryOptions(CommonFileQuery.OrderByName, new String[] { ".mp3", ".wav", ".ogg", ".aiff", ".flac" });
            options.FolderDepth = FolderDepth.Deep;
            options.SetThumbnailPrefetch(ThumbnailMode.MusicView, 300, ThumbnailOptions.UseCurrentScale);
            // Change to DoNotUseIndexer for trial 3
            options.IndexerOption = IndexerOption.UseIndexerWhenAvailable;      
            options.SetPropertyPrefetch(PropertyPrefetchOptions.MusicProperties, new String[] { "System.Music.AlbumTitle", "System.Music.Artist", "System.Music.Title", "System.Music.Genre", "System.Music.Year" });
            StorageFileQueryResult queryResult = folder.CreateFileQueryWithOptions(options);
            uint index = 0, stepSize = 20;
            IReadOnlyList<StorageFile> files = await queryResult.GetFilesAsync(index,stepSize);           
            return files;
        }
    }
}
