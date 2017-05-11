using BreadPlayer.Web.BaiduLyricsAPI.Interface;

namespace BreadPlayer.Web.BaiduLyricsAPI.Models
{
    internal class Artist : IQueryResult
    {
        private string _author;
        public string GetName()
        {
            return _author;
        }        
        public QueryType GetSearchResultType()
        {
            return QueryType.Artist;
        }
    }
}
