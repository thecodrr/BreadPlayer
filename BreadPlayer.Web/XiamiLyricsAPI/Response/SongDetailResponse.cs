using BreadPlayer.Web.XiamiLyricsAPI.Models;
using Newtonsoft.Json;

namespace BreadPlayer.Web.XiamiLyricsAPI.Responses
{
    public class SongDetailResponse
    {
        [JsonProperty("data")]
        public Data Data { get; set; }
    }
}