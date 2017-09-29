using BreadPlayer.Core.Models;
using BreadPlayer.Web.XiamiLyricsAPI.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Web.XiamiLyricsAPI
{
    public class XiamiClient : ILyricAPI
    {
        public async Task<string> FetchLyrics(Mediafile mediaFile)
        {
            using (HttpClient client = new HttpClient())
            {
                var results = await SearchAsync(WebUtility.UrlEncode(mediaFile.Title + " " + mediaFile.LeadArtist));
                if (results.Data.Songs.Any())
                {
                    var xResult = results.Data.Songs.First(t => t.SongName.ToLower().Contains(mediaFile.Title.ToLower()));
                    var xSong = await GetSongDetailAsync(xResult.SongId.ToString());
                    if (xSong.Data.TrackList?.Any() == true && !string.IsNullOrEmpty(xSong.Data.TrackList[0].LyricUrl))
                        return await client.GetStringAsync(xSong.Data.TrackList[0].LyricUrl).ConfigureAwait(false);
                    else if (!string.IsNullOrEmpty(xResult.Lyric) && xSong.Data.TrackList == null)
                        return await client.GetStringAsync(xResult.Lyric).ConfigureAwait(false);
                }
                return "";
            }
        }
        public async Task<SongDetailResponse> GetSongDetailAsync(string id)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync($"http://www.xiami.com/song/playlist/id/{id}/object_name/default/object_id/0/cat/json").ConfigureAwait(false);
                return JsonConvert.DeserializeObject<SongDetailResponse>(response);
            }
        }
        public async Task<SearchResponse> SearchAsync(string query)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Referrer = new Uri("http://h.xiami.com/");
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.75 Safari/537.36");
                var response = await client.GetStringAsync(string.Format(Endpoints.SearchURI, query, 10)).ConfigureAwait(false);
                return JsonConvert.DeserializeObject<SearchResponse>(response);
            }
        }
    }
}
