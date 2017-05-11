using System.Collections.Generic;
using BreadPlayer.Web.BaiduLyricsAPI.Models;

namespace BreadPlayer.Web.BaiduLyricsAPI.Response
{
    public class MusicSearchSuggestionsResponse : BaseResponse
    {
        public string Songid { get; set; }
        public string Songname { get; set; }
        public string EncryptedSongid { get; set; }
        public string HasMv { get; set; }
        public string YyrArtist { get; set; }
        public string Artistname { get; set; }
        public string Control { get; set; }
        public List<SongSug> Song;
        public List<ArtistSug> Artist;
        public List<AlbumSug> Album;
        public string Order;
    }
}
