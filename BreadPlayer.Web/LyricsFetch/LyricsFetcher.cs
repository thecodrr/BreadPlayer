using BreadPlayer.Core.Models;
using BreadPlayer.Web.BaiduLyricsAPI;
using BreadPlayer.Web.NeteaseLyricsAPI;
using BreadPlayer.Web.XiamiLyricsAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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
                List<string> Lyrics = new List<string>();
                Lyrics.Add(await Sources[0].FetchLyrics(file));
                
                return Lyrics;
            }
            catch
            {
                return null;
            }
        }
    }
}
