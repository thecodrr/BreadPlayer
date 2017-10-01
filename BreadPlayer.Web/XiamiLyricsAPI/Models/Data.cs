using Newtonsoft.Json;
using System.Collections.Generic;

namespace BreadPlayer.Web.XiamiLyricsAPI.Models
{
    public class Data
    {
        [JsonProperty("trackList")]
        public IList<TrackList> TrackList { get; set; }

        [JsonProperty("songs")]
        public IList<Song> Songs { get; set; }
    }
}