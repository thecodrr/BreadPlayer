using System.Collections.Generic;
using System.Runtime.Serialization;
using BreadPlayer.Web.BaiduLyricsAPI.Models;

namespace BreadPlayer.Web.BaiduLyricsAPI.Response
{
    public class SongInfoResponse : BaseResponse
    {
        private UrlList _songUrl;
        private SongInfo _songInfo;
        [DataMember(Name = "songurl")]
        public UrlList SongUrl
        {
            get => _songUrl;
            set => _songUrl = value;
        }

        [DataMember(Name = "songinfo")]
        public SongInfo SongInfo
        {
            get => _songInfo;
            set => _songInfo = value;
        }
    }

    public class UrlList
    {
        private List<SongUrl> _url;

        public List<SongUrl> Url
        {
            get => _url;
            set => _url = value;
        }
    }
}
