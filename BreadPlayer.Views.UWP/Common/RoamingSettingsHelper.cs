using Windows.Storage;
using BreadPlayer.Core.Common;
using BreadPlayer.Core.Models;
using Newtonsoft.Json;

namespace BreadPlayer.Common
{
    public class RoamingSettingsHelper : IEqualizerSettingsHelper
    {
        public static void SaveSetting(string key, object value)
        {
            ApplicationData.Current.RoamingSettings.Values[key] = value;
        }
        public static T GetSetting<T>(string key, object def)
        {
            object setting = ApplicationData.Current.RoamingSettings.Values[key] ?? def;
            return (T)setting;
        }

        public (EqualizerSettings settings, float PreAMP) LoadEqualizerSettings(string eqConfigName)
        {
            var eqJson = GetSetting<string>(eqConfigName, "{}");
            var settings = JsonConvert.DeserializeObject<EqualizerSettings>(eqJson);
            return (settings, GetSetting<float>("PreAMP", 0.0f));
        }

        public void SaveEqualizerSettings(EqualizerSettings settings, float preAmp)
        {
            var eqJson = JsonConvert.SerializeObject(settings);
            SaveSetting(settings.Name, eqJson);
            SaveSetting("PreAMP", preAmp);
        }
    }
}
