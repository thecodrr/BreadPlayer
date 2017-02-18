using BreadPlayer.Web.BaiduLyricsAPI.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Web.BaiduLyricsAPI.Models
{
    public class Album : IQueryResult
    {
        public String album_id { get; set; }
        public String author { get; set; }
        public String title { get; set; }
        public String publishcompany { get; set; }
        public String prodcompany { get; set; }
        public String country { get; set; }
        public String language { get; set; }
        public int songs_total { get; set; }
        public String info { get; set; }
        public String styles { get; set; }
        public String style_id { get; set; }
        public String publishtime { get; set; }
        public String artist_ting_uid { get; set; }
        public String all_artist_ting_uid { get; set; }
        public String gender { get; set; }
        public String area { get; set; }
        public String pic_small { get; set; }
        public String pic_big { get; set; }
        public int hot { get; set; }
        public int favorites_num { get; set; }
        public int recommend_num { get; set; }
        public String artist_id { get; set; }
        public String all_artist_id { get; set; }
        public String pic_radio { get; set; }
        public String pic_s180 { get; set; }
        public String GetName()
        {
            return title;
        }
        public QueryType GetSearchResultType()
        {
            return QueryType.Album;
        }

    }
}
