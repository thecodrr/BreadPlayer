using BreadPlayer.Web.BaiduLyricsAPI.Interface;
using System;

namespace BreadPlayer.Web.BaiduLyricsAPI.Models
{
    public class SongSug : AbstractMusic, IQueryResult
    {
        public string songid;
        public string songname;
        public string encrypted_songid;
        public string has_mv;
        public string yyr_artist;
        public string artistname;
        public string control;

        public BitRate bitrate;
        public SongInfo songInfo;
        public string GetName()
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
        
        public override string GetTitle()
        {
            return songname;
        }
        
        public override string GetArtist()
        {
            return artistname;
        }

        //返回""加载默认的图片
        public string GetArtPic()
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
