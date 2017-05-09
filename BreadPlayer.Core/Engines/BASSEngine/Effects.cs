/* 
	BreadPlayer. A music player made for Windows 10 store.
    Copyright (C) 2016  theweavrs (Abdullah Atta)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using ManagedBass;
using ManagedBass.Fx;
using System.Collections.ObjectModel;
using System.Linq;
using BreadPlayer.Core.Models;

namespace BreadPlayer.Core
{
    public class Effects : ObservableObject
    {
        #region Fields
        int handle = 0;
        int fxEQ;
        float[] DefaultCenterFrequencyList = new float[] { 60, 170, 310, 600, 1000, 3000, 6000, 12000, 14000, 16000 };
        float[] OldEqualizerSettings = new float[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private DSPProcedure _myDSPAddr; // make it global, so that the GC can not remove it  
        #endregion

        #region Initialize
        public Effects()
        {
            EqualizerBands = new ObservableCollection<EqualizerBand>();
            InitializeEqualizer();
            LoadEqualizerSettings();
            this.PropertyChanged += Effects_PropertyChanged;
        }

      
       
        #endregion

        #region Properties
        ObservableCollection<EqualizerBand> bands;
        public ObservableCollection<EqualizerBand> EqualizerBands
        {
            get { return bands; }
            set { Set(ref bands, value); }
        }

        float _preamp = 1f;
        public float Preamp
        {
            get { return _preamp; }
            set
            {
                _preamp = value;
            }
        }
        bool enableEqualizer;
        public bool EnableEq
        {
            get => enableEqualizer;
            set => Set(ref enableEqualizer, value);
        }
        #endregion

        #region Private Methods
        private void EnableEqualizer(float[] frequencies = null)
        {
            InitializeEqualizer();
            LoadEqualizerSettings();
            SetAllEqualizerBandsFrequencies(frequencies ?? OldEqualizerSettings);
        }
        private void DisableEqualizer()
        {
            Bass.ChannelRemoveFX(handle, fxEQ);
        }
        private void InitializeEqualizer()
        {
            // Set peaking equalizer effect with no bands          
            PeakEQParameters eq = new PeakEQParameters();
            fxEQ = Bass.ChannelSetFX(handle, EffectType.PeakEQ, 0);
            eq.fQ = 0f;
            eq.fBandwidth = 2.5f;
            eq.lChannel = FXChannelFlags.All;
            //init equalizer bands
            if (EqualizerBands.Count < 10)
                InitializeEqualizerBands(eq);
        }
        private void LoadEqualizerSettings()
        {
           //// var eqConfig = InitializeCore.EqualizerSettingsHelper.LoadEqualizerSettings();
           //// OldEqualizerSettings = eqConfig.EqConfig;
           //// EnableEq = eqConfig.IsEnabled;
           // Preamp = eqConfig.PreAMP;
           // for (int i = 0; i < 10; i++)
           // {
           //     EqualizerBands[i].Gain = OldEqualizerSettings[i];
           // }
        }
        private void SaveEqualizerSettings()
        {
           // InitializeCore.EqualizerSettingsHelper.SaveEqualizerSettings(EqualizerBands.Select(t => t.Gain).ToArray(), EnableEq, Preamp);
        }
        private void SetAllEqualizerBandsFrequencies(float[] frequencies)
        {
            for (int i = 0; i < 10; i++)
            {
                UpdateEQBand(i, frequencies[i]);
            }
        }
        private void InitializeEqualizerBands(PeakEQParameters eq)
        {
            for (int i = 0; i < 10; i++)
            {
                eq.lBand = i;
                eq.fCenter = DefaultCenterFrequencyList[i];
                var res = Bass.FXSetParameters(fxEQ, eq);
                EqualizerBand band = new EqualizerBand();
                band.Center = eq.fCenter;
                band.Gain = eq.fGain;
                band.PropertyChanged += Band_PropertyChanged;
                EqualizerBands.Add(band);
            }
        }
        /// <summary>
        /// Sets Single Equalizer Band
        /// </summary>
        /// <param name="band">Band to set</param>
        /// <param name="freq">Gain to set</param>
        private void UpdateEQBand(int band, float freq)
        {
            PeakEQParameters eq = new PeakEQParameters();
            // get values of the selected band
            eq.lBand = band;
            Bass.FXGetParameters(fxEQ, eq);
            eq.fGain = freq;
            Bass.FXSetParameters(fxEQ, eq);
        }
        #endregion

        #region Public Methods
        public void UpdateHandle(int coreHandle)
        {
            handle = coreHandle;
            var version = BassFx.Version;
            _myDSPAddr = new DSPProcedure(SetPreamp);
            Bass.ChannelSetDSP(handle, _myDSPAddr, IntPtr.Zero, 0);
            EnableDisableEqualizer();
        }
        public void EnableDisableEqualizer()
        {
            SaveEqualizerSettings();
            if (EnableEq)
            {
                EnableEqualizer();
                //LoadEqualizerSettings();
            }
            else
                DisableEqualizer();
        }
        public void ResetEqualizer()
        {
            SetAllEqualizerBandsFrequencies(new float[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        }
        #endregion

        #region Events  
        private void Effects_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "EnableEq")
            {
                EnableDisableEqualizer();
            }
            SaveEqualizerSettings();
        }
        private void Band_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var band = sender as EqualizerBand;
            if (e.PropertyName == "Gain")
            {
                UpdateEQBand(EqualizerBands.IndexOf(EqualizerBands.FirstOrDefault(t => t.Center == band.Center)), band.Gain);
            }
            SaveEqualizerSettings();
        }
        #endregion

        #region unsafe Methods            
        private unsafe void SetPreamp(int handle, int channel, IntPtr buffer, int length, IntPtr user)
        {
            if (_preamp == 1f || length == 0 || buffer == IntPtr.Zero)
                return;
            var pointer = (float*)buffer;

            var n = length / 4; // float is 4 bytes

            for (int i = 0; i < n; ++i)
                pointer[i] *= _preamp;
        }
        #endregion
    }
    
}
