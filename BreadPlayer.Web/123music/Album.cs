namespace BreadPlayer.Web._123music
{
    public class Album
    {
        public string AlbumId
        {
            get; set;
        }

        public string ArtistId
        {
            get; set;
        }

        public string ArtistName { get; set; }
        public string AlbumName { get; set; }
        public string AlbumArt { get; set; }

        public Album(string albumName, string albumId, string albumArt, string artistName, string artistId)
        {
            AlbumName = albumName;
            AlbumId = albumId;
            AlbumArt = albumArt;
            ArtistName = artistName;
            ArtistId = artistId;
        }
    }
}