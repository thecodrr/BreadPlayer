using System.Linq;
using BreadPlayer.Web.Lastfm;
using BreadPlayer.Common;
using BreadPlayer.Core.Common;
using IF.Lastfm.Core.Objects;

namespace BreadPlayer.ViewModels
{
    public class AccountsViewModel : ViewModelBase
    {
        #region Lastfm Configuration

        private RelayCommand lastfmLoginCommand;
        private string lastfmUsername;
        private string lastfmPassword;
        public string LastfmUsername
        {
            get => lastfmUsername;
            set => Set(ref lastfmUsername, value);
        }
        public string LastfmPassword
        {
            get => lastfmPassword;
            set => Set(ref lastfmPassword, value);
        }

        private string loginStatus = "(Not Logged In)";
        public string LoginStatus
        {
            get => loginStatus;
            set => Set(ref loginStatus, value);
        }
        public ICommand LastfmLoginCommand
        {
            get { if(lastfmLoginCommand == null) lastfmLoginCommand = new RelayCommand(LastfmLogin); return lastfmLoginCommand; }
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
            Core.SharedLogic.LastfmScrobbler = lastfm;

            if (lastfm.LastfmClient.Auth.Authenticated)
            {
                LoginStatus = "(Logged In)";
                SaveUserSession(lastfm.LastfmClient.Auth.UserSession);
                await NotificationManager.ShowMessageAsync("Successfully logged in!");
            }
            else
                await NotificationManager.ShowMessageAsync("Bad username/password. Please reenter.");
        }
        #endregion
        private LastUserSession GetUserSessionFromSettings()
        {
            string token = RoamingSettingsHelper.GetSetting<string>("LastfmSessionToken", "");
            bool IsSubscriber = RoamingSettingsHelper.GetSetting<bool>("LastfmIsSubscriber", false);
            string username = RoamingSettingsHelper.GetSetting<string>("LastfmSessionUsername", "");
            return new LastUserSession()
            {
                Token = token,
                IsSubscriber = IsSubscriber,
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
            LastfmPassword = RoamingSettingsHelper.GetSetting<string>("LastfmPassword", "");
            LastfmUsername = RoamingSettingsHelper.GetSetting<string>("LastfmUsername", "");
            LastfmLogin(false);
        }      
    }
}
