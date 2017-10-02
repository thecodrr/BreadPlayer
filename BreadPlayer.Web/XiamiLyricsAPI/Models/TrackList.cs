using Newtonsoft.Json;
using System.Collections.Generic;

namespace BreadPlayer.Web.XiamiLyricsAPI.Models
{
    public class TrackList
    {
        [JsonProperty("songId")]
        public string SongId { get; set; }

        [JsonProperty("songName")]
        public string SongName { get; set; }

        [JsonProperty("subName")]
        public string SubName { get; set; }

        [JsonProperty("albumId")]
        public int AlbumId { get; set; }

        [JsonProperty("artistId")]
        public int ArtistId { get; set; }

        [JsonProperty("singers")]
        public string Singers { get; set; }

        [JsonProperty("mvId")]
        public int MvId { get; set; }

        [JsonProperty("cdSerial")]
        public int CdSerial { get; set; }

        [JsonProperty("track")]
        public int Track { get; set; }

        [JsonProperty("pinyin")]
        public string Pinyin { get; set; }

        [JsonProperty("bakSongId")]
        public int BakSongId { get; set; }

        [JsonProperty("panFlag")]
        public int PanFlag { get; set; }

        [JsonProperty("musicType")]
        public string MusicType { get; set; }

        [JsonProperty("bakSong")]
        public object BakSong { get; set; }

        [JsonProperty("songwriters")]
        public string Songwriters { get; set; }

        [JsonProperty("composer")]
        public string Composer { get; set; }

        [JsonProperty("arrangement")]
        public string Arrangement { get; set; }

        [JsonProperty("boughtCount")]
        public int BoughtCount { get; set; }

        [JsonProperty("pace")]
        public int Pace { get; set; }

        [JsonProperty("albumLanguage")]
        public string AlbumLanguage { get; set; }

        [JsonProperty("playCount")]
        public int PlayCount { get; set; }

        [JsonProperty("offline")]
        public bool Offline { get; set; }

        [JsonProperty("downloadCount")]
        public bool DownloadCount { get; set; }

        [JsonProperty("originOffline")]
        public bool OriginOffline { get; set; }

        [JsonProperty("recommendCount")]
        public int RecommendCount { get; set; }

        [JsonProperty("collectCount")]
        public int CollectCount { get; set; }

        [JsonProperty("songStringId")]
        public string SongStringId { get; set; }

        [JsonProperty("albumStringId")]
        public string AlbumStringId { get; set; }

        [JsonProperty("artistStringId")]
        public string ArtistStringId { get; set; }

        [JsonProperty("singerIds")]
        public IList<int> SingerIds { get; set; }

        [JsonProperty("demoCreateTime")]
        public long DemoCreateTime { get; set; }

        [JsonProperty("artist")]
        public string Artist { get; set; }

        [JsonProperty("songOpt")]
        public string SongOpt { get; set; }

        [JsonProperty("publishStatus")]
        public string PublishStatus { get; set; }

        [JsonProperty("demo")]
        public bool Demo { get; set; }

        [JsonProperty("s")]
        public int S { get; set; }

        [JsonProperty("song_id")]
        public string Song_Id { get; set; }

        [JsonProperty("album_id")]
        public int Album_Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("album_name")]
        public string AlbumName { get; set; }

        [JsonProperty("sub_title")]
        public string SubTitle { get; set; }

        [JsonProperty("song_sub_title")]
        public string SongSubTitle { get; set; }

        [JsonProperty("artist_name")]
        public string ArtistName { get; set; }

        [JsonProperty("artist_id")]
        public int Artist_Id { get; set; }

        [JsonProperty("length")]
        public int Length { get; set; }

        [JsonProperty("recommends")]
        public int Recommends { get; set; }

        [JsonProperty("collects")]
        public int Collects { get; set; }

        [JsonProperty("key_words")]
        public object KeyWords { get; set; }

        [JsonProperty("play_volume")]
        public int PlayVolume { get; set; }

        [JsonProperty("flag")]
        public object Flag { get; set; }

        [JsonProperty("album_logo")]
        public string AlbumLogo { get; set; }

        [JsonProperty("needpay")]
        public int Needpay { get; set; }

        [JsonProperty("mvUrl")]
        public string MvUrl { get; set; }

        [JsonProperty("playstatus")]
        public int Playstatus { get; set; }

        [JsonProperty("downloadstatus")]
        public int Downloadstatus { get; set; }

        [JsonProperty("downloadjson")]
        public string Downloadjson { get; set; }

        [JsonProperty("can_show")]
        public int CanShow { get; set; }

        [JsonProperty("can_check")]
        public int CanCheck { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("lyric")]
        public string Lyric { get; set; }

        [JsonProperty("lyric_url")]
        public string LyricUrl { get; set; }

        [JsonProperty("object_id")]
        public string ObjectId { get; set; }

        [JsonProperty("object_name")]
        public string ObjectName { get; set; }

        [JsonProperty("insert_type")]
        public int InsertType { get; set; }

        [JsonProperty("background")]
        public string Background { get; set; }

        [JsonProperty("aritst_type")]
        public string AritstType { get; set; }

        [JsonProperty("artist_url")]
        public string ArtistUrl { get; set; }

        [JsonProperty("grade")]
        public int Grade { get; set; }

        [JsonProperty("tryhq")]
        public int Tryhq { get; set; }

        [JsonProperty("pic")]
        public string Pic { get; set; }

        [JsonProperty("album_pic")]
        public string AlbumPic { get; set; }

        [JsonProperty("rec_note")]
        public string RecNote { get; set; }
    }
}