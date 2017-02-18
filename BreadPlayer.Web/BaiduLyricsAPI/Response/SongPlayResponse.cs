using BreadPlayer.Web.BaiduLyricsAPI.Models;

namespace BreadPlayer.Web.BaiduLyricsAPI.Response
{
    public class SongPlayResponse : BaseResponse
    {
        public BitRate bitrate { get; set; }
        public SongInfo songinfo { get; set; }
    }
}
