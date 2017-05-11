using System.Collections.ObjectModel;
using BreadPlayer.Core.Common;
using BreadPlayer.Core.Engines.Interfaces;
using BreadPlayer.Core.Models;
using BreadPlayer.Fmod;
using BreadPlayer.Fmod.Enums;
using BreadPlayer.Fmod.CoreDSP;

namespace BreadPlayer.Core.Engines.FMODEngine
{
    public class FmodEqualizer : Equalizer
    {
        private FmodSystem FSystem { get; set; }
        private Channel FChannel { get; set; }
        public FmodEqualizer(FmodSystem system, Channel channel)
        {
            IsPreampAvailable = false;
            FSystem = system;
            FChannel = channel;
            Name = "DefaultEqualizer";
            Bands = new ObservableCollection<IEqualizerBand>();
            Presets = new ObservableCollection<EqualizerSettings>(new ConfigSaver().GetSettings());
            EqualizerSettings = InitializeCore.EqualizerSettingsHelper.LoadEqualizerSettings(Name).settings;
            IsEnabled = EqualizerSettings == null || EqualizerSettings.IsEnabled;
            Init();
        }
        public override void Dispose()
        {
            DeInit();
            Bands.Clear();
            FSystem = null;
        }
        public void ReInit(FmodSystem system, Channel channel)
        {
            DeInit();
            FSystem = system;
            FChannel = channel;
            Init();
        }
        public override void DeInit()
        {           
            FSystem.LockDsp();

            foreach (var band in Bands)
            {
                band.Remove();
            }

            FSystem.UnlockDsp();
        }

        public override void Init(bool setToDefaultValues = false)
        {
            FSystem.LockDsp();
            Bands.Clear();

            var gainValues = !setToDefaultValues && EqualizerSettings != null ? EqualizerSettings.GainValues : null;
            foreach (var value in EqDefaultValues)
            {
                var band = GetEqualizerBand(IsEnabled, value[0], value[1], value[2]);

                if (band == null)
                {
                    continue;
                } ((FmodEqualizerBand) band).PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == "Gain")
                    {
                        SaveEqualizerSettings();
                    }
                };

                if (gainValues != null && gainValues.TryGetValue(band.BandCaption, out float savedValue))
                {
                    band.Gain = savedValue;
                }
                Bands.Add(band);
            }
            FSystem.UnlockDsp();
        }

        public override IEqualizerBand GetEqualizerBand(bool isActive, float centerValue, float bandwithValue, float gainValue)
        {
            Dsp dspParamEq = null;

            if (isActive)
            {
                FSystem.CreateDspByType(Fmod.CoreDSP.DspType.Parameq, out dspParamEq);
                
                FChannel.AddDsp(ChannelControlDspIndex.Tail, dspParamEq);

                dspParamEq.SetParameterFloat((int)DspParamEq.Center, centerValue);

                dspParamEq.SetParameterFloat((int)DspParamEq.Center, bandwithValue);

                dspParamEq.SetParameterFloat((int)DspParamEq.Gain, gainValue);

                dspParamEq.SetActive(true);
            }

            var band = new FmodEqualizerBand(FChannel, dspParamEq, centerValue, gainValue, isActive);
            return band;
        }
    }
}
