using BreadPlayer.Web.BaiduLyricsAPI.Models;
using System.Collections.Generic;

namespace BreadPlayer.Web.BaiduLyricsAPI.Response
{
    internal class GetAlbumInfoResponse
    {
        private AlbumInfo albumInfo;
        private List<SongInfo> songlist;

        public AlbumInfo AlbumInfo
        {
            get => albumInfo;
            set => albumInfo = value;
        }        

        public List<SongInfo> Songlist
        {
            get => songlist;
            set => songlist = value;
        }        
    }
}
