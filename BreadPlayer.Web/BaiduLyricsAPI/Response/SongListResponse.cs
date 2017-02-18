using BreadPlayer.Web.BaiduLyricsAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Web.BaiduLyricsAPI.Response
{
    public class SongListResponse : BaseResponse
    {
        [JsonProperty("songlist")]
        private List<SongInfo> songlist { get; set; }
    }
}
