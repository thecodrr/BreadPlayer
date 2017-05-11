using BreadPlayer.Web.BaiduLyricsAPI.Interface;

namespace BreadPlayer.Web.BaiduLyricsAPI.Models
{
    public class AlbumSug : IQueryResult
    {
        public string Albumid { get; set; }
        public string Albumname { get; set; }
        public string Artistname { get; set; }
        public string Artistpic { get; set; }
      
        public string GetName()
        {
            return Albumname;
        }
        
        public QueryType GetSearchResultType()
        {
            return QueryType.Album;
        }
    }
}
