using System.Linq;
using BreadPlayer.Common;
using BreadPlayer.Core;
using BreadPlayer.Core.Common;
using BreadPlayer.Web.Lastfm;
using IF.Lastfm.Core.Objects;

namespace BreadPlayer.ViewModels
{
    public class AccountsViewModel : ViewModelBase
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
            get { if(_lastfmLoginCommand == null) { _lastfmLoginCommand = new RelayCommand(LastfmLogin); } return _lastfmLoginCommand; }
        }
        private async void LastfmLogin(object para)
        {
            if (!LastfmPassword.Any() || !LastfmUsername.Any())
            {
                if ((bool)para)
                {
                    await NotificationManager.ShowMessageAsync("You need to enter username and password first!");
                }
                return;
            }

            Lastfm lastfm = new Lastfm();
            var session = GetUserSessionFromSettings();
            if(!string.IsNullOrEmpty(session.Token) && session.Username == LastfmUsername)
            {
                lastfm.LastfmClient.Auth.LoadSession(session);
            }
            else
            {
                await lastfm.Login(LastfmUsername, LastfmPassword);
            }
            SharedLogic.LastfmScrobbler = lastfm;

            if (lastfm.LastfmClient.Auth.Authenticated)
            {
                LoginStatus = "(Logged In)";
                SaveUserSession(lastfm.LastfmClient.Auth.UserSession);
                await NotificationManager.ShowMessageAsync("Successfully logged in!");
            }
            else
            {
                await NotificationManager.ShowMessageAsync("Bad username/password. Please reenter.");
            }
        }
        #endregion

        string _noOfArtistsToFetchInfoFor;
        string _lyricType;
        string _lyricSource;
        public string NoOfArtistsToFetchInfoFor
        {
            get => _noOfArtistsToFetchInfoFor;
            set
            {
                Set(ref _noOfArtistsToFetchInfoFor, value);
                RoamingSettingsHelper.SaveSetting("NoOfArtistsToFetchInfoFor", _noOfArtistsToFetchInfoFor);
            }
        }
        public string LyricType
        {
            get => _lyricType;
            set
            {
                Set(ref _lyricType, value);
                RoamingSettingsHelper.SaveSetting("LyricType", _lyricType);
            }
        }
        public string LyricSource
        {
            get => _lyricSource;
            set
            {
                Set(ref _lyricSource, value);
                RoamingSettingsHelper.SaveSetting("LyricSource", _lyricSource);
            }
        }
        private LastUserSession GetUserSessionFromSettings()
        {
            string token = RoamingSettingsHelper.GetSetting<string>("LastfmSessionToken", "");
            bool isSubscriber = RoamingSettingsHelper.GetSetting<bool>("LastfmIsSubscriber", false);
            string username = RoamingSettingsHelper.GetSetting<string>("LastfmSessionUsername", "");
            return new LastUserSession
            {
                Token = token,
                IsSubscriber = isSubscriber,
                Username = username
            };
        }
        private void SaveUserSession(LastUserSession usersession)
        {
            RoamingSettingsHelper.SaveSetting("LastfmSessionToken", usersession.Token);
            RoamingSettingsHelper.SaveSetting("LastfmIsSubscriber", usersession.IsSubscriber);
            RoamingSettingsHelper.SaveSetting("LastfmSessionUsername", usersession.Username);
            RoamingSettingsHelper.SaveSetting("LastfmPassword", LastfmPassword);
            RoamingSettingsHelper.SaveSetting("LastfmUsername", LastfmUsername);
        }
        public AccountsViewModel()
        {
            LyricSource = RoamingSettingsHelper.GetSetting<string>("LyricSource", "Auto (recommended)");
            LyricType = RoamingSettingsHelper.GetSetting<string>("LyricType", "Synced (scrollable)");
            NoOfArtistsToFetchInfoFor = RoamingSettingsHelper.GetSetting<string>("NoOfArtistsToFetchInfoFor", "Lead artist");
            LastfmPassword = RoamingSettingsHelper.GetSetting<string>("LastfmPassword", "");
            LastfmUsername = RoamingSettingsHelper.GetSetting<string>("LastfmUsername", "");
            LastfmLogin(false);
        }      
    }
}
