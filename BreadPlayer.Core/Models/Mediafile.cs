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
using LiteDB;
using Newtonsoft.Json;
using System;

namespace BreadPlayer.Core.Models
{
    public class Mediafile : ObservableObject, IComparable<IMediafile>, IDbRecord, ISelectable, IPinnable, IMediafile
    {
        #region Fields

        private PlayerState _state = PlayerState.Stopped;
        private bool _isSelected = false;
        private string _path;
        private string _encryptedMetaFile;
        private string _attachedPicture;
        private string _comment;
        private string _folderPath;
        private string _synchronizedLyric;
        private string _album;
        private string _beatsperminutes;
        private string _composer;
        private string _genre;
        private string _copyrightMessage;
        private string _date;
        private string _encodedBy;
        private string _lyric;
        private string _contentGroupDescription;
        private string _title;
        private string _subtitle;
        private string _length;
        private string _orginalFilename;
        private string _leadArtist;
        private string _publisher;
        private int _trackNumber;
        private string _size;
        private string _year;
        private string _naN = "NaN";
        private int _playCount;

        #endregion Fields

        #region Properties
        private bool _isPinned;
        public bool IsPinned
        {
            get => _isPinned;
            set => Set(ref _isPinned, value);
        }
        public string TileId => "Mediafile=" + Id;
        public long Id { get; set; }
        private DateTime _lastPlayed;
        public DateTime LastPlayed { get => _lastPlayed; set => Set(ref _lastPlayed, value); }

        private DateTime _addedDate;
        public DateTime AddedDate { get => _addedDate; set => Set(ref _addedDate, value); }
        private bool _isFavorite;

        public bool IsFavorite
        {
            get => _isFavorite;
            set => Set(ref _isFavorite, value);
        }

        private bool _isPlaylistSong;

        public bool IsPlaylistSong
        {
            get => _isPlaylistSong;
            set => Set(ref _isPlaylistSong, value);
        }
        public int PlayCount { get => _playCount; set => Set(ref _playCount, value); }
        public string Path { get => _path; set => Set(ref _path, value); }

        //public long Id { get => id; set => Set(ref id, value); }
        public string AttachedPicture { get => _attachedPicture; set => Set(ref _attachedPicture, value); }

        public string FolderPath { get => _folderPath; set => _folderPath = string.IsNullOrEmpty(value) ? _folderPath = "" : value; }
        public string Album
        {
            get => _album;
            set => Set(ref _album, value);
        }
        public string Genre { get => _genre; set => _genre = string.IsNullOrEmpty(value) ? _genre = "Other" : value; }
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }
        public int TrackNumber
        {
            get => _trackNumber;
            set => _trackNumber = value;
        }
        public string Year { get => _year; set => _year = value == "0" || string.IsNullOrEmpty(value) ? "" : value; }
        public string LeadArtist
        {
            get => _leadArtist;
            set => Set(ref _leadArtist, value);
        }
        public string OrginalFilename { get => _orginalFilename; set => _orginalFilename = string.IsNullOrEmpty(value) ? _orginalFilename = _naN : value; }
        public string Length { get => _length; set => _length = string.IsNullOrEmpty(value) ? _length = _naN : value; }
        public string SynchronizedLyric
        {
            get => _synchronizedLyric;
            set => _synchronizedLyric = value;
        }
        public MediaLocationType MediaLocation { get; set; }
        #region JsonIgnore Properties
        [BsonIgnore]
        [JsonIgnore]
        public byte[] AttachedPictureBytes { get; set; }
        [BsonIgnore]
        [JsonIgnore]
        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }
        [BsonIgnore]
        [JsonIgnore]
        public byte[] ByteArray { get; set; }
        [BsonIgnore]
        [JsonIgnore]
        public string Comment { get => _comment; set => _comment = string.IsNullOrEmpty(value) ? _comment = _naN : value; }

        [BsonIgnore]
        [JsonIgnore]
        public PlayerState State { get => _state; set => Set(ref _state, value); }

        [BsonIgnore]
        [JsonIgnore]
        public string EncryptedMetaFile { get => _encryptedMetaFile; set => _encryptedMetaFile = string.IsNullOrEmpty(value) ? _encryptedMetaFile = _naN : value; }

        [BsonIgnore]
        [JsonIgnore]
        public string Size { get => _size; set => _size = string.IsNullOrEmpty(value) ? _size = _naN : value; }

        [BsonIgnore]
        [JsonIgnore]
        public string Publisher { get => _publisher; set => _publisher = string.IsNullOrEmpty(value) ? _publisher = _naN : value; }

        [BsonIgnore]
        [JsonIgnore]
        public string Subtitle { get => _subtitle; set => _subtitle = string.IsNullOrEmpty(value) ? _subtitle = _naN : value; }

        [BsonIgnore]
        [JsonIgnore]
        public string CopyrightMessage { get => _copyrightMessage; set => _copyrightMessage = string.IsNullOrEmpty(value) ? _copyrightMessage = _naN : value; }

        [BsonIgnore]
        [JsonIgnore]
        public string Date { get => _date; set => _date = string.IsNullOrEmpty(value) ? _date = _naN : value; }

        [BsonIgnore]
        [JsonIgnore]
        public string EncodedBy { get => _encodedBy; set => _encodedBy = string.IsNullOrEmpty(value) ? _encodedBy = _naN : value; }

        [BsonIgnore]
        [JsonIgnore]
        public string Lyric { get => _lyric; set => _lyric = string.IsNullOrEmpty(value) ? _lyric = _naN : value; }

        [BsonIgnore]
        [JsonIgnore]
        public string ContentGroupDescription { get => _contentGroupDescription; set => _contentGroupDescription = string.IsNullOrEmpty(value) ? _contentGroupDescription = _naN : value; }

        [BsonIgnore]
        [JsonIgnore]
        public string BeatsPerMinutes { get => _beatsperminutes; set => _beatsperminutes = string.IsNullOrEmpty(value) ? _beatsperminutes = _naN : value; }

        [BsonIgnore]
        [JsonIgnore]
        public string Composer { get => _composer; set => _composer = string.IsNullOrEmpty(value) ? _composer = _naN : value; }

        #endregion JsonIgnore Properties

        #endregion Properties

        public string TextSearchKey => GetTextSearchKey().ToLower();

        public int CompareTo(IMediafile compareTo)
        {
            return Title.CompareTo(compareTo.Title);
        }

        public string GetTextSearchKey()
        {
            return string.Format("id={0}&title={1}&album={2}&artist={3}&year={4}&genre={5}&folderpath={6}", Id, Title, Album, LeadArtist, Year, Genre, FolderPath?.ToUpperInvariant());
        }
    }
}