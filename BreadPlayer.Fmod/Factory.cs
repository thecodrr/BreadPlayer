/* ========================================================================================== */
/*                                                                                            */
/* BreadPlayer.Fmod Studio - C# Wrapper . Copyright (c), Firelight Technologies Pty, Ltd. 2004-2016.      */
/*                                                                                            */
/* ========================================================================================== */

using BreadPlayer.Fmod.Enums;
using System;
using System.Runtime.InteropServices;

namespace BreadPlayer.Fmod
{
    /*
            BreadPlayer.Fmod FMODSystem factory functions.  Use this to create an BreadPlayer.Fmod FMODSystem Instance.  below you will see FMODSystem init/close to get started.
        */
    public class Factory
    {
        public static Result SystemCreate(out FMODSystem system)
        {
            system = null;

            Result result   = Result.OK;
            IntPtr rawPtr   = new IntPtr();

            result = FMOD_System_Create(out rawPtr);
            if (result != Result.OK)
            {
                return result;
            }

            system = new FMODSystem(rawPtr);

            return result;
        }


        #region importfunctions

        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_Create                      (out IntPtr system);

        #endregion
    }
}
