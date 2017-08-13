using BreadPlayer.Core.Models;
using BreadPlayer.Web.BaiduLyricsAPI;
using BreadPlayer.Web.NeteaseLyricsAPI;
using BreadPlayer.Web.XiamiLyricsAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BreadPlayer.Web.LyricsFetch
{
    public class LyricsFetcher
    {
        public static async Task<List<string>> FetchLyrics(Mediafile file)
        {
            ILyricAPI[] Sources = new ILyricAPI[] { new XiamiClient(), new NeteaseClient(), new BaiduClient() };
            try
            {
                var mediaFile = file;
                mediaFile.Title = Regex.Replace(mediaFile.Title, @"\(.*?\)|\[.*?\]|\(.*|\[.*|&.*", "").Trim();
                mediaFile.LeadArtist = Regex.Replace(mediaFile.LeadArtist, @"\(.*?\)|\[.*?\]|\(.*|\[.*|&.*", "").Trim();
                List<string> Lyrics = new List<string>();
                foreach (var source in Sources)
                    Lyrics.Add(await source.FetchLyrics(mediaFile).ConfigureAwait(false));
                return Lyrics;
            }
            catch
            {
                return null;
            }
        }
    }
}
