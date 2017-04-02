namespace BreadPlayer.Web._123music
{
    class Endpoints
    {
        public const string BaseURL = "https://123music.to/";
        public const string SearchEndpoint = BaseURL + "search/{0}/{1}/{2}"; // {0} must be either 'songs', 'albums' or 'artists'. {1} is the search keyword. {2} is the page no. if required + .html.
        public const string SearchSuggestionEndpoint = BaseURL + "ajax/search/suggest"; //this must be a POST request with search keyword in POST Body
        public const string SongSourceEndpoint = BaseURL + "ajax/song/sources/{0}"; //{0} must be song-id.
        public const string BillboardSongsEndpoint = BaseURL + "ajax/song/top_playback/{0}"; //{0} must either be 'today', 'week' or 'month'
        public const string BillboardAlbumsEndpoint = BaseURL + "ajax/album/top_playback/{0}";//{0} must either be 'today', 'week' or 'month'
        public const string ArtistsListEndpoint = BaseURL + "ajax/artist/list/{0}"; //{0} must be 'hot'
        public const string AlbumsListEndpoint = BaseURL + "ajax/album/list/{0}"; //{0} must be 'new'
        public const string SongsListEndpoint = BaseURL + "ajax/song/list/{0}"; //{0} must be 'hot'
    }
}
