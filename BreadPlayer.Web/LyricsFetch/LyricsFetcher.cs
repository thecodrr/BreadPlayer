using BreadPlayer.Core.Models;
using BreadPlayer.Parsers.LRCParser;
using BreadPlayer.Parsers.TagParser;
using BreadPlayer.Web.BaiduLyricsAPI;
using BreadPlayer.Web.NeteaseLyricsAPI;
using BreadPlayer.Web.XiamiLyricsAPI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BreadPlayer.Web.LyricsFetch
{
    public class LyricsFetcher
    {
        static ILyricAPI[] Sources = new ILyricAPI[] { new NeteaseClient(), new XiamiClient(), new BaiduClient() };

        public static async Task<List<string>> FetchLyrics(Mediafile file)
        {
            try
            {
                var mediaFile = new Mediafile();
                mediaFile.Title = TagParser.ParseTitle(file.Title.ToString());
                var cleanedArtist = TagParser.ParseTitle(file.LeadArtist.ToString());
                List<string> parsedArtists = TagParser.ParseArtists(cleanedArtist);
                if (string.IsNullOrEmpty(cleanedArtist))
                {
                    cleanedArtist = file.Title;
                    parsedArtists = TagParser.ParseArtists(cleanedArtist);
                    parsedArtists.RemoveAt(0);
                }
                mediaFile.LeadArtist = !parsedArtists.Any() ? file.LeadArtist : TagParser.ParseTitle(parsedArtists[0]);
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