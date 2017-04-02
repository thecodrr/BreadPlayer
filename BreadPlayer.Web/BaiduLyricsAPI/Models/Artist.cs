using BreadPlayer.Web.BaiduLyricsAPI.Interface;

namespace BreadPlayer.Web.BaiduLyricsAPI.Models
{
    class Artist : IQueryResult
    {
        public string artist_id;
        public string author;
        public string ting_uid;
        public string avatar_middle;
        public int album_num;
        public int song_num;
        public string country;
        public string artist_desc;
        public string artist_source;
        public string GetName()
        {
            return author;
        }
        
        public QueryType GetSearchResultType()
        {
            return QueryType.Artist;
        }
    }
}
