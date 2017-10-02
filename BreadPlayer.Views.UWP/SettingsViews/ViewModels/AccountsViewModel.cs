using BreadPlayer.Common;
using BreadPlayer.Core;
using BreadPlayer.Core.Common;
using BreadPlayer.Web.Lastfm;
using IF.Lastfm.Core.Objects;
using System.Linq;

namespace BreadPlayer.ViewModels
{
    public class AccountsViewModel : ObservableObject
    {
        #region Lastfm Configuration

        private RelayCommand _lastfmLoginCommand;
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

        private string _loginStatus = "(Not Logged In)";

        public string LoginStatus
        {
            get => _loginStatus;
            set => Set(ref _loginStatus, value);
        }

        public ICommand LastfmLoginCommand
        {
            get { if (_lastfmLoginCommand == null) { _lastfmLoginCommand = new RelayCommand(LastfmLogin); } return _lastfmLoginCommand; }
        }

        private async void LastfmLogin(object para)
        {
            if (!LastfmPassword.Any() || !LastfmUsername.Any())
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
                LoginStatus = "(Logged In)";
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
            LyricSource = SettingsHelper.GetRoamingSetting<string>("LyricSource", "Auto (recommended)");
            LyricType = SettingsHelper.GetRoamingSetting<string>("LyricType", "Synced (scrollable)");
            NoOfArtistsToFetchInfoFor = SettingsHelper.GetRoamingSetting<string>("NoOfArtistsToFetchInfoFor", "Lead artist");
            LastfmPassword = SettingsHelper.GetRoamingSetting<string>("LastfmPassword", "");
            LastfmUsername = SettingsHelper.GetRoamingSetting<string>("LastfmUsername", "");
            LastfmLogin(false);
        }
    }
}