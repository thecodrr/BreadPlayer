using BreadPlayer.Core.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Core.Models
{
    public class Artist : ObservableObject, IDbRecord
    {
        public long Id { get; set; }
        public string Name { get; set; }
        string picture;
        public string Picture
        {
            get => picture;
            set => Set(ref picture, value);
        }
        public string Bio { get; set; }
        public string DOB { get; set; }
        public string TextSearchKey => GetTextSearchKey();
        bool _isSelected;
        [JsonIgnore]
        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }
        public string GetTextSearchKey()
        {
            return Name;
        }
    }
}
