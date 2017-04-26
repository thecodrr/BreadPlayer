using BreadPlayer.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Models
{
    public class ChildSong : IDBRecord
    {
        public long Id { get; set; }
        public long SongId { get; set; }
        public long PlaylistId { get; set; }

        public string GetTextSearchKey()
        {
            return string.Format("pId={0};songid={1}", PlaylistId, SongId);
        }
    }
}
