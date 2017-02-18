using BreadPlayer.Web.BaiduLyricsAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Web.BaiduLyricsAPI.Response
{
    public class AlbumDetailResponse : BaseResponse
    {
        public AlbumInfo albumInfo { get; set; }
        public List<Song> songlist { get; set; }
    }
}
