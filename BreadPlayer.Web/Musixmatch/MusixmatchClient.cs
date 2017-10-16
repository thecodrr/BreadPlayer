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

namespace BreadPlayer.Web.Musixmatch
{
    public class MusixmatchClient : ILyricAPI
    {
        public async Task<string> FetchLyrics(Mediafile mediaFile)
        {
            HttpHelper.MusixmatchHttpClient.CancelPendingRequests();
            string requestURI = string.Format(@"http://apic-desktop.musixmatch.com/ws/1.1/macro.subtitles.get?format=json&q_track={0}&q_artist={1}&q_album={2}&user_language=en&f_subtitle_length=0&f_subtitle_length_max_deviation=0&subtitle_format=lrc&app_id=web-desktop-app-v1.0&usertoken=1710149d15ba9db2a5a545aadd4f93928e90c783ab83565d105693", mediaFile.Title, mediaFile.LeadArtist, mediaFile.Album);

            //Send to the server
            var result = await HttpHelper.MusixmatchHttpClient.GetAsync(requestURI);
            if (!result.IsSuccessStatusCode)
                return null;
            //Read the content of the result response from the server
            using (Stream stream = await result.Content.ReadAsStreamAsync())
            using (Stream decompressed = new GZipStream(stream, CompressionMode.Decompress))
            using (StreamReader reader = new StreamReader(decompressed))
            {
                try
                {
                    var json = await reader.ReadToEndAsync().ConfigureAwait(false);
                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.NullValueHandling = NullValueHandling.Ignore;
                    settings.MissingMemberHandling = MissingMemberHandling.Ignore;
                    var lyrics = JsonConvert.DeserializeObject<Lyrics>(json, settings).Message?.Body?.MacroCalls?.Subtitles?.Message?.Body?.SubtitleList;
                    if (lyrics?.Any() == true)
                    {
                        return lyrics[0].Subtitle.Lyrics;
                    }
                    else
                        return null;
                }
                catch(JsonSerializationException)
                {
                    return null;
                }
            }
        }
    }
}
