using BreadPlayer.Core.Models;
using BreadPlayer.Web.BaiduLyricsAPI.Models;
using BreadPlayer.Web.BaiduLyricsAPI.Response;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BreadPlayer.Web.BaiduLyricsAPI
{
    public class BaiduClient : ILyricAPI
    {
        private Helpers _helpers = new Helpers();

        public async Task<string> FetchLyrics(Mediafile mediaFile)
        {
            var results = await Search(WebUtility.UrlEncode(mediaFile.Title + " " + mediaFile.LeadArtist)).ConfigureAwait(false);
            if (results.Result.SongInfo?.SongList?.Any() == true)
            {
                var bSong = results.Result.SongInfo.SongList.First(t => t.Title.Contains(mediaFile.Title));
                return (await RequestSongLrc(bSong.SongId).ConfigureAwait(false)).LrcContent;
            }
            return "";
        }

        public async Task<Lrc> RequestSongLrc(string songId)
        {
            var url = _helpers.GetCallUrl(Endpoints.MethodSongLrc, _helpers.GetSongsInfoParameterString(songId));
            string response = await _helpers.MakeRequest(url).ConfigureAwait(false);
            var obj = (Lrc)JsonConvert.DeserializeObject(response, typeof(Lrc));
            return obj;
        }

        public async Task<QueryMergeResponse> Search(string query)
        {
            var url = _helpers.GetCallUrl(Endpoints.MethodQueryMerge, _helpers.GetQueryParameterString(query));
            string response = await _helpers.MakeRequest(url).ConfigureAwait(false);
            var obj = (QueryMergeResponse)JsonConvert.DeserializeObject(response, typeof(QueryMergeResponse));
            return obj;
        }
    }
}