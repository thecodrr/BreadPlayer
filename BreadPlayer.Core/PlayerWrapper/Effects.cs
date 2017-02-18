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
        ManagedBass.Fx.PeakEQParameters eq;
        int fxEQ;
        private float[] equalizerTmp = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        #endregion

        #region Initialize
        public Effects(int coreHandle)
        {
            handle = coreHandle;
            _myDSPAddr = new DSPProcedure(SetPreAmp);
        }
        #endregion
        public void TurnOnEQ()
        {
            var res = BassFx.Load();
            eq = new PeakEQParameters();
            fxEQ = Bass.ChannelSetFX(handle, EffectType.PeakEQ, 0);
            eq.fBandwidth = 2.5f;
            eq.lChannel = FXChannelFlags.All;
            eq.fGain = 0.0f; // the initial gain (here 0dB)
            eq.fQ = 0f;
                        
            //Band 1
            eq.lBand = 0;
            eq.fCenter = 60f;
            Bass.FXSetParameters(fxEQ, eq);

            //Band 2
            eq.lBand = 1;
            eq.fCenter = 170f;
            Bass.FXSetParameters(fxEQ, eq);
            
            //Band 3
            eq.lBand = 2;
            eq.fCenter = 310f;
            Bass.FXSetParameters(fxEQ, eq);

            //Band 4
            eq.lBand = 3;
            eq.fCenter = 600f;
            Bass.FXSetParameters(fxEQ, eq);

            //Band 5
            eq.lBand = 4;
            eq.fCenter = 1000f;
            Bass.FXSetParameters(fxEQ, eq);

            //Band 6
            eq.lBand = 5;
            eq.fCenter = 3000f;
            Bass.FXSetParameters(fxEQ, eq);

            //Band 7
            eq.lBand = 6;
            eq.fCenter = 6000f;
            Bass.FXSetParameters(fxEQ, eq);

            //Band 8
            eq.lBand = 7;
            eq.fCenter = 12000f;
            Bass.FXSetParameters(fxEQ, eq);

            //Band 9
            eq.lBand = 8;
            eq.fCenter = 14000f;
            Bass.FXSetParameters(fxEQ, eq);

            //Band 10
            eq.lBand = 9;
            eq.fCenter = 16000f;
            Bass.FXSetParameters(fxEQ, eq);
        } 
            /// <summary>
          /// Sets Single Equalizer Band
          /// </summary>
          /// <param name="band">Band to set</param>
          /// <param name="freq">Gain to set</param>
        public void UpdateEqBand(int band, float freq)
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
        float preamp = 6f;
        public float Preamp
        {
            get { return preamp; }
            set
            {
                preamp = value;
                Bass.ChannelSetDSP(handle, _myDSPAddr, IntPtr.Zero, 1);
            }
        }
        private unsafe void SetPreAmp(int handle, int channel, IntPtr buffer, int length, IntPtr user)
        {
            if (preamp == 1f || length == 0 || buffer == IntPtr.Zero)
                return;

            // convert the _gainDB value to a float
            float _gainAmplification = (float)Math.Pow(10d, preamp / 20d);
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
