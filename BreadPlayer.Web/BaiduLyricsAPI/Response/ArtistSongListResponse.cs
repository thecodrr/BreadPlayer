using BreadPlayer.Web.BaiduLyricsAPI.Models;
using System;
using System.Collections.Generic;

namespace BreadPlayer.Web.BaiduLyricsAPI.Response
{
    public class ArtistSongListResponse : BaseResponse
    {
        public List<Song> songlist { get; set; }
        public string songnums { get; set; }
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
