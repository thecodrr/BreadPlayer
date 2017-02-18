using BreadPlayer.Web.BaiduLyricsAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
