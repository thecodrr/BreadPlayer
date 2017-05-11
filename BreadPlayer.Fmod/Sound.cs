/* ========================================================================================== */
/*                                                                                            */
/* BreadPlayer.Fmod Studio - C# Wrapper . Copyright (c), Firelight Technologies Pty, Ltd. 2004-2016.      */
/*                                                                                            */
/* ========================================================================================== */

using System;
using System.Runtime.InteropServices;
using System.Text;
using BreadPlayer.Fmod.Enums;
using BreadPlayer.Fmod.Structs;

namespace BreadPlayer.Fmod
{
    /*
            'Sound' API.
        */
    public class Sound : HandleBase
    {
        public Result Release                 ()
        {
            Result result = FMOD_Sound_Release(RawPtr);
            if (result == Result.Ok)
            {
                RawPtr = IntPtr.Zero;
            }
            return result;
        }
        public Result GetSystemObject         (out FmodSystem system)
        {
            system = null;

            IntPtr systemraw;
            Result result = FMOD_Sound_GetSystemObject(RawPtr, out systemraw);
            system = new FmodSystem(systemraw);

            return result;
        }

        // Standard sound manipulation functions.
        public Result Lock                   (uint offset, uint length, out IntPtr ptr1, out IntPtr ptr2, out uint len1, out uint len2)
        {
            return FMOD_Sound_Lock(RawPtr, offset, length, out ptr1, out ptr2, out len1, out len2);
        }
        public Result Unlock                  (IntPtr ptr1,  IntPtr ptr2, uint len1, uint len2)
        {
            return FMOD_Sound_Unlock(RawPtr, ptr1, ptr2, len1, len2);
        }
        public Result SetDefaults             (float frequency, int priority)
        {
            return FMOD_Sound_SetDefaults(RawPtr, frequency, priority);
        }
        public Result GetDefaults             (out float frequency, out int priority)
        {
            return FMOD_Sound_GetDefaults(RawPtr, out frequency, out priority);
        }
        public Result Set3DMinMaxDistance     (float min, float max)
        {
            return FMOD_Sound_Set3DMinMaxDistance(RawPtr, min, max);
        }
        public Result Get3DMinMaxDistance     (out float min, out float max)
        {
            return FMOD_Sound_Get3DMinMaxDistance(RawPtr, out min, out max);
        }
        public Result Set3DConeSettings       (float insideconeangle, float outsideconeangle, float outsidevolume)
        {
            return FMOD_Sound_Set3DConeSettings(RawPtr, insideconeangle, outsideconeangle, outsidevolume);
        }
        public Result Get3DConeSettings       (out float insideconeangle, out float outsideconeangle, out float outsidevolume)
        {
            return FMOD_Sound_Get3DConeSettings(RawPtr, out insideconeangle, out outsideconeangle, out outsidevolume);
        }
        public Result Set3DCustomRolloff      (ref Vector points, int numpoints)
        {
            return FMOD_Sound_Set3DCustomRolloff(RawPtr, ref points, numpoints);
        }
        public Result Get3DCustomRolloff      (out IntPtr points, out int numpoints)
        {
            return FMOD_Sound_Get3DCustomRolloff(RawPtr, out points, out numpoints);
        }
        public Result GetSubSound             (int index, out Sound subsound)
        {
            subsound = null;

            IntPtr subsoundraw;
            Result result = FMOD_Sound_GetSubSound(RawPtr, index, out subsoundraw);
            subsound = new Sound(subsoundraw);

            return result;
        }
        public Result GetSubSoundParent(out Sound parentsound)
        {
            parentsound = null;

            IntPtr subsoundraw;
            Result result = FMOD_Sound_GetSubSoundParent(RawPtr, out subsoundraw);
            parentsound = new Sound(subsoundraw);

            return result;
        }
        public Result GetName                 (StringBuilder name, int namelen)
        {
            IntPtr stringMem = Marshal.AllocHGlobal(name.Capacity);

            Result result = FMOD_Sound_GetName(RawPtr, stringMem, namelen);

            StringMarshalHelper.NativeToBuilder(name, stringMem);
            Marshal.FreeHGlobal(stringMem);

            return result;
        }
        public uint LengthInMilliseconds
        {
            get
            {
                FMOD_Sound_GetLength(RawPtr, out uint length, TimeUnit.Ms);
                return length;
            }
        }
        public Result GetLength               (out uint length, TimeUnit lengthtype)
        {
            return FMOD_Sound_GetLength(RawPtr, out length, lengthtype);
        }
        public Result GetFormat               (out SoundType type, out SoundFormat format, out int channels, out int bits)
        {
            return FMOD_Sound_GetFormat(RawPtr, out type, out format, out channels, out bits);
        }
        public Result GetNumSubSounds         (out int numsubsounds)
        {
            return FMOD_Sound_GetNumSubSounds(RawPtr, out numsubsounds);
        }
        public Result GetNumTags              (out int numtags, out int numtagsupdated)
        {
            return FMOD_Sound_GetNumTags(RawPtr, out numtags, out numtagsupdated);
        }
        public Result GetTag                  (string name, int index, out Tag tag)
        {
            return FMOD_Sound_GetTag(RawPtr, name, index, out tag);
        }
        public Result GetOpenState            (out OpenState openstate, out uint percentbuffered, out bool starving, out bool diskbusy)
        {
            return FMOD_Sound_GetOpenState(RawPtr, out openstate, out percentbuffered, out starving, out diskbusy);
        }
        public Result ReadData                (IntPtr buffer, uint lenbytes, out uint read)
        {
            return FMOD_Sound_ReadData(RawPtr, buffer, lenbytes, out read);
        }
        public Result SeekData                (uint pcm)
        {
            return FMOD_Sound_SeekData(RawPtr, pcm);
        }
        public Result SetSoundGroup           (SoundGroup soundgroup)
        {
            return FMOD_Sound_SetSoundGroup(RawPtr, soundgroup.GetRaw());
        }
        public Result GetSoundGroup           (out SoundGroup soundgroup)
        {
            soundgroup = null;

            IntPtr soundgroupraw;
            Result result = FMOD_Sound_GetSoundGroup(RawPtr, out soundgroupraw);
            soundgroup = new SoundGroup(soundgroupraw);

            return result;
        }

        // Synchronization point API.  These points can come from markers embedded in wav files, and can also generate channel callbacks.
        public Result GetNumSyncPoints        (out int numsyncpoints)
        {
            return FMOD_Sound_GetNumSyncPoints(RawPtr, out numsyncpoints);
        }
        public Result GetSyncPoint            (int index, out IntPtr point)
        {
            return FMOD_Sound_GetSyncPoint(RawPtr, index, out point);
        }
        public Result GetSyncPointInfo        (IntPtr point, StringBuilder name, int namelen, out uint offset, TimeUnit offsettype)
        {
            IntPtr stringMem = Marshal.AllocHGlobal(name.Capacity);

            Result result = FMOD_Sound_GetSyncPointInfo(RawPtr, point, stringMem, namelen, out offset, offsettype);

            StringMarshalHelper.NativeToBuilder(name, stringMem);
            Marshal.FreeHGlobal(stringMem);

            return result;
        }
        public Result AddSyncPoint            (uint offset, TimeUnit offsettype, string name, out IntPtr point)
        {
            return FMOD_Sound_AddSyncPoint(RawPtr, offset, offsettype, name, out point);
        }
        public Result DeleteSyncPoint         (IntPtr point)
        {
            return FMOD_Sound_DeleteSyncPoint(RawPtr, point);
        }

        // Functions also in Channel class but here they are the 'default' to save having to change it in Channel all the time.
        public Result SetMode                 (Mode mode)
        {
            return FMOD_Sound_SetMode(RawPtr, mode);
        }
        public Result GetMode                 (out Mode mode)
        {
            return FMOD_Sound_GetMode(RawPtr, out mode);
        }
        public Result SetLoopCount            (int loopcount)
        {
            return FMOD_Sound_SetLoopCount(RawPtr, loopcount);
        }
        public Result GetLoopCount            (out int loopcount)
        {
            return FMOD_Sound_GetLoopCount(RawPtr, out loopcount);
        }
        public Result SetLoopPoints           (uint loopstart, TimeUnit loopstarttype, uint loopend, TimeUnit loopendtype)
        {
            return FMOD_Sound_SetLoopPoints(RawPtr, loopstart, loopstarttype, loopend, loopendtype);
        }
        public Result GetLoopPoints           (out uint loopstart, TimeUnit loopstarttype, out uint loopend, TimeUnit loopendtype)
        {
            return FMOD_Sound_GetLoopPoints(RawPtr, out loopstart, loopstarttype, out loopend, loopendtype);
        }

        // For MOD/S3M/XM/IT/MID sequenced formats only.
        public Result GetMusicNumChannels     (out int numchannels)
        {
            return FMOD_Sound_GetMusicNumChannels(RawPtr, out numchannels);
        }
        public Result SetMusicChannelVolume   (int channel, float volume)
        {
            return FMOD_Sound_SetMusicChannelVolume(RawPtr, channel, volume);
        }
        public Result GetMusicChannelVolume   (int channel, out float volume)
        {
            return FMOD_Sound_GetMusicChannelVolume(RawPtr, channel, out volume);
        }
        public Result SetMusicSpeed(float speed)
        {
            return FMOD_Sound_SetMusicSpeed(RawPtr, speed);
        }
        public Result GetMusicSpeed(out float speed)
        {
            return FMOD_Sound_GetMusicSpeed(RawPtr, out speed);
        }

        // Userdata set/get.
        public Result SetUserData             (IntPtr userdata)
        {
            return FMOD_Sound_SetUserData(RawPtr, userdata);
        }
        public Result GetUserData             (out IntPtr userdata)
        {
            return FMOD_Sound_GetUserData(RawPtr, out userdata);
        }


        #region importfunctions
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_Release                 (IntPtr sound);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_GetSystemObject         (IntPtr sound, out IntPtr system);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_Lock                   (IntPtr sound, uint offset, uint length, out IntPtr ptr1, out IntPtr ptr2, out uint len1, out uint len2);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_Unlock                  (IntPtr sound, IntPtr ptr1,  IntPtr ptr2, uint len1, uint len2);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_SetDefaults             (IntPtr sound, float frequency, int priority);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_GetDefaults             (IntPtr sound, out float frequency, out int priority);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_Set3DMinMaxDistance     (IntPtr sound, float min, float max);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_Get3DMinMaxDistance     (IntPtr sound, out float min, out float max);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_Set3DConeSettings       (IntPtr sound, float insideconeangle, float outsideconeangle, float outsidevolume);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_Get3DConeSettings       (IntPtr sound, out float insideconeangle, out float outsideconeangle, out float outsidevolume);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_Set3DCustomRolloff      (IntPtr sound, ref Vector points, int numpoints);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_Get3DCustomRolloff      (IntPtr sound, out IntPtr points, out int numpoints);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_GetSubSound             (IntPtr sound, int index, out IntPtr subsound);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_GetSubSoundParent       (IntPtr sound, out IntPtr parentsound);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_GetName                 (IntPtr sound, IntPtr name, int namelen);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_GetLength               (IntPtr sound, out uint length, TimeUnit lengthtype);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_GetFormat               (IntPtr sound, out SoundType type, out SoundFormat format, out int channels, out int bits);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_GetNumSubSounds         (IntPtr sound, out int numsubsounds);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_GetNumTags              (IntPtr sound, out int numtags, out int numtagsupdated);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_GetTag                  (IntPtr sound, string name, int index, out Tag tag);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_GetOpenState            (IntPtr sound, out OpenState openstate, out uint percentbuffered, out bool starving, out bool diskbusy);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_ReadData                (IntPtr sound, IntPtr buffer, uint lenbytes, out uint read);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_SeekData                (IntPtr sound, uint pcm);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_SetSoundGroup           (IntPtr sound, IntPtr soundgroup);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_GetSoundGroup           (IntPtr sound, out IntPtr soundgroup);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_GetNumSyncPoints        (IntPtr sound, out int numsyncpoints);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_GetSyncPoint            (IntPtr sound, int index, out IntPtr point);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_GetSyncPointInfo        (IntPtr sound, IntPtr point, IntPtr name, int namelen, out uint offset, TimeUnit offsettype);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_AddSyncPoint            (IntPtr sound, uint offset, TimeUnit offsettype, string name, out IntPtr point);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_DeleteSyncPoint         (IntPtr sound, IntPtr point);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_SetMode                 (IntPtr sound, Mode mode);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_GetMode                 (IntPtr sound, out Mode mode);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_SetLoopCount            (IntPtr sound, int loopcount);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_GetLoopCount            (IntPtr sound, out int loopcount);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_SetLoopPoints           (IntPtr sound, uint loopstart, TimeUnit loopstarttype, uint loopend, TimeUnit loopendtype);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_GetLoopPoints           (IntPtr sound, out uint loopstart, TimeUnit loopstarttype, out uint loopend, TimeUnit loopendtype);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_GetMusicNumChannels     (IntPtr sound, out int numchannels);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_SetMusicChannelVolume   (IntPtr sound, int channel, float volume);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_GetMusicChannelVolume   (IntPtr sound, int channel, out float volume);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_SetMusicSpeed           (IntPtr sound, float speed);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_GetMusicSpeed           (IntPtr sound, out float speed);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_SetUserData             (IntPtr sound, IntPtr userdata);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Sound_GetUserData             (IntPtr sound, out IntPtr userdata);
        #endregion

        #region wrapperinternal

        public Sound(IntPtr raw)
            : base(raw)
        {
        }

        #endregion
    }
}
