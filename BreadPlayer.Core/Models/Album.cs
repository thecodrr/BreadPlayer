using BreadPlayer.Interfaces;
using LiteDB;
using System;

namespace BreadPlayer.Core.Models
{
    public class Album : ObservableObject, IDbRecord, IComparable<Album>, ISelectable, IPinnable
    {
        public long Id { get; set; }
        public string AlbumName { get; set; }
        public string Artist { get; set; }
        public string AlbumArt { get; set; }
        private bool _isSelected;

        [BsonIgnore]
        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        private bool _isPinned;
        public bool IsPinned
        {
            get => _isPinned;
            set => Set(ref _isPinned, value);
        }
        public string TileId => "Album=" + Id;
        public string TextSearchKey => GetTextSearchKey().ToLower();

        public string GetTextSearchKey()
        {
            return string.Format("album={0}&artist={1}", AlbumName, Artist);
        }

        public int CompareTo(Album other)
        {
            return this.AlbumName.CompareTo(other.AlbumName);
        }
    }
}