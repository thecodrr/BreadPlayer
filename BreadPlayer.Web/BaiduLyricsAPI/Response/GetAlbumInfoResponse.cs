using System.Collections.Generic;
using BreadPlayer.Web.BaiduLyricsAPI.Models;

namespace BreadPlayer.Web.BaiduLyricsAPI.Response
{
    internal class GetAlbumInfoResponse
    {
        private AlbumInfo _albumInfo;
        private List<SongInfo> _songlist;

        public AlbumInfo AlbumInfo
        {
            get => _albumInfo;
            set => _albumInfo = value;
        }        

        public List<SongInfo> Songlist
        {
            get => _songlist;
            set => _songlist = value;
        }        
    }
}
