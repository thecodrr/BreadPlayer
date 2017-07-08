using System.Collections.Generic;
using BreadPlayer.Core.Models;
using Newtonsoft.Json;

namespace BreadPlayer.Core.Common
{
    public class ConfigSaver
    {
        //Config from VLC
        private Config _rock = new Config { Name = "Rock", Values = new float[] { 20, 12, 14, -20, -8, 1, 22, 28, 28, 28 } };

        private Config _classical = new Config
        {
            Name = "Classical",
            Values = new float[] { 0, 0, 0, 0, 0, 0, -18, -18, -18, -24 }
        };

        private Config _club = new Config { Name = "Club", Values = new float[] { 0, 0, 20, 14, 14, 14, 8, 0, 0, 0 } };

        private Config _dance = new Config
        {
            Name = "Dance",
            Values = new float[] { 24, 18, 6, 0, 0, -14, -18, -18, 0, 0 }
        };

        private Config _fullBass = new Config
        {
            Name = "FullBass",
            Values = new[] { -20, 24, 24, 14, -4, -1, -20, -25.75f, -28, -28 }
        };

        private Config _fullBassAndTreble = new Config
        {
            Name = "FullBassAndTreble",
            Values = new float[] { 18, 14, 0, -18, -12, 4, 20, 28, 30, 30 }
        };
        public static readonly float[][] EqDefaultValues = {
              new[] {32f, 1f, 0f},
              new[] {64f, 1f, 0f},
              new[] {125f, 1f, 0f},
              new[] {250f, 1f, 0f},
              new[] {500f, 1f, 0f},
              new[] {1000f, 1f, 0f},
              new[] {2000f, 1f, 0f},
              new[] {4000f, 1f, 0f},
              new[] {8000f, 1f, 0f},
              new[] {16000f, 1f, 0f }
        };
        public EqualizerSettings MakeSetting(string name, Dictionary<string, float> bands)
        {
            var equalizerSettings = new EqualizerSettings { Name = name };
            equalizerSettings.GainValues = bands;
            return equalizerSettings;
        }
        public List<EqualizerSettings> GetSettings()
        {
            List<EqualizerSettings> equalizerSettings = new List<EqualizerSettings>();
            IEnumerable<Config> listConfigs = new Config[] { _rock, _classical, _club, _dance, _fullBass, _fullBassAndTreble };
            foreach (var config in listConfigs)
            {
                Dictionary<string, float> bands = new Dictionary<string, float>();
                for (int i = 0; i < EqDefaultValues.Length; i++)
                {
                    string bandCaption = EqDefaultValues[i][0] >= 1000 ? EqDefaultValues[i][0] / 1000 + "KHz" : EqDefaultValues[i][0] + "Hz";
                    float gain = (config.Values[i] / 100) * 14;
                    bands.Add(bandCaption, gain);
                }
                equalizerSettings.Add(MakeSetting(config.Name, bands));
            }
            return equalizerSettings;
        }
        public string SaveSettings()
        {           
            string json = "";
            json += JsonConvert.SerializeObject(GetSettings());            
            return json;
        }
    }
    public class Config
    {
        public float[] Values { get; set; }
        public string Name { get; set; }
    }
}
