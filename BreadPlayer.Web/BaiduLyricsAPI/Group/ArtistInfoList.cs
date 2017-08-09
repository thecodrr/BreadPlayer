using System.Collections.Generic;
using BreadPlayer.Web.BaiduLyricsAPI.Models;
using Newtonsoft.Json;

namespace BreadPlayer.Web.BaiduLyricsAPI.Group
{
    public class ArtistInfoList
    {
        public int Total { get; set; }
        [JsonProperty("artist_list")]
        public List<ArtistInfo> ArtistList { get; set; }
    }
}
