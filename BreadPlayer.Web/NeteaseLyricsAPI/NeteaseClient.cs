using BreadPlayer.Web.NeteaseLyricsAPI.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadPlayer.Core.Models;
using System.Net;

namespace BreadPlayer.Web.NeteaseLyricsAPI
{
    public class NeteaseClient : ILyricAPI
    {
        public async Task<SearchResponse> SearchSongs(string query)
        {
            var results = JsonConvert.DeserializeObject<SearchResponse>(await NeteaseHttpHelper.PostAsync("http://music.163.com/api/search/get/",$"s={query}&type=1&limit=10&offset=0"));
            return results;
        }
        public async Task<LyricsResponse> GetLyrics(string id)
        {
            var results = JsonConvert.DeserializeObject<LyricsResponse>(await NeteaseHttpHelper.GetAsync(string.Format(Endpoints.LyricsURL, id)));
            return results;
        }

        public async Task<string> FetchLyrics(Mediafile mediaFile)
        {
            var results = await SearchSongs(WebUtility.UrlEncode(mediaFile.Title + " " + mediaFile.LeadArtist));
            var bSong = results.Result.Songs.First(t => t.Name.Contains(mediaFile.Title));
            return (await GetLyrics(bSong.Id.ToString())).Lrc.Lyric;
        }
    }
}
