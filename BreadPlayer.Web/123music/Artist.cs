namespace BreadPlayer.Web._123music
{
    public class Artist
    {
        public string ArtistId
        {
            get; set;
        }

        public string ArtistName { get; set; }
        public string ArtistPhoto { get; set; }

        public Artist(string artistName, string artistPhoto, string artistId)
        {
            ArtistName = artistName;
            ArtistPhoto = artistPhoto;
            ArtistId = artistId;
        }
    }
}