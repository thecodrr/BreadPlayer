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
using Windows.Storage;
using BreadPlayer.Core;
using Windows.Media;
using Windows.UI.Notifications;
using Windows.Storage.AccessCache;
using Windows.Media.Playback;
using Windows.Foundation.Metadata;
using Windows.Media.Core;
using Windows.System;
using BreadPlayer.Common;

namespace BreadPlayer
{
    public class CoreWindowLogic : SharedLogic
    {
        #region Fields
        private const string pathKey = "path";
        private const string posKey = "position";
        private const string volKey = "volume";
        private const string foldersKey = "folders";
        private const string timeclosedKey = "timeclosed";
        static SystemMediaTransportControls _smtc;
        static string path = "";
        #endregion

        #region Load/Save Logic
        public static async void LoadSettings(bool onlyVol = false, bool play = false)
        {
            var volume = RoamingSettingsHelper.GetSetting<double>(volKey, 50.0);
            if (!onlyVol)
            {
                path = RoamingSettingsHelper.GetSetting<string>(pathKey, "");
                if (path != "")
                {
                    double position = RoamingSettingsHelper.GetSetting<double>(posKey, 0);
                    Player.PlayerState = PlayerState.Paused;
                    try
                    {
                        Messengers.Messenger.Instance.NotifyColleagues(Messengers.MessageTypes.MSG_EXECUTE_CMD, new List<object> { await StorageFile.GetFileFromPathAsync(path), position, play, volume }); 
                    }
                    catch (UnauthorizedAccessException) { }
                }
            }

            var folderPaths = RoamingSettingsHelper.GetSetting<string>(foldersKey, null);
            if (folderPaths != null)
            {
                foreach (var folder in folderPaths.Split('|'))
                {
                    if (!string.IsNullOrEmpty(folder))
                    {
                        var storageFolder = await StorageFolder.GetFolderFromPathAsync(folder);
                        SettingsVM.LibraryFoldersCollection.Add(storageFolder);
                    }
                }
            }
           
        }

        //
        //we will replace this later
        private static void LibVM_MusicLibraryLoaded()
        {
           // 
          //  ShellVM.UpcomingSong = await ShellVM.GetUpcomingSong().ConfigureAwait(false);
        }

        public static void SaveSettings()
        {
            if (Player.CurrentlyPlayingFile != null && !string.IsNullOrEmpty(Player.CurrentlyPlayingFile.Path))
            {
                ApplicationData.Current.RoamingSettings.Values[pathKey] = Player.CurrentlyPlayingFile.Path;
                ApplicationData.Current.RoamingSettings.Values[posKey] = Player.Position;
            }
            ApplicationData.Current.RoamingSettings.Values[volKey] = Player.Volume;
            ApplicationData.Current.RoamingSettings.Values[timeclosedKey] = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string folderPaths = "";
            SettingsVM.LibraryFoldersCollection.ToList().ForEach(new Action<StorageFolder>((StorageFolder folder) => { folderPaths += folder.Path + "|"; }));
            ApplicationData.Current.RoamingSettings.Values[foldersKey] = folderPaths;
        }
        #endregion

        #region SystemMediaTransportControls Methods/Events
        static MediaPlayer player;
        public static void InitSmtc()
        {           
            player = new MediaPlayer();
            player.CommandManager.IsEnabled = false;
            player.PlaybackSession.PlaybackStateChanged += PlaybackSession_PlaybackStateChanged;
            _smtc = SystemMediaTransportControls.GetForCurrentView();
            _smtc.IsEnabled = true;
            _smtc.ButtonPressed += _smtc_ButtonPressed;
           
            _smtc.IsPlayEnabled = true;
            _smtc.IsPauseEnabled = true;
            _smtc.IsStopEnabled = true;
            _smtc.IsNextEnabled = true;
            _smtc.IsPreviousEnabled = true;
            _smtc.PlaybackStatus = MediaPlaybackStatus.Closed;
            _smtc.AutoRepeatMode = MediaPlaybackAutoRepeatMode.Track;
            Player.MediaStateChanged += Player_MediaStateChanged;
        }

        private async static void PlaybackSession_PlaybackStateChanged(MediaPlaybackSession sender, object args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (sender.PlaybackState == MediaPlaybackState.Paused && isBackground == true)
                {
                    if (Player.PlayerState == PlayerState.Playing && !isforwardbackword)
                       Messengers.Messenger.Instance.NotifyColleagues(Messengers.MessageTypes.MSG_EXECUTE_CMD, "PlayPause");
                    else if (isforwardbackword)
                    {
                        isforwardbackword = false;
                    }
                }
            });
        }
        public static bool isBackground =false;
        public static void EnableDisableSmtc()
        {
            if (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1))
            {
                if (_smtc.IsEnabled == true)
                {
                    _smtc.IsEnabled = false;
                    update = true;
                }
                else if (_smtc.IsEnabled == false)
                {
                    update = false;
                    _smtc.IsEnabled = true;
                }
            }
            else
            {
                _smtc.IsEnabled = true;                
            }
        }
       static bool isPlaying = true;
        static bool update = true;
        static bool isforwardbackword = false;
        public async static void UpdateSmtc(bool play = false)
        {
            System.Diagnostics.Debug.Write(MemoryManager.AppMemoryUsage / 1024 + " | " + MemoryManager.AppMemoryUsageLimit + " | " + isBackground);
            _smtc.DisplayUpdater.Type = MediaPlaybackType.Music;
            var musicProps = _smtc.DisplayUpdater.MusicProperties;
            _smtc.DisplayUpdater.ClearAll();
            if (Player.CurrentlyPlayingFile != null)
            {
                if (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1))
                {
                    var file = await (await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets\")).GetFileAsync("5minsilence.mp3");
                    player.IsLoopingEnabled = true;
                    player.Source = MediaSource.CreateFromStorageFile(file);
                    player.CommandManager.IsEnabled = false;
                    if (isPlaying || play)
                    {
                        player.Play();
                        isPlaying = false;
                    }
                    else
                        player.Pause();
                    player.Volume = 0;

                }
                musicProps.Title = Player.CurrentlyPlayingFile.Title;
                musicProps.Artist = Player.CurrentlyPlayingFile.LeadArtist;
                musicProps.AlbumTitle = Player.CurrentlyPlayingFile.Album;
            }
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
                        Messengers.Messenger.Instance.NotifyColleagues(Messengers.MessageTypes.MSG_EXECUTE_CMD, "PlayPause");
                        break;
                    case SystemMediaTransportControlsButton.Next:
                        isforwardbackword = true;
                        Messengers.Messenger.Instance.NotifyColleagues(Messengers.MessageTypes.MSG_EXECUTE_CMD, "PlayNext");
                        break;
                    case SystemMediaTransportControlsButton.Previous:
                        isforwardbackword = true;
                        Messengers.Messenger.Instance.NotifyColleagues(Messengers.MessageTypes.MSG_EXECUTE_CMD, "PlayPrevious");
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
                        if (_smtc.IsEnabled == false)
                        {
                            if (update)
                            {
                                UpdateSmtc(true);
                                update = false;
                                player.Pause();
                            }
                        }
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
        public void DisposeObjects()
        {
            Player.Dispose();           
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
            var volume = RoamingSettingsHelper.GetSetting<double>(volKey, 50.0);
            Player.Volume = volume;
        }
        #endregion

    }
}
