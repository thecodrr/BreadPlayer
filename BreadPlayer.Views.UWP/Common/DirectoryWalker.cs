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

using Windows.Storage.FileProperties;
using Windows.Storage.Search;

namespace BreadPlayer.Common
{
    internal static class DirectoryWalker
    {
        public static QueryOptions GetQueryOptions(string aqsQuery = null)
        {
            QueryOptions options = new QueryOptions(CommonFileQuery.DefaultQuery,
                new[] { ".mp3", ".wav", ".ogg", ".flac", ".m4a", ".aif", ".wma" });
            options.FolderDepth = FolderDepth.Deep;
            options.SetThumbnailPrefetch(ThumbnailMode.MusicView, 300, ThumbnailOptions.UseCurrentScale);
            options.SetPropertyPrefetch(PropertyPrefetchOptions.MusicProperties, new[] { "System.Music.AlbumTitle", "System.Music.Artist", "System.Music.Genre" });
            if(aqsQuery != null)
            {
                options.ApplicationSearchFilter += "kind:music " + aqsQuery;
            }

            return options;
        }
    }
}
