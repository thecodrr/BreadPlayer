/* ========================================================================================== */
/*                                                                                            */
/* BreadPlayer.Fmod Studio - C# Wrapper . Copyright (c), Firelight Technologies Pty, Ltd. 2004-2016.      */
/*                                                                                            */
/* ========================================================================================== */

using System;
using System.Runtime.InteropServices;
using BreadPlayer.Fmod.Enums;
using BreadPlayer.Fmod.Structs;

namespace BreadPlayer.Fmod
{
    /*
            'Reverb3D' API
        */
    public class Reverb3D : HandleBase
    {
        public Result Release()
        {
            Result result = FMOD_Reverb3D_Release(GetRaw());
            if (result == Result.Ok)
            {
                RawPtr = IntPtr.Zero;
            }
            return result;
        }

        // Reverb manipulation.
        public Result Set3DAttributes(ref Vector position, float mindistance, float maxdistance)
        {
            return FMOD_Reverb3D_Set3DAttributes(RawPtr, ref position, mindistance, maxdistance);
        }
        public Result Get3DAttributes(ref Vector position, ref float mindistance, ref float maxdistance)
        {
            return FMOD_Reverb3D_Get3DAttributes(RawPtr, ref position, ref mindistance, ref maxdistance);
        }
        public Result SetProperties(ref ReverbProperties properties)
        {
            return FMOD_Reverb3D_SetProperties(RawPtr, ref properties);
        }
        public Result GetProperties(ref ReverbProperties properties)
        {
            return FMOD_Reverb3D_GetProperties(RawPtr, ref properties);
        }
        public Result SetActive(bool active)
        {
            return FMOD_Reverb3D_SetActive(RawPtr, active);
        }
        public Result GetActive(out bool active)
        {
            return FMOD_Reverb3D_GetActive(RawPtr, out active);
        }

        // Userdata set/get.
        public Result SetUserData(IntPtr userdata)
        {
            return FMOD_Reverb3D_SetUserData(RawPtr, userdata);
        }
        public Result GetUserData(out IntPtr userdata)
        {
            return FMOD_Reverb3D_GetUserData(RawPtr, out userdata);
        }

        #region importfunctions
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Reverb3D_Release(IntPtr reverb);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Reverb3D_Set3DAttributes(IntPtr reverb, ref Vector position, float mindistance, float maxdistance);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Reverb3D_Get3DAttributes(IntPtr reverb, ref Vector position, ref float mindistance, ref float maxdistance);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Reverb3D_SetProperties(IntPtr reverb, ref ReverbProperties properties);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Reverb3D_GetProperties(IntPtr reverb, ref ReverbProperties properties);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Reverb3D_SetActive(IntPtr reverb, bool active);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Reverb3D_GetActive(IntPtr reverb, out bool active);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Reverb3D_SetUserData(IntPtr reverb, IntPtr userdata);
        [DllImport(FmodVersion.Dll)]
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
