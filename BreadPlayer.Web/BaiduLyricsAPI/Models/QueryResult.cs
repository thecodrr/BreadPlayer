using BreadPlayer.Web.BaiduLyricsAPI.Group;

namespace BreadPlayer.Web.BaiduLyricsAPI.Models
{
    public class QueryResult
    {
        public const int SearchTypeAll = -1;
        public const int SearchTypeSong = 1;
        public const int SearchTypeArtist = 2;
        public const int SearchTypeAlbum = 3;

        /**
         * query : 七里香
         * syn_words :
         * rqt_type : 1
         */
        public string Query { get; set; }
        public string SynWords { get; set; }
        public int RqtType { get; set; }   //专辑3 歌手2 歌曲1
        public Song_Info SongInfo { get; set; }
        public Album_info AlbumInfo { get; set; }
        public Artist_Info ArtistInfo { get; set; }
    }
}
