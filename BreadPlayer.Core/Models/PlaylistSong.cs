using BreadPlayer.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Models
{
    public class PlaylistSong : IDBRecord
    {
        public long Id { get; set; }
        public long SongId { get; set; }
        public long PlaylistId { get; set; }

        public string GetTextSearchKey()
        {
            return PlaylistId.ToString();
        }
    }
}
