/* ========================================================================================== */
/*                                                                                            */
/* BreadPlayer.Fmod Studio - C# Wrapper . Copyright (c), Firelight Technologies Pty, Ltd. 2004-2016.      */
/*                                                                                            */
/* ========================================================================================== */

using System;
using System.Text;
using System.Runtime.InteropServices;
using BreadPlayer.Fmod.Enums;

namespace BreadPlayer.Fmod
{
    /*
            'SoundGroup' API
        */
    public class SoundGroup : HandleBase
    {
        public Result Release                ()
        {
            Result result = FMOD_SoundGroup_Release(GetRaw());
            if (result == Result.Ok)
            {
                RawPtr = IntPtr.Zero;
            }
            return result;
        }

        public Result GetSystemObject        (out FmodSystem system)
        {
            system = null;

            IntPtr systemraw;
            Result result = FMOD_SoundGroup_GetSystemObject(RawPtr, out systemraw);
            system = new FmodSystem(systemraw);

            return result;
        }

        // SoundGroup control functions.
        public Result SetMaxAudible          (int maxaudible)
        {
            return FMOD_SoundGroup_SetMaxAudible(RawPtr, maxaudible);
        }
        public Result GetMaxAudible          (out int maxaudible)
        {
            return FMOD_SoundGroup_GetMaxAudible(RawPtr, out maxaudible);
        }
        public Result SetMaxAudibleBehavior  (SoundGroupBehavior behavior)
        {
            return FMOD_SoundGroup_SetMaxAudibleBehavior(RawPtr, behavior);
        }
        public Result GetMaxAudibleBehavior  (out SoundGroupBehavior behavior)
        {
            return FMOD_SoundGroup_GetMaxAudibleBehavior(RawPtr, out behavior);
        }
        public Result SetMuteFadeSpeed       (float speed)
        {
            return FMOD_SoundGroup_SetMuteFadeSpeed(RawPtr, speed);
        }
        public Result GetMuteFadeSpeed       (out float speed)
        {
            return FMOD_SoundGroup_GetMuteFadeSpeed(RawPtr, out speed);
        }
        public Result SetVolume       (float volume)
        {
            return FMOD_SoundGroup_SetVolume(RawPtr, volume);
        }
        public Result GetVolume       (out float volume)
        {
            return FMOD_SoundGroup_GetVolume(RawPtr, out volume);
        }
        public Result Stop       ()
        {
            return FMOD_SoundGroup_Stop(RawPtr);
        }

        // Information only functions.
        public Result GetName                (StringBuilder name, int namelen)
        {
            IntPtr stringMem = Marshal.AllocHGlobal(name.Capacity);

            Result result = FMOD_SoundGroup_GetName(RawPtr, stringMem, namelen);

            StringMarshalHelper.NativeToBuilder(name, stringMem);
            Marshal.FreeHGlobal(stringMem);

            return result;
        }
        public Result GetNumSounds           (out int numsounds)
        {
            return FMOD_SoundGroup_GetNumSounds(RawPtr, out numsounds);
        }
        public Result GetSound               (int index, out Sound sound)
        {
            sound = null;

            IntPtr soundraw;
            Result result = FMOD_SoundGroup_GetSound(RawPtr, index, out soundraw);
            sound = new Sound(soundraw);

            return result;
        }
        public Result GetNumPlaying          (out int numplaying)
        {
            return FMOD_SoundGroup_GetNumPlaying(RawPtr, out numplaying);
        }

        // Userdata set/get.
        public Result SetUserData            (IntPtr userdata)
        {
            return FMOD_SoundGroup_SetUserData(RawPtr, userdata);
        }
        public Result GetUserData            (out IntPtr userdata)
        {
            return FMOD_SoundGroup_GetUserData(RawPtr, out userdata);
        }

        #region importfunctions
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_SoundGroup_Release            (IntPtr soundgroup);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_SoundGroup_GetSystemObject    (IntPtr soundgroup, out IntPtr system);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_SoundGroup_SetMaxAudible      (IntPtr soundgroup, int maxaudible);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_SoundGroup_GetMaxAudible      (IntPtr soundgroup, out int maxaudible);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_SoundGroup_SetMaxAudibleBehavior(IntPtr soundgroup, SoundGroupBehavior behavior);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_SoundGroup_GetMaxAudibleBehavior(IntPtr soundgroup, out SoundGroupBehavior behavior);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_SoundGroup_SetMuteFadeSpeed   (IntPtr soundgroup, float speed);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_SoundGroup_GetMuteFadeSpeed   (IntPtr soundgroup, out float speed);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_SoundGroup_SetVolume          (IntPtr soundgroup, float volume);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_SoundGroup_GetVolume          (IntPtr soundgroup, out float volume);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_SoundGroup_Stop               (IntPtr soundgroup);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_SoundGroup_GetName            (IntPtr soundgroup, IntPtr name, int namelen);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_SoundGroup_GetNumSounds       (IntPtr soundgroup, out int numsounds);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_SoundGroup_GetSound           (IntPtr soundgroup, int index, out IntPtr sound);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_SoundGroup_GetNumPlaying      (IntPtr soundgroup, out int numplaying);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_SoundGroup_SetUserData        (IntPtr soundgroup, IntPtr userdata);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_SoundGroup_GetUserData        (IntPtr soundgroup, out IntPtr userdata);
        #endregion

        #region wrapperinternal

        public SoundGroup(IntPtr raw)
            : base(raw)
        {
        }

        #endregion
    }
}
