namespace BreadPlayer.Web._123music
{
    public class Artist
    {
        public string ArtistID
        {
            get; set;
        }
        public string ArtistName { get; set; }
        public string ArtistPhoto { get; set; }

        public Artist(string artistName, string artistPhoto, string artistID)
        {
            ArtistName = artistName;
            ArtistPhoto = artistPhoto;
            ArtistID = artistID;
        }
    }
}
