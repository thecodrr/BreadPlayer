using Newtonsoft.Json;

namespace BreadPlayer.Web.BaiduLyricsAPI.Models
{
    public class AlbumInfo
    {
        [JsonProperty("resource_type_ext")]
        public string ResourceTypeExt { get; set; }

        [JsonProperty("all_artist_id")]
        public string AllArtistId { get; set; }

        [JsonProperty("publishtime")]
        public string Publishtime { get; set; }

        [JsonProperty("company")]
        public string Company { get; set; }

        [JsonProperty("album_desc")]
        public string AlbumDesc { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("album_id")]
        public string AlbumId { get; set; }

        [JsonProperty("pic_small")]
        public string PicSmall { get; set; }

        [JsonProperty("hot")]
        public int Hot { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("artist_id")]
        public string ArtistId { get; set; }
    }
}