namespace BreadPlayer.Web.BaiduLyricsAPI
{
    public class Endpoints
    {
        public const string API_FROM = "qianqian";
        public const string API_VERSION = "2.1.0";
        public const string API_FORMAT = "json";
        public const string BASE_API_URL = "http://tingapi.ting.baidu.com/v1/restserver/ting";
        public const string METHOD_SEARCH_CATALOGSUG = "baidu.ting.search.catalogSug";
        public const string METHOD_SONG_LRC = "baidu.ting.song.lry";
        public const string METHOD_SONG_PLAY = "baidu.ting.song.play";
        public const string METHOD_GET_SONGINFO = "baidu.ting.song.getInfos";
        public const string METHOD_GET_ARTISTINFO = "baidu.ting.artist.getinfo";    //获取歌手信息
        public const string METHOD_GET_ARTISTSONGLIST = "baidu.ting.artist.getSongList"; //获取歌手的歌曲列表
        public const string METHOD_GET_ARTISTALUBMLIST = "baidu.ting.artist.getAlbumList";   //获取歌手的专辑列表;
        public const string METHOD_GET_ALBUMINFO = "baidu.ting.album.getAlbumInfo";
        public const string METHOD_QUERY_MERGE = "baidu.ting.search.merge";
    }
}
