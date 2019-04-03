using BreadPlayer.Core.Models;
using BreadPlayer.Parsers.LRCParser;
using BreadPlayer.Parsers.TagParser;
using BreadPlayer.Web.BaiduLyricsAPI;
using BreadPlayer.Web.Musixmatch;
using BreadPlayer.Web.NeteaseLyricsAPI;
using BreadPlayer.Web.XiamiLyricsAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BreadPlayer.Web.LyricsFetch
{
    public class LyricsFetcher
    {
        static ILyricAPI[] Sources = new ILyricAPI[] { new MusixmatchClient(), new NeteaseClient(), new XiamiClient(), new BaiduClient() };

        public static async Task<string> FetchLyrics(Mediafile file, string lyricSource)
        {
            try
            {
                var mediaFile = new Mediafile();
                mediaFile.Title = TagParser.ParseTitle(file.Title?.ToString());
                if (mediaFile.Title == null)
                    return null;
                var cleanedArtist = TagParser.ParseTitle(file.LeadArtist?.ToString());
                List<string> parsedArtists = TagParser.ParseArtists(cleanedArtist);
                if (string.IsNullOrEmpty(cleanedArtist))
                {
                    cleanedArtist = file.Title;
                    parsedArtists = TagParser.ParseArtists(cleanedArtist);
                    parsedArtists.RemoveAt(0);
                }
                mediaFile.LeadArtist = !parsedArtists.Any() ? file.LeadArtist : TagParser.ParseTitle(parsedArtists[0]);
                if (mediaFile.LeadArtist == null || mediaFile.LeadArtist.Equals("Unknown Artist", System.StringComparison.CurrentCultureIgnoreCase))
                    return null;
                string Lyrics = "";
                switch (lyricSource)
                {
                    case "Auto":
                        for (int i = 0; i < Sources.Length; i++)
                        {
                            var lrc = await Sources[i].FetchLyrics(mediaFile).ConfigureAwait(false);
                            if (LrcParser.IsLrc(lrc))
                            {
                                Lyrics = lrc;
                                break;
                            }
                        }
                        break;
                    case "Musixmatch":
                        Lyrics = await Sources[0].FetchLyrics(mediaFile).ConfigureAwait(false);
                        break;
                    case "Netease":
                        Lyrics = await Sources[1].FetchLyrics(mediaFile).ConfigureAwait(false);
                        break;
                    case "Baidu":
                        Lyrics = await Sources[3].FetchLyrics(mediaFile).ConfigureAwait(false);
                        break;
                    case "Xiami":
                        Lyrics = await Sources[2].FetchLyrics(mediaFile).ConfigureAwait(false);
                        break;
                }
                return Lyrics;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}