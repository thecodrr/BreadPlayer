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
using Windows.Media;

namespace Macalifa
{
    public class CoreWindowLogic : CoreMethods
    {
        #region Fields
        private const string pathKey = "path";
        private const string posKey = "position";
        private const string volKey = "volume";
        SystemMediaTransportControls _smtc;
        #endregion

        #region Load/Save Logic
        public static async void Replay(bool onlyVol = false)
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
                        Player.Volume = volume;
                    }
                    else
                    {
                        path = jsonObject.GetNamedString(pathKey, "");
                        double position = jsonObject.GetNamedNumber(posKey);
                        Player.PlayerState = PlayerState.Paused;
                        if (LibVM.TracksCollection.Elements.Any(t => t.State == PlayerState.Playing))
                        {
                            var sa = LibVM.TracksCollection.Elements.Where(l => l.State == PlayerState.Playing);
                            foreach (var mp3 in sa) mp3.State = PlayerState.Stopped;
                        }
                        ShellVM.Play(await StorageFile.GetFileFromPathAsync(path), null, position, false, volume);
                    }
                }
            }
            if (path != "" && LibVM.TracksCollection != null && LibVM.TracksCollection.Elements.Any(t => t.Path == path))
                LibVM.TracksCollection.Elements.Single(t => t.Path == path).State = PlayerState.Playing;

        }
        public static async void Stringify()
        {
            JsonObject jsonObject = new JsonObject();
            if (Player.CurrentlyPlayingFile != null && !string.IsNullOrEmpty(Player.CurrentlyPlayingFile.Path))
            {
                jsonObject[pathKey] = JsonValue.CreateStringValue(Player.CurrentlyPlayingFile.Path);
                jsonObject[posKey] = JsonValue.CreateNumberValue(Player.Position);
            }
            jsonObject[volKey] = JsonValue.CreateNumberValue(Player.Volume);
            StorageFile file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("lastplaying.mc", CreationCollisionOption.ReplaceExisting);

            using (var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
            {
                using (var outputStream = stream.GetOutputStreamAt(0))
                {
                    using (var dataWriter = new Windows.Storage.Streams.DataWriter(outputStream))
                    {
                        dataWriter.WriteString(jsonObject.Stringify());
                        await dataWriter.StoreAsync();
                        await outputStream.FlushAsync();
                    }
                }
            }
        }
        #endregion

        #region SystemMediaTransportControls Methods/Events
        void InitSmtc()
        {
            _smtc = SystemMediaTransportControls.GetForCurrentView();
            _smtc.IsEnabled = true;
            _smtc.IsPlayEnabled = true;
            _smtc.IsPauseEnabled = true;
            _smtc.ButtonPressed += _smtc_ButtonPressed;
            Player.MediaStateChanged += Player_MediaStateChanged;
        }
        private async void _smtc_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                switch (args.Button)
                {
                    case SystemMediaTransportControlsButton.Play:
                        await Player.Play();
                        break;
                    case SystemMediaTransportControlsButton.Pause:
                        await Player.Pause();
                        break;
                    default:
                        break;
                }
            });
        }
        private void Player_MediaStateChanged(object sender, Events.MediaStateChangedEventArgs e)
        {
            if (_smtc != null)
                switch (e.NewState)
                {
                    case PlayerState.Playing:
                        _smtc.PlaybackStatus = MediaPlaybackStatus.Playing;
                        break;
                    case PlayerState.Paused:
                        _smtc.PlaybackStatus = MediaPlaybackStatus.Paused;
                        break;
                    case PlayerState.Stopped:
                        _smtc.PlaybackStatus = MediaPlaybackStatus.Stopped;
                        break;
                    default:
                        break;
                }
        }
        #endregion

        #region CoreWindow Titlebar Methods/Events

        #endregion
        #region Ctor
        public CoreWindowLogic()
        {
            InitSmtc();
        }
        #endregion

    }
}
