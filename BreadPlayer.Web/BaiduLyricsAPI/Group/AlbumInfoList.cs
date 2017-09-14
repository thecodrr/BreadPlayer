using System.Collections.Generic;
using BreadPlayer.Web.BaiduLyricsAPI.Models;
using Newtonsoft.Json;

namespace BreadPlayer.Web.BaiduLyricsAPI.Group
{
    public class AlbumInfoList
    {
        public int Total { get; set; }
        [JsonProperty("album_list")]
        public List<AlbumInfo> AlbumList { get; set; }
    }
}
