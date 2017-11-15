using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BreadPlayer.Core.Models;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using BreadPlayer.Web.Musixmatch.Models;
using RestSharp.Portable.HttpClient;
using RestSharp.Portable;

namespace BreadPlayer.Web.Musixmatch
{
    public class MusixmatchClient : ILyricAPI
    {
        public async Task<string> FetchLyrics(Mediafile mediaFile)
        {  
            string requestURI = string.Format(@"https://apic-desktop.musixmatch.com/ws/1.1/macro.subtitles.get?format=json&q_track={0}&q_artist={1}&q_album={2}&user_language=en&f_subtitle_length=0&f_subtitle_length_max_deviation=0&subtitle_format=lrc&app_id=web-desktop-app-v1.0&guid=e08e6c63-edd1-4207-86dc-d350cdf7f4bc&usertoken=1710144894f79b194e5a5866d9e084d48f227d257dcd8438261277", mediaFile.Title, mediaFile.LeadArtist, mediaFile.Album);

            using (var client = new RestClient(requestURI))
            {
                var request = new RestRequest(Method.GET);
                request.AddHeader("connection", "keep-alive");
                request.AddHeader("cookie", "x-mxm-user-id=; x-mxm-token-guid=e08e6c63-edd1-4207-86dc-d350cdf7f4bc; mxm-encrypted-token=; AWSELB=55578B011601B1EF8BC274C33F9043CA947F99DCFF6AB1B746DBF1E96A6F2B997493EE03F2DD5F516C3BC8E8DE7FE9C81FF414E8E76CF57330A3F26A0D86825F74794F3C94");
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
                request.AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.91 Safari/537.36");
                request.AddHeader("upgrade-insecure-requests", "1");
                request.AddHeader("accept-language", "en-US,en;q=0.8");
                request.AddHeader("accept-encoding", "gzip, deflate");
                request.AddHeader("dnt", "1");
                IRestResponse response = await client.Execute(request);
                try
                {
                    var json = response.Content;
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    var lyrics = JsonConvert.DeserializeObject<Lyrics>(json, settings).Message?.Body?.MacroCalls?.Subtitles?.Message?.Body?.SubtitleList;
                    if (lyrics?.Any() == true)
                    {
                        return lyrics[0].Subtitle.Lyrics;
                    }
                    else
                        return null;
                }
                catch (JsonSerializationException)
                {
                    return null;
                }
            }            
        }
    }
}
