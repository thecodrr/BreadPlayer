using BreadPlayer.Converters;
using BreadPlayer.Core;
using BreadPlayer.Core.Models;
using BreadPlayer.Extensions;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace BreadPlayer.Helpers
{
    public class TagReaderHelper
    {
        /// <summary>
        /// Create mediafile from StorageFile.
        /// </summary>
        /// <param name="file">the storage file</param>
        /// <param name="cache">cache into the system for future access?</param>
        /// <returns>The created mediafile</returns>
        public static async Task<Mediafile> CreateMediafile(StorageFile file, bool cache = false)
        {
            try
            {
                if (cache)
                {
                    StorageApplicationPermissions.FutureAccessList.Add(file);
                }

                var properties = await file.Properties.GetMusicPropertiesAsync();                
                var mediafile = new Mediafile()
                {
                    Path = file.Path,
                    OrginalFilename = file.DisplayName,
                    Title = properties.Title.GetStringForNullOrEmptyProperty(Path.GetFileNameWithoutExtension(file.Path)),
                    Album = properties.Album.GetStringForNullOrEmptyProperty("Unknown Album"),
                    LeadArtist = properties.Artist.GetStringForNullOrEmptyProperty("Unknown Artist"),
                    Genre = string.Join(",", properties.Genre),
                    Year = properties.Year.ToString(),
                    TrackNumber = properties.TrackNumber.ToString(),
                    Length = new DoubleToTimeConverter().Convert(properties.Duration.TotalSeconds, typeof(double), null, "").ToString(),
                    AddedDate = DateTime.Now.ToString()
                };

                var albumartFolder = ApplicationData.Current.LocalFolder;
                var albumartLocation = albumartFolder.Path + @"\AlbumArts\" + (mediafile.Album + mediafile.LeadArtist).ToLower().ToSha1() + ".jpg";

                if (System.IO.File.Exists(albumartLocation))
                {
                    mediafile.AttachedPicture = albumartLocation;
                }
                return mediafile;
            }
            catch (Exception ex)
            {
                await SharedLogic.NotificationManager.ShowMessageAsync(ex.Message + "||" + file.Path);
                return null;
            }
        }

        /// <summary>
        /// Asynchronously saves all the album arts in the library. 
        /// </summary>
        /// <param name="Data">ID3 tag of the song to get album art data from.</param>
        public static async Task<bool> SaveAlbumArtsAsync(StorageFile file, Mediafile mediafile)
        {
            var albumArt = AlbumArtFileExists(mediafile);
            if (!albumArt.NotExists)
            {
                return false;
            }

            try
            {
                using (StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView, 300, ThumbnailOptions.UseCurrentScale))
                {
                    if (thumbnail == null)
                    {
                        return false;
                    }

                    switch (thumbnail.Type)
                    {
                        case ThumbnailType.Image:
                            var albumart = await ApplicationData.Current.LocalFolder.CreateFileAsync(@"AlbumArts\" + albumArt.FileName + ".jpg", CreationCollisionOption.FailIfExists);
                            IBuffer buf;
                            Windows.Storage.Streams.Buffer inputBuffer = new Windows.Storage.Streams.Buffer(1024);
                            using (IRandomAccessStream albumstream = await albumart.OpenAsync(FileAccessMode.ReadWrite))
                            {
                                while ((buf = await thumbnail.ReadAsync(inputBuffer, inputBuffer.Capacity, InputStreamOptions.None)).Length > 0)
                                {
                                    await albumstream.WriteAsync(buf);
                                }

                                return true;
                            }
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                await SharedLogic.NotificationManager.ShowMessageAsync(ex.Message + "||" + file.Path);
            }

            return false;
        }

        /// <summary>
        /// Generate album art path for a mediafile.
        /// </summary>
        /// <param name="mediafile">the mediafile</param>
        /// <returns></returns>
        private static string GetAlbumArtPath(Mediafile mediafile)
        {
            var albumartFolder = ApplicationData.Current.LocalFolder;
            var albumartLocation = albumartFolder.Path + @"\AlbumArts\" + (mediafile.Album + mediafile.LeadArtist).ToLower().ToSha1() + ".jpg";
            return albumartLocation;
        }

        /// <summary>
        /// check to see if album art exists or not and return a tuple of the result.
        /// </summary>
        /// <param name="file">the mediafile of which to check the album art</param>
        /// <returns>tuple containing bool and the filename of the album art.</returns>
        private static (bool NotExists, string FileName) AlbumArtFileExists(Mediafile file)
        {
            if (!System.IO.File.Exists(GetAlbumArtPath(file)))
            {
                return (true, Path.GetFileNameWithoutExtension(GetAlbumArtPath(file)));
            }
            return (false, Path.GetFileNameWithoutExtension(GetAlbumArtPath(file)));
        }
    }
}
