using BreadPlayer.Web.BaiduLyricsAPI.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BreadPlayer.Web.BaiduLyricsAPI.Group
{
    public class SongInfoList
    {
        public int Total { get; set; }

        [JsonProperty("song_list")]
        public List<SongInfo> SongList { get; set; }
    }
}