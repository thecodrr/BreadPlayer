using Windows.Storage;
using BreadPlayer.Core.Common;
using BreadPlayer.Core.Models;
using Newtonsoft.Json;

namespace BreadPlayer.Common
{
    public class SettingsHelper : IEqualizerSettingsHelper
    {
        public static void SaveRoamingSetting(string key, object value)
        {
            ApplicationData.Current.RoamingSettings.Values[key] = value;
        }
        public static T GetRoamingSetting<T>(string key, object def)
        {
            object setting = ApplicationData.Current.RoamingSettings.Values[key] ?? def;
            return (T)setting;
        }
        public static void SaveLocalSetting(string key, object value)
        {
            ApplicationData.Current.LocalSettings.Values[key] = value;
        }
        public static T GetLocalSetting<T>(string key, object def)
        {
            object setting = ApplicationData.Current.LocalSettings.Values[key] ?? def;
            return (T)setting;
        }
        public (EqualizerSettings settings, float PreAMP) LoadEqualizerSettings(string eqConfigName)
        {
            var eqJson = GetRoamingSetting<string>(eqConfigName, "{}");
            var settings = JsonConvert.DeserializeObject<EqualizerSettings>(eqJson);
            return (settings, GetRoamingSetting<float>("PreAMP", 0.0f));
        }

        public void SaveEqualizerSettings(EqualizerSettings settings, float preAmp)
        {
            var eqJson = JsonConvert.SerializeObject(settings);
            SaveRoamingSetting(settings.Name, eqJson);
            SaveRoamingSetting("PreAMP", preAmp);
        }
    }
}
