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

using BreadPlayer.Helpers;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;

namespace BreadPlayer.Common
{
    internal static class DirectoryWalker
    {
        public static QueryOptions GetQueryOptions(string aqsQuery = null, bool useIndexer = true, FolderDepth folderDepth = FolderDepth.Deep)
        {
            QueryOptions options = new QueryOptions(CommonFileQuery.OrderByName,
                new[] { ".mp3", ".wav", ".ogg", ".flac", ".m4a", ".aif", ".wma", ".aac", ".mp4" });
            options.FolderDepth = folderDepth;
            options.IndexerOption = useIndexer ? IndexerOption.UseIndexerWhenAvailable : IndexerOption.DoNotUseIndexer;
            options.SetThumbnailPrefetch(ThumbnailMode.MusicView, 512, ThumbnailOptions.ReturnOnlyIfCached);
            options.SetPropertyPrefetch(PropertyPrefetchOptions.MusicProperties, TagReaderHelper.GetExtraPropertiesNames());
            //options.ApplicationSearchFilter += "System.Kind:=System.Kind#Music" + aqsQuery;

            return options;
        }
    }
}