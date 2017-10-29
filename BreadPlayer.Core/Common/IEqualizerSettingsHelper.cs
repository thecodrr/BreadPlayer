using BreadPlayer.Core.Models;
using System.Collections.Generic;

namespace BreadPlayer.Core.Common
{
    public interface IEqualizerSettingsHelper
    {
        (EqualizerSettings settings, float PreAMP) LoadEqualizerSettings(string eqConfigName);

        void SaveEqualizerSettings(EqualizerSettings settings, float preAmp);

        void SaveEqualizerPresets(IEnumerable<EqualizerSettings> presets);
        IEnumerable<EqualizerSettings> LoadEqualizerPresets();
    }
}