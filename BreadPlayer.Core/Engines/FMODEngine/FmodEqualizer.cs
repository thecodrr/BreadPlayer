using BreadPlayer.Fmod;
using BreadPlayer.Fmod.CoreDSP;
using BreadPlayer.Fmod.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Core.PlayerEngines
{
    public class FmodEqualizer : IEqualizer
    {
        FMODSystem FSystem { get; set; }
        Channel FChannel { get; set; }
        public FmodEqualizer(FMODSystem system, Channel channel)
        {
            IsPreampAvailable = false;
            FSystem = system;
            FChannel = channel;
            Name = "DefaultEqualizer";
            Bands = new ObservableCollection<IEqualizerBand>();
            EqualizerSettings = InitializeCore.EqualizerSettingsHelper.LoadEqualizerSettings(Name).settings;
            IsEnabled = EqualizerSettings == null || EqualizerSettings.IsEnabled;
            Init();
        }
        public override void Dispose()
        {
            this.DeInit();
            this.Bands.Clear();
            this.FSystem = null;
        }

        public override void DeInit()
        {           
            FSystem.LockDSP();

            foreach (var band in this.Bands)
            {
                band.Remove();
            }

            FSystem.UnlockDSP();
        }

        public override void Init(bool setToDefaultValues = false)
        {
            FSystem.LockDSP();
            this.Bands.Clear();

            var gainValues = !setToDefaultValues && this.EqualizerSettings != null ? this.EqualizerSettings.GainValues : null;
            foreach (var value in EqDefaultValues)
            {
                var band = GetEqualizerBand(this.IsEnabled, value[0], value[1], value[2]);
                if (band != null)
                {
                    if (gainValues != null && gainValues.TryGetValue(band.BandCaption, out float savedValue))
                    {
                        band.Gain = savedValue;
                    }
                    this.Bands.Add(band);
                }
            }
            FSystem.UnlockDSP();
        }

        public override IEqualizerBand GetEqualizerBand(bool isActive, float centerValue, float bandwithValue, float gainValue)
        {
            DSP dspParamEq = null;

            if (isActive)
            {
                var result = FSystem.CreateDSPByType(Fmod.CoreDSP.DspType.PARAMEQ, out dspParamEq);

                result = FChannel.addDSP(ChannelControlDspIndex.TAIL, dspParamEq);

                result = dspParamEq.setParameterFloat((int)DspParamEQ.CENTER, centerValue);

                result = dspParamEq.setParameterFloat((int)DspParamEQ.BANDWIDTH, bandwithValue);

                result = dspParamEq.setParameterFloat((int)DspParamEQ.GAIN, gainValue);

                result = dspParamEq.setActive(true);

            }

            var band = new FmodEqualizerBand(FChannel, dspParamEq, centerValue, gainValue, isActive);
            return band;
        }
    }
}
