using BreadPlayer.Common;
using BreadPlayer.Core;
using BreadPlayer.Helpers;

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
        private double deviceBufferSize;
        public double DeviceBufferSize
        {
            get => deviceBufferSize;
            set
            {
                Set(ref deviceBufferSize, value);
                SharedLogic.Instance.Player.DeviceBufferSize = deviceBufferSize;
                SettingsHelper.SaveLocalSetting("DeviceBufferSize", value);
            }
        }
        public AudioSettingsViewModel()
        {
            CrossfadeEnabled = SettingsHelper.GetRoamingSetting<bool>("CrossfadeEnabled", true);
            DeviceBufferSize = SettingsHelper.GetLocalSetting<double>("DeviceBufferSize", 350.00);
        }
    }
}