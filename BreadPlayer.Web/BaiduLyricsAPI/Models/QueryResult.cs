using BreadPlayer.Web.BaiduLyricsAPI.Group;
using Newtonsoft.Json;

namespace BreadPlayer.Web.BaiduLyricsAPI.Models
{
    public class QueryResult
    {
        public const int SearchTypeAll = -1;
        public const int SearchTypeSong = 1;
        public const int SearchTypeArtist = 2;
        public const int SearchTypeAlbum = 3;

        [JsonProperty("query")]
        public string Query { get; set; }

        [JsonProperty("syn_words")]
        public string SynWords { get; set; }

        [JsonProperty("rqt_type")]
        public int RqtType { get; set; }   //专辑3 歌手2 歌曲1

        [JsonProperty("song_info")]
        public SongInfoList SongInfo { get; set; }

        [JsonProperty("album_info")]
        public AlbumInfoList AlbumInfo { get; set; }

        [JsonProperty("artist_info")]
        public ArtistInfoList ArtistInfo { get; set; }
    }
}