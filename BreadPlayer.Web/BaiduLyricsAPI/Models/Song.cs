using System;
using BreadPlayer.Web.BaiduLyricsAPI.Interface;

namespace BreadPlayer.Web.BaiduLyricsAPI.Models
{
    public class Song : AbstractMusic, IQueryResult
    {
        public string Content;
        public string CopyType;
        public string Toneid;
        public string Info;
        public string AllRate;
        public int ResourceType;
        public int RelateStatus;
        public int HasMvMobile;
        public string SongId;
        public string Title;
        public string TingUid;
        public string Author;
        public string AlbumId;
        public string AlbumTitle;
        public int IsFirstPublish;
        public int Havehigh;
        public int Charge;
        public int HasMv;
        public int Learn;
        public string SongSource;
        public string PiaoId;
        public string KoreanBbSong;
        public string ResourceTypeExt;
        public string ArtistId;
        public string AllArtistId;
        public string Lrclink;
        public int DataSource;
        public int ClusterId;

        public BitRate Bitrate;
        public SongInfo Songinfo;

        public string GetName()
        {
            return Title;
        }

        public QueryType GetSearchResultType()
        {
            return QueryType.Song;
        }

        public override Uri GetDataSoure()
        {
            string url = Bitrate != null ? Bitrate.FileLink : Util.Util.GetDownloadUrlBySongId(SongId);
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
            return Title;
        }

        public override string GetArtist()
        {
            return Author;
        }

        public override void LoadArtPic(PicSizeType picSizeType, IOnLoadListener loadListener)
        {
            string uri = "";
            switch (picSizeType)
            {
                case PicSizeType.Small:
                    uri = new Uri(Songinfo != null ? Songinfo.PicSmall : "").AbsolutePath;
                    break;
                case PicSizeType.Big:
                    uri = new Uri(Songinfo != null ? Songinfo.PicBig : "").AbsolutePath;
                    break;
                case PicSizeType.Preium:
                    uri = new Uri(Songinfo != null ? Songinfo.PicPremium : "").AbsolutePath;
                    break;
                case PicSizeType.Huge:
                    uri = new Uri(Songinfo != null ? Songinfo.PicHuge : "").AbsolutePath;
                    break;
            }
            loadArtPic(uri, loadListener);
        }

        /**
         * 默认加载samll
         * @param loadListener
         */
        public override void LoadArtPic(IOnLoadListener loadListener)
        {
            LoadArtPic(PicSizeType.Small, loadListener);
        }

        private void loadArtPic(string artUri, IOnLoadListener loadListener)
        {
        }

        public override int BlurValueOfPlaying()
        {
            return 80;
        }

        public bool HasGetDetailInfo()
        {
            return Bitrate != null || Songinfo != null;
        }
    }

    public class SongUrl
    {
        /**
         * show_link :
         * down_type : 0
         * original : 0
         * free : 1
         * replay_gain : 2.150002
         * song_file_id : 129449752
         * file_size : 26527401
         * file_extension : flac
         * file_duration : 239
         * can_see : 1
         * can_load : true
         * preload : 554.375
         * file_bitrate : 887
         * file_link :
         * is_udition_url : 0
         * hash : 6ca6a562894d4aa29eaef756e2824113d48a419e
         */

        public string ShowLink { get; set; }

        public int DownType { get; set; }


        public int Original { get; set; }

        public int Free { get; set; }

        public string ReplayGain { get; set; }

        public int SongFileId { get; set; }

       
        public int FileSize { get; set; }

     

        public string GetFileExtension { get; set; }

        public int GetFileDuration { get; set; }

        public int GetCanSee { get; set; }

        public bool IsCanLoad { get; set; }


        public double Preload { get; set; }


        public int FileBitrate { get; set; }
        public string FileLink { get; set; }
        public int IsUditionUrl { get; set; }

        public string Hash { get; set; }
    }
}
