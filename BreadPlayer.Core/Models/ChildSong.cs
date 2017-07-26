using BreadPlayer.Core.Common;

namespace BreadPlayer.Core.Models
{
    public class ChildSong : IDbRecord
    {
        public long Id { get; set; }
        public long SongId { get; set; }
        public long PlaylistId { get; set; }
        public string TextSearchKey => GetTextSearchKey();

        public string GetTextSearchKey()
        {
            return string.Format("pId={0};songid={1}", PlaylistId, SongId);
        }
    }
}
