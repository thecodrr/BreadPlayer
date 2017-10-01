using System.Runtime.InteropServices;

namespace BreadPlayer.Core.Engines
{
    public class NativeMethods
    {
        [DllImport("bass.dll")]
        public static extern bool BASS_SetConfig(int config, int newValue);

        public const int BassConfigDevBuffer = 27;
    }
}