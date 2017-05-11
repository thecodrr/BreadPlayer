using BreadPlayer.Web.BaiduLyricsAPI.Interface;

namespace BreadPlayer.Web.BaiduLyricsAPI.Models
{
    public class ArtistSug : IQueryResult
    {
        public string Artistid { get; set; }
        public string Artistname { get; set; }
        public string Artistpic { get; set; }
        public string YyrArtist { get; set; }
        public string GetName()
        {
            return Artistname;
        }
        public QueryType GetSearchResultType()
        {
            return QueryType.Artist;
        }
    }
}
