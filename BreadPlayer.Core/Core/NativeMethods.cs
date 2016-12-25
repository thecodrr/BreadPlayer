using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Core
{
    class NativeMethods
    {
        [DllImport("bass.dll")]
        public static extern bool BASS_SetConfig(int config, int newValue);      
    }
}
