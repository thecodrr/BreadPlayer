using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Web.NeteaseLyricsAPI
{
    public class Endpoints
    {
        public const string SearchURL = "http://music.163.com/api/search/get?csrf_token=";
        public const string LyricsURL = "http://music.163.com/api/song/lyric?os=osx&id={0}&lv=-1&kv=-1&tv=-1";
    }
}
