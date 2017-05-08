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
        public Result release                ()
        {
            Result result = FMOD_SoundGroup_Release(getRaw());
            if (result == Result.OK)
            {
                rawPtr = IntPtr.Zero;
            }
            return result;
        }

        public Result getSystemObject        (out FMODSystem system)
        {
            system = null;

            IntPtr systemraw;
            Result result = FMOD_SoundGroup_GetSystemObject(rawPtr, out systemraw);
            system = new FMODSystem(systemraw);

            return result;
        }

        // SoundGroup control functions.
        public Result setMaxAudible          (int maxaudible)
        {
            return FMOD_SoundGroup_SetMaxAudible(rawPtr, maxaudible);
        }
        public Result getMaxAudible          (out int maxaudible)
        {
            return FMOD_SoundGroup_GetMaxAudible(rawPtr, out maxaudible);
        }
        public Result setMaxAudibleBehavior  (SoundGroupBehavior behavior)
        {
            return FMOD_SoundGroup_SetMaxAudibleBehavior(rawPtr, behavior);
        }
        public Result getMaxAudibleBehavior  (out SoundGroupBehavior behavior)
        {
            return FMOD_SoundGroup_GetMaxAudibleBehavior(rawPtr, out behavior);
        }
        public Result setMuteFadeSpeed       (float speed)
        {
            return FMOD_SoundGroup_SetMuteFadeSpeed(rawPtr, speed);
        }
        public Result getMuteFadeSpeed       (out float speed)
        {
            return FMOD_SoundGroup_GetMuteFadeSpeed(rawPtr, out speed);
        }
        public Result setVolume       (float volume)
        {
            return FMOD_SoundGroup_SetVolume(rawPtr, volume);
        }
        public Result getVolume       (out float volume)
        {
            return FMOD_SoundGroup_GetVolume(rawPtr, out volume);
        }
        public Result stop       ()
        {
            return FMOD_SoundGroup_Stop(rawPtr);
        }

        // Information only functions.
        public Result getName                (StringBuilder name, int namelen)
        {
            IntPtr stringMem = Marshal.AllocHGlobal(name.Capacity);

            Result result = FMOD_SoundGroup_GetName(rawPtr, stringMem, namelen);

            StringMarshalHelper.NativeToBuilder(name, stringMem);
            Marshal.FreeHGlobal(stringMem);

            return result;
        }
        public Result getNumSounds           (out int numsounds)
        {
            return FMOD_SoundGroup_GetNumSounds(rawPtr, out numsounds);
        }
        public Result getSound               (int index, out Sound sound)
        {
            sound = null;

            IntPtr soundraw;
            Result result = FMOD_SoundGroup_GetSound(rawPtr, index, out soundraw);
            sound = new Sound(soundraw);

            return result;
        }
        public Result getNumPlaying          (out int numplaying)
        {
            return FMOD_SoundGroup_GetNumPlaying(rawPtr, out numplaying);
        }

        // Userdata set/get.
        public Result setUserData            (IntPtr userdata)
        {
            return FMOD_SoundGroup_SetUserData(rawPtr, userdata);
        }
        public Result getUserData            (out IntPtr userdata)
        {
            return FMOD_SoundGroup_GetUserData(rawPtr, out userdata);
        }

        #region importfunctions
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_SoundGroup_Release            (IntPtr soundgroup);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_SoundGroup_GetSystemObject    (IntPtr soundgroup, out IntPtr system);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_SoundGroup_SetMaxAudible      (IntPtr soundgroup, int maxaudible);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_SoundGroup_GetMaxAudible      (IntPtr soundgroup, out int maxaudible);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_SoundGroup_SetMaxAudibleBehavior(IntPtr soundgroup, SoundGroupBehavior behavior);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_SoundGroup_GetMaxAudibleBehavior(IntPtr soundgroup, out SoundGroupBehavior behavior);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_SoundGroup_SetMuteFadeSpeed   (IntPtr soundgroup, float speed);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_SoundGroup_GetMuteFadeSpeed   (IntPtr soundgroup, out float speed);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_SoundGroup_SetVolume          (IntPtr soundgroup, float volume);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_SoundGroup_GetVolume          (IntPtr soundgroup, out float volume);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_SoundGroup_Stop               (IntPtr soundgroup);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_SoundGroup_GetName            (IntPtr soundgroup, IntPtr name, int namelen);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_SoundGroup_GetNumSounds       (IntPtr soundgroup, out int numsounds);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_SoundGroup_GetSound           (IntPtr soundgroup, int index, out IntPtr sound);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_SoundGroup_GetNumPlaying      (IntPtr soundgroup, out int numplaying);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_SoundGroup_SetUserData        (IntPtr soundgroup, IntPtr userdata);
        [DllImport(FMODVersion.DLL)]
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
