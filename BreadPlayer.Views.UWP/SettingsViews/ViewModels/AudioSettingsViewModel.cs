using BreadPlayer.Common;
using BreadPlayer.Core;

namespace BreadPlayer.SettingsViews.ViewModels
{
    public class AudioSettingsViewModel : ObservableObject
    {
        private bool crossfadeEnabled;

        public bool CrossfadeEnabled
        {
            get => crossfadeEnabled;
            set
            {
                Set(ref crossfadeEnabled, value);
                SharedLogic.Instance.Player.CrossfadeEnabled = crossfadeEnabled;
                SettingsHelper.SaveRoamingSetting("CrossfadeEnabled", value);
            }
        }

        public AudioSettingsViewModel()
        {
            CrossfadeEnabled = SettingsHelper.GetRoamingSetting<bool>("CrossfadeEnabled", true);
        }
    }
}