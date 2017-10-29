using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadPlayer.Helpers;
using BreadPlayer.Core.Common;
using BreadPlayer.Database;
using BreadPlayer.Core;
using Windows.Storage;
using BreadPlayer.Messengers;
using Windows.System.Display;
using System.ComponentModel;

namespace BreadPlayer.SettingsViews.ViewModels
{
    public class CoreSettingsViewModel : ObservableObject
    {
        private DelegateCommand _resetCommand;
        public DelegateCommand ResetCommand { get { if (_resetCommand == null) { _resetCommand = new DelegateCommand(Reset); } return _resetCommand; } }
        private async void Reset()
        {
            try
            {
                Messenger.Instance.NotifyColleagues(MessageTypes.MsgDispose);
                await ApplicationData.Current.ClearAsync();
                ResetCommand.IsEnabled = false;
                await Task.Delay(200);
                ResetCommand.IsEnabled = true;
                var libraryService = new LibraryService(new KeyValueStoreDatabaseService(SharedLogic.Instance.DatabasePath, "Tracks"));
                libraryService = null;
            }
            catch (Exception ex)
            {
                BLogger.E("Error occured while resetting the player.", ex);
            }
        }

        private bool _preventScreenFromLocking;

        public bool PreventScreenFromLocking
        {
            get => _preventScreenFromLocking;
            set
            {
                Set(ref _preventScreenFromLocking, value);
                if (value)
                {
                    KeepScreenActive();
                }
                else
                {
                    ReleaseDisplayRequest();
                }
            }
        }

        private bool _replaceLockscreenWithAlbumArt;

        public bool ReplaceLockscreenWithAlbumArt
        {
            get => _replaceLockscreenWithAlbumArt;
            set
            {
                Set(ref _replaceLockscreenWithAlbumArt, value);
                SettingsHelper.SaveLocalSetting("ReplaceLockscreenWithAlbumArt", value);
            }
        }

        private bool _upcomingSongNotifcationsEnabled;

        public bool UpcomingSongNotifcationsEnabled
        {
            get => _upcomingSongNotifcationsEnabled;
            set
            {
                Set(ref _upcomingSongNotifcationsEnabled, value);
                SettingsHelper.SaveLocalSetting("UpcomingSongNotifcationsEnabled", value);
            }
        }
        private bool _tileNotifcationsEnabled;

        public bool TileNotifcationsEnabled
        {
            get => _tileNotifcationsEnabled;
            set
            {
                Set(ref _tileNotifcationsEnabled, value);
                SettingsHelper.SaveLocalSetting("TileNotifcationsEnabled", value);
            }
        }
        public CoreSettingsViewModel()
        {
            _tileNotifcationsEnabled = SettingsHelper.GetLocalSetting<bool>("TileNotifcationsEnabled", true);
            _upcomingSongNotifcationsEnabled = SettingsHelper.GetLocalSetting<bool>("UpcomingSongNotifcationsEnabled", true);
            _replaceLockscreenWithAlbumArt = SettingsHelper.GetLocalSetting<bool>("ReplaceLockscreenWithAlbumArt", false);
            this.PropertyChanged += OnPropertyChanged;
        }

        #region General Settings Methods

        private DisplayRequest _displayRequest;

        private void KeepScreenActive()
        {
            if (_displayRequest == null)
            {
                _displayRequest = new DisplayRequest();
                // This call activates a display-required request. If successful,
                // the screen is guaranteed not to turn off automatically due to user inactivity.
                _displayRequest.RequestActive();
            }
        }

        private void ReleaseDisplayRequest()
        {
            // This call de-activates the display-required request. If successful, the screen
            // might be turned off automatically due to a user inactivity, depending on the
            // power policy settings of the system. The requestRelease method throws an exception
            // if it is called before a successful requestActive call on this object.
            if (_displayRequest != null)
            {
                _displayRequest.RequestRelease();
                _displayRequest = null;
            }
        }

        #endregion General Settings Methods

        private async void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ReplaceLockscreenWithAlbumArt")
            {
                if (ReplaceLockscreenWithAlbumArt)
                {
                    _replaceLockscreenWithAlbumArt = await LockscreenHelper.SaveCurrentLockscreenImage();
                }
                else
                {
                    await LockscreenHelper.ResetLockscreenImage();
                }
            }
        }
    }
}
