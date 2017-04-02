using BreadPlayer.Web.BaiduLyricsAPI.Models;
using System.Collections.Generic;

namespace BreadPlayer.Web.BaiduLyricsAPI.Group
{
    public class Artist_Info
    {
        public int total { get; set; }
        public List<ArtistInfo> artist_list { get; set; }
        public Artist_Info()
        {
        }
    }
}
