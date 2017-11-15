using System.Collections.Generic;
using BreadPlayer.Interfaces;
using BreadPlayer.Core.Models;
using Newtonsoft.Json;
using Windows.Storage;

namespace BreadPlayer.Helpers
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
        public (IEqualizerSettings settings, float PreAMP) LoadEqualizerSettings(string eqConfigName)
        {
            var eqJson = GetRoamingSetting<string>(eqConfigName, "{}");
            var settings = JsonConvert.DeserializeObject<EqualizerSettings>(eqJson);
            return (settings, GetRoamingSetting<float>("PreAMP", 1.0f));
        }

        public void SaveEqualizerSettings(IEqualizerSettings settings, float preAmp)
        {
            var eqJson = JsonConvert.SerializeObject(settings);
            SaveRoamingSetting("CustomEq", eqJson);
            SaveRoamingSetting("PreAMP", preAmp);
        }

        public void SaveEqualizerPresets(IEnumerable<IEqualizerSettings> presets)
        {
            var presetJson = JsonConvert.SerializeObject(presets);
            SaveLocalSetting("Presets", presetJson);
        }

        public IEnumerable<IEqualizerSettings> LoadEqualizerPresets()
        {
            var presets = JsonConvert.DeserializeObject<IEnumerable<EqualizerSettings>>(GetLocalSetting<string>("Presets", "[]"));
            return presets;
        }
    }
}