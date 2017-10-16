using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Web.Musixmatch.Models
{
    public class MacroCalls
    {
        [JsonProperty("track.subtitles.get")]
        public TrackSubtitlesGet Subtitles { get; set; }
    }
    public class TrackSubtitlesGet
    {
        public TrackSubtitlesGetMessage Message { get; set; }
    }
    public class TrackSubtitlesGetMessage
    {
        [JsonProperty("body")]
        public TrackSubtitlesGetBody Body { get; set; }
    }
    public class TrackSubtitlesGetBody
    {
        [JsonProperty("subtitle_list")]
        public List<SubtitleList> SubtitleList { get; set; }
    }
    public class SubtitleList
    {
        [JsonProperty("subtitle")]
        public Subtitle Subtitle { get; set; }
    }
    public class Subtitle
    {
        [JsonProperty("publisher_list")]
        public object[] PublisherList { get; set; }

        [JsonProperty("subtitle_id")]
        public long SubtitleId { get; set; }

        [JsonProperty("lyrics_copyright")]
        public string LyricsCopyright { get; set; }

        [JsonProperty("html_tracking_url")]
        public string HtmlTrackingUrl { get; set; }

        [JsonProperty("pixel_tracking_url")]
        public string PixelTrackingUrl { get; set; }

        [JsonProperty("script_tracking_url")]
        public string ScriptTrackingUrl { get; set; }

        [JsonProperty("restricted")]
        public long Restricted { get; set; }

        [JsonProperty("subtitle_body")]
        public string Lyrics { get; set; }

        [JsonProperty("subtitle_language_description")]
        public string SubtitleLanguageDescription { get; set; }

        [JsonProperty("updated_time")]
        public string UpdatedTime { get; set; }

        [JsonProperty("subtitle_language")]
        public string SubtitleLanguage { get; set; }

        [JsonProperty("subtitle_length")]
        public long SubtitleLength { get; set; }

        [JsonProperty("writer_list")]
        public object[] WriterList { get; set; }
    }
    public class Body
    {
        [JsonProperty("macro_calls")]
        public MacroCalls MacroCalls { get; set; }
    }
    public class Lyrics
    {
        [JsonProperty("message")]
        public Message Message { get; set; }
    }
    public class Message
    {
        [JsonProperty("body")]
        public Body Body { get; set; }
    }
}
