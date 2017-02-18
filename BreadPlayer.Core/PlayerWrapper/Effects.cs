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

namespace BreadPlayer.Core
{
    public class Effects
    {
        #region Fields
        int handle = 0;
        int fxEQ;
        float[] DefaultCenterFrequencyList = new float[] { 60, 170, 310, 600, 1000, 3000, 6000, 12000, 14000, 16000 };
        #endregion

        #region Initialize
        public Effects(int coreHandle)
        {
            handle = coreHandle;
            bool loadbassfx = BassFx.Load();
            _myDSPAddr = new DSPProcedure(SetPreamp);
        }
        #endregion
        public void DisableEqualizer()
        {
            Bass.ChannelRemoveFX(handle, fxEQ);
        }
        public void EnableEqualizer(float[] frequencies = null)
        {
            InitializeEqualizer();
            SetAllEqualizerBandsFrequencies(frequencies ?? new float[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        }
        void InitializeEqualizer()
        {
            // Set peaking equalizer effect with no bands          
            PeakEQParameters eq = new PeakEQParameters();
            fxEQ = Bass.ChannelSetFX(handle, EffectType.PeakEQ, 0);
            eq.fQ = 0f;
            eq.fBandwidth = 2.5f;
            eq.lChannel = FXChannelFlags.All;
            //init equalizer bands
            InitializeEqualizerBands(eq);
        }
        public void SetAllEqualizerBandsFrequencies(float[] frequencies)
        {
            for (int i = 0; i < 10; i++)
            {
                UpdateEQBand(i, frequencies[i]);
            }
        }
        public void ResetEqualizer()
        {
            SetAllEqualizerBandsFrequencies(new float[] {0,0,0,0,0,0,0,0,0,0});
        } 
        void InitializeEqualizerBands(PeakEQParameters eq)
        {
            for (int i = 0; i < 10; i++)
            {
                eq.lBand = i;
                eq.fCenter = DefaultCenterFrequencyList[i];
                Bass.FXSetParameters(fxEQ, eq);
            }           
        }
        /// <summary>
        /// Sets Single Equalizer Band
        /// </summary>
        /// <param name="band">Band to set</param>
        /// <param name="freq">Gain to set</param>
        public void UpdateEQBand(int band, float freq)
        {
            PeakEQParameters eq = new PeakEQParameters();
            // get values of the selected band
            eq.lBand = band;
            Bass.FXGetParameters(fxEQ, eq);
            eq.fGain = freq;
            Bass.FXSetParameters(fxEQ, eq);
        }
        private DSPProcedure _myDSPAddr; // make it global, so that the GC can not remove it
        private float[] _data; // local data buffer
        float _preamp = 6f;
        public float Preamp
        {
            get { return _preamp; }
            set
            {
                _preamp = value;
                Bass.ChannelSetDSP(handle, _myDSPAddr, IntPtr.Zero, 1);
            }
        }
        private unsafe void SetPreamp(int handle, int channel, IntPtr buffer, int length, IntPtr user)
        {
            if (_preamp == 1f || length == 0 || buffer == IntPtr.Zero)
                return;

            // convert the _gainDB value to a float
            float _gainAmplification = (float)Math.Pow(10d, _preamp / 20d);
            // length is in bytes, so the number of floats to process is length/4 
            int l4 = length / 4;
            // cast the given buffer IntPtr to a native pointer to float values
            float* data = (float*)buffer;
            for (int a = 0; a < l4; a++)
            {
                data[a] = data[a] * _gainAmplification;
            }
        }
    }
}
