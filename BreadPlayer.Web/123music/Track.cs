namespace BreadPlayer.Web._123music
{
    public class Track
    {
        public string SongId { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string ArtistId { get; set; }
        public int ListenCount { get; set; }

        public Track(string songId, string title, string artist, string artistId, int listenCount)
        {
            SongId = songId;
            Title = title;
            Artist = artist;
            ArtistId = artistId;
            ListenCount = listenCount;
        }
    }
}