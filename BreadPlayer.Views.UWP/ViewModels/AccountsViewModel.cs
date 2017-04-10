using System.Linq;
using BreadPlayer.Web.Lastfm;
using BreadPlayer.Common;

namespace BreadPlayer.ViewModels
{
    public class AccountsViewModel : ViewModelBase
    {
        #region Lastfm Configuration
        DelegateCommand lastfmLoginCommand;
        string lastfmUsername;
        string lastfmPassword;
        public string LastfmUsername
        {
            get { return lastfmUsername; }
            set
            {
                Set(ref lastfmUsername, value);
                RoamingSettingsHelper.SaveSetting("LastfmUsername", value);
            }
        }
        public string LastfmPassword
        {
            get { return lastfmPassword; }
            set
            {
                Set(ref lastfmPassword, value);
                RoamingSettingsHelper.SaveSetting("LastfmPassword", value);
            }
        }
        public ICommand LastfmLoginCommand
        {
            get { if(lastfmLoginCommand == null) lastfmLoginCommand = new DelegateCommand(LastfmLogin); return lastfmLoginCommand; }
        }
        private async void LastfmLogin()
        {
            if (!LastfmPassword.Any() || !LastfmUsername.Any())
            {
                await NotificationManager.ShowMessageAsync("You need to enter username and password first!");
                return;
            }
            InitializeLastfm lastfm = new InitializeLastfm(LastfmUsername, LastfmPassword);
            await lastfm.Login(LastfmUsername, LastfmPassword);
            LastfmScrobbler = new Lastfm(lastfm.Auth.Auth);
            if (lastfm.Auth.Auth.Authenticated)
            {
                LastfmLoginCommand.IsEnabled = false;
                await NotificationManager.ShowMessageAsync("Successfully logged in!");
            }
            else
                await NotificationManager.ShowMessageAsync("Bad username/password. Please reenter.");

        }
        #endregion

        public AccountsViewModel()
        {
            LastfmPassword = RoamingSettingsHelper.GetSetting<string>("LastfmPassword", "");
            LastfmUsername = RoamingSettingsHelper.GetSetting<string>("LastfmUsername", "");
            LastfmLogin();
        }
    }
}
