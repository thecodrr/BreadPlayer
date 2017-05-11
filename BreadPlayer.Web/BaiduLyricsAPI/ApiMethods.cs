using System.Threading.Tasks;
using BreadPlayer.Web.BaiduLyricsAPI.Models;
using BreadPlayer.Web.BaiduLyricsAPI.Response;
using Newtonsoft.Json;

namespace BreadPlayer.Web.BaiduLyricsAPI
{
    public class ApiMethods
    {
        private Helpers _helpers = new Helpers();
        public async Task<SongListResponse> RequestSongListFromArtist(string artistId)
        {
            var url = _helpers.GetCallUrl(Endpoints.MethodGetArtistsonglist, _helpers.GetAlbumByArtistParameterString(artistId));
            string response = await _helpers.MakeRequest(url);   
            var obj =  (SongListResponse)JsonConvert.DeserializeObject(response, typeof(SongListResponse));
            return obj;
        }
        public async Task<AlbumDetailResponse> RequestAlbumDetail(string albumId)
        {
            var url = _helpers.GetCallUrl(Endpoints.MethodGetAlbuminfo, _helpers.GetAlbumDetailParameterString(albumId));
            string response = await _helpers.MakeRequest(url);
            var obj = (AlbumDetailResponse)JsonConvert.DeserializeObject(response, typeof(AlbumDetailResponse));
            return obj;
        }
        public async Task<QueryMergeResponse> Search(string query)
        {
            var url = _helpers.GetCallUrl(Endpoints.MethodQueryMerge, _helpers.GetQueryParameterString(query));
            string response = await _helpers.MakeRequest(url);
            var obj = (QueryMergeResponse)JsonConvert.DeserializeObject(response, typeof(QueryMergeResponse));
            return obj;
        }
        public async Task<ArtistAlbumListResponse> RequestAlbumByArtist(string artistId)
        {
            var url = _helpers.GetCallUrl(Endpoints.MethodGetArtistalubmlist, _helpers.GetAlbumByArtistParameterString(artistId));
            string response = await _helpers.MakeRequest(url);
            var obj = (ArtistAlbumListResponse)JsonConvert.DeserializeObject(response, typeof(ArtistAlbumListResponse));
            return obj;
        }
        public async Task<ArtistInfo> RequestArtistInfo(string artistId)
        {
            var url = _helpers.GetCallUrl(Endpoints.MethodGetArtistinfo, _helpers.GetMusicAndArtistInfoParameterString(artistId));
            string response = await _helpers.MakeRequest(url);
            var obj = (ArtistInfo)JsonConvert.DeserializeObject(response, typeof(ArtistInfo));
            return obj;
        }
        public async Task<SongInfo> RequestMusicInfo(string artistId)
        {
            var url = _helpers.GetCallUrl(Endpoints.MethodGetSonginfo, _helpers.GetMusicAndArtistInfoParameterString(artistId));
            string response = await _helpers.MakeRequest(url);
            var obj = (SongInfo)JsonConvert.DeserializeObject(response, typeof(SongInfo));
            return obj;
        }
    }
}
