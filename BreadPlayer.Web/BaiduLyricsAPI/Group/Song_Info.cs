using BreadPlayer.Web.BaiduLyricsAPI.Models;
using System.Collections.Generic;

namespace BreadPlayer.Web.BaiduLyricsAPI.Group
{
    public class Song_Info
    {
        public int total { get; set; }
        public List<SongInfo> song_list { get; set; }
        public Song_Info()
        {
        }
    }
}
