using System.Collections.Generic;
using BreadPlayer.Web.BaiduLyricsAPI.Models;

namespace BreadPlayer.Web.BaiduLyricsAPI.Group
{
    public class Artist_Info
    {
        public int Total { get; set; }
        public List<ArtistInfo> ArtistList { get; set; }
    }
}
