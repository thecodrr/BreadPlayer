using System;
namespace BreadPlayer.Interfaces
{
    public interface IMediafile
    {
        DateTime AddedDate { get; set; }
        string Album { get; set; }
        string AttachedPicture { get; set; }
        byte[] AttachedPictureBytes { get; set; }
        string BeatsPerMinutes { get; set; }
        byte[] ByteArray { get; set; }
        string Comment { get; set; }
        string Composer { get; set; }
        string ContentGroupDescription { get; set; }
        string CopyrightMessage { get; set; }
        string Date { get; set; }
        string EncodedBy { get; set; }
        string EncryptedMetaFile { get; set; }
        string FolderPath { get; set; }
        string Genre { get; set; }
        long Id { get; set; }
        bool IsFavorite { get; set; }
        bool IsPinned { get; set; }
        bool IsPlaylistSong { get; set; }
        bool IsSelected { get; set; }
        DateTime LastPlayed { get; set; }
        string LeadArtist { get; set; }
        string Length { get; set; }
        string Lyric { get; set; }
        //MediaLocationType MediaLocation { get; set; }
        string OrginalFilename { get; set; }
        string Path { get; set; }
        int PlayCount { get; set; }
        string Publisher { get; set; }
        string Size { get; set; }
        //PlayerState State { get; set; }
        string Subtitle { get; set; }
        string SynchronizedLyric { get; set; }
        string TextSearchKey { get; }
        string TileId { get; }
        string Title { get; set; }
        int TrackNumber { get; set; }
        string Year { get; set; }

        int CompareTo(IMediafile compareTo);
        string GetTextSearchKey();
    }
}