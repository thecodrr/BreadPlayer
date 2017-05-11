using BreadPlayer.Web.BaiduLyricsAPI.Models;

namespace BreadPlayer.Web.BaiduLyricsAPI.Response
{
    public class SongPlayResponse : BaseResponse
    {
        public BitRate Bitrate { get; set; }
        public SongInfo Songinfo { get; set; }
    }
}
