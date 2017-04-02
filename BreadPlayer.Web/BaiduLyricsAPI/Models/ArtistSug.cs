using BreadPlayer.Web.BaiduLyricsAPI.Interface;
using System;

namespace BreadPlayer.Web.BaiduLyricsAPI.Models
{
    public class ArtistSug : IQueryResult
    {
        public string artistid { get; set; }
        public string artistname { get; set; }
        public string artistpic { get; set; }
        public string yyr_artist { get; set; }
        public string GetName()
        {
            return artistname;
        }
        public QueryType GetSearchResultType()
        {
            return QueryType.Artist;
        }
    }
}
