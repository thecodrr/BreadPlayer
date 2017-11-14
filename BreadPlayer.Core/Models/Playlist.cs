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

using BreadPlayer.Core.Common;
using BreadPlayer.Interfaces;

namespace BreadPlayer.Core.Models
{
    public class Playlist : ObservableObject, IDbRecord, IPinnable
    {
        public string TextSearchKey => GetTextSearchKey().ToLower();

        private long _id;

        public long Id
        {
            get => _id;
            set => Set(ref _id, value);
        }

        private string _name;

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        private bool _isPrivate;

        public bool IsPrivate
        {
            get => _isPrivate;
            set => Set(ref _isPrivate, value);
        }

        private string _description;

        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }

        private string _hash;

        public string Hash
        {
            get => _hash;
            set => Set(ref _hash, value);
        }

        private string _salt;

        public string Salt
        {
            get => _salt;
            set => Set(ref _salt, value);
        }

        public bool IsExternal { get; set; }
        public string Path { get; set; }
        private string imagePath;

        public string ImagePath
        {
            get => imagePath;
            set => Set(ref imagePath, value);
        }

        private string imageColor;

        public string ImageColor
        {
            get => imageColor;
            set => Set(ref imageColor, value);
        }

        private string duration;

        public string Duration
        {
            get => duration;
            set => Set(ref duration, value);
        }
        private string songsCount;

        public string SongsCount
        {
            get => songsCount;
            set => Set(ref songsCount, value);
        }
        private bool _isPinned;
        public bool IsPinned
        {
            get => _isPinned;
            set => Set(ref _isPinned, value);
        }
        public string TileId => "Playlist=" + Id;
        public string GetTextSearchKey()
        {
            return Name;
        }
    }
}