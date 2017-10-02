using Newtonsoft.Json;

namespace BreadPlayer.Web.BaiduLyricsAPI.Models
{
    public class SongInfo
    {
        [JsonProperty("resource_type_ext")]
        public string ResourceTypeExt { get; set; }

        [JsonProperty("has_filmtv")]
        public string HasFilmtv { get; set; }

        [JsonProperty("resource_type")]
        public int ResourceType { get; set; }

        [JsonProperty("mv_provider")]
        public string MvProvider { get; set; }

        [JsonProperty("del_status")]
        public string DelStatus { get; set; }

        [JsonProperty("havehigh")]
        public int Havehigh { get; set; }

        [JsonProperty("si_proxycompany")]
        public string SiProxycompany { get; set; }

        [JsonProperty("versions")]
        public string Versions { get; set; }

        [JsonProperty("toneid")]
        public string Toneid { get; set; }

        [JsonProperty("info")]
        public string Info { get; set; }

        [JsonProperty("has_mv")]
        public int HasMv { get; set; }

        [JsonProperty("album_title")]
        public string AlbumTitle { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("piao_id")]
        public string PiaoId { get; set; }

        [JsonProperty("artist_id")]
        public string ArtistId { get; set; }

        [JsonProperty("lrclink")]
        public string Lrclink { get; set; }

        [JsonProperty("data_source")]
        public int DataSource { get; set; }

        [JsonProperty("relate_status")]
        public int RelateStatus { get; set; }

        [JsonProperty("learn")]
        public int Learn { get; set; }

        [JsonProperty("album_id")]
        public string AlbumId { get; set; }

        [JsonProperty("biaoshi")]
        public string Biaoshi { get; set; }

        [JsonProperty("bitrate_fee")]
        public string BitrateFee { get; set; }

        [JsonProperty("song_source")]
        public string SongSource { get; set; }

        [JsonProperty("is_first_publish")]
        public int IsFirstPublish { get; set; }

        [JsonProperty("cluster_id")]
        public int ClusterId { get; set; }

        [JsonProperty("charge")]
        public int Charge { get; set; }

        [JsonProperty("copy_type")]
        public string CopyType { get; set; }

        [JsonProperty("korean_bb_song")]
        public string KoreanBbSong { get; set; }

        [JsonProperty("all_rate")]
        public string AllRate { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("has_mv_mobile")]
        public int HasMvMobile { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("pic_small")]
        public string PicSmall { get; set; }

        [JsonProperty("song_id")]
        public string SongId { get; set; }

        [JsonProperty("all_artist_id")]
        public string AllArtistId { get; set; }

        [JsonProperty("ting_uid")]
        public string TingUid { get; set; }
    }
}