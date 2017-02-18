using BreadPlayer.Web.BaiduLyricsAPI.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Web.BaiduLyricsAPI.Models
{
    public class ArtistSug : IQueryResult
    {
        public string artistid { get; set; }
        public string artistname { get; set; }
        public string artistpic { get; set; }
        public string yyr_artist { get; set; }
        public String GetName()
        {
            return artistname;
        }
        public QueryType GetSearchResultType()
        {
            return QueryType.Artist;
        }
    }
}
