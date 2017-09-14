using BreadPlayer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.SettingsViews.ViewModels
{
    public class AudioSettingsViewModel : ViewModelBase
    {
        bool crossfadeEnabled;
        public bool CrossfadeEnabled
        {
            get => crossfadeEnabled;
            set
            {
                Set(ref crossfadeEnabled, value);
                Player.CrossfadeEnabled = crossfadeEnabled;
                SettingsHelper.SaveRoamingSetting("CrossfadeEnabled", value);
            }
        }

        public AudioSettingsViewModel()
        {
           // CrossfadeEnabled = SettingsHelper.GetSetting<bool>("CrossfadeEnabled", true);
        }
    }
}
