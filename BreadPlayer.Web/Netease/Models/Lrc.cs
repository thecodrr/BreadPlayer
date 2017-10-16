using Newtonsoft.Json;

namespace BreadPlayer.Web.NeteaseLyricsAPI.Models
{
    public class Lrc
    {
        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("lyric")]
        public string Lyric { get; set; }
    }
}