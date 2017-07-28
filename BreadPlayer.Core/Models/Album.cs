using System;
using BreadPlayer.Core.Common;
using Newtonsoft.Json;

namespace BreadPlayer.Core.Models
{
    public class Album : ObservableObject, IDbRecord
    {
        public long Id { get; set; }
        public string AlbumName { get; set; }
        public string Artist { get; set; }
        public string AlbumArt { get; set; }
        bool _isSelected;
        [JsonIgnore]
        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        public string TextSearchKey => GetTextSearchKey().ToLower();

        public string GetTextSearchKey()
        {
            return string.Format("{0} {1}", AlbumName, Artist);
        }
    }
}
