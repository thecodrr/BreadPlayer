
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Core.Common
{
    public interface IEqualizerSettingsHelper
    {
        (float[] EqConfig, bool IsEnabled) LoadEqualizerSettings();
        void SaveEqualizerSettings(float[] eqConfig, bool isEnabled);
    }
}
