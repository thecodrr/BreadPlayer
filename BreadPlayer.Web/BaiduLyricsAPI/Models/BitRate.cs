namespace BreadPlayer.Web.BaiduLyricsAPI.Models
{
    public class BitRate
    {
        public int FileBitrate { get; set; }
        public string FileLink { get; set; }
        public string FileExtension { get; set; }
        public int Original { get; set; }
        public int FileSize { get; set; }
        public int FileDuration { get; set; }
        public string ShowLink { get; set; }
        public int SongFileId { get; set; }
        public string ReplayGain { get; set; }
        public int Free { get; set; }
    }
}