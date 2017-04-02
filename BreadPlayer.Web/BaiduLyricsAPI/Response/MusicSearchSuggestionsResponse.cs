using BreadPlayer.Web.BaiduLyricsAPI.Models;
using System.Collections.Generic;

namespace BreadPlayer.Web.BaiduLyricsAPI.Response
{
    public class MusicSearchSuggestionsResponse : BaseResponse
    {
        public string songid { get; set; }
        public string songname { get; set; }
        public string encrypted_songid { get; set; }
        public string has_mv { get; set; }
        public string yyr_artist { get; set; }
        public string artistname { get; set; }
        public string control { get; set; }
        public List<SongSug> song;
        public List<ArtistSug> artist;
        public List<AlbumSug> album;
        public string order;
    }
}
