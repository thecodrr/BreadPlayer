using BreadPlayer.Common;
using BreadPlayer.Core;
using BreadPlayer.Core.Common;
using BreadPlayer.Helpers;
using BreadPlayer.Interfaces;
using BreadPlayer.Web.Lastfm;
using IF.Lastfm.Core.Objects;
using System.Linq;

namespace BreadPlayer.ViewModels
{
    public class AccountsViewModel : ObservableObject
    {
        #region Lastfm Configuration

        private RelayCommand _lastfmLoginCommand;
        private RelayCommand _lastfmLogoutCommand;
        private string _lastfmUsername;
        private string _lastfmPassword;

        public string LastfmUsername
        {
            get => _lastfmUsername;
            set => Set(ref _lastfmUsername, value);
        }

        public string LastfmPassword
        {
            get => _lastfmPassword;
            set => Set(ref _lastfmPassword, value);
        }        

        public ICommand LastfmLoginCommand
        {
            get { if (_lastfmLoginCommand == null) { _lastfmLoginCommand = new RelayCommand(LastfmLogin); } return _lastfmLoginCommand; }
        }
        public ICommand LastfmLogoutCommand
        {
            get { if (_lastfmLogoutCommand == null) { _lastfmLogoutCommand = new RelayCommand(LastfmLogout); } return _lastfmLogoutCommand; }
        }
        private async void LastfmLogout(object para)
        {
            LastfmPassword = "";
            LastfmUsername = "";
            ClearUserSessionFromSettings();
            await SharedLogic.Instance.NotificationManager.ShowMessageAsync("Successfully logged out!");
        }
        private async void LastfmLogin(object para)
        {
            if (string.IsNullOrEmpty(LastfmPassword) || string.IsNullOrEmpty(LastfmUsername))
            {
                if ((bool)para)
                {
                    await SharedLogic.Instance.NotificationManager.ShowMessageAsync("You need to enter username and password first!");
                }
                return;
            }

            Lastfm lastfm = new Lastfm();
            var session = GetUserSessionFromSettings();
            if (!string.IsNullOrEmpty(session.Token) && session.Username == LastfmUsername)
            {
                lastfm.LastfmClient.Auth.LoadSession(session);
            }
            else
            {
                await lastfm.Login(LastfmUsername, LastfmPassword);
            }
            SharedLogic.Instance.LastfmScrobbler = lastfm;

            if (lastfm.LastfmClient.Auth.Authenticated)
            {
                SaveUserSession(lastfm.LastfmClient.Auth.UserSession);
                await SharedLogic.Instance.NotificationManager.ShowMessageAsync("Successfully logged in!");
            }
            else
            {
                await SharedLogic.Instance.NotificationManager.ShowMessageAsync("Bad username/password. Please reenter.");
            }
        }

        #endregion Lastfm Configuration

        private string _noOfArtistsToFetchInfoFor;
        private string _lyricType;
        private string _lyricSource;

        public string NoOfArtistsToFetchInfoFor
        {
            get => _noOfArtistsToFetchInfoFor;
            set
            {
                Set(ref _noOfArtistsToFetchInfoFor, value);
                SettingsHelper.SaveRoamingSetting("NoOfArtistsToFetchInfoFor", _noOfArtistsToFetchInfoFor);
            }
        }

        public string LyricType
        {
            get => _lyricType;
            set
            {
                Set(ref _lyricType, value);
                SettingsHelper.SaveRoamingSetting("LyricType", _lyricType);
            }
        }

        public string LyricSource
        {
            get => _lyricSource;
            set
            {
                Set(ref _lyricSource, value);
                SettingsHelper.SaveRoamingSetting("LyricSource", _lyricSource);
            }
        }
        private void ClearUserSessionFromSettings()
        {
            SettingsHelper.SaveRoamingSetting("LastfmSessionToken", null);
            SettingsHelper.SaveRoamingSetting("LastfmIsSubscriber", null);
            SettingsHelper.SaveRoamingSetting("LastfmSessionUsername", null);
            SettingsHelper.SaveRoamingSetting("LastfmPassword", null);
            SettingsHelper.SaveRoamingSetting("LastfmUsername", null);
        }
        private LastUserSession GetUserSessionFromSettings()
        {
            string token = SettingsHelper.GetRoamingSetting<string>("LastfmSessionToken", "");
            bool isSubscriber = SettingsHelper.GetRoamingSetting<bool>("LastfmIsSubscriber", false);
            string username = SettingsHelper.GetRoamingSetting<string>("LastfmSessionUsername", "");
            return new LastUserSession
            {
                Token = token,
                IsSubscriber = isSubscriber,
                Username = username
            };
        }

        private void SaveUserSession(LastUserSession usersession)
        {
            SettingsHelper.SaveRoamingSetting("LastfmSessionToken", usersession.Token);
            SettingsHelper.SaveRoamingSetting("LastfmIsSubscriber", usersession.IsSubscriber);
            SettingsHelper.SaveRoamingSetting("LastfmSessionUsername", usersession.Username);
            SettingsHelper.SaveRoamingSetting("LastfmPassword", LastfmPassword);
            SettingsHelper.SaveRoamingSetting("LastfmUsername", LastfmUsername);
        }

        public AccountsViewModel()
        {
            _lyricSource = SettingsHelper.GetRoamingSetting<string>("LyricSource", "Auto");
            _lyricType = SettingsHelper.GetRoamingSetting<string>("LyricType", "Synced");
            _noOfArtistsToFetchInfoFor = SettingsHelper.GetRoamingSetting<string>("NoOfArtistsToFetchInfoFor", "Lead artist");
            _lastfmPassword = SettingsHelper.GetRoamingSetting<string>("LastfmPassword", null);
            _lastfmUsername = SettingsHelper.GetRoamingSetting<string>("LastfmUsername", null);
            LastfmLogin(false);
        }
    }
}