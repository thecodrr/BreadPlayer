using BreadPlayer.Web.BaiduLyricsAPI.Interface;

namespace BreadPlayer.Web.BaiduLyricsAPI.Models
{
    class Artist : IQueryResult
    {
        private string author;
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
