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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Xaml;
using Macalifa.Services;
using Windows.Storage;
using System.IO;
using Macalifa.Core;
using Macalifa.ViewModels;
namespace Macalifa
{
    public class CoreWindowLogic
    {
        Macalifa.Core.MacalifaPlayer Player => MacalifaPlayerService.Instance.Player;
        ShellViewModel ShellVM => ShellViewService.Instance.ShellVM;
        LibraryViewModel LibVM => LibraryViewService.Instance.LibVM;
        private const string pathKey = "path";
        private const string posKey = "position";
        private const string volKey = "volume";
        public CoreWindowLogic()
        {
           
        }

        public async void Replay(bool onlyVol = false)
        {
            string path = "";
            if (File.Exists(ApplicationData.Current.TemporaryFolder.Path + @"\lastplaying.mc"))
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(ApplicationData.Current.TemporaryFolder.Path + "\\lastplaying.mc");
                string text = await FileIO.ReadTextAsync(file);
                if (!string.IsNullOrEmpty(text))
                {
                    JsonObject jsonObject = JsonObject.Parse(text);
                    var volume = jsonObject.GetNamedNumber(volKey, 50);
                    if (jsonObject.Count == 1 || onlyVol)
                    {
                        ShellVM.Volume = volume;
                    }
                    else
                    {
                        path = jsonObject.GetNamedString(pathKey, "");
                        double position = jsonObject.GetNamedNumber(posKey);
                        Player.PlayerState = PlayerState.Paused;
                        if (LibVM.TracksCollection.Elements.Any(t => t.State == PlayerState.Playing))
                            LibVM.TracksCollection.Elements.SingleOrDefault(t => t.State == PlayerState.Playing).State = PlayerState.Stopped;
                        ShellVM.Play(await StorageFile.GetFileFromPathAsync(path), position, false, volume);
                    }

                    GC.Collect();
                }               
            }
            if (path != "" && LibVM.TracksCollection != null && LibVM.TracksCollection.Elements.Any(t => t.Path == path))
                LibVM.TracksCollection.Elements.Single(t => t.Path == path).State = PlayerState.Playing;
            //else if (LibVM.TracksCollection.Elements != null)
            //    LibVM.TracksCollection.Elements.SingleOrDefault(t => t.State == PlayerState.Playing).State = PlayerState.Stopped;

        }    
        public async void Stringify()
        {
            JsonObject jsonObject = new JsonObject();           
            if (!string.IsNullOrEmpty(Player.CurrentlyPlayingFile))
            {
                jsonObject[pathKey] = JsonValue.CreateStringValue(Player.CurrentlyPlayingFile);
                jsonObject[posKey] = JsonValue.CreateNumberValue(Player.Position);               
            }
            jsonObject[volKey] = JsonValue.CreateNumberValue(Player.Volume);
            if (File.Exists(ApplicationData.Current.TemporaryFolder.Path + @"\lastplaying.mc"))
                File.Delete(ApplicationData.Current.TemporaryFolder.Path + @"\lastplaying.mc");
            StorageFile file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("lastplaying.mc");
            Windows.Storage.CachedFileManager.DeferUpdates(file);
            await FileIO.WriteTextAsync(file, jsonObject.Stringify());
            GC.Collect();
        }
    }
}
