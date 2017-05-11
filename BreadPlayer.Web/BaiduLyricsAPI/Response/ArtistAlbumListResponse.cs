using BreadPlayer.Web.BaiduLyricsAPI.Models;
using System.Collections.Generic;

namespace BreadPlayer.Web.BaiduLyricsAPI.Response
{
    public class ArtistAlbumListResponse : BaseResponse
    {
        public List<Album> albumlist { get; set; }
        public int albumnums { get; set; }
        public int havemore { get; set; }

        public bool HasMore => havemore == 1;
    }
}
