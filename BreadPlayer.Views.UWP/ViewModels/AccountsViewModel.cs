using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadPlayer.Web.Lastfm;
using BreadPlayer.Common;

namespace BreadPlayer.ViewModels
{
    public class AccountsViewModel : ViewModelBase
    {
        #region Lastfm Configuration
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
        private void LastfmLogin()
        {
            InitializeLastfm lastfm = new InitializeLastfm(LastfmUsername, LastfmPassword);
            LastfmScrobbler = new Lastfm(lastfm.Auth.Auth);
        }
        #endregion

    }
}
