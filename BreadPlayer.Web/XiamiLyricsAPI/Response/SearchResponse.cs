using BreadPlayer.Web.XiamiLyricsAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Web.XiamiLyricsAPI.Responses
{
    public class SearchResponse
    {

        [JsonProperty("state")]
        public int State { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("request_id")]
        public string RequestId { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }
    }

}
