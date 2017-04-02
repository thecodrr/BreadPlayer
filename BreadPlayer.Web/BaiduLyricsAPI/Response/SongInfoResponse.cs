using BreadPlayer.Web.BaiduLyricsAPI.Models;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BreadPlayer.Web.BaiduLyricsAPI.Response
{
    public class SongInfoResponse : BaseResponse
    {
        private UrlList songUrl;
        private SongInfo songInfo;
        [DataMember(Name = "songurl")]
        public UrlList SongUrl
        {
            get
            {
                return songUrl;
            }
            set
            {
                songUrl = value;
            }
        }

        [DataMember(Name = "songinfo")]
        public SongInfo SongInfo
        {
            get
            {
                return songInfo;
            }
            set
            {
                songInfo = value;
            }
        }
    }

    public class UrlList
    {
        private List<SongUrl> url;

        public List<SongUrl> Url
        {
            get
            {
                return url;
            }
            set
            {
                url = value;
            }
        }
    }
}
