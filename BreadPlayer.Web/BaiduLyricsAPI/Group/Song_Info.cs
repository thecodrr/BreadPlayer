using System.Collections.Generic;
using BreadPlayer.Web.BaiduLyricsAPI.Models;

namespace BreadPlayer.Web.BaiduLyricsAPI.Group
{
    public class Song_Info
    {
        public int Total { get; set; }
        public List<SongInfo> SongList { get; set; }
    }
}
