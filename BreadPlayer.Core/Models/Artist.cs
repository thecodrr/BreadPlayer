using BreadPlayer.Interfaces;
using LiteDB;

namespace BreadPlayer.Core.Models
{
    public class Artist : ObservableObject, IDbRecord, ISelectable, IPinnable
    {
        public long Id { get; set; }
        public string Name { get; set; }
        private string picture;

        public string Picture
        {
            get => picture;
            set => Set(ref picture, value);
        }

        private string pictureColor;

        public string PictureColor
        {
            get => pictureColor;
            set => Set(ref pictureColor, value);
        }

        public string Bio { get; set; }
        public string DOB { get; set; }
        public string TextSearchKey => GetTextSearchKey().ToLower();
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
        public string TileId => "Artist=" + Id;
        private bool hasFetchedInfo;

        public bool HasFetchedInfo
        {
            get => hasFetchedInfo;
            set => Set(ref hasFetchedInfo, value);
        }

        public string GetTextSearchKey()
        {
            return "artist=" + Name;
        }
    }
}