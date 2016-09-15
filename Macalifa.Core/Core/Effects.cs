using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagedBass;
namespace Macalifa.Core
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
