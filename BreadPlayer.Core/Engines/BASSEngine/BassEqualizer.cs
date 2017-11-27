using BreadPlayer.Core.Common;
using BreadPlayer.Core.Engines.Interfaces;
using BreadPlayer.Core.Models;
using BreadPlayer.Interfaces;
using ManagedBass;
using ManagedBass.Fx;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BreadPlayer.Core.Engines.BASSEngine
{
    public class BassEqualizer : Equalizer
    {
        private int _handle;
        private DSPProcedure _myDspAddr; // make it global, so that the GC can not remove it
        private PeakEQParameters _eq;
        private int _fxEq;
        public BassEqualizer(int coreHandle)
        {
            _handle = coreHandle;
            var version = BassFx.Version;
            IsPreampAvailable = true;
            
            Bands = new ThreadSafeObservableCollection<IEqualizerBand>();
            Presets = new ThreadSafeObservableCollection<EqualizerSettings>((IEnumerable<EqualizerSettings>)new ConfigSaver().GetSettings());           
            EqualizerSettings = InitializeSwitch.EqualizerSettingsHelper.LoadEqualizerSettings("CustomEq").settings;
            Name = EqualizerSettings.Name;
            SelectedPreset = Presets.IndexOf(Presets.FirstOrDefault(t => t.Name == EqualizerSettings.Name));
            
            IsEnabled = EqualizerSettings == null || EqualizerSettings.IsEnabled;
            Init();
            this.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "SelectedPreset")
            {
                ChangePreset(SelectedPreset);
            }
        }

        public void ReInit(int coreHandle)
        {
            DeInit();
            _handle = coreHandle;
            var version = BassFx.Version;
            _myDspAddr = SetPreamp;
            Bass.ChannelSetDSP(_handle, _myDspAddr, IntPtr.Zero, 0);
            Init();
        }

        public override void DeInit()
        {
            Bass.ChannelRemoveFX(_handle, _fxEq);          
        }

        public override void Dispose()
        {
            DeInit();
            Bands.Clear();
            _handle = -1;
            _fxEq = -1;
        }

        public override IEqualizerBand GetEqualizerBand(bool isActive, float centerValue, float bandwithValue, float gainValue)
        {
            return null;
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
            Bands = new ThreadSafeObservableCollection<IEqualizerBand>();

            var gainValues = !setToDefaultValues && EqualizerSettings != null ? EqualizerSettings.GainValues : null;
            for (int i = 0; i < EqDefaultValues.Length; i++)
            {
                _eq.lBand = i;
                _eq.fCenter = EqDefaultValues[i][0];
                var res = Bass.FXSetParameters(_fxEq, _eq);
                var band = new BassEqualizerBand(_fxEq, i, EqDefaultValues[i][0], EqDefaultValues[i][1], IsEnabled);

                if (band == null)
                {
                    continue;
                }                

                if (gainValues != null && gainValues.TryGetValue(band.BandCaption, out float savedValue))
                {
                    band.Gain = savedValue;
                }
                band.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == "Gain")
                    {
                        SaveEqualizerSettings();
                    }
                };
                Bands.Add(band);
            }
        }

        #region unsafe Methods

        private unsafe void SetPreamp(int handle, int channel, IntPtr buffer, int length, IntPtr user)
        {
            if (Preamp == 1f || length == 0 || buffer == IntPtr.Zero)
            {
                return;
            }

            var pointer = (float*)buffer;

            var n = length / 4; // float is 4 bytes

            for (int i = 0; i < n; ++i)
            {
                pointer[i] *= Preamp;
            }
        }

        #endregion unsafe Methods
    }
}