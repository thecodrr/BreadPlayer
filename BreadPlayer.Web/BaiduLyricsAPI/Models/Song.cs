using BreadPlayer.Web.BaiduLyricsAPI.Interface;
using System;

namespace BreadPlayer.Web.BaiduLyricsAPI.Models
{
    public class Song : AbstractMusic, IQueryResult
    {
        public string content;
        public string copy_type;
        public string toneid;
        public string info;
        public string all_rate;
        public int resource_type;
        public int relate_status;
        public int has_mv_mobile;
        public string song_id;
        public string title;
        public string ting_uid;
        public string author;
        public string album_id;
        public string album_title;
        public int is_first_publish;
        public int havehigh;
        public int charge;
        public int has_mv;
        public int learn;
        public string song_source;
        public string piao_id;
        public string korean_bb_song;
        public string resource_type_ext;
        public string artist_id;
        public string all_artist_id;
        public string lrclink;
        public int data_source;
        public int cluster_id;

        public BitRate bitrate;
        public SongInfo songinfo;

        public string GetName()
        {
            return title;
        }

        public QueryType GetSearchResultType()
        {
            return QueryType.Song;
        }

        public override Uri GetDataSoure()
        {
            string url = bitrate != null ? bitrate.file_link : Util.Util.GetDownloadUrlBySongId(song_id);
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
            return title;
        }

        public override string GetArtist()
        {
            return author;
        }

        public override void LoadArtPic(PicSizeType picSizeType, OnLoadListener loadListener)
        {
            string uri = "";
            switch (picSizeType)
            {
                case PicSizeType.SMALL:
                    uri = new Uri(songinfo != null ? songinfo.pic_small : "").AbsolutePath;
                    break;
                case PicSizeType.BIG:
                    uri = new Uri(songinfo != null ? songinfo.pic_big : "").AbsolutePath;
                    break;
                case PicSizeType.PREIUM:
                    uri = new Uri(songinfo != null ? songinfo.pic_premium : "").AbsolutePath;
                    break;
                case PicSizeType.HUGE:
                    uri = new Uri(songinfo != null ? songinfo.pic_huge : "").AbsolutePath;
                    break;
            }
            loadArtPic(uri, loadListener);
        }

        /**
         * 默认加载samll
         * @param loadListener
         */
        public override void LoadArtPic(OnLoadListener loadListener)
        {
            LoadArtPic(PicSizeType.SMALL, loadListener);
        }

        private void loadArtPic(string artUri, OnLoadListener loadListener)
        {
        }

        public override int BlurValueOfPlaying()
        {
            return 80;
        }

        public bool hasGetDetailInfo()
        {
            return bitrate != null || songinfo != null;
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

        public string Show_link { get; set; }

        public int Down_type { get; set; }


        public int Original { get; set; }

        public int Free { get; set; }

        public string Replay_gain { get; set; }

        public int Song_file_id { get; set; }

       
        public int File_size { get; set; }

     

        public string getFile_extension { get; set; }

        public int getFile_duration { get; set; }

        public int getCan_see { get; set; }

        public bool isCan_load { get; set; }


        public double Preload { get; set; }


        public int File_bitrate { get; set; }
        public string File_link { get; set; }
        public int Is_udition_url { get; set; }

        public string Hash { get; set; }

        public SongUrl()
        {
        }
    }
}
