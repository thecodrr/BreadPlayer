using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Web.BaiduLyricsAPI.Util
{
    public class Util
    {
        public const String API_URL = "http://tingapi.ting.baidu.com";
        public static String GetDownloadUrlBySongId(String songId)
        {
            return "http://ting.baidu.com/data/music/links?songIds=" + songId;
        }
    }
}
