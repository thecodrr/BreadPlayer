using System.Collections.Generic;
using Newtonsoft.Json;

namespace BreadPlayer.Web.BaiduLyricsAPI.Models
{
    public class AlbumInfo
    {
        public string AlbumId { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Publishcompany { get; set; }
        public string Prodcompany { get; set; }
        public string Country { get; set; }
        public string Language { get; set; }
        public string SongsTotal { get; set; }
        public string Info { get; set; }
        public string Styles { get; set; }
        public string StyleId { get; set; }
        public string Publishtime { get; set; }
        public string ArtistTingUid { get; set; }
        public object AllArtistTingUid { get; set; }
        public string Gender { get; set; }
        public string Area { get; set; }
        public string PicSmall { get; set; }
        public string PicBig { get; set; }
        public string Hot { get; set; }
        public object FavoritesNum { get; set; }
        public object RecommendNum { get; set; }
        public string ArtistId { get; set; }
        public string AllArtistId { get; set; }
        public string PicRadio { get; set; }
        public string PicS500 { get; set; }
        public string PicS1000 { get; set; }
        [JsonProperty("songlist")]
        private List<SongInfo> Songlist { get; set; }
    }
}
