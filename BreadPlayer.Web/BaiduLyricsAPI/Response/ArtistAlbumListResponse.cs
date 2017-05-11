using System.Collections.Generic;
using BreadPlayer.Web.BaiduLyricsAPI.Models;

namespace BreadPlayer.Web.BaiduLyricsAPI.Response
{
    public class ArtistAlbumListResponse : BaseResponse
    {
        public List<Album> Albumlist { get; set; }
        public int Albumnums { get; set; }
        public int Havemore { get; set; }

        public bool HasMore => Havemore == 1;
    }
}
