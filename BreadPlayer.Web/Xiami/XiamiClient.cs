using BreadPlayer.Core.Models;
using BreadPlayer.Web.XiamiLyricsAPI.Responses;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BreadPlayer.Web.XiamiLyricsAPI
{
    public class XiamiClient : ILyricAPI
    {
        private HttpClient XiamiHttpClient = new HttpClient();
        public XiamiClient()
        {
            XiamiHttpClient.DefaultRequestHeaders.Referrer = new Uri("http://h.xiami.com/");
            XiamiHttpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.75 Safari/537.36");
        }
        public async Task<string> FetchLyrics(Mediafile mediaFile)
        {
            XiamiHttpClient.CancelPendingRequests();
            var results = await SearchAsync(WebUtility.UrlEncode(mediaFile.Title + " " + mediaFile.LeadArtist));
            if (results?.Data?.Songs?.Any(t => t.SongName.ToLower().Contains(mediaFile.Title.ToLower())) == true)
            {
                var xResult = results.Data.Songs.First(t => t.SongName.ToLower().Contains(mediaFile.Title.ToLower()));
                var xSong = await GetSongDetailAsync(xResult.SongId.ToString());
                if (xSong.Data.TrackList?.Any() == true && !string.IsNullOrEmpty(xSong.Data.TrackList[0].LyricUrl))
                    return await XiamiHttpClient.GetStringAsync(xSong.Data.TrackList[0].LyricUrl).ConfigureAwait(false);
                else if (!string.IsNullOrEmpty(xResult.Lyric) && xSong.Data.TrackList == null)
                    return await XiamiHttpClient.GetStringAsync(xResult.Lyric).ConfigureAwait(false);
            }
            return "";
        }

        public async Task<SongDetailResponse> GetSongDetailAsync(string id)
        {
            var response = await XiamiHttpClient.GetStringAsync($"http://www.xiami.com/song/playlist/id/{id}/object_name/default/object_id/0/cat/json").ConfigureAwait(false);
            return JsonConvert.DeserializeObject<SongDetailResponse>(response);
        }

        public async Task<SearchResponse> SearchAsync(string query)
        {
            try
            {
                var response = await XiamiHttpClient.GetAsync(string.Format(Endpoints.SearchURI, query, 10)).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<SearchResponse>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                else
                    return null;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
    }
}