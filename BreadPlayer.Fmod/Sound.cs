/* ========================================================================================== */
/*                                                                                            */
/* BreadPlayer.Fmod Studio - C# Wrapper . Copyright (c), Firelight Technologies Pty, Ltd. 2004-2016.      */
/*                                                                                            */
/* ========================================================================================== */

using BreadPlayer.Fmod;
using BreadPlayer.Fmod.Enums;
using BreadPlayer.Fmod.Structs;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BreadPlayer.Fmod
{
    /*
            'Sound' API.
        */
    public class Sound : HandleBase
    {
        public Result release                 ()
        {
            Result result = FMOD_Sound_Release(rawPtr);
            if (result == Result.OK)
            {
                rawPtr = IntPtr.Zero;
            }
            return result;
        }
        public Result getSystemObject         (out FMODSystem system)
        {
            system = null;

            IntPtr systemraw;
            Result result = FMOD_Sound_GetSystemObject(rawPtr, out systemraw);
            system = new FMODSystem(systemraw);

            return result;
        }

        // Standard sound manipulation functions.
        public Result @lock                   (uint offset, uint length, out IntPtr ptr1, out IntPtr ptr2, out uint len1, out uint len2)
        {
            return FMOD_Sound_Lock(rawPtr, offset, length, out ptr1, out ptr2, out len1, out len2);
        }
        public Result unlock                  (IntPtr ptr1,  IntPtr ptr2, uint len1, uint len2)
        {
            return FMOD_Sound_Unlock(rawPtr, ptr1, ptr2, len1, len2);
        }
        public Result setDefaults             (float frequency, int priority)
        {
            return FMOD_Sound_SetDefaults(rawPtr, frequency, priority);
        }
        public Result getDefaults             (out float frequency, out int priority)
        {
            return FMOD_Sound_GetDefaults(rawPtr, out frequency, out priority);
        }
        public Result set3DMinMaxDistance     (float min, float max)
        {
            return FMOD_Sound_Set3DMinMaxDistance(rawPtr, min, max);
        }
        public Result get3DMinMaxDistance     (out float min, out float max)
        {
            return FMOD_Sound_Get3DMinMaxDistance(rawPtr, out min, out max);
        }
        public Result set3DConeSettings       (float insideconeangle, float outsideconeangle, float outsidevolume)
        {
            return FMOD_Sound_Set3DConeSettings(rawPtr, insideconeangle, outsideconeangle, outsidevolume);
        }
        public Result get3DConeSettings       (out float insideconeangle, out float outsideconeangle, out float outsidevolume)
        {
            return FMOD_Sound_Get3DConeSettings(rawPtr, out insideconeangle, out outsideconeangle, out outsidevolume);
        }
        public Result set3DCustomRolloff      (ref Vector points, int numpoints)
        {
            return FMOD_Sound_Set3DCustomRolloff(rawPtr, ref points, numpoints);
        }
        public Result get3DCustomRolloff      (out IntPtr points, out int numpoints)
        {
            return FMOD_Sound_Get3DCustomRolloff(rawPtr, out points, out numpoints);
        }
        public Result getSubSound             (int index, out Sound subsound)
        {
            subsound = null;

            IntPtr subsoundraw;
            Result result = FMOD_Sound_GetSubSound(rawPtr, index, out subsoundraw);
            subsound = new Sound(subsoundraw);

            return result;
        }
        public Result getSubSoundParent(out Sound parentsound)
        {
            parentsound = null;

            IntPtr subsoundraw;
            Result result = FMOD_Sound_GetSubSoundParent(rawPtr, out subsoundraw);
            parentsound = new Sound(subsoundraw);

            return result;
        }
        public Result getName                 (StringBuilder name, int namelen)
        {
            IntPtr stringMem = Marshal.AllocHGlobal(name.Capacity);

            Result result = FMOD_Sound_GetName(rawPtr, stringMem, namelen);

            StringMarshalHelper.NativeToBuilder(name, stringMem);
            Marshal.FreeHGlobal(stringMem);

            return result;
        }
        public uint LengthInMilliseconds
        {
            get
            {
                FMOD_Sound_GetLength(rawPtr, out uint length, TimeUnit.MS);
                return length;
            }
        }
        public Result getLength               (out uint length, TimeUnit lengthtype)
        {
            return FMOD_Sound_GetLength(rawPtr, out length, lengthtype);
        }
        public Result getFormat               (out SoundType type, out SoundFormat format, out int channels, out int bits)
        {
            return FMOD_Sound_GetFormat(rawPtr, out type, out format, out channels, out bits);
        }
        public Result getNumSubSounds         (out int numsubsounds)
        {
            return FMOD_Sound_GetNumSubSounds(rawPtr, out numsubsounds);
        }
        public Result getNumTags              (out int numtags, out int numtagsupdated)
        {
            return FMOD_Sound_GetNumTags(rawPtr, out numtags, out numtagsupdated);
        }
        public Result getTag                  (string name, int index, out Tag tag)
        {
            return FMOD_Sound_GetTag(rawPtr, name, index, out tag);
        }
        public Result getOpenState            (out OpenState openstate, out uint percentbuffered, out bool starving, out bool diskbusy)
        {
            return FMOD_Sound_GetOpenState(rawPtr, out openstate, out percentbuffered, out starving, out diskbusy);
        }
        public Result readData                (IntPtr buffer, uint lenbytes, out uint read)
        {
            return FMOD_Sound_ReadData(rawPtr, buffer, lenbytes, out read);
        }
        public Result seekData                (uint pcm)
        {
            return FMOD_Sound_SeekData(rawPtr, pcm);
        }
        public Result setSoundGroup           (SoundGroup soundgroup)
        {
            return FMOD_Sound_SetSoundGroup(rawPtr, soundgroup.getRaw());
        }
        public Result getSoundGroup           (out SoundGroup soundgroup)
        {
            soundgroup = null;

            IntPtr soundgroupraw;
            Result result = FMOD_Sound_GetSoundGroup(rawPtr, out soundgroupraw);
            soundgroup = new SoundGroup(soundgroupraw);

            return result;
        }

        // Synchronization point API.  These points can come from markers embedded in wav files, and can also generate channel callbacks.
        public Result getNumSyncPoints        (out int numsyncpoints)
        {
            return FMOD_Sound_GetNumSyncPoints(rawPtr, out numsyncpoints);
        }
        public Result getSyncPoint            (int index, out IntPtr point)
        {
            return FMOD_Sound_GetSyncPoint(rawPtr, index, out point);
        }
        public Result getSyncPointInfo        (IntPtr point, StringBuilder name, int namelen, out uint offset, TimeUnit offsettype)
        {
            IntPtr stringMem = Marshal.AllocHGlobal(name.Capacity);

            Result result = FMOD_Sound_GetSyncPointInfo(rawPtr, point, stringMem, namelen, out offset, offsettype);

            StringMarshalHelper.NativeToBuilder(name, stringMem);
            Marshal.FreeHGlobal(stringMem);

            return result;
        }
        public Result addSyncPoint            (uint offset, TimeUnit offsettype, string name, out IntPtr point)
        {
            return FMOD_Sound_AddSyncPoint(rawPtr, offset, offsettype, name, out point);
        }
        public Result deleteSyncPoint         (IntPtr point)
        {
            return FMOD_Sound_DeleteSyncPoint(rawPtr, point);
        }

        // Functions also in Channel class but here they are the 'default' to save having to change it in Channel all the time.
        public Result setMode                 (Mode mode)
        {
            return FMOD_Sound_SetMode(rawPtr, mode);
        }
        public Result getMode                 (out Mode mode)
        {
            return FMOD_Sound_GetMode(rawPtr, out mode);
        }
        public Result setLoopCount            (int loopcount)
        {
            return FMOD_Sound_SetLoopCount(rawPtr, loopcount);
        }
        public Result getLoopCount            (out int loopcount)
        {
            return FMOD_Sound_GetLoopCount(rawPtr, out loopcount);
        }
        public Result setLoopPoints           (uint loopstart, TimeUnit loopstarttype, uint loopend, TimeUnit loopendtype)
        {
            return FMOD_Sound_SetLoopPoints(rawPtr, loopstart, loopstarttype, loopend, loopendtype);
        }
        public Result getLoopPoints           (out uint loopstart, TimeUnit loopstarttype, out uint loopend, TimeUnit loopendtype)
        {
            return FMOD_Sound_GetLoopPoints(rawPtr, out loopstart, loopstarttype, out loopend, loopendtype);
        }

        // For MOD/S3M/XM/IT/MID sequenced formats only.
        public Result getMusicNumChannels     (out int numchannels)
        {
            return FMOD_Sound_GetMusicNumChannels(rawPtr, out numchannels);
        }
        public Result setMusicChannelVolume   (int channel, float volume)
        {
            return FMOD_Sound_SetMusicChannelVolume(rawPtr, channel, volume);
        }
        public Result getMusicChannelVolume   (int channel, out float volume)
        {
            return FMOD_Sound_GetMusicChannelVolume(rawPtr, channel, out volume);
        }
        public Result setMusicSpeed(float speed)
        {
            return FMOD_Sound_SetMusicSpeed(rawPtr, speed);
        }
        public Result getMusicSpeed(out float speed)
        {
            return FMOD_Sound_GetMusicSpeed(rawPtr, out speed);
        }

        // Userdata set/get.
        public Result setUserData             (IntPtr userdata)
        {
            return FMOD_Sound_SetUserData(rawPtr, userdata);
        }
        public Result getUserData             (out IntPtr userdata)
        {
            return FMOD_Sound_GetUserData(rawPtr, out userdata);
        }


        #region importfunctions
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_Release                 (IntPtr sound);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_GetSystemObject         (IntPtr sound, out IntPtr system);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_Lock                   (IntPtr sound, uint offset, uint length, out IntPtr ptr1, out IntPtr ptr2, out uint len1, out uint len2);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_Unlock                  (IntPtr sound, IntPtr ptr1,  IntPtr ptr2, uint len1, uint len2);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_SetDefaults             (IntPtr sound, float frequency, int priority);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_GetDefaults             (IntPtr sound, out float frequency, out int priority);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_Set3DMinMaxDistance     (IntPtr sound, float min, float max);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_Get3DMinMaxDistance     (IntPtr sound, out float min, out float max);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_Set3DConeSettings       (IntPtr sound, float insideconeangle, float outsideconeangle, float outsidevolume);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_Get3DConeSettings       (IntPtr sound, out float insideconeangle, out float outsideconeangle, out float outsidevolume);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_Set3DCustomRolloff      (IntPtr sound, ref Vector points, int numpoints);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_Get3DCustomRolloff      (IntPtr sound, out IntPtr points, out int numpoints);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_GetSubSound             (IntPtr sound, int index, out IntPtr subsound);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_GetSubSoundParent       (IntPtr sound, out IntPtr parentsound);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_GetName                 (IntPtr sound, IntPtr name, int namelen);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_GetLength               (IntPtr sound, out uint length, TimeUnit lengthtype);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_GetFormat               (IntPtr sound, out SoundType type, out SoundFormat format, out int channels, out int bits);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_GetNumSubSounds         (IntPtr sound, out int numsubsounds);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_GetNumTags              (IntPtr sound, out int numtags, out int numtagsupdated);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_GetTag                  (IntPtr sound, string name, int index, out Tag tag);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_GetOpenState            (IntPtr sound, out OpenState openstate, out uint percentbuffered, out bool starving, out bool diskbusy);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_ReadData                (IntPtr sound, IntPtr buffer, uint lenbytes, out uint read);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_SeekData                (IntPtr sound, uint pcm);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_SetSoundGroup           (IntPtr sound, IntPtr soundgroup);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_GetSoundGroup           (IntPtr sound, out IntPtr soundgroup);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_GetNumSyncPoints        (IntPtr sound, out int numsyncpoints);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_GetSyncPoint            (IntPtr sound, int index, out IntPtr point);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_GetSyncPointInfo        (IntPtr sound, IntPtr point, IntPtr name, int namelen, out uint offset, TimeUnit offsettype);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_AddSyncPoint            (IntPtr sound, uint offset, TimeUnit offsettype, string name, out IntPtr point);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_DeleteSyncPoint         (IntPtr sound, IntPtr point);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_SetMode                 (IntPtr sound, Mode mode);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_GetMode                 (IntPtr sound, out Mode mode);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_SetLoopCount            (IntPtr sound, int loopcount);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_GetLoopCount            (IntPtr sound, out int loopcount);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_SetLoopPoints           (IntPtr sound, uint loopstart, TimeUnit loopstarttype, uint loopend, TimeUnit loopendtype);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_GetLoopPoints           (IntPtr sound, out uint loopstart, TimeUnit loopstarttype, out uint loopend, TimeUnit loopendtype);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_GetMusicNumChannels     (IntPtr sound, out int numchannels);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_SetMusicChannelVolume   (IntPtr sound, int channel, float volume);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_GetMusicChannelVolume   (IntPtr sound, int channel, out float volume);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_SetMusicSpeed           (IntPtr sound, float speed);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_GetMusicSpeed           (IntPtr sound, out float speed);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Sound_SetUserData             (IntPtr sound, IntPtr userdata);
        [DllImport(FMODVersion.DLL)]
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
