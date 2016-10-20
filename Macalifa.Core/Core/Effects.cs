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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagedBass;
namespace BreadPlayer.Core
{
    public class Effects
    {
        #region Fields
        int handle = 0;
        #endregion

        #region Initialize
        public Effects(int coreHandle)
        {
            handle = coreHandle;
            _myDSPAddr = new DSPProcedure(MyDSPGain);
        }
        #endregion

        private DSPProcedure _myDSPAddr; // make it global, so that the GC can not remove it
        private float[] _data; // local data buffer
        float _gainDB = 6f;
        public float GainDB
        {
            get { return _gainDB; }
            set
            {
                _gainDB = value;
                Bass.ChannelSetDSP(handle, _myDSPAddr, IntPtr.Zero, 1);
            }
        }
        private unsafe void MyDSPGain(int handle, int channel, IntPtr buffer, int length, IntPtr user)
        {
            if (_gainDB == 1f || length == 0 || buffer == IntPtr.Zero)
                return;

            // convert the _gainDB value to a float
            float _gainAmplification = (float)Math.Pow(10d, _gainDB / 20d);
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
