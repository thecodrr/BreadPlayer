using Newtonsoft.Json;

namespace BreadPlayer.Web.BaiduLyricsAPI.Models
{
    public class ArtistInfo
    {
        [JsonProperty("ting_uid")]
        public string TingUid { get; set; }

        [JsonProperty("song_num")]
        public int SongNum { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("avatar_middle")]
        public string AvatarMiddle { get; set; }

        [JsonProperty("album_num")]
        public int AlbumNum { get; set; }

        [JsonProperty("artist_desc")]
        public string ArtistDesc { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("artist_source")]
        public string ArtistSource { get; set; }

        [JsonProperty("artist_id")]
        public string ArtistId { get; set; }
    }
}