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

using BreadPlayer.Common;
using BreadPlayer.Controls;
using BreadPlayer.Core;
using BreadPlayer.Core.Common;
using BreadPlayer.Core.Events;
using BreadPlayer.Core.Models;
using BreadPlayer.Database;
using BreadPlayer.Dispatcher;
using BreadPlayer.Helpers;
using BreadPlayer.Interfaces;
using BreadPlayer.Messengers;
using BreadPlayer.Services;
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
using BreadPlayer.Extensions;

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
        private static string _path = "";

        #endregion Fields

        #region Load/Save Logic

        public static void LoadSettings(bool onlyVol = false, bool play = false)
        {
            var volume = SettingsHelper.GetLocalSetting<double>(VolKey, 50.0);
            if (!onlyVol)
            {
                double position = SettingsHelper.GetLocalSetting<double>(PosKey, 0.0D);
                SharedLogic.Instance.Player.PlayerState = PlayerState.Paused;
                Messenger.Instance.NotifyColleagues(MessageTypes.MsgExecuteCmd,
                        new List<object> { "", position, play, volume });
            }
        }

        public static void SaveSettings()
        {
            try
            {
                if (SharedLogic.Instance.Player.CurrentlyPlayingFile != null && !string.IsNullOrEmpty(SharedLogic.Instance.Player.CurrentlyPlayingFile.Path))
                {
                    SettingsHelper.SaveLocalSetting("NowPlayingPicture", SharedLogic.Instance.Player.CurrentlyPlayingFile.AttachedPicture);
                    SettingsHelper.SaveLocalSetting("NowPlayingID", SharedLogic.Instance.Player.CurrentlyPlayingFile.Id);
                    SettingsHelper.SaveLocalSetting(PathKey, SharedLogic.Instance.Player.CurrentlyPlayingFile.Path);
                    SettingsHelper.SaveLocalSetting(PosKey, SharedLogic.Instance.Player.Position);
                }
                SettingsHelper.SaveLocalSetting(VolKey, SharedLogic.Instance.Player.Volume);
                SettingsHelper.SaveLocalSetting(TimeclosedKey, DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                string folderPaths = "";
                SharedLogic.Instance.SettingsVm.LibraryFoldersCollection.ToList().ForEach(folder => { folderPaths += folder.Path + "|"; });
                if (!string.IsNullOrEmpty(folderPaths))
                {
                    SettingsHelper.SaveLocalSetting(FoldersKey, folderPaths.Remove(folderPaths.LastIndexOf('|')));
                }
            }
            catch (Exception ex)
            {
                BLogger.E("Error while saving settings.", ex);
            }
        }

        #endregion Load/Save Logic

        #region SystemMediaTransportControls Methods/Events

        private static MediaPlayer _player;

        public async void InitSmtc()
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

            SharedLogic.Instance.Player.MediaStateChanged += Player_MediaStateChanged;
        }
        static bool externalPaused = false;
        private async static void PlaybackSession_PlaybackStateChanged(MediaPlaybackSession sender, object args)
        {
            // BLogger.I("state has been changed (PLAYBACK SESSION).");

            await BreadDispatcher.InvokeAsync(() =>
            {
                if (sender.PlaybackState == MediaPlaybackState.Paused && SharedLogic.Instance.Player.PlayerState != PlayerState.Paused)
                {
                    // BLogger.I("state has been changed (PLAYBACK SESSION).");
                    Messenger.Instance.NotifyColleagues(MessageTypes.MsgExecuteCmd, "PlayPause");
                    _player.Pause();
                    externalPaused = true;
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
                if (SharedLogic.Instance.Player.CurrentlyPlayingFile != null)
                {
                    musicProps.Title = SharedLogic.Instance.Player.CurrentlyPlayingFile.Title.GetStringForNullOrEmptyProperty("Unknown Title");
                    musicProps.Artist = SharedLogic.Instance.Player.CurrentlyPlayingFile.LeadArtist.GetStringForNullOrEmptyProperty("Unknown Artist");
                    musicProps.AlbumTitle = SharedLogic.Instance.Player.CurrentlyPlayingFile.Album.GetStringForNullOrEmptyProperty("Unknown Album");
                    if (!string.IsNullOrEmpty(SharedLogic.Instance.Player.CurrentlyPlayingFile.AttachedPicture))
                    {
                        _smtc.DisplayUpdater.Thumbnail = RandomAccessStreamReference.CreateFromFile(await StorageFile.GetFileFromPathAsync(SharedLogic.Instance.Player.CurrentlyPlayingFile.AttachedPicture));
                    }
                }
                _smtc.DisplayUpdater.Update();
            }
            catch (Exception ex)
            {
                BLogger.E("Error occured while updating SMTC.", ex);
            }
        }

        private static async void _smtc_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            if (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1))
            {
                //we do not want to pause the background player.
                //pausing may cause stutter, that's why.
                _player?.Play();
            }
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
                    case SystemMediaTransportControlsButton.FastForward:
                        Messenger.Instance.NotifyColleagues(MessageTypes.MsgExecuteCmd, "SeekForward");
                        break;
                    case SystemMediaTransportControlsButton.Rewind:
                        Messenger.Instance.NotifyColleagues(MessageTypes.MsgExecuteCmd, "SeekBackward");
                        break;
                    default:
                        break;
                }
            });
        }

        private void Player_MediaStateChanged(object sender, MediaStateChangedEventArgs e)
        {
            if (_smtc == null)
            {
                return;
            }
            try
            {
                switch (e.NewState)
                {
                    case PlayerState.Playing:
                        if (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1))
                        {
                            _player?.Play();
                            //check if the player was paused by another app
                            if (externalPaused)
                            {
                                UpdateSmtc();
                                externalPaused = false;
                            }
                        }
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
            catch(Exception ex)
            {
                BLogger.E($"CoreWindowLogic.Player_MediaStateChanged PlayerState: {e.NewState.ToString()}.", ex);
            }
        }

        #endregion SystemMediaTransportControls Methods/Events

        #region CoreWindow Dispose Methods

        public static void DisposeObjects()
        {
            try
            {
                if (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1))
                {
                    BLogger.I("Background Player ran for: " + _player?.PlaybackSession.Position.TotalSeconds);
                    _player?.Dispose();
                }
                _smtc?.DisplayUpdater.ClearAll();
            }
            catch(Exception)
            {
                //if we are here, there is nothing to be done.
                //because this function is only called when the app is suspending.
            }
        }

        #endregion CoreWindow Dispose Methods

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
            if (!SharedLogic.Instance.SettingsVm.CoreSettingsVM.TileNotifcationsEnabled)
            {
                return;
            }
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
                BLogger.E("Error occured while updating tile.", ex);
            }
        }

        public static void LoadAppWithArguments(string arguments)
        {
            var args = arguments.Split('=', '&');

            if (!args?.Any() == null || string.IsNullOrEmpty(arguments) || args.Length < 2)
                return;
            string action = args[1];
            if (action.Contains("view"))
            {
                if (args.Length < 6)
                    return;
                string pageParameter = args[5];
                IDbRecord record = null;
                if (action.Contains("Album"))
                {
                    record = SharedLogic.Instance.AlbumArtistService.GetAlbumByIdAsync(Convert.ToInt64(pageParameter));
                }
                else if (action.Contains("Artist"))
                {
                    record = SharedLogic.Instance.AlbumArtistService.GetArtistByIdAsync(Convert.ToInt64(pageParameter));
                }
                else if (action.Contains("Playlist"))
                {
                    record = SharedLogic.Instance.PlaylistService.GetPlaylistByIdAsync(Convert.ToInt64(pageParameter));
                }
                if (record == null)
                    return;
                SplitViewMenu.UnSelectAll();
                NavigationService.Instance.Frame.Navigate(typeof(PlaylistView), record);
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
            InitializeSwitch.IsMobile = Window.Current.Bounds.Width <= 600;
        }

        private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            InitializeSwitch.IsMobile = e.Size.Width <= 600;
        }

        #endregion Ctor
    }
}