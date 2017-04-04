
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Core.Common
{
    public interface IEqualizerSettingsHelper
    {
        (float[] EqConfig, bool IsEnabled, float PreAMP) LoadEqualizerSettings();
        void SaveEqualizerSettings(float[] eqConfig, bool isEnabled, float PreAmp);
    }
}
