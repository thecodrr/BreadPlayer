using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Web.BaiduLyricsAPI.Interface
{
    public abstract class AbstractMusic
    {

        public AbstractMusic()
        {
            //给CREATOR赋值
        }

        public abstract Uri GetDataSoure();

        public abstract Int32 GetDuration();

        public abstract MusicType GetMusicType();

        /**
         * 获取歌曲名
         */
        public abstract String GetTitle();

        public abstract String GetArtist();

        //    /**
        //     * 获取艺术家图片
        //     * @return   uri
        //     */
        //    public abstract String getArtPic();

        /**
         * 加载艺术家图片  上面的方法废弃
         *
         * @param loadListener
         */
        public abstract void LoadArtPic(OnLoadListener loadListener);


        public abstract void LoadArtPic(PicSizeType picSizeType, OnLoadListener loadListener);

        /**
         * 专辑图片高斯模糊值
         * @return
         */
        public abstract int BlurValueOfPlaying();

        /**
         * 获取时间字符串
         *
         * @return
         */
        public String GetDurationStr()
        {
            return GetDuration().ToString(@"mm\:ss");           
        }

        public bool IsOnlineMusic()
        {
            return GetMusicType() == MusicType.Online;
        }


        public enum MusicType
        {
            Local, Online
        }

        public interface OnLoadListener
        {
            //void OnSuccessLoad(Bitmap bitmap);
        }

        /**
         * PIC 尺寸枚举
         */
        public enum PicSizeType
        {
            SMALL, BIG, PREIUM, HUGE
        }
    }
}
