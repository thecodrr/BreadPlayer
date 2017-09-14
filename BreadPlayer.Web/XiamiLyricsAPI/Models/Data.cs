using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
