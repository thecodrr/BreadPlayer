/* ========================================================================================== */
/*                                                                                            */
/* BreadPlayer.Fmod Studio - C# Wrapper . Copyright (c), Firelight Technologies Pty, Ltd. 2004-2016.      */
/*                                                                                            */
/* ========================================================================================== */

using BreadPlayer.Fmod.Enums;
using BreadPlayer.Fmod.Structs;
using System;
using System.Runtime.InteropServices;

namespace BreadPlayer.Fmod
{
    /*
            'Reverb3D' API
        */
    public class Reverb3D : HandleBase
    {
        public Result release()
        {
            Result result = FMOD_Reverb3D_Release(getRaw());
            if (result == Result.OK)
            {
                rawPtr = IntPtr.Zero;
            }
            return result;
        }

        // Reverb manipulation.
        public Result set3DAttributes(ref Vector position, float mindistance, float maxdistance)
        {
            return FMOD_Reverb3D_Set3DAttributes(rawPtr, ref position, mindistance, maxdistance);
        }
        public Result get3DAttributes(ref Vector position, ref float mindistance, ref float maxdistance)
        {
            return FMOD_Reverb3D_Get3DAttributes(rawPtr, ref position, ref mindistance, ref maxdistance);
        }
        public Result setProperties(ref ReverbProperties properties)
        {
            return FMOD_Reverb3D_SetProperties(rawPtr, ref properties);
        }
        public Result getProperties(ref ReverbProperties properties)
        {
            return FMOD_Reverb3D_GetProperties(rawPtr, ref properties);
        }
        public Result setActive(bool active)
        {
            return FMOD_Reverb3D_SetActive(rawPtr, active);
        }
        public Result getActive(out bool active)
        {
            return FMOD_Reverb3D_GetActive(rawPtr, out active);
        }

        // Userdata set/get.
        public Result setUserData(IntPtr userdata)
        {
            return FMOD_Reverb3D_SetUserData(rawPtr, userdata);
        }
        public Result getUserData(out IntPtr userdata)
        {
            return FMOD_Reverb3D_GetUserData(rawPtr, out userdata);
        }

        #region importfunctions
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Reverb3D_Release(IntPtr reverb);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Reverb3D_Set3DAttributes(IntPtr reverb, ref Vector position, float mindistance, float maxdistance);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Reverb3D_Get3DAttributes(IntPtr reverb, ref Vector position, ref float mindistance, ref float maxdistance);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Reverb3D_SetProperties(IntPtr reverb, ref ReverbProperties properties);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Reverb3D_GetProperties(IntPtr reverb, ref ReverbProperties properties);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Reverb3D_SetActive(IntPtr reverb, bool active);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Reverb3D_GetActive(IntPtr reverb, out bool active);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Reverb3D_SetUserData(IntPtr reverb, IntPtr userdata);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Reverb3D_GetUserData(IntPtr reverb, out IntPtr userdata);
        #endregion

        #region wrapperinternal

        public Reverb3D(IntPtr raw)
            : base(raw)
        {
        }

        #endregion
    }
}
