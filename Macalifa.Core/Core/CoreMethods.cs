using Macalifa.Extensions;
using Macalifa.Models;
using Macalifa.Services;
using Macalifa.Tags.ID3;
using Macalifa.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Macalifa;
using Windows.UI.Core;
using ManagedBass;
using ManagedBass.Tags;

namespace Macalifa.Core
{
    public class CoreMethods
    {
        public static LibraryViewModel LibVM => GenericService<LibraryViewModel>.Instance.GenericClass;
        public static ShellViewModel ShellVM => GenericService<ShellViewModel>.Instance.GenericClass;
        public static MacalifaPlayer Player => GenericService<MacalifaPlayer>.Instance.GenericClass;
        public static PlaylistViewModel PlaylistVM => GenericService<PlaylistViewModel>.Instance.GenericClass;
       
        public static String GetStringForNullOrEmptyProperty(string data, string setInstead)
        {
            return string.IsNullOrEmpty(data) ? setInstead : data;
        }
        public static async Task<Mediafile> CreateMediafile(StorageFile file)
        {
            Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(file);
               var Mediafile = new Mediafile();
            using (var stream = await file.OpenStreamForWriteAsync())
            {
                ID3v2 Data = new ID3v2(true, stream);

                Mediafile._id = LiteDB.ObjectId.NewObjectId();
                Mediafile.Path = file.Path;
                Mediafile.Title = GetStringForNullOrEmptyProperty((await LibVM.GetTextFrame(Data, "TIT2")), System.IO.Path.GetFileNameWithoutExtension(LibraryViewModel.Path));
                Mediafile.LeadArtist = GetStringForNullOrEmptyProperty((await LibVM.GetTextFrame(Data, "TPE1")), "Unknown Artist");
                Mediafile.Album = GetStringForNullOrEmptyProperty((await LibVM.GetTextFrame(Data, "TALB")), "Unknown Album");
                Mediafile.State = PlayerState.Stopped;
                //var genre = (await LibVM.GetTextFrame(Data, "TCON"));
                //Mediafile.Genre = genre.Remove(0, (genre).IndexOf(')') + 1);
                //Mediafile.Date = DateTime.Now.Date.ToString("dd/MM/yyyy");
                //if (Mediafile.Genre != null && Mediafile.Genre != "NaN")
                //{
                //    LibVM.GenreCollection.Add(Mediafile.Genre);
                //}
                if (Data.AttachedPictureFrames.Count > 0)
                {
                    var albumartFolder = ApplicationData.Current.LocalFolder;
                    LibVM.SaveImages(Data, Mediafile);
                    Mediafile.AttachedPicture = albumartFolder.Path + @"\AlbumArts\" + (Mediafile.Album + Mediafile.LeadArtist).ToLower().ToSha1() + ".jpg";
                }
            }
            return Mediafile;
        }
    }

}
