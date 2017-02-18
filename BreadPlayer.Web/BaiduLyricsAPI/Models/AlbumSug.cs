using BreadPlayer.Web.BaiduLyricsAPI.Interface;

namespace BreadPlayer.Web.BaiduLyricsAPI.Models
{
    public class AlbumSug : IQueryResult
    {
        public string albumid { get; set; }
        public string albumname { get; set; }
        public string artistname { get; set; }
        public string artistpic { get; set; }
      
        public string GetName()
        {
            return albumname;
        }
        
        public QueryType GetSearchResultType()
        {
            return QueryType.Album;
        }
    }
}
