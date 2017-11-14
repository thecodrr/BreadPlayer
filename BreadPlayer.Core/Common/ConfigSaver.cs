using BreadPlayer.Core.Models;
using BreadPlayer.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace BreadPlayer.Core.Common
{
    public class ConfigSaver
    {
        //Config from VLC
        private Config _flat = new Config { Name = "Flat", Values = new float[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } };

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
            Name = "Full Bass",
            Values = new[] { -20, 24, 24, 14, -4, -1, -20, -25.75f, -28, -28 }
        };

        private Config _fullBassAndTreble = new Config
        {
            Name = "Full B&T",
            Values = new float[] { 18, 14, 0, -18, -12, 4, 20, 28, 30, 30 }
        };

        private Config _fulltreble = new Config { Name = "Full Treble", Values = new float[] { -24, -24, -24, -10, 6, 28, 40, 40, 40, 41.75F } };
        private Config _headphones = new Config { Name = "Headphones", Values = new float[] { 12, 28, 14, -8, -6, 4, 12, 24, 32, 36 } };
        private Config _largehall = new Config { Name = "Large Hall", Values = new float[] { 25.75F, 25.75F, 14, 14, 0, -12, -12, -12, 0, 0 } };
        private Config _live = new Config { Name = "Live", Values = new float[] { -12, 0, 10, 14, 14, 14, 10, 6, 6, 6 } };
        private Config _party = new Config { Name = "Party", Values = new float[] { 18, 18, 0, 0, 0, 0, 0, 0, 18, 18 } };
        private Config _pop = new Config { Name = "Pop", Values = new float[] { -4, 12, 18, 20, 14, 0, -6, -6, -4, -4 } };
        private Config _rock = new Config { Name = "Rock", Values = new float[] { 20, 12, -14, -20, -8, -10, 22, 28, 28, 28 } };
        private Config _custom = new Config { Name = "Custom", Values = new float[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } };
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

        public IEnumerable<IEqualizerSettings> GetSettings()
        {
            var savedPresets = InitializeSwitch.EqualizerSettingsHelper.LoadEqualizerPresets();
            if(savedPresets?.Any() == true)
            {
                return savedPresets;
            }
            List<EqualizerSettings> equalizerSettings = new List<EqualizerSettings>();
            IEnumerable<Config> listConfigs = new Config[]
            {
                _custom,
                _flat,
                _rock,
                _classical,
                _club,
                _dance,
                _fullBass,
                _fullBassAndTreble,
                _fulltreble,
                _headphones,
                _largehall,
                _live,
                _party,
                _pop
            };
            foreach (var config in listConfigs)
            {
                Dictionary<string, float> bands = new Dictionary<string, float>();
                for (int i = 0; i < EqDefaultValues.Length; i++)
                {
                    string bandCaption = EqDefaultValues[i][0] >= 1000 ? EqDefaultValues[i][0] / 1000 + "KHz" : EqDefaultValues[i][0] + "Hz";
                    float gain = (config.Values[i] / 100) * 28;
                    bands.Add(bandCaption, gain);
                }
                equalizerSettings.Add(MakeSetting(config.Name, bands));
            }
            return equalizerSettings;
        }
    }

    public class Config
    {
        public float[] Values { get; set; }
        public string Name { get; set; }
    }
}