using BreadPlayer.Web.BaiduLyricsAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Web.BaiduLyricsAPI.Response
{
    public class ArtistSongListResponse : BaseResponse
    {
        public List<Song> songlist { get; set; }
        public String songnums { get; set; }
        public int havemore { get; set; }

        public bool HasMore
        {
            get
            {
                return havemore == 1;
            }
        }
    }
}
