using BreadPlayer.Web.BaiduLyricsAPI.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BreadPlayer.Web.BaiduLyricsAPI.Response
{
    public class SongListResponse : BaseResponse
    {
        [JsonProperty("songlist")]
        private List<SongInfo> songlist { get; set; }
    }
}
