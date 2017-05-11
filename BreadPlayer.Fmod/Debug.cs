/* ========================================================================================== */
/*                                                                                            */
/* BreadPlayer.Fmod Studio - C# Wrapper . Copyright (c), Firelight Technologies Pty, Ltd. 2004-2016.      */
/*                                                                                            */
/* ========================================================================================== */

using BreadPlayer.Fmod.Enums;
using System.Runtime.InteropServices;
using static BreadPlayer.Fmod.Callbacks;

namespace BreadPlayer.Fmod
{
    public class Debug
    {
        public static Result Initialize(DebugFlags flags, DebugMode mode, DebugCallback callback, string filename)
        {
            return FMOD_Debug_Initialize(flags, mode, callback, filename);
        }


        #region importfunctions

        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Debug_Initialize(DebugFlags flags, DebugMode mode, DebugCallback callback, string filename);

        #endregion
    }
}
