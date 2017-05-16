using BreadPlayer.Core.Common;
using BreadPlayer.Core.Engines.Interfaces;
using BreadPlayer.Core.Models;
using ManagedBass;
using ManagedBass.Fx;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Core.Engines.BASSEngine
{
    public class BassEqualizer : Equalizer
    {
        private int _handle;
        private int _fxEq;
        private DSPProcedure _myDspAddr; // make it global, so that the GC can not remove it  
        private PeakEQParameters _eq;
        public BassEqualizer(int coreHandle)
        {
            _handle = coreHandle;
            var version = BassFx.Version;
            IsPreampAvailable = true;
            Name = "DefaultEqualizer";
            Bands = new ObservableCollection<IEqualizerBand>();
            Presets = new ObservableCollection<EqualizerSettings>(new ConfigSaver().GetSettings());
            EqualizerSettings = InitializeCore.EqualizerSettingsHelper.LoadEqualizerSettings(Name).settings;
            IsEnabled = EqualizerSettings == null || EqualizerSettings.IsEnabled;
            Init();
        }
        public void ReInit(int coreHandle)
        {
            DeInit();
            _handle = coreHandle;
            var version = BassFx.Version;
            _myDspAddr = SetPreamp;
            Bass.ChannelSetDSP(_handle, _myDspAddr, IntPtr.Zero, 0);
            //EnableDisableEqualizer();
            Init();
        }
        public override void DeInit()
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override IEqualizerBand GetEqualizerBand(bool isActive, float centerValue, float bandwithValue, float gainValue)
        {
            _eq.lBand = i;
            _eq.fCenter = centerValue;
            var res = Bass.FXSetParameters(_fxEq, _eq);
            EqualizerBand band = new EqualizerBand()
            {
                Center = _eq.fCenter,
                Gain = _eq.fGain
            };
        }

        public override void Init(bool setToDefaultValues = false)
        {
            // Set peaking equalizer effect with no bands          
            _eq = new PeakEQParameters();
            _fxEq = Bass.ChannelSetFX(_handle, EffectType.PeakEQ, 0);
            _eq.fQ = 0f;
            _eq.fBandwidth = 2.5f;
            _eq.lChannel = FXChannelFlags.All;
            
            //init equalizer bands
            Bands.Clear();

            var gainValues = !setToDefaultValues && EqualizerSettings != null ? EqualizerSettings.GainValues : null;
            foreach (var value in EqDefaultValues)
            {
                var band = GetEqualizerBand(IsEnabled, value[0], value[1], value[2]);

                if (band == null)
                {
                    continue;
                }
                ((BassEqualizerBand)band).PropertyChanged += (sender, e) =>
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
        }

        #region unsafe Methods            
        private unsafe void SetPreamp(int handle, int channel, IntPtr buffer, int length, IntPtr user)
        {
            if (_preamp == 1f || length == 0 || buffer == IntPtr.Zero)
            {
                return;
            }

            var pointer = (float*)buffer;

            var n = length / 4; // float is 4 bytes

            for (int i = 0; i < n; ++i)
            {
                pointer[i] *= _preamp;
            }
        }
        #endregion
    }
}
