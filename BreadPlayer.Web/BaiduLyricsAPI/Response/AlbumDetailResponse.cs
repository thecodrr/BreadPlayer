using BreadPlayer.Web.BaiduLyricsAPI.Models;
using System.Collections.Generic;

namespace BreadPlayer.Web.BaiduLyricsAPI.Response
{
    public class AlbumDetailResponse : BaseResponse
    {
        public AlbumInfo albumInfo { get; set; }
        public List<Song> songlist { get; set; }
    }
}
