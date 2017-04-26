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
        public string GetTextSearchKey()
        {
            return string.Format("{0} {1}", AlbumName, Artist);
        }
    }
}
