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

namespace Macalifa.Core
{
    public class CoreMethods
    {
        LibraryViewModel LibVM => LibraryViewService.Instance.LibVM;
        String GetStringForNullOrEmptyProperty(string data, string setInstead)
        {
            return string.IsNullOrEmpty(data) ? setInstead : data;
        }
        public async Task<Mediafile> CreateMediafile(Stream stream, StorageFile file)
        {
            var Mediafile = new Mediafile();
            ID3v2 Data = new ID3v2(true, stream);
            Mediafile._id = LiteDB.ObjectId.NewObjectId();
            Mediafile.Path = LibraryViewModel.Path;
            Mediafile.Title = GetStringForNullOrEmptyProperty((await LibVM.GetTextFrame(Data, "TIT2")), System.IO.Path.GetFileNameWithoutExtension(LibraryViewModel.Path));
            Mediafile.LeadArtist = GetStringForNullOrEmptyProperty(await LibVM.GetTextFrame(Data, "TPE1"), "Unknown Artist");
            Mediafile.Album = GetStringForNullOrEmptyProperty(await LibVM.GetTextFrame(Data, "TALB"), "Unknown Album");
          
            //Mediafile.Length = (await GetMediaDuration(stream)).ToString();/* GetStringForNullOrEmptyProperty(await LibVM.GetTextFrame(Data, "TLEN"), "0")*/;
            Mediafile.State = PlayerState.Stopped;
            Mediafile.Genre = (await LibVM.GetTextFrame(Data, "TCON")).Remove(0, (await LibVM.GetTextFrame(Data, "TCON")).IndexOf(')') + 1);
            Mediafile.Date = DateTime.Now.Date.ToString("dd/MM/yyyy");
            if (Mediafile.Genre != null && Mediafile.Genre != "NaN")
            {
                LibVM.GenreCollection.Add(Mediafile.Genre);
            }
            if (Data.AttachedPictureFrames.Count > 0)
            {
                var albumartFolder = ApplicationData.Current.LocalFolder;//Data.AttachedPictureFrames[0].Picture;
                LibVM.SaveImages(Data, Mediafile);
                Mediafile.AttachedPicture = albumartFolder.Path + @"\AlbumArts\" + (Mediafile.Album + Mediafile.LeadArtist).ToLower().ToSha1() + ".jpg";
                GC.Collect();
            }
            return Mediafile;
        }

        async Task<double> GetMediaDuration(Stream fs)
        {
            double duration = 0.0;
            await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {

                Mp3Frame frame = Mp3Frame.LoadFromStream(fs);
                while (frame != null)
                {
                    duration += (double)frame.SampleCount / (double)frame.SampleRate;
                    frame = Mp3Frame.LoadFromStream(fs);
                }

            });


                return duration;
        }
    }
    
}
