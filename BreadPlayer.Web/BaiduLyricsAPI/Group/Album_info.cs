using BreadPlayer.Web.BaiduLyricsAPI.Models;
using System.Collections.Generic;

namespace BreadPlayer.Web.BaiduLyricsAPI.Group
{
    public class Album_info
    {
        public int total { get; set; }
        public List<AlbumInfo> album_list { get; set; }
        public Album_info()
        {
        }
    }
}
