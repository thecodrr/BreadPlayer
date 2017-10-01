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
        private int deviceBufferSize;
        public int DeviceBufferSize
        {
            get => deviceBufferSize;
            set
            {
                Set(ref deviceBufferSize, value);
                SharedLogic.Instance.Player.DeviceBufferSize = deviceBufferSize;
                SettingsHelper.SaveRoamingSetting("DeviceBufferSize", value);
            }
        }
        public AudioSettingsViewModel()
        {
            CrossfadeEnabled = SettingsHelper.GetRoamingSetting<bool>("CrossfadeEnabled", true);
            DeviceBufferSize = SettingsHelper.GetRoamingSetting<int>("DeviceBufferSize", 350);
        }
    }
}