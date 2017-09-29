using BreadPlayer.Common;

namespace BreadPlayer.SettingsViews.ViewModels
{
    public class AudioSettingsViewModel : ViewModelBase
    {
        private bool crossfadeEnabled;

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