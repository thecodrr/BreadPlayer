using BreadPlayer.Web.BaiduLyricsAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


