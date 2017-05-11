using BreadPlayer.Core.Common;

namespace BreadPlayer.Core.Models
{
    public class Album : IDbRecord
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
