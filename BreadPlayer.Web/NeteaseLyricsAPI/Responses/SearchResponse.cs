using BreadPlayer.Web.NeteaseLyricsAPI.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BreadPlayer.Web.NeteaseLyricsAPI.Responses
{
    public class Result
    {
        [JsonProperty("songs")]
        public IList<Song> Songs { get; set; }

        [JsonProperty("songCount")]
        public int SongCount { get; set; }
    }

    public class SearchResponse
    {
        [JsonProperty("result")]
        public Result Result { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }
    }
}