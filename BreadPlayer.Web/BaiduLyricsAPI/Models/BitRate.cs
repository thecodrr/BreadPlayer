using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Web.BaiduLyricsAPI.Models
{
    public class BitRate
    {
        public int file_bitrate { get; set; }
        public string file_link { get; set; }
        public string file_extension { get; set; }
        public int original { get; set; }
        public int file_size { get; set; }
        public int file_duration { get; set; }
        public string show_link { get; set; }
        public int song_file_id { get; set; }
        public string replay_gain { get; set; }
        public int free { get; set; }
    }
}
