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
using BreadPlayer.Common;
using Windows.Data.Xml.Dom;
using BreadPlayer.Models;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using System.IO;
using BreadPlayer.ViewModels;

namespace BreadPlayer
{
    public class CoreWindowLogic
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
                string folders = RoamingSettingsHelper.GetSetting<string>(foldersKey, "");
                folders.Split('|').ToList().ForEach(async (str) =>
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        var folder = await StorageFolder.GetFolderFromPathAsync(str);
                        SharedLogic.SettingsVM.LibraryFoldersCollection.Add(folder);
                    }
                });
                // SettingsVM.LibraryFoldersCollection.ToList().ForEach(new Action<StorageFolder>((StorageFolder folder) => { folderPaths += folder.Path + "|"; }));
                if (path != "" && SharedLogic.VerifyFileExists(path, 300))
                {
                    double position = RoamingSettingsHelper.GetSetting<double>(posKey, 0);
                    SharedLogic.Player.PlayerState = PlayerState.Paused;
                    try
                    {
                        Messengers.Messenger.Instance.NotifyColleagues(Messengers.MessageTypes.MSG_EXECUTE_CMD,
                            new List<object> { await StorageFile.GetFileFromPathAsync(path), position, play, volume });
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        BLogger.Logger.Error("Access denied while trying to play file on startup.", ex);
                    }
                }
            }
        }

        public static void SaveSettings()
        {
            if (SharedLogic.Player.CurrentlyPlayingFile != null && !string.IsNullOrEmpty(SharedLogic.Player.CurrentlyPlayingFile.Path))
            {
                ApplicationData.Current.RoamingSettings.Values[pathKey] = SharedLogic.Player.CurrentlyPlayingFile.Path;
                ApplicationData.Current.RoamingSettings.Values[posKey] = SharedLogic.Player.Position;
            }
            ApplicationData.Current.RoamingSettings.Values[volKey] = SharedLogic.Player.Volume;
            ApplicationData.Current.RoamingSettings.Values[timeclosedKey] = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string folderPaths = "";
            SharedLogic.SettingsVM.LibraryFoldersCollection.ToList().ForEach(new Action<StorageFolder>((StorageFolder folder) => { folderPaths += folder.Path + "|"; }));
            if(!string.IsNullOrEmpty(folderPaths))
                ApplicationData.Current.RoamingSettings.Values[foldersKey] = folderPaths.Remove(folderPaths.LastIndexOf('|'));
        }
        #endregion

        #region SystemMediaTransportControls Methods/Events
        static MediaPlayer player;
        public async static void InitSmtc()
        {
            if (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1))
            {
                var file = await(await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets\")).GetFileAsync("5minsilence.mp3");
                player = new MediaPlayer();
                player.PlaybackSession.PlaybackStateChanged += PlaybackSession_PlaybackStateChanged;
                player.CommandManager.IsEnabled = false;
                player.IsLoopingEnabled = true;
                player.Source = MediaSource.CreateFromStorageFile(file);
                player.Play();
            }
            _smtc = SystemMediaTransportControls.GetForCurrentView();
            _smtc.ButtonPressed += _smtc_ButtonPressed;
            _smtc.IsEnabled = true;
            _smtc.IsPlayEnabled = true;
            _smtc.IsPauseEnabled = true;
            _smtc.IsStopEnabled = true;
            _smtc.IsNextEnabled = true;
            _smtc.IsPreviousEnabled = true;
            _smtc.PlaybackStatus = MediaPlaybackStatus.Closed;
            _smtc.AutoRepeatMode = MediaPlaybackAutoRepeatMode.Track;
            SharedLogic.Player.MediaStateChanged += Player_MediaStateChanged;
        }

        private async static void PlaybackSession_PlaybackStateChanged(MediaPlaybackSession sender, object args)
        {
            await SharedLogic.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (sender.PlaybackState == MediaPlaybackState.Paused)
                {
                    Messengers.Messenger.Instance.NotifyColleagues(Messengers.MessageTypes.MSG_EXECUTE_CMD, "PlayPause");
                }
            });
        }

        public async static void UpdateSmtc()
        {
            if (_smtc == null) return;

            _smtc.DisplayUpdater.Type = MediaPlaybackType.Music;
            var musicProps = _smtc.DisplayUpdater.MusicProperties;
            _smtc.DisplayUpdater.ClearAll();
            if (SharedLogic.Player.CurrentlyPlayingFile != null)
            {              
                musicProps.Title = SharedLogic.Player.CurrentlyPlayingFile.Title;
                musicProps.Artist = SharedLogic.Player.CurrentlyPlayingFile.LeadArtist;
                musicProps.AlbumTitle = SharedLogic.Player.CurrentlyPlayingFile.Album;
                if (!string.IsNullOrEmpty(SharedLogic.Player.CurrentlyPlayingFile.AttachedPicture))
                    _smtc.DisplayUpdater.Thumbnail = RandomAccessStreamReference.CreateFromFile(await StorageFile.GetFileFromPathAsync(SharedLogic.Player.CurrentlyPlayingFile.AttachedPicture));
            }
            _smtc.DisplayUpdater.Update();
        }
        private static async void _smtc_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            //we do not want to pause the background player.
            //pausing may cause stutter, that's why.
            player.Play();
            await SharedLogic.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                switch (args.Button)
                {
                    case SystemMediaTransportControlsButton.Play:
                    case SystemMediaTransportControlsButton.Pause:
                        Messengers.Messenger.Instance.NotifyColleagues(Messengers.MessageTypes.MSG_EXECUTE_CMD, "PlayPause");
                        break;
                    case SystemMediaTransportControlsButton.Next:
                        Messengers.Messenger.Instance.NotifyColleagues(Messengers.MessageTypes.MSG_EXECUTE_CMD, "PlayNext");
                        break;
                    case SystemMediaTransportControlsButton.Previous:
                        Messengers.Messenger.Instance.NotifyColleagues(Messengers.MessageTypes.MSG_EXECUTE_CMD, "PlayPrevious");
                        break;
                    default:
                        break;
                }
            });
        }
        private static void Player_MediaStateChanged(object sender, Events.MediaStateChangedEventArgs e)
        {
            if (_smtc == null)
                return;
           
            switch (e.NewState)
            {
                case PlayerState.Playing:
                    player?.Play();
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
            SharedLogic.Player.Dispose();
            BLogger.Logger.Info("Background Player ran for: " + player?.PlaybackSession.Position.TotalSeconds);
            BLogger.Logger.Info("Application is being suspended, disposing everything.");
            player?.Dispose();
            Messengers.Messenger.Instance.NotifyColleagues(Messengers.MessageTypes.MSG_DISPOSE);
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

            //var toastImageElements = toastXml.GetElementsByTagName("image");
            var toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(msg));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode(subMsg));

            var toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
        public static void UpdateTile(Mediafile mediaFile)
        {
            try
            {
                string title = System.Net.WebUtility.HtmlEncode(mediaFile.Title);
                string artist = System.Net.WebUtility.HtmlEncode(mediaFile.LeadArtist);
                string album = System.Net.WebUtility.HtmlEncode(mediaFile.Album);
                string albumart = string.IsNullOrEmpty(mediaFile.AttachedPicture) ? "Assets/Square44x44Logo.scale-400.png" : mediaFile.AttachedPicture;
                string xml = "<tile> <visual displayName=\"Now Playing\" branding=\"nameAndLogo\">" +
                    "<binding template=\"TileSmall\"> <image placement=\"background\" src=\"" + albumart + "\"/> </binding>" +
                    "<binding template=\"TileMedium\"> <image placement=\"background\" src=\"" + mediaFile.AttachedPicture + "\" hint-overlay=\"50\"/> <text hint-style=\"body\" hint-wrap=\"true\">{0}</text> <text hint-style=\"caption\">{1}</text> <text hint-style=\"captionSubtle\">{2}</text> </binding>" +
                    "<binding template=\"TileWide\" hint-textStacking=\"center\"> <image placement=\"background\" src=\"" + mediaFile.AttachedPicture + "\" hint-overlay=\"70\"/> <text hint-style=\"subtitle\" hint-align=\"center\">{0}</text> <text hint-style=\"body\" hint-align=\"center\">{1}</text> <text hint-style=\"caption\" hint-align=\"center\">{2}</text></binding>" +
                    "<binding template=\"TileLarge\"> <image placement=\"background\" src=\"" + mediaFile.AttachedPicture + "\" hint-overlay=\"80\"/> <group> <subgroup hint-weight=\"1\"/> <subgroup hint-weight=\"2\"> <image src=\"" + mediaFile.AttachedPicture + "\" hint-crop=\"circle\"/> </subgroup> <subgroup hint-weight=\"1\"/> </group> <text hint-style=\"subtitle\" hint-align=\"center\">{0}</text> <text hint-style=\"body\" hint-align=\"center\">{1}</text> <text hint-style=\"caption\" hint-align=\"center\">{2}</text> </binding> </visual> </tile>";
                var formattedXML = string.Format(xml, title, artist, album);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(formattedXML);
                var notification = new TileNotification(doc);
                TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
            }
            catch (Exception ex)
            {
                BLogger.Logger.Error("Error occured while updating tile.", ex);
            }
        }

        #region Ctor
        public CoreWindowLogic()
        {
            if (StorageApplicationPermissions.FutureAccessList.Entries.Count >= 999)
                StorageApplicationPermissions.FutureAccessList.Clear();
            InitSmtc();
            SharedLogic.Player.Volume = RoamingSettingsHelper.GetSetting<double>(volKey, 50.0);
            Window.Current.SizeChanged += Current_SizeChanged;
            var vm = (App.Current.Resources["AccountsVM"] as AccountsViewModel);
        }

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            InitializeCore.IsMobile = e.Size.Width <= 600;
        }
        #endregion

    }
}
