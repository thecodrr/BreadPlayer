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
using System.Net;
using Windows.ApplicationModel;
using Windows.Data.Xml.Dom;
using Windows.Foundation.Metadata;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using BreadPlayer.Common;
using BreadPlayer.Core;
using BreadPlayer.Core.Common;
using BreadPlayer.Core.Events;
using BreadPlayer.Core.Models;
using BreadPlayer.Messengers;
using BreadPlayer.ViewModels;
using BreadPlayer.Dispatcher;

namespace BreadPlayer
{
    public class CoreWindowLogic
    {
        #region Fields
        private const string PathKey = "path";
        private const string PosKey = "position";
        private const string VolKey = "volume";
        private const string FoldersKey = "folders";
        private const string TimeclosedKey = "timeclosed";
        private static SystemMediaTransportControls _smtc;
#pragma warning disable CS0414 // The field 'CoreWindowLogic._path' is assigned but its value is never used
        private static string _path = "";
#pragma warning restore CS0414 // The field 'CoreWindowLogic._path' is assigned but its value is never used
        #endregion

        #region Load/Save Logic
        public static void LoadSettings(bool onlyVol = false, bool play = false)
        {
            var volume = SettingsHelper.GetLocalSetting<double>(VolKey, 50.0);
            if (!onlyVol)
            {
                    double position = SettingsHelper.GetLocalSetting<double>(PosKey, 0.0D);
                    SharedLogic.Player.PlayerState = PlayerState.Paused;
                    Messenger.Instance.NotifyColleagues(MessageTypes.MsgExecuteCmd,
                            new List<object> { "", position, play, volume });                
            }
        }

        public static void SaveSettings()
        {
            try
            {
                if (SharedLogic.Player.CurrentlyPlayingFile != null && !string.IsNullOrEmpty(SharedLogic.Player.CurrentlyPlayingFile.Path))
                {
                    SettingsHelper.SaveLocalSetting("NowPlayingID", SharedLogic.Player.CurrentlyPlayingFile.Id);
                    SettingsHelper.SaveLocalSetting(PathKey,SharedLogic.Player.CurrentlyPlayingFile.Path);
                    SettingsHelper.SaveLocalSetting(PosKey, SharedLogic.Player.Position);
                }
                SettingsHelper.SaveLocalSetting(VolKey, SharedLogic.Player.Volume);
                SettingsHelper.SaveLocalSetting(TimeclosedKey, DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                string folderPaths = "";
                SharedLogic.SettingsVm.LibraryFoldersCollection.ToList().ForEach(folder => { folderPaths += folder.Path + "|"; });
                if (!string.IsNullOrEmpty(folderPaths))
                {
                    SettingsHelper.SaveLocalSetting(FoldersKey, folderPaths.Remove(folderPaths.LastIndexOf('|')));
                }
            }
            catch(Exception ex)
            {
                BLogger.Logger.Error("Error while saving settings.", ex);
            }
        }
        #endregion

        #region SystemMediaTransportControls Methods/Events

        private static MediaPlayer _player;
        public async static void InitSmtc()
        {
            if (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1))
            {
                var file = await (await Package.Current.InstalledLocation.GetFolderAsync(@"Assets\")).GetFileAsync("5minsilence.mp3");
                _player = new MediaPlayer();
                _player.PlaybackSession.PlaybackStateChanged += PlaybackSession_PlaybackStateChanged;
                _player.CommandManager.IsEnabled = false;
                _player.IsLoopingEnabled = true;
                _player.Source = MediaSource.CreateFromStorageFile(file);
                _player.Play();
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
           // BLogger.Logger?.Info("state has been changed (PLAYBACK SESSION).");

            await BreadDispatcher.InvokeAsync(() =>
            {
                if (sender.PlaybackState == MediaPlaybackState.Paused && SharedLogic.Player.PlayerState != PlayerState.Paused)
                {
                   // BLogger.Logger?.Info("state has been changed (PLAYBACK SESSION).");
                    Messenger.Instance.NotifyColleagues(MessageTypes.MsgExecuteCmd, "PlayPause");
                }
            });
        }

        public async static void UpdateSmtc()
        {
            if (_smtc == null)
            {
                return;
            }
            try
            {
                _smtc.DisplayUpdater.Type = MediaPlaybackType.Music;
                var musicProps = _smtc.DisplayUpdater.MusicProperties;
                _smtc.DisplayUpdater.ClearAll();
                if (SharedLogic.Player.CurrentlyPlayingFile != null)
                {
                    musicProps.Title = SharedLogic.Player.CurrentlyPlayingFile.Title;
                    musicProps.Artist = SharedLogic.Player.CurrentlyPlayingFile.LeadArtist;
                    musicProps.AlbumTitle = SharedLogic.Player.CurrentlyPlayingFile.Album;
                    if (!string.IsNullOrEmpty(SharedLogic.Player.CurrentlyPlayingFile.AttachedPicture))
                    {
                        _smtc.DisplayUpdater.Thumbnail = RandomAccessStreamReference.CreateFromFile(await StorageFile.GetFileFromPathAsync(SharedLogic.Player.CurrentlyPlayingFile.AttachedPicture));
                    }
                }
                _smtc.DisplayUpdater.Update();
            }
            catch(Exception ex)
            {
                BLogger.Logger.Error("Error occured while updating SMTC.", ex);
            }
        }
        private static async void _smtc_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            //we do not want to pause the background player.
            //pausing may cause stutter, that's why.
            _player?.Play();
            await BreadDispatcher.InvokeAsync(() =>
            {
                switch (args.Button)
                {
                    case SystemMediaTransportControlsButton.Play:
                    case SystemMediaTransportControlsButton.Pause:
                        Messenger.Instance.NotifyColleagues(MessageTypes.MsgExecuteCmd, "PlayPause");
                        break;
                    case SystemMediaTransportControlsButton.Next:
                        Messenger.Instance.NotifyColleagues(MessageTypes.MsgExecuteCmd, "PlayNext");
                        break;
                    case SystemMediaTransportControlsButton.Previous:
                        Messenger.Instance.NotifyColleagues(MessageTypes.MsgExecuteCmd, "PlayPrevious");
                        break;
                    default:
                        break;
                }
            });
        }
        private static void Player_MediaStateChanged(object sender, MediaStateChangedEventArgs e)
        {
            if (_smtc == null)
            {
                return;
            }

            switch (e.NewState)
            {
                case PlayerState.Playing:
                    _player?.Play();
                    _smtc.PlaybackStatus = MediaPlaybackStatus.Playing;
                    break;
                case PlayerState.Paused:
                   // BLogger.Logger?.Info("state has been changed to paused.");
                    _smtc.PlaybackStatus = MediaPlaybackStatus.Paused;
                    break;
                case PlayerState.Stopped:
                   // BLogger.Logger?.Info("state has been changed to stopped.");
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
            //SharedLogic.SettingsVm.TimeClosed = DateTime.Now.ToString();
            SharedLogic.Player.Dispose();
            BLogger.Logger.Info("Background Player ran for: " + _player?.PlaybackSession.Position.TotalSeconds);
            BLogger.Logger.Info("Application is being suspended, disposing everything.");
            _player?.Dispose();
            Messenger.Instance.NotifyColleagues(MessageTypes.MsgDispose);
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
                string title = WebUtility.HtmlEncode(mediaFile.Title);
                string artist = WebUtility.HtmlEncode(mediaFile.LeadArtist);
                string album = WebUtility.HtmlEncode(mediaFile.Album);
                string albumart = string.IsNullOrEmpty(mediaFile.AttachedPicture) ? "Assets/Square44x44Logo.scale-400.png" : mediaFile.AttachedPicture;
                string xml = "<tile> <visual displayName=\"Now Playing\" branding=\"nameAndLogo\">" +
                    "<binding template=\"TileSmall\"> <image placement=\"background\" src=\"" + albumart + "\"/> </binding>" +
                    "<binding template=\"TileMedium\"> <image placement=\"background\" src=\"" + mediaFile.AttachedPicture + "\" hint-overlay=\"50\"/> <text hint-style=\"body\" hint-wrap=\"true\">{0}</text> <text hint-style=\"caption\">{1}</text> <text hint-style=\"captionSubtle\">{2}</text> </binding>" +
                    "<binding template=\"TileWide\" hint-textStacking=\"center\"> <image placement=\"background\" src=\"" + mediaFile.AttachedPicture + "\" hint-overlay=\"70\"/> <text hint-style=\"subtitle\" hint-align=\"center\">{0}</text> <text hint-style=\"body\" hint-align=\"center\">{1}</text> <text hint-style=\"caption\" hint-align=\"center\">{2}</text></binding>" +
                    "<binding template=\"TileLarge\"> <image placement=\"background\" src=\"" + mediaFile.AttachedPicture + "\" hint-overlay=\"80\"/> <group> <subgroup hint-weight=\"1\"/> <subgroup hint-weight=\"2\"> <image src=\"" + mediaFile.AttachedPicture + "\" hint-crop=\"circle\"/> </subgroup> <subgroup hint-weight=\"1\"/> </group> <text hint-style=\"subtitle\" hint-align=\"center\">{0}</text> <text hint-style=\"body\" hint-align=\"center\">{1}</text> <text hint-style=\"caption\" hint-align=\"center\">{2}</text> </binding> </visual> </tile>";
                var formattedXml = string.Format(xml, title, artist, album);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(formattedXml);
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
            {
                StorageApplicationPermissions.FutureAccessList.Clear();
            }

            InitSmtc();
            Window.Current.SizeChanged += Current_SizeChanged;
        }

        private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            InitializeCore.IsMobile = e.Size.Width <= 600;
        }
        #endregion

    }
}
