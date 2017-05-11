using BreadPlayer.Web.BaiduLyricsAPI.Interface;

namespace BreadPlayer.Web.BaiduLyricsAPI.Models
{
    public class Album : IQueryResult
    {
        public string album_id { get; set; }
        public string author { get; set; }
        public string title { get; set; }
        public string publishcompany { get; set; }
        public string prodcompany { get; set; }
        public string country { get; set; }
        public string language { get; set; }
        public int songs_total { get; set; }
        public string info { get; set; }
        public string styles { get; set; }
        public string style_id { get; set; }
        public string publishtime { get; set; }
        public string artist_ting_uid { get; set; }
        public string all_artist_ting_uid { get; set; }
        public string gender { get; set; }
        public string area { get; set; }
        public string pic_small { get; set; }
        public string pic_big { get; set; }
        public int hot { get; set; }
        public int favorites_num { get; set; }
        public int recommend_num { get; set; }
        public string artist_id { get; set; }
        public string all_artist_id { get; set; }
        public string pic_radio { get; set; }
        public string pic_s180 { get; set; }
        public string GetName()
        {
            return title;
        }
        public QueryType GetSearchResultType()
        {
            return QueryType.Album;
        }
    }
}
