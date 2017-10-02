namespace BreadPlayer.Web.BaiduLyricsAPI
{
    public class Endpoints
    {
        public const string ApiFrom = "qianqian";
        public const string ApiVersion = "2.1.0";
        public const string ApiFormat = "json";
        public const string BaseApiUrl = "http://tingapi.ting.baidu.com/v1/restserver/ting";
        public const string MethodSearchCatalogsug = "baidu.ting.search.catalogSug";
        public const string MethodSongLrc = "baidu.ting.song.lry";
        public const string MethodSongPlay = "baidu.ting.song.play";
        public const string MethodGetSonginfo = "baidu.ting.song.getInfos";
        public const string MethodGetArtistinfo = "baidu.ting.artist.getinfo";    //获取歌手信息
        public const string MethodGetArtistsonglist = "baidu.ting.artist.getSongList"; //获取歌手的歌曲列表
        public const string MethodGetArtistalubmlist = "baidu.ting.artist.getAlbumList";   //获取歌手的专辑列表;
        public const string MethodGetAlbuminfo = "baidu.ting.album.getAlbumInfo";
        public const string MethodQueryMerge = "baidu.ting.search.merge";
    }
}