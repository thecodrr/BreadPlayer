using BreadPlayer.Core.Common;
using Newtonsoft.Json;
using Windows.Storage;
using BreadPlayer.Core.Models;

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

        public (EqualizerSettings settings, float PreAMP) LoadEqualizerSettings(string EqConfigName)
        {
            var eqJson = GetSetting<string>(EqConfigName, "{}");
            var settings = JsonConvert.DeserializeObject<EqualizerSettings>(eqJson);
            return (settings, GetSetting<float>("PreAMP", 0.0f));
        }

        public void SaveEqualizerSettings(EqualizerSettings settings, float PreAMP)
        {
            var eqJson = JsonConvert.SerializeObject(settings);
            SaveSetting(settings.Name, eqJson);
            SaveSetting("PreAMP", PreAMP);
        }
    }
}
