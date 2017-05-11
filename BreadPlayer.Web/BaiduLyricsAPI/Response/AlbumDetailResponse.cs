using System.Collections.Generic;
using BreadPlayer.Web.BaiduLyricsAPI.Models;

namespace BreadPlayer.Web.BaiduLyricsAPI.Response
{
    public class AlbumDetailResponse : BaseResponse
    {
        public AlbumInfo AlbumInfo { get; set; }
        public List<Song> Songlist { get; set; }
    }
}
