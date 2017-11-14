using BreadPlayer.Interfaces;
using System.Collections.Generic;

namespace BreadPlayer.Interfaces
{
    public interface IEqualizerSettingsHelper
    {
        (IEqualizerSettings settings, float PreAMP) LoadEqualizerSettings(string eqConfigName);

        void SaveEqualizerSettings(IEqualizerSettings settings, float preAmp);

        void SaveEqualizerPresets(IEnumerable<IEqualizerSettings> presets);
        IEnumerable<IEqualizerSettings> LoadEqualizerPresets();
    }
}