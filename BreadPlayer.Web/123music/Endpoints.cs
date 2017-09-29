namespace BreadPlayer.Web._123music
{
    internal class Endpoints
    {
        public const string BaseUrl = "https://123music.to/";
        public const string SearchEndpoint = BaseUrl + "search/{0}/{1}/{2}"; // {0} must be either 'songs', 'albums' or 'artists'. {1} is the search keyword. {2} is the page no. if required + .html.
        public const string SearchSuggestionEndpoint = BaseUrl + "ajax/search/suggest"; //this must be a POST request with search keyword in POST Body
        public const string SongSourceEndpoint = BaseUrl + "ajax/song/sources/{0}"; //{0} must be song-id.
        public const string BillboardSongsEndpoint = BaseUrl + "ajax/song/top_playback/{0}"; //{0} must either be 'today', 'week' or 'month'
        public const string BillboardAlbumsEndpoint = BaseUrl + "ajax/album/top_playback/{0}";//{0} must either be 'today', 'week' or 'month'
        public const string ArtistsListEndpoint = BaseUrl + "ajax/artist/list/{0}"; //{0} must be 'hot'
        public const string AlbumsListEndpoint = BaseUrl + "ajax/album/list/{0}"; //{0} must be 'new'
        public const string SongsListEndpoint = BaseUrl + "ajax/song/list/{0}"; //{0} must be 'hot'
    }
}