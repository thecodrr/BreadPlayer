using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Web.BaiduLyricsAPI
{
    public class Helpers
    {
        public string GetQueryParameterString(string query, string page_no = "1", string page_size = "50", string type = "-1", string data_source = "0", string use_cluster="1")
        {
            var formatString = "&query={0}&page_no={1}&page_size={2}&type={3}&data_source={4}&use_cluster={5}";
            return string.Format(formatString, query, page_no, page_size, type, data_source, use_cluster);
        }
        public string GetSongsInfoParameterString(string songid)
        {
            var formatString = "&songid={0}";
            return string.Format(formatString, songid);
        } 
        public string GetMusicAndArtistInfoParameterString(string tinguid)
        {
            var formatString = "&tinguid={0}";
            return string.Format(formatString, tinguid);
        }  
        //same for SongListFromArtistParameterString
        public string GetAlbumByArtistParameterString(string tinguid, string order = "1", string offset = "0", string limits = "30")
        {
            var formatString = "&order={0}&tinguid={1}&offset={2}&limits={3}";
            return string.Format(formatString, order, tinguid, offset, limits);
        } 
        public string GetAlbumDetailParameterString(string album_id)
        {
            var formatString = "&album_id={0}";
            return string.Format(formatString, album_id);
        }  
        public string GetCallURL(string method, string parameters)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Endpoints.BASE_API_URL);
            builder.AppendFormat("?from={0}&version={1}&method={2}&format={3}{4}", Endpoints.API_FROM, Endpoints.API_VERSION, method, Endpoints.API_FORMAT, parameters);
            return builder.ToString();
        }
        public async Task<string> MakeRequest(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                return await client.GetStringAsync(url);
            }
        }
    }
}
