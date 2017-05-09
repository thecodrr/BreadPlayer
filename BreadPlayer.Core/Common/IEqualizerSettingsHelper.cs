
using BreadPlayer.Core.PlayerEngines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Core.Common
{
    public interface IEqualizerSettingsHelper
    {
        (EqualizerSettings settings, float PreAMP) LoadEqualizerSettings(string EqConfigName);
        void SaveEqualizerSettings(EqualizerSettings settings, float PreAmp);
    }
}
