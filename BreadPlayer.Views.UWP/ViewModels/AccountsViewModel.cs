using System.Linq;
using BreadPlayer.Web.Lastfm;
using BreadPlayer.Common;

namespace BreadPlayer.ViewModels
{
    public class AccountsViewModel : ViewModelBase
    {
        #region Lastfm Configuration
        RelayCommand lastfmLoginCommand;
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
        string loginStatus = "(Not Logged In)";
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
            if (Core.SharedLogic.LastfmScrobbler == null)
            {
                if (!LastfmPassword.Any() || !LastfmUsername.Any())
                {
                    if ((bool)para)
                    {
                        await NotificationManager.ShowMessageAsync("You need to enter username and password first!");
                    }
                    return;
                }
                InitializeLastfm lastfm = new InitializeLastfm(LastfmUsername, LastfmPassword);
                await lastfm.Login(LastfmUsername, LastfmPassword);
                BreadPlayer.Core.SharedLogic.LastfmScrobbler = new Lastfm(lastfm.Auth.Auth);
                if (lastfm.Auth.Auth.Authenticated)
                {
                    LoginStatus = "(Logged In)";
                    await NotificationManager.ShowMessageAsync("Successfully logged in!");
                }
                else
                    await NotificationManager.ShowMessageAsync("Bad username/password. Please reenter.");
            }
        }
        #endregion

        public AccountsViewModel()
        {
            LastfmPassword = RoamingSettingsHelper.GetSetting<string>("LastfmPassword", "");
            LastfmUsername = RoamingSettingsHelper.GetSetting<string>("LastfmUsername", "");
            LastfmLogin(false);
        }      
    }
}
