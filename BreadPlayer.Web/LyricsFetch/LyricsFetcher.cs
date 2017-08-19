using BreadPlayer.Core.Models;
using BreadPlayer.Parsers.LRCParser;
using BreadPlayer.Parsers.TagParser;
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
            ILyricAPI[] Sources = new ILyricAPI[] { new NeteaseClient(), new XiamiClient(), new BaiduClient() };
            try
            {
                var mediaFile = new Mediafile();
                mediaFile.Title = TagParser.ParseTitle(file.Title);
                mediaFile.LeadArtist = TagParser.ParseTitle(file.LeadArtist);
                List<string> Lyrics = new List<string>();
                for (int i = 0; i < Sources.Length; i++)
                {
                    var lyrics = await Sources[i].FetchLyrics(mediaFile).ConfigureAwait(false);
                    if (LrcParser.IsLrc(lyrics))
                    {
                        Lyrics.Add(lyrics);
                        break;
                    }
                }
                return Lyrics;
            }
            catch
            {
                return null;
            }
        }
    }
}
