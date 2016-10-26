/* 
	BreadPlayer. A music player made for Windows 10 store.
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
using BreadPlayer.Services;
using Windows.Storage;
using System.IO;
using BreadPlayer.Core;
using BreadPlayer.ViewModels;
using Windows.Media;
using Windows.UI.Notifications;
using Windows.Storage.AccessCache;

namespace BreadPlayer
{
    public class CoreWindowLogic : CoreMethods
    {
        #region Fields
        private const string pathKey = "path";
        private const string posKey = "position";
        private const string volKey = "volume";
        private const string shuffleKey = "shuffle";
        private const string repeatKey = "repeat";
        private const string foldersKey = "folders";
        static SystemMediaTransportControls _smtc;
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
                    ShellVM.Repeat = jsonObject.GetNamedBoolean(repeatKey, false);
                    ShellVM.Shuffle = jsonObject.GetNamedBoolean(shuffleKey, false);
                    var volume = jsonObject.GetNamedNumber(volKey, 50);
                    if (jsonObject.ContainsKey(foldersKey))
                    {
                        var folderPaths = jsonObject[foldersKey].GetArray().ToList();
                        if (folderPaths != null)
                        {
                            foreach (var folder in folderPaths)
                            {
                                var storageFolder = await StorageFolder.GetFolderFromPathAsync(folder.GetString());
                                SettingsVM.LibraryFoldersCollection.Add(storageFolder);
                            }
                        }
                    }
                    if (jsonObject.Count <= 4 || onlyVol)
                    {
                        Player.Volume = volume;                       
                    }
                    else
                    {               
                        path = jsonObject.GetNamedString(pathKey, "");
                        double position = jsonObject.GetNamedNumber(posKey);
                        Player.PlayerState = PlayerState.Paused;
                     try {   
                        ShellVM.Play(await StorageFile.GetFileFromPathAsync(path), null, position, false, volume);
                        }
                        catch (UnauthorizedAccessException ex) { }
                    }
                }
            }
            if (LibVM.TracksCollection.Elements.Any(t => t.State == PlayerState.Playing))
            {
                var sa = LibVM.TracksCollection.Elements.Where(l => l.State == PlayerState.Playing);
                foreach (var mp3 in sa) mp3.State = PlayerState.Stopped;
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
            if(Core.CoreMethods.SettingsVM.LibraryFoldersCollection.Any())
            {
                JsonArray array = new JsonArray();
                foreach (var folder in SettingsVM.LibraryFoldersCollection)
                    array.Add(JsonValue.CreateStringValue(folder.Path));
                jsonObject[foldersKey] = array;
            }
            jsonObject[shuffleKey] = JsonValue.CreateBooleanValue(ShellVM.Shuffle);
            jsonObject[repeatKey] = JsonValue.CreateBooleanValue(ShellVM.Repeat);
            jsonObject[volKey] = JsonValue.CreateNumberValue(Player.Volume);
            try
            {
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
            catch(UnauthorizedAccessException ex) { ShellVM.Status = ex.Message; }
        }
        #endregion

        #region SystemMediaTransportControls Methods/Events
        static void InitSmtc()
        {
            _smtc = SystemMediaTransportControls.GetForCurrentView();
            _smtc.IsEnabled = false;
            _smtc.ButtonPressed += _smtc_ButtonPressed;
           
            _smtc.IsPlayEnabled = true;
            _smtc.IsPauseEnabled = true;
            _smtc.IsStopEnabled = true;
            _smtc.PlaybackStatus = MediaPlaybackStatus.Closed;
            _smtc.AutoRepeatMode = MediaPlaybackAutoRepeatMode.Track;
            Player.MediaStateChanged += Player_MediaStateChanged;
        }
       public static void UpdateSmtc()
        {
            _smtc.IsEnabled = true;
            _smtc.DisplayUpdater.Type = MediaPlaybackType.Music;
            var musicProps = _smtc.DisplayUpdater.MusicProperties;
            if (Player.CurrentlyPlayingFile != null)
            {
                //await _smtc.DisplayUpdater.CopyFromFileAsync(MediaPlaybackType.Music, await StorageFile.GetFileFromPathAsync(Player.CurrentlyPlayingFile.Path));
                musicProps.Title = Player.CurrentlyPlayingFile.Title;
                musicProps.Artist = Player.CurrentlyPlayingFile.LeadArtist;
                musicProps.AlbumTitle = Player.CurrentlyPlayingFile.Album;
            }
            _smtc.DisplayUpdater.ClearAll();
            _smtc.DisplayUpdater.Update();
        }
        private static async void _smtc_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                switch (args.Button)
                {
                    case SystemMediaTransportControlsButton.Play:
                    case SystemMediaTransportControlsButton.Pause:
                        ShellVM.PlayPauseCommand.Execute(null);
                        break;
                    case SystemMediaTransportControlsButton.Next:
                        ShellVM.PlayNextCommand.Execute(null);
                        break;
                    case SystemMediaTransportControlsButton.Previous:
                        ShellVM.PlayPreviousCommand.Execute(null);
                        break;
                    default:
                        break;
                }
            });
        }
        private static void Player_MediaStateChanged(object sender, Events.MediaStateChangedEventArgs e)
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

        #region CoreWindow Dispose Methods
        public static void DisposeObjects()
        {
            Player.Dispose();
            LibVM.db.Dispose();
        }
        #endregion

        /// <summary>
        /// Sends a toast notification
        /// </summary>
        /// <param name="msg">Message to send</param>
        /// <param name="subMsg">Sub message</param>
        public void ShowToast(string msg, string subMsg = null)
        {
            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText02);

            var toastImageElements = toastXml.GetElementsByTagName("image");
            var toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(msg));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode(subMsg));

            var toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
        public static async void ShowMessage(string msg, string title)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(msg, title);           
            await dialog.ShowAsync();
        }
        #region Ctor
        public CoreWindowLogic()
        {
            if (StorageApplicationPermissions.FutureAccessList.Entries.Count >= 999)
                StorageApplicationPermissions.FutureAccessList.Clear();
            InitSmtc();
        }
        #endregion

    }
}
