using System;
using BreadPlayer.Web.BaiduLyricsAPI.Interface;

namespace BreadPlayer.Web.BaiduLyricsAPI.Models
{
    public class SongSug : AbstractMusic, IQueryResult
    {
        public string Songid;
        public string Songname;
        public string EncryptedSongid;
        public string HasMv;
        public string YyrArtist;
        public string Artistname;
        public string Control;

        public BitRate Bitrate;
        public SongInfo SongInfo;
        public string GetName()
        {
            return Songname;
        }
        
        public QueryType GetSearchResultType()
        {
            return QueryType.Song;
        }
        
        public override Uri GetDataSoure()
        {
            string url = Bitrate != null ? Bitrate.FileLink : Util.Util.GetDownloadUrlBySongId(Songid);
            return new Uri(url);
        }
        
        public override Int32 GetDuration()
        {
            return Bitrate != null ? Bitrate.FileDuration * 1000 : 0;
        }
        
        public override MusicType GetMusicType()
        {
            return MusicType.Online;
        }
        
        public override string GetTitle()
        {
            return Songname;
        }
        
        public override string GetArtist()
        {
            return Artistname;
        }

        //返回""加载默认的图片
        public string GetArtPic()
        {
            return SongInfo != null ? SongInfo.PicSmall : "";
        }
        
        public override void LoadArtPic(IOnLoadListener loadListener)
        {
        }
        public override void LoadArtPic(PicSizeType picSizeType, IOnLoadListener loadListener)
        {

        }
        
        public override int BlurValueOfPlaying()
        {
            return 0;
        }
    }
}
