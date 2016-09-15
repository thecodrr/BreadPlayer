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
        private const string pathKey = "path";
        private const string posKey = "position";
        private const string volKey = "volume";
        public CoreWindowLogic()
        {
           
        }

        public async void Replay(bool onlyVol = false)
        {
            if (File.Exists(ApplicationData.Current.TemporaryFolder.Path + @"\lastplaying.mc"))
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(ApplicationData.Current.TemporaryFolder.Path + "\\lastplaying.mc");
                string text = await FileIO.ReadTextAsync(file);
                JsonObject jsonObject = JsonObject.Parse(text);
                var volume = jsonObject.GetNamedNumber(volKey, 50);
                if (jsonObject.Count == 1 || onlyVol)
                {
                    ShellVM.Volume = volume;
                }
                else
                {
                    string path = jsonObject.GetNamedString(pathKey, "");
                    double position = jsonObject.GetNamedNumber(posKey);
                    Player.PlayerState = PlayerState.Paused;
                    Shell.Play(await StorageFile.GetFileFromPathAsync(path), position, false, volume);
                }
                GC.Collect();              
            }
        }    
        public async void Stringify()
        {
            JsonObject jsonObject = new JsonObject();           
            if (!string.IsNullOrEmpty(Player.CurrentlyPlayingFile))
            {
                jsonObject[pathKey] = JsonValue.CreateStringValue(Player.CurrentlyPlayingFile);
                jsonObject[posKey] = JsonValue.CreateNumberValue(Player.Position);               
            }
            jsonObject[volKey] = JsonValue.CreateNumberValue(Player.Volume * 100);
            if (File.Exists(ApplicationData.Current.TemporaryFolder.Path + @"\lastplaying.mc"))
                File.Delete(ApplicationData.Current.TemporaryFolder.Path + @"\lastplaying.mc");
            StorageFile file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("lastplaying.mc");
            Windows.Storage.CachedFileManager.DeferUpdates(file);
            await FileIO.WriteTextAsync(file, jsonObject.Stringify());
            GC.Collect();
        }
    }
}
