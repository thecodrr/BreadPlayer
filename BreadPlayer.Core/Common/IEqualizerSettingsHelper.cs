
using BreadPlayer.Core.Models;

namespace BreadPlayer.Core.Common
{
    public interface IEqualizerSettingsHelper
    {
        (EqualizerSettings settings, float PreAMP) LoadEqualizerSettings(string eqConfigName);
        void SaveEqualizerSettings(EqualizerSettings settings, float preAmp);
    }
}
