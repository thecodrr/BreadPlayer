using BreadPlayer.Core.Common;
using BreadPlayer.Interfaces;

namespace BreadPlayer.Core.Models
{
    public class ChildSong : IDbRecord
    {
        public long Id { get; set; }
        public long SongId { get; set; }
        public long PlaylistId { get; set; }
        public string TextSearchKey => GetTextSearchKey().ToLower();

        public string GetTextSearchKey()
        {
            return string.Format("pid={0}songid={1}", PlaylistId, SongId);
        }
    }
}