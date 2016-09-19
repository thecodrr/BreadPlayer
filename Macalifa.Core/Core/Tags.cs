/* 
	Macalifa. A music player made for Windows 10 store.
    Copyright (C) 2016  theweavrs (Abdullah Atta)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.IO;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;
using ManagedBass.Tags;
using Windows.Storage.FileProperties;
using Macalifa.Tags.ID3.ID3v2Frames.TextFrames;
using Macalifa.Tags;
using Macalifa.Tags.ID3;
using System.Threading.Tasks;
namespace Macalifa.Core
{
   public class Tags : ViewModelBase
    {
        ID3v2 Data;
        string Path;
        FileStream inStream;
        public Tags(int coreHandle, string filePath)
        {
            inStream = new FileStream(filePath, FileMode.Open,
                              FileAccess.Read, FileShare.ReadWrite);
            Stream FS = new MemoryStream();            
            inStream.CopyTo(FS);
            Path = filePath;
            Data = new ID3v2(true, FS);
            Init();
            inStream.Dispose();
            FS.Dispose();
        }
        async void Init()
        {
           
          await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync( Windows.UI.Core.CoreDispatcherPriority.Normal,
async() =>
{
    Title = string.IsNullOrEmpty((await GetTextFrame(Data, "TIT2"))) ? System.IO.Path.GetFileNameWithoutExtension(Path) : (await GetTextFrame(Data, "TIT2"));
    Artist = string.IsNullOrEmpty(await GetTextFrame(Data, "TPE1")) ? "Unknown Artist" : (await GetTextFrame(Data, "TPE1"));
    Album = string.IsNullOrEmpty(await GetTextFrame(Data, "TALB")) ? "Unknown Album" : (await GetTextFrame(Data, "TALB"));
    foreach (Macalifa.Tags.ID3.ID3v2Frames.BinaryFrames.AttachedPictureFrame TF in Data.AttachedPictureFrames)
        if (TF.FrameID == "APIC")
        {
            AlbumArt = TF.Picture;
        }
}
);
        }
        string title;
        public string Title
        {
            get { return !string.IsNullOrEmpty(title) ? title : System.IO.Path.GetFileNameWithoutExtension(Path); }
            set { Set(ref title, value); }
        }
        string artist;
        public string Artist
        {
            get { return !string.IsNullOrEmpty(artist) ? artist : "Unknown Artist"; }
            set { Set(ref artist, value); }
        }
        string album;
        public string Album
        {
            get { return !string.IsNullOrEmpty(album) ? album : "Unknown Album"; }
            set { Set(ref album, value); }
        }
        ImageSource albumart;
        public ImageSource AlbumArt
        {
            get { return albumart; }
            set { Set(ref albumart, value); }
        }
        // public async Task<ImageSource> GetImage(ID3v2 info)
        //{
        //    return await Task.Run(() =>
        //    {
                
        //    });
        //}
        public async Task<string> GetTextFrame(ID3v2 info, string FrameID)
        {
            // string Text = "";
            return await Task.Run(() =>
            {
                foreach (TextFrame TF in info.TextFrames)
                    if (TF.FrameID == FrameID)
                    {
                        return TF.Text;
                    }
                return "";
            });

        }

    }
}
