using BreadPlayer.Extensions;
using BreadPlayer.Models;
using BreadPlayer.Services;
using BreadPlayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using BreadPlayer;
using Windows.UI.Core;
using ManagedBass;
using ManagedBass.Tags;
using Windows.Storage.FileProperties;

namespace BreadPlayer.Core
{
    public class CoreMethods
    {
        public static LibraryViewModel LibVM => GenericService<LibraryViewModel>.Instance.GenericClass;
        public static ShellViewModel ShellVM => GenericService<ShellViewModel>.Instance.GenericClass;
        public static MacalifaPlayer Player => GenericService<MacalifaPlayer>.Instance.GenericClass;
        public static PlaylistViewModel PlaylistVM => GenericService<PlaylistViewModel>.Instance.GenericClass;
        public static AlbumArtistViewModel AlbumArtistVM => GenericService<AlbumArtistViewModel>.Instance.GenericClass;
        public static CoreDispatcher Dispatcher { get; set; } = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
        public static SettingsViewModel SettingsVM => GenericService<SettingsViewModel>.Instance.GenericClass;

        public static String GetStringForNullOrEmptyProperty(string data, string setInstead)
        {
            return string.IsNullOrEmpty(data) ? setInstead : data;
        }
        public static async Task<Mediafile> CreateMediafile(StorageFile file, bool cache = true)
        {
           // if(cache == true)
          //      Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(file);

            var Mediafile = new Mediafile();

            Mediafile._id = LiteDB.ObjectId.NewObjectId();
            Mediafile.Path = file.Path;
            Mediafile.OrginalFilename = file.DisplayName;
            Mediafile.State = PlayerState.Stopped;
            var properties = await file.Properties.GetMusicPropertiesAsync();
            Mediafile.Title = GetStringForNullOrEmptyProperty(properties.Title, System.IO.Path.GetFileNameWithoutExtension(file.Path));
            Mediafile.Album = GetStringForNullOrEmptyProperty(properties.Album, "Unknown Album");
            Mediafile.LeadArtist = GetStringForNullOrEmptyProperty(properties.Artist, "Unknown Artist");
            Mediafile.Genre = string.Join(",", properties.Genre);
            Mediafile.Year = properties.Year.ToString();
            Mediafile.TrackNumber = properties.TrackNumber.ToString();
            Mediafile.Length = GetStringForNullOrEmptyProperty(properties.Duration.ToString(@"mm\:ss"), "00:00");
            var albumartFolder = ApplicationData.Current.LocalFolder;
            var albumartLocation = albumartFolder.Path + @"\AlbumArts\" + (Mediafile.Album + Mediafile.LeadArtist).ToLower().ToSha1() + ".jpg";

            if (!File.Exists(albumartLocation))
            {
                StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView, 300);
                if (thumbnail != null && thumbnail.Type == ThumbnailType.Image)
                {
                    LibVM.SaveImages(thumbnail, Mediafile);
                    Mediafile.AttachedPicture = albumartLocation;
                }
            }
            else
            {
                Mediafile.AttachedPicture = albumartLocation;
            }
            return Mediafile;
        }
    }

}
