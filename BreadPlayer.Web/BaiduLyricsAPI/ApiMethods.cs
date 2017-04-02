using BreadPlayer.Web.BaiduLyricsAPI.Response;
using System.Threading.Tasks;
using Newtonsoft.Json;
using BreadPlayer.Web.BaiduLyricsAPI.Models;

namespace BreadPlayer.Web.BaiduLyricsAPI
{
    public class ApiMethods
    {
        Helpers helpers = new Helpers();
        public async Task<SongListResponse> RequestSongListFromArtist(string artistId)
        {
            var url = helpers.GetCallURL(Endpoints.METHOD_GET_ARTISTSONGLIST, helpers.GetAlbumByArtistParameterString(artistId));
            string response = await helpers.MakeRequest(url);   
            var obj =  (SongListResponse)JsonConvert.DeserializeObject(response, typeof(SongListResponse));
            return obj;
        }
        public async Task<AlbumDetailResponse> RequestAlbumDetail(string albumId)
        {
            var url = helpers.GetCallURL(Endpoints.METHOD_GET_ALBUMINFO, helpers.GetAlbumDetailParameterString(albumId));
            string response = await helpers.MakeRequest(url);
            var obj = (AlbumDetailResponse)JsonConvert.DeserializeObject(response, typeof(AlbumDetailResponse));
            return obj;
        }
        public async Task<QueryMergeResponse> Search(string query)
        {
            var url = helpers.GetCallURL(Endpoints.METHOD_QUERY_MERGE, helpers.GetQueryParameterString(query));
            string response = await helpers.MakeRequest(url);
            var obj = (QueryMergeResponse)JsonConvert.DeserializeObject(response, typeof(QueryMergeResponse));
            return obj;
        }
        public async Task<ArtistAlbumListResponse> RequestAlbumByArtist(string artistId)
        {
            var url = helpers.GetCallURL(Endpoints.METHOD_GET_ARTISTALUBMLIST, helpers.GetAlbumByArtistParameterString(artistId));
            string response = await helpers.MakeRequest(url);
            var obj = (ArtistAlbumListResponse)JsonConvert.DeserializeObject(response, typeof(ArtistAlbumListResponse));
            return obj;
        }
        public async Task<ArtistInfo> RequestArtistInfo(string artistId)
        {
            var url = helpers.GetCallURL(Endpoints.METHOD_GET_ARTISTINFO, helpers.GetMusicAndArtistInfoParameterString(artistId));
            string response = await helpers.MakeRequest(url);
            var obj = (ArtistInfo)JsonConvert.DeserializeObject(response, typeof(ArtistInfo));
            return obj;
        }
        public async Task<SongInfo> RequestMusicInfo(string artistId)
        {
            var url = helpers.GetCallURL(Endpoints.METHOD_GET_SONGINFO, helpers.GetMusicAndArtistInfoParameterString(artistId));
            string response = await helpers.MakeRequest(url);
            var obj = (SongInfo)JsonConvert.DeserializeObject(response, typeof(SongInfo));
            return obj;
        }
    }
}
