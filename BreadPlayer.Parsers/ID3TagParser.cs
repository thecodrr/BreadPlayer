using BreadPlayer.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BreadPlayer.Parsers
{
    public class ID3TagParser
    {
        public static void WriteTagsToMediafile(IMediafile mediaFile, byte[] array, double Length)
        {
            using (var stream = new MemoryStream(array))
            {
                try
                {
                    var file = TagLib.File.Create(new TagLib.StreamFileAbstraction(mediaFile.Title, stream, stream));
                    mediaFile.OrginalFilename = mediaFile.Title;
                    mediaFile.Title = file.Tag.Title ?? mediaFile.Title;
                    mediaFile.LeadArtist = file.Tag.FirstPerformer ?? "Unknown Artist";
                    mediaFile.Album = file.Tag.Album ?? "Unknown Album";
                    mediaFile.TrackNumber = Convert.ToInt32(file.Tag.Track);
                    mediaFile.Year = file.Tag.Year.ToString();
                    mediaFile.Path = "Playing from Network.";
                    mediaFile.Genre = file.Tag.FirstGenre;
                    if (file.Tag.Pictures.Length > 0)
                        mediaFile.AttachedPictureBytes = file.Tag.Pictures[0].Data.Data;
                    if (Length <= 1)
                    {
                        int intKiloBitFileSize = (int)((8 * Convert.ToInt64(mediaFile.Size)) / 1000);
                        Length = intKiloBitFileSize / file.Properties.AudioBitrate;
                    }
                    mediaFile.Length = TimeSpan.FromSeconds(Length).ToString(@"mm\:ss");
                }
                catch
                {
                }
            }
        }
    }
}
