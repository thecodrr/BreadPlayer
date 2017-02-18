using BreadPlayer.Web.BaiduLyricsAPI.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Web.BaiduLyricsAPI.Models
{
    class Artist : IQueryResult
    {
        public String artist_id;
        public String author;
        public String ting_uid;
        public String avatar_middle;
        public int album_num;
        public int song_num;
        public String country;
        public String artist_desc;
        public String artist_source;
        public String GetName()
        {
            return author;
        }
        
        public QueryType GetSearchResultType()
        {
            return QueryType.Artist;
        }
    }
}
