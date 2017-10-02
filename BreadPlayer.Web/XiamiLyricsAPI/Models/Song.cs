using Newtonsoft.Json;

namespace BreadPlayer.Web.XiamiLyricsAPI.Models
{
    public class Song
    {
        [JsonProperty("song_id")]
        public int SongId { get; set; }

        [JsonProperty("song_name")]
        public string SongName { get; set; }

        [JsonProperty("album_id")]
        public int AlbumId { get; set; }

        [JsonProperty("album_name")]
        public string AlbumName { get; set; }

        [JsonProperty("album_logo")]
        public string AlbumLogo { get; set; }

        [JsonProperty("artist_id")]
        public int ArtistId { get; set; }

        [JsonProperty("artist_name")]
        public string ArtistName { get; set; }

        [JsonProperty("artist_logo")]
        public string ArtistLogo { get; set; }

        [JsonProperty("listen_file")]
        public string ListenFile { get; set; }

        [JsonProperty("demo")]
        public int Demo { get; set; }

        [JsonProperty("need_pay_flag")]
        public int NeedPayFlag { get; set; }

        [JsonProperty("lyric")]
        public string Lyric { get; set; }

        [JsonProperty("is_play")]
        public int IsPlay { get; set; }

        [JsonProperty("play_counts")]
        public int PlayCounts { get; set; }

        [JsonProperty("singer")]
        public string Singer { get; set; }
    }
}