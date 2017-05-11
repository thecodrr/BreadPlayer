using System.Collections.Generic;
using BreadPlayer.Web.BaiduLyricsAPI.Models;

namespace BreadPlayer.Web.BaiduLyricsAPI.Response
{
    public class ArtistSongListResponse : BaseResponse
    {
        public List<Song> Songlist { get; set; }
        public string Songnums { get; set; }
        public int Havemore { get; set; }

        public bool HasMore => Havemore == 1;
    }
}
