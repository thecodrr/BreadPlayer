using BreadPlayer.Web.BaiduLyricsAPI.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadPlayer.Web.BaiduLyricsAPI.Util;

namespace BreadPlayer.Web.BaiduLyricsAPI.Models
{
    public class SongSug : AbstractMusic, IQueryResult
    {
        public String songid;
        public String songname;
        public String encrypted_songid;
        public String has_mv;
        public String yyr_artist;
        public String artistname;
        public String control;

        public BitRate bitrate;
        public SongInfo songInfo;
        public String GetName()
        {
            return songname;
        }
        
        public QueryType GetSearchResultType()
        {
            return QueryType.Song;
        }
        
        public override Uri GetDataSoure()
        {
            string url = bitrate != null ? bitrate.file_link : Util.Util.GetDownloadUrlBySongId(songid);
            return new Uri(url);
        }
        
        public override Int32 GetDuration()
        {
            return bitrate != null ? bitrate.file_duration * 1000 : 0;
        }
        
        public override MusicType GetMusicType()
        {
            return MusicType.Online;
        }
        
        public override String GetTitle()
        {
            return songname;
        }
        
        public override String GetArtist()
        {
            return artistname;
        }

        //返回""加载默认的图片
        public String GetArtPic()
        {
            return songInfo != null ? songInfo.pic_small : "";
        }
        
        public override void LoadArtPic(OnLoadListener loadListener)
        {
        }
        public override void LoadArtPic(PicSizeType picSizeType, OnLoadListener loadListener)
        {

        }
        
        public override int BlurValueOfPlaying()
        {
            return 0;
        }
    }
}
