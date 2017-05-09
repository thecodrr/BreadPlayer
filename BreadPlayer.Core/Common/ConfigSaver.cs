using BreadPlayer.Core.PlayerEngines;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Core.Common
{
    public class ConfigSaver
    {
        //Config from VLC
        Config Rock = new Config()
        { Name = "Rock", Values = new float[] { 20, 12, 14, -20, -8, 1, 22, 28, 28, 28 } };

        Config Classical = new Config()
        {
            Name = "Classical",
            Values = new float[] { 0, 0, 0, 0, 0, 0, -18, -18, -18, -24 }
        };
        Config Club = new Config() { Name = "Club", Values = new float[] { 0, 0, 20, 14, 14, 14, 8, 0, 0, 0 } };
        Config Dance = new Config()
        {
            Name = "Dance",
            Values = new float[] { 24, 18, 6, 0, 0, -14, -18, -18, 0, 0 }
        };
        Config FullBass = new Config()
        {
            Name = "FullBass",
            Values = new float[] { -20, 24, 24, 14, -4, -1, -20, -25.75f, -28, -28 }
        };
        Config FullBassAndTreble = new Config()
        {
            Name = "FullBassAndTreble",
            Values = new float[] { 18, 14, 0, -18, -12, 4, 20, 28, 30, 30 },
        };
        public static readonly float[][] EqDefaultValues = new[]
           {
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
        public EqualizerSettings MakeSetting(string name, Dictionary<string, float> Bands)
        {
            var equalizerSettings = new EqualizerSettings { Name = name };
            equalizerSettings.GainValues = Bands;
            return equalizerSettings;
        }
        public List<EqualizerSettings> GetSettings()
        {
            List<EqualizerSettings> EqualizerSettings = new List<PlayerEngines.EqualizerSettings>();
            List<Config> ListConfigs = new List<Config>() { Rock, Classical, Club, Dance, FullBass };
            foreach (var config in ListConfigs)
            {
                Dictionary<string, float> Bands = new Dictionary<string, float>();
                for (int i = 0; i < EqDefaultValues.Length; i++)
                {
                    string BandCaption = EqDefaultValues[i][0] >= 1000 ? EqDefaultValues[i][0] / 1000 + "KHz" : EqDefaultValues[i][0] + "Hz";
                    float gain = (config.Values[i] / 100) * 14;
                    Bands.Add(BandCaption, gain);
                }
                EqualizerSettings.Add(MakeSetting(config.Name, Bands));
            }
            return EqualizerSettings;
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
