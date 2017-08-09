using System.Threading.Tasks;
using BreadPlayer.Web.BaiduLyricsAPI.Models;
using BreadPlayer.Web.BaiduLyricsAPI.Response;
using Newtonsoft.Json;

namespace BreadPlayer.Web.BaiduLyricsAPI
{
    public class ApiMethods
    {
        private Helpers _helpers = new Helpers();

        public async Task<Lrc> RequestSongLrc(string songId)
        {
            var url = _helpers.GetCallUrl(Endpoints.MethodSongLrc, _helpers.GetSongsInfoParameterString(songId));
            string response = await _helpers.MakeRequest(url);
            var obj = (Lrc)JsonConvert.DeserializeObject(response, typeof(Lrc));
            return obj;
        }
        public async Task<QueryMergeResponse> Search(string query)
        {
            var url = _helpers.GetCallUrl(Endpoints.MethodQueryMerge, _helpers.GetQueryParameterString(query));
            string response = await _helpers.MakeRequest(url);
            var obj = (QueryMergeResponse)JsonConvert.DeserializeObject(response, typeof(QueryMergeResponse));
            return obj;
        }        
    }
}
