using System.Collections.Generic;
using BreadPlayer.Web.BaiduLyricsAPI.Models;
using Newtonsoft.Json;

namespace BreadPlayer.Web.BaiduLyricsAPI.Response
{
    public class SongListResponse : BaseResponse
    {
        [JsonProperty("songlist")]
        private List<SongInfo> Songlist { get; set; }
    }
}
