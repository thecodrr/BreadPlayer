using BreadPlayer.Extensions;
using BreadPlayer.Models;
using BreadPlayer.Services;
using BreadPlayer.Tags.ID3;
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
using BreadPlayer.ViewModels;
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
 
        public static String GetStringForNullOrEmptyProperty(string data, string setInstead)
        {
            return string.IsNullOrEmpty(data) ? setInstead : data;
        }
        public static string CurrentFileToken;
        public static async Task<Mediafile> CreateMediafile(StorageFile file)
        {
            CurrentFileToken = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(file);
            var Mediafile = new Mediafile();

            Mediafile._id = LiteDB.ObjectId.NewObjectId();
            //using (Stream stream = await file.OpenStreamForWriteAsync())
            //{
            //    ID3v2 Data = new ID3v2(true, stream);
            //    Mediafile._id = LiteDB.ObjectId.NewObjectId();
            //    Mediafile.Path = file.Path;
            //    Mediafile.Title = GetStringForNullOrEmptyProperty((await LibVM.GetTextFrame(Data, "TIT2")), System.IO.Path.GetFileNameWithoutExtension(LibraryViewModel.Path));
            //    Mediafile.LeadArtist = GetStringForNullOrEmptyProperty((await LibVM.GetTextFrame(Data, "TPE1")), "Unknown Artist");
            //    Mediafile.Album = GetStringForNullOrEmptyProperty((await LibVM.GetTextFrame(Data, "TALB")), "Unknown Album");
            //    Mediafile.State = PlayerState.Stopped;
            //    var genre = (await LibVM.GetTextFrame(Data, "TCON"));
            //    Mediafile.Genre = genre.Remove(0, (genre).IndexOf(')') + 1);
            //    Mediafile.Year = GetStringForNullOrEmptyProperty((await LibVM.GetTextFrame(Data, "TYER")), "");
            //    Mediafile.TrackNumber = GetStringForNullOrEmptyProperty((await LibVM.GetTextFrame(Data, "TRCK")), "");
            //    Mediafile.OrginalFilename = file.DisplayName;
            //    //Mediafile.Date = DateTime.Now.Date.ToString("dd/MM/yyyy");
            //    //if (Mediafile.Genre != null && Mediafile.Genre != "NaN")
            //    //{
            //    //    LibVM.GenreCollection.Add(Mediafile.Genre);
            //    //}
            //    if (Data.AttachedPictureFrames.Count > 0)
            //    {
            //        var albumartFolder = ApplicationData.Current.LocalFolder;
            //        LibVM.SaveImages(Data, Mediafile);
            //        Mediafile.AttachedPicture = albumartFolder.Path + @"\AlbumArts\" + (Mediafile.Album + Mediafile.LeadArtist).ToLower().ToSha1() + ".jpg";
            //    }
            //}
            Mediafile.Path = file.Path;
            Mediafile.OrginalFilename = file.DisplayName;
            Mediafile.State = PlayerState.Stopped;
            var properties = await file.Properties.GetMusicPropertiesAsync();
            Mediafile.Title = GetStringForNullOrEmptyProperty(properties.Title, System.IO.Path.GetFileNameWithoutExtension(file.Path));
            Mediafile.Album = GetStringForNullOrEmptyProperty(properties.Album, "Unknown Album"); 
            Mediafile.LeadArtist = GetStringForNullOrEmptyProperty(properties.Artist, "Unknown Artist");
            Mediafile.Genre = string.Join("," ,properties.Genre);
            Mediafile.Year = properties.Year.ToString();
            Mediafile.TrackNumber = properties.TrackNumber.ToString();           
            Mediafile.Length = GetStringForNullOrEmptyProperty(properties.Duration.ToString(@"mm\:ss"), "00:00");
            //var x = await file.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.MusicView, 1000, Windows.Storage.FileProperties.ThumbnailOptions.UseCurrentScale);
            //StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView, 300);            
            //    if (thumbnail != null && thumbnail.Type == ThumbnailType.Image)
            //    {
            //        var albumartFolder = ApplicationData.Current.LocalFolder;
            //        LibVM.SaveImages(thumbnail, Mediafile);
            //        Mediafile.AttachedPicture = albumartFolder.Path + @"\AlbumArts\" + (Mediafile.Album + Mediafile.LeadArtist).ToLower().ToSha1() + ".jpg";
            //    }
            
            return Mediafile;
        }
    }

}
