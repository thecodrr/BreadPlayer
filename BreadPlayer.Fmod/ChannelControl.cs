/* ========================================================================================== */
/*                                                                                            */
/* BreadPlayer.Fmod Studio - C# Wrapper . Copyright (c), Firelight Technologies Pty, Ltd. 2004-2016.      */
/*                                                                                            */
/* ========================================================================================== */

using BreadPlayer.Fmod.Enums;
using BreadPlayer.Fmod.Structs;
using System;
using System.Runtime.InteropServices;
using static BreadPlayer.Fmod.Callbacks;

namespace BreadPlayer.Fmod
{
    /*
            'ChannelControl' API
        */
    public class ChannelControl : HandleBase
    {
        public Result getSystemObject(out System system)
        {
            system = null;

            IntPtr systemraw;
            Result result = FMOD_ChannelGroup_GetSystemObject(rawPtr, out systemraw);
            system = new System(systemraw);

            return result;
        }

        // General control functionality for Channels and ChannelGroups.
        public Result Stop()
        {
            return FMOD_ChannelGroup_Stop(rawPtr);
        }
        public Result SetPaused(bool paused)
        {
            return FMOD_ChannelGroup_SetPaused(rawPtr, paused);
        }
        public Result GetPaused(out bool paused)
        {
            return FMOD_ChannelGroup_GetPaused(rawPtr, out paused);
        }
        public float Volume
        {
            get
            {
                FMOD_ChannelGroup_GetVolume(rawPtr, out float volume);
                return volume;
            }
            set
            {
                FMOD_ChannelGroup_SetVolume(rawPtr, value);
            }
        }
       
        public Result SetVolumeRamp(bool ramp)
        {
            return FMOD_ChannelGroup_SetVolumeRamp(rawPtr, ramp);
        }
        public Result GetVolumeRamp(out bool ramp)
        {
            return FMOD_ChannelGroup_GetVolumeRamp(rawPtr, out ramp);
        }
        public Result getAudibility(out float audibility)
        {
            return FMOD_ChannelGroup_GetAudibility(rawPtr, out audibility);
        }
        public Result setPitch(float pitch)
        {
            return FMOD_ChannelGroup_SetPitch(rawPtr, pitch);
        }
        public Result getPitch(out float pitch)
        {
            return FMOD_ChannelGroup_GetPitch(rawPtr, out pitch);
        }
        public Result setMute(bool mute)
        {
            return FMOD_ChannelGroup_SetMute(rawPtr, mute);
        }
        public Result getMute(out bool mute)
        {
            return FMOD_ChannelGroup_GetMute(rawPtr, out mute);
        }
        public Result setReverbProperties(int instance, float wet)
        {
            return FMOD_ChannelGroup_SetReverbProperties(rawPtr, instance, wet);
        }
        public Result getReverbProperties(int instance, out float wet)
        {
            return FMOD_ChannelGroup_GetReverbProperties(rawPtr, instance, out wet);
        }
        public Result setLowPassGain(float gain)
        {
            return FMOD_ChannelGroup_SetLowPassGain(rawPtr, gain);
        }
        public Result getLowPassGain(out float gain)
        {
            return FMOD_ChannelGroup_GetLowPassGain(rawPtr, out gain);
        }
        public Result setMode(Mode mode)
        {
            return FMOD_ChannelGroup_SetMode(rawPtr, mode);
        }
        public Result getMode(out Mode mode)
        {
            return FMOD_ChannelGroup_GetMode(rawPtr, out mode);
        }
        public Result setCallback(CHANNEL_CALLBACK callback)
        {
            return FMOD_ChannelGroup_SetCallback(rawPtr, callback);
        }
        public Result isPlaying(out bool isplaying)
        {
            return FMOD_ChannelGroup_IsPlaying(rawPtr, out isplaying);
        }

        // Panning and level adjustment.
        public Result setPan(float pan)
        {
            return FMOD_ChannelGroup_SetPan(rawPtr, pan);
        }
        public Result setMixLevelsOutput(float frontleft, float frontright, float center, float lfe, float surroundleft, float surroundright, float backleft, float backright)
        {
            return FMOD_ChannelGroup_SetMixLevelsOutput(rawPtr, frontleft, frontright, center, lfe,
                surroundleft, surroundright, backleft, backright);
        }
        public Result setMixLevelsInput(float[] levels, int numlevels)
        {
            return FMOD_ChannelGroup_SetMixLevelsInput(rawPtr, levels, numlevels);
        }
        public Result setMixMatrix(float[] matrix, int outchannels, int inchannels, int inchannel_hop)
        {
            return FMOD_ChannelGroup_SetMixMatrix(rawPtr, matrix, outchannels, inchannels, inchannel_hop);
        }
        public Result getMixMatrix(float[] matrix, out int outchannels, out int inchannels, int inchannel_hop)
        {
            return FMOD_ChannelGroup_GetMixMatrix(rawPtr, matrix, out outchannels, out inchannels, inchannel_hop);
        }

        // Clock based functionality.
        public Result getDSPClock(out ulong dspclock, out ulong parentclock)
        {
            return FMOD_ChannelGroup_GetDSPClock(rawPtr, out dspclock, out parentclock);
        }
        public Result setDelay(ulong dspclock_start, ulong dspclock_end, bool stopchannels)
        {
            return FMOD_ChannelGroup_SetDelay(rawPtr, dspclock_start, dspclock_end, stopchannels);
        }
        public Result getDelay(out ulong dspclock_start, out ulong dspclock_end, out bool stopchannels)
        {
            return FMOD_ChannelGroup_GetDelay(rawPtr, out dspclock_start, out dspclock_end, out stopchannels);
        }
        public Result addFadePoint(ulong dspclock, float volume)
        {
            return FMOD_ChannelGroup_AddFadePoint(rawPtr, dspclock, volume);
        }
        public Result setFadePointRamp(ulong dspclock, float volume)
        {
            return FMOD_ChannelGroup_SetFadePointRamp(rawPtr, dspclock, volume);
        }
        public Result removeFadePoints(ulong dspclock_start, ulong dspclock_end)
        {
            return FMOD_ChannelGroup_RemoveFadePoints(rawPtr, dspclock_start, dspclock_end);
        }
        public Result getFadePoints(ref uint numpoints, ulong[] point_dspclock, float[] point_volume)
        {
            return FMOD_ChannelGroup_GetFadePoints(rawPtr, ref numpoints, point_dspclock, point_volume);
        }

        // DSP effects.
        public Result getDSP(int index, out DSP dsp)
        {
            dsp = null;

            IntPtr dspraw;
            Result result = FMOD_ChannelGroup_GetDSP(rawPtr, index, out dspraw);
            dsp = new DSP(dspraw);

            return result;
        }
        public Result addDSP(int index, DSP dsp)
        {
            return FMOD_ChannelGroup_AddDSP(rawPtr, index, dsp.getRaw());
        }
        public Result removeDSP(DSP dsp)
        {
            return FMOD_ChannelGroup_RemoveDSP(rawPtr, dsp.getRaw());
        }
        public Result getNumDSPs(out int numdsps)
        {
            return FMOD_ChannelGroup_GetNumDSPs(rawPtr, out numdsps);
        }
        public Result setDSPIndex(DSP dsp, int index)
        {
            return FMOD_ChannelGroup_SetDSPIndex(rawPtr, dsp.getRaw(), index);
        }
        public Result getDSPIndex(DSP dsp, out int index)
        {
            return FMOD_ChannelGroup_GetDSPIndex(rawPtr, dsp.getRaw(), out index);
        }
        public Result overridePanDSP(DSP pan)
        {
            return FMOD_ChannelGroup_OverridePanDSP(rawPtr, pan.getRaw());
        }

        // 3D functionality.
        public Result set3DAttributes(ref Vector pos, ref Vector vel, ref Vector alt_pan_pos)
        {
            return FMOD_ChannelGroup_Set3DAttributes(rawPtr, ref pos, ref vel, ref alt_pan_pos);
        }
        public Result get3DAttributes(out Vector pos, out Vector vel, out Vector alt_pan_pos)
        {
            return FMOD_ChannelGroup_Get3DAttributes(rawPtr, out pos, out vel, out alt_pan_pos);
        }
        public Result set3DMinMaxDistance(float mindistance, float maxdistance)
        {
            return FMOD_ChannelGroup_Set3DMinMaxDistance(rawPtr, mindistance, maxdistance);
        }
        public Result get3DMinMaxDistance(out float mindistance, out float maxdistance)
        {
            return FMOD_ChannelGroup_Get3DMinMaxDistance(rawPtr, out mindistance, out maxdistance);
        }
        public Result set3DConeSettings(float insideconeangle, float outsideconeangle, float outsidevolume)
        {
            return FMOD_ChannelGroup_Set3DConeSettings(rawPtr, insideconeangle, outsideconeangle, outsidevolume);
        }
        public Result get3DConeSettings(out float insideconeangle, out float outsideconeangle, out float outsidevolume)
        {
            return FMOD_ChannelGroup_Get3DConeSettings(rawPtr, out insideconeangle, out outsideconeangle, out outsidevolume);
        }
        public Result set3DConeOrientation(ref Vector orientation)
        {
            return FMOD_ChannelGroup_Set3DConeOrientation(rawPtr, ref orientation);
        }
        public Result get3DConeOrientation(out Vector orientation)
        {
            return FMOD_ChannelGroup_Get3DConeOrientation(rawPtr, out orientation);
        }
        public Result set3DCustomRolloff(ref Vector points, int numpoints)
        {
            return FMOD_ChannelGroup_Set3DCustomRolloff(rawPtr, ref points, numpoints);
        }
        public Result get3DCustomRolloff(out IntPtr points, out int numpoints)
        {
            return FMOD_ChannelGroup_Get3DCustomRolloff(rawPtr, out points, out numpoints);
        }
        public Result set3DOcclusion(float directocclusion, float reverbocclusion)
        {
            return FMOD_ChannelGroup_Set3DOcclusion(rawPtr, directocclusion, reverbocclusion);
        }
        public Result get3DOcclusion(out float directocclusion, out float reverbocclusion)
        {
            return FMOD_ChannelGroup_Get3DOcclusion(rawPtr, out directocclusion, out reverbocclusion);
        }
        public Result set3DSpread(float angle)
        {
            return FMOD_ChannelGroup_Set3DSpread(rawPtr, angle);
        }
        public Result get3DSpread(out float angle)
        {
            return FMOD_ChannelGroup_Get3DSpread(rawPtr, out angle);
        }
        public Result set3DLevel(float level)
        {
            return FMOD_ChannelGroup_Set3DLevel(rawPtr, level);
        }
        public Result get3DLevel(out float level)
        {
            return FMOD_ChannelGroup_Get3DLevel(rawPtr, out level);
        }
        public Result set3DDopplerLevel(float level)
        {
            return FMOD_ChannelGroup_Set3DDopplerLevel(rawPtr, level);
        }
        public Result get3DDopplerLevel(out float level)
        {
            return FMOD_ChannelGroup_Get3DDopplerLevel(rawPtr, out level);
        }
        public Result set3DDistanceFilter(bool custom, float customLevel, float centerFreq)
        {
            return FMOD_ChannelGroup_Set3DDistanceFilter(rawPtr, custom, customLevel, centerFreq);
        }
        public Result get3DDistanceFilter(out bool custom, out float customLevel, out float centerFreq)
        {
            return FMOD_ChannelGroup_Get3DDistanceFilter(rawPtr, out custom, out customLevel, out centerFreq);
        }

        // Userdata set/get.
        public Result setUserData(IntPtr userdata)
        {
            return FMOD_ChannelGroup_SetUserData(rawPtr, userdata);
        }
        public Result getUserData(out IntPtr userdata)
        {
            return FMOD_ChannelGroup_GetUserData(rawPtr, out userdata);
        }

        #region importfunctions

        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_Stop(IntPtr channelgroup);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_SetPaused(IntPtr channelgroup, bool paused);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_GetPaused(IntPtr channelgroup, out bool paused);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_GetVolume(IntPtr channelgroup, out float volume);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_SetVolumeRamp(IntPtr channelgroup, bool ramp);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_GetVolumeRamp(IntPtr channelgroup, out bool ramp);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_GetAudibility(IntPtr channelgroup, out float audibility);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_SetPitch(IntPtr channelgroup, float pitch);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_GetPitch(IntPtr channelgroup, out float pitch);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_SetMute(IntPtr channelgroup, bool mute);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_GetMute(IntPtr channelgroup, out bool mute);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_SetReverbProperties(IntPtr channelgroup, int instance, float wet);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_GetReverbProperties(IntPtr channelgroup, int instance, out float wet);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_SetLowPassGain(IntPtr channelgroup, float gain);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_GetLowPassGain(IntPtr channelgroup, out float gain);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_SetMode(IntPtr channelgroup, Mode mode);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_GetMode(IntPtr channelgroup, out Mode mode);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_SetCallback(IntPtr channelgroup, CHANNEL_CALLBACK callback);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_IsPlaying(IntPtr channelgroup, out bool isplaying);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_SetPan(IntPtr channelgroup, float pan);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_SetMixLevelsOutput(IntPtr channelgroup, float frontleft, float frontright, float center, float lfe, float surroundleft, float surroundright, float backleft, float backright);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_SetMixLevelsInput(IntPtr channelgroup, float[] levels, int numlevels);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_SetMixMatrix(IntPtr channelgroup, float[] matrix, int outchannels, int inchannels, int inchannel_hop);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_GetMixMatrix(IntPtr channelgroup, float[] matrix, out int outchannels, out int inchannels, int inchannel_hop);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_GetDSPClock(IntPtr channelgroup, out ulong dspclock, out ulong parentclock);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_SetDelay(IntPtr channelgroup, ulong dspclock_start, ulong dspclock_end, bool stopchannels);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_GetDelay(IntPtr channelgroup, out ulong dspclock_start, out ulong dspclock_end, out bool stopchannels);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_AddFadePoint(IntPtr channelgroup, ulong dspclock, float volume);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_SetFadePointRamp(IntPtr channelgroup, ulong dspclock, float volume);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_RemoveFadePoints(IntPtr channelgroup, ulong dspclock_start, ulong dspclock_end);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_GetFadePoints(IntPtr channelgroup, ref uint numpoints, ulong[] point_dspclock, float[] point_volume);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_Set3DAttributes(IntPtr channelgroup, ref Vector pos, ref Vector vel, ref Vector alt_pan_pos);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_Get3DAttributes(IntPtr channelgroup, out Vector pos, out Vector vel, out Vector alt_pan_pos);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_Set3DMinMaxDistance(IntPtr channelgroup, float mindistance, float maxdistance);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_Get3DMinMaxDistance(IntPtr channelgroup, out float mindistance, out float maxdistance);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_Set3DConeSettings(IntPtr channelgroup, float insideconeangle, float outsideconeangle, float outsidevolume);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_Get3DConeSettings(IntPtr channelgroup, out float insideconeangle, out float outsideconeangle, out float outsidevolume);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_Set3DConeOrientation(IntPtr channelgroup, ref Vector orientation);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_Get3DConeOrientation(IntPtr channelgroup, out Vector orientation);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_Set3DCustomRolloff(IntPtr channelgroup, ref Vector points, int numpoints);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_Get3DCustomRolloff(IntPtr channelgroup, out IntPtr points, out int numpoints);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_Set3DOcclusion(IntPtr channelgroup, float directocclusion, float reverbocclusion);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_Get3DOcclusion(IntPtr channelgroup, out float directocclusion, out float reverbocclusion);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_Set3DSpread(IntPtr channelgroup, float angle);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_Get3DSpread(IntPtr channelgroup, out float angle);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_Set3DLevel(IntPtr channelgroup, float level);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_Get3DLevel(IntPtr channelgroup, out float level);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_Set3DDopplerLevel(IntPtr channelgroup, float level);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_Get3DDopplerLevel(IntPtr channelgroup, out float level);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_Set3DDistanceFilter(IntPtr channelgroup, bool custom, float customLevel, float centerFreq);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_Get3DDistanceFilter(IntPtr channelgroup, out bool custom, out float customLevel, out float centerFreq);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_GetSystemObject(IntPtr channelgroup, out IntPtr system);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_SetVolume(IntPtr channelgroup, float volume);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_GetDSP(IntPtr channelgroup, int index, out IntPtr dsp);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_AddDSP(IntPtr channelgroup, int index, IntPtr dsp);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_RemoveDSP(IntPtr channelgroup, IntPtr dsp);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_GetNumDSPs(IntPtr channelgroup, out int numdsps);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_SetDSPIndex(IntPtr channelgroup, IntPtr dsp, int index);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_GetDSPIndex(IntPtr channelgroup, IntPtr dsp, out int index);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_OverridePanDSP(IntPtr channelgroup, IntPtr pan);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_SetUserData(IntPtr channelgroup, IntPtr userdata);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_GetUserData(IntPtr channelgroup, out IntPtr userdata);

        #endregion

        #region wrapperinternal

        protected ChannelControl(IntPtr raw)
            : base(raw)
        {
        }

        #endregion
    }
}
