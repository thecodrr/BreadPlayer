namespace BreadPlayer.Web._123music
{
    public class Album
    {
        public string AlbumID
        {
            get; set;
        }
        public string ArtistID
        {
            get; set;
        }
        public string ArtistName { get; set; }
        public string AlbumName { get; set; }
        public string AlbumArt { get; set; }

        public Album(string albumName, string albumID, string albumArt, string artistName, string artistID)
        {
            AlbumName = albumName;
            AlbumID = albumID;
            AlbumArt = albumArt;
            ArtistName = artistName;
            ArtistID = artistID;
        }
    }
}
