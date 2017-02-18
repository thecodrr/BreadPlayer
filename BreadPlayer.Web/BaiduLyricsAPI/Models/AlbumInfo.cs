using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Web.BaiduLyricsAPI.Models
{
    public class AlbumInfo
    {
        public string album_id { get; set; }
        public string author { get; set; }
        public string title { get; set; }
        public string publishcompany { get; set; }
        public string prodcompany { get; set; }
        public string country { get; set; }
        public string language { get; set; }
        public string songs_total { get; set; }
        public string info { get; set; }
        public string styles { get; set; }
        public string style_id { get; set; }
        public string publishtime { get; set; }
        public string artist_ting_uid { get; set; }
        public object all_artist_ting_uid { get; set; }
        public string gender { get; set; }
        public string area { get; set; }
        public string pic_small { get; set; }
        public string pic_big { get; set; }
        public string hot { get; set; }
        public object favorites_num { get; set; }
        public object recommend_num { get; set; }
        public string artist_id { get; set; }
        public string all_artist_id { get; set; }
        public string pic_radio { get; set; }
        public string pic_s500 { get; set; }
        public string pic_s1000 { get; set; }
        [JsonProperty("songlist")]
        private List<SongInfo> songlist { get; set; }
    }
}
