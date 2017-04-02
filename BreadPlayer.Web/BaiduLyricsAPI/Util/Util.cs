namespace BreadPlayer.Web.BaiduLyricsAPI.Util
{
    public class Util
    {
        public const string API_URL = "http://tingapi.ting.baidu.com";
        public static string GetDownloadUrlBySongId(string songId)
        {
            return "http://ting.baidu.com/data/music/links?songIds=" + songId;
        }
    }
}
