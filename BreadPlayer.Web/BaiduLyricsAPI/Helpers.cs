using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Web.BaiduLyricsAPI
{
    public class Helpers
    {
        public string GetQueryParameterString(string query, string pageNo = "1", string pageSize = "50", string type = "-1", string dataSource = "0", string useCluster = "1")
        {
            var formatString = "&query={0}&page_no={1}&page_size={2}&type={3}&data_source={4}&use_cluster={5}";
            return string.Format(formatString, query, pageNo, pageSize, type, dataSource, useCluster);
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

        public string GetAlbumDetailParameterString(string albumId)
        {
            var formatString = "&album_id={0}";
            return string.Format(formatString, albumId);
        }

        public string GetCallUrl(string method, string parameters)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Endpoints.BaseApiUrl);
            builder.AppendFormat("?from={0}&version={1}&method={2}&format={3}{4}", Endpoints.ApiFrom, Endpoints.ApiVersion, method, Endpoints.ApiFormat, parameters);
            return builder.ToString();
        }
        HttpClient BiaduHttpClient = new HttpClient();
        public async Task<string> MakeRequest(string url)
        {
            BiaduHttpClient.CancelPendingRequests();
            return await BiaduHttpClient.GetStringAsync(url).ConfigureAwait(false);
        }
    }
}