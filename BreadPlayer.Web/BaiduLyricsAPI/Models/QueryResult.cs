using BreadPlayer.Web.BaiduLyricsAPI.Group;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Web.BaiduLyricsAPI.Models
{
    public class QueryResult
    {
        public const int SEARCH_TYPE_ALL = -1;
        public const int SEARCH_TYPE_SONG = 1;
        public const int SEARCH_TYPE_ARTIST = 2;
        public const int SEARCH_TYPE_ALBUM = 3;

        /**
         * query : 七里香
         * syn_words :
         * rqt_type : 1
         */
        public String query { get; set; }
        public String syn_words { get; set; }
        public int rqt_type { get; set; }   //专辑3 歌手2 歌曲1
        public Song_Info song_info { get; set; }
        public Album_info album_info { get; set; }
        public Artist_Info artist_info { get; set; }
    }
}
