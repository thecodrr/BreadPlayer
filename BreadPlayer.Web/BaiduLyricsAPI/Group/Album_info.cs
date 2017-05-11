using System.Collections.Generic;
using BreadPlayer.Web.BaiduLyricsAPI.Models;

namespace BreadPlayer.Web.BaiduLyricsAPI.Group
{
    public class Album_info
    {
        public int Total { get; set; }
        public List<AlbumInfo> AlbumList { get; set; }
    }
}
