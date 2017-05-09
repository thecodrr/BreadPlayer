using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Core.PlayerEngines
{
    public abstract class IEqualizer : ObservableObject
    {
        // Center: Frequency center. 20.0 to 22000.0. Default = 8000.0
        // Bandwith: Octave range around the center frequency to filter. 0.2 to 5.0. Default = 1.0
        // Gain: Frequency Gain. -30 to +30. Default = 0
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
        public EqualizerSettings EqualizerSettings { get; set; }
        public ObservableCollection<IEqualizerBand> Bands { get; set; }
        bool isEnabled;
        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set
            {
                Set(ref isEnabled, value);
                if (value)
                {
                    this.Init();
                }
                else
                {
                    this.SaveEqualizerSettings();
                    this.DeInit();
                }
            }
        }
        public bool IsPreampAvailable { get; set; }
        public string Name { get; set; }
       
        public abstract void Init(bool setToDefaultValues = false);
        public abstract void DeInit();
        public abstract void Dispose();
        public void SaveEqualizerSettings()
        {
            var equalizerSettings = this.EqualizerSettings;
            if (equalizerSettings == null || equalizerSettings.Name == null)
            {
                equalizerSettings = new EqualizerSettings { Name = this.Name };
                this.EqualizerSettings = equalizerSettings;
            }
            equalizerSettings.GainValues = this.Bands.ToDictionary(b => b.BandCaption, b => b.Gain);
            equalizerSettings.IsEnabled = this.IsEnabled;
            InitializeCore.EqualizerSettingsHelper.SaveEqualizerSettings(equalizerSettings, 1);
        }
        public void SetToDefault()
        {
            this.DeInit();
            this.Init();
        }
        public abstract IEqualizerBand GetEqualizerBand(bool isActive, float centerValue, float bandwithValue, float gainValue);
    }
}
