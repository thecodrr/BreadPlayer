using BreadPlayer.Database;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System;

namespace BreadPlayer.Models
{
    public class Album : IDBRecord
    {
        public long Id { get; set; }
        public string AlbumName { get; set; }
        public string Artist { get; set; }
        public string AlbumArt { get; set; }
        public long[] SongsIds { get; set; }
        [JsonIgnore]
        public ObservableCollection<Mediafile> AlbumSongs { get; set; } = new ObservableCollection<Mediafile>();

        public string GetTextSearchKey()
        {
            return string.Format("{0} {1}", AlbumName, Artist);
        }
    }
}
