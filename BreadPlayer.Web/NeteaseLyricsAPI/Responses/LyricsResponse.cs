using BreadPlayer.Web.NeteaseLyricsAPI.Models;
using Newtonsoft.Json;

namespace BreadPlayer.Web.NeteaseLyricsAPI.Responses
{
    public class LyricsResponse
    {
        [JsonProperty("sgc")]
        public bool Sgc { get; set; }

        [JsonProperty("sfy")]
        public bool Sfy { get; set; }

        [JsonProperty("qfy")]
        public bool Qfy { get; set; }

        [JsonProperty("lrc")]
        public Lrc Lrc { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }
    }
}