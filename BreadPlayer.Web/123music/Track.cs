namespace BreadPlayer.Web._123music
{
    public class Track
    {
        public string SongID { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string ArtistID { get; set; }
        public int ListenCount { get; set; }

        public Track(string songID, string title, string artist, string artistID, int listenCount)
        {
            SongID = songID;
            Title = title;
            Artist = artist;
            ArtistID = artistID;
            ListenCount = listenCount;
        }
    }
}
