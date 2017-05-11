using BreadPlayer.Web.BaiduLyricsAPI.Interface;

namespace BreadPlayer.Web.BaiduLyricsAPI.Models
{
    public class Album : IQueryResult
    {
        public string AlbumId { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Publishcompany { get; set; }
        public string Prodcompany { get; set; }
        public string Country { get; set; }
        public string Language { get; set; }
        public int SongsTotal { get; set; }
        public string Info { get; set; }
        public string Styles { get; set; }
        public string StyleId { get; set; }
        public string Publishtime { get; set; }
        public string ArtistTingUid { get; set; }
        public string AllArtistTingUid { get; set; }
        public string Gender { get; set; }
        public string Area { get; set; }
        public string PicSmall { get; set; }
        public string PicBig { get; set; }
        public int Hot { get; set; }
        public int FavoritesNum { get; set; }
        public int RecommendNum { get; set; }
        public string ArtistId { get; set; }
        public string AllArtistId { get; set; }
        public string PicRadio { get; set; }
        public string PicS180 { get; set; }
        public string GetName()
        {
            return Title;
        }
        public QueryType GetSearchResultType()
        {
            return QueryType.Album;
        }
    }
}
