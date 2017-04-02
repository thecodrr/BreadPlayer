using BreadPlayer.Web.BaiduLyricsAPI.Models;
using System.Collections.Generic;

namespace BreadPlayer.Web.BaiduLyricsAPI.Response
{
    class GetAlbumInfoResponse
    {
        private AlbumInfo albumInfo;
        private List<SongInfo> songlist;

        public AlbumInfo AlbumInfo
        {
            get
            {
                return albumInfo;
            }
            set
            {
                albumInfo = value;
            }
        }        

        public List<SongInfo> Songlist
        {
            get
            {
                return songlist;
            }
            set
            {
                songlist = value;
            }
        }        
    }
}
