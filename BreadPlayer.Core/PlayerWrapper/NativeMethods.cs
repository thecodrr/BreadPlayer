using System.Runtime.InteropServices;

namespace BreadPlayer.Core
{
    class NativeMethods
    {
        [DllImport("bass.dll")]
        public static extern bool BASS_SetConfig(int config, int newValue);      
    }
}
