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
        public Result GetSystemObject(out FmodSystem system)
        {
            system = null;

            IntPtr systemraw;
            Result result = FMOD_ChannelGroup_GetSystemObject(RawPtr, out systemraw);
            system = new FmodSystem(systemraw);

            return result;
        }

        // General control functionality for Channels and ChannelGroups.
        public Result Stop()
        {
            return FMOD_ChannelGroup_Stop(RawPtr);
        }
        public Result SetPaused(bool paused)
        {
            return FMOD_ChannelGroup_SetPaused(RawPtr, paused);
        }
        public Result GetPaused(out bool paused)
        {
            return FMOD_ChannelGroup_GetPaused(RawPtr, out paused);
        }
        public float Volume
        {
            get
            {
                float volume = 0;
                FMOD_ChannelGroup_GetVolume(RawPtr, out volume);
                return volume;
            }
            set
            {
                if(RawPtr != null)
                    FMOD_ChannelGroup_SetVolume(RawPtr, value);
            }
        }
       
        public Result SetVolumeRamp(bool ramp)
        {
            return FMOD_ChannelGroup_SetVolumeRamp(RawPtr, ramp);
        }
        public Result GetVolumeRamp(out bool ramp)
        {
            return FMOD_ChannelGroup_GetVolumeRamp(RawPtr, out ramp);
        }
        public Result GetAudibility(out float audibility)
        {
            return FMOD_ChannelGroup_GetAudibility(RawPtr, out audibility);
        }
        public Result SetPitch(float pitch)
        {
            return FMOD_ChannelGroup_SetPitch(RawPtr, pitch);
        }
        public Result GetPitch(out float pitch)
        {
            return FMOD_ChannelGroup_GetPitch(RawPtr, out pitch);
        }
        public Result SetMute(bool mute)
        {
            return FMOD_ChannelGroup_SetMute(RawPtr, mute);
        }
        public Result GetMute(out bool mute)
        {
            return FMOD_ChannelGroup_GetMute(RawPtr, out mute);
        }
        public Result SetReverbProperties(int instance, float wet)
        {
            return FMOD_ChannelGroup_SetReverbProperties(RawPtr, instance, wet);
        }
        public Result GetReverbProperties(int instance, out float wet)
        {
            return FMOD_ChannelGroup_GetReverbProperties(RawPtr, instance, out wet);
        }
        public Result SetLowPassGain(float gain)
        {
            return FMOD_ChannelGroup_SetLowPassGain(RawPtr, gain);
        }
        public Result GetLowPassGain(out float gain)
        {
            return FMOD_ChannelGroup_GetLowPassGain(RawPtr, out gain);
        }
        public Result SetMode(Mode mode)
        {
            return FMOD_ChannelGroup_SetMode(RawPtr, mode);
        }
        public Result GetMode(out Mode mode)
        {
            return FMOD_ChannelGroup_GetMode(RawPtr, out mode);
        }
        public Result SetCallback(ChannelCallback callback)
        {
            return FMOD_ChannelGroup_SetCallback(RawPtr, callback);
        }
        public Result IsPlaying(out bool isplaying)
        {
            return FMOD_ChannelGroup_IsPlaying(RawPtr, out isplaying);
        }

        // Panning and level adjustment.
        public Result SetPan(float pan)
        {
            return FMOD_ChannelGroup_SetPan(RawPtr, pan);
        }
        public Result SetMixLevelsOutput(float frontleft, float frontright, float center, float lfe, float surroundleft, float surroundright, float backleft, float backright)
        {
            return FMOD_ChannelGroup_SetMixLevelsOutput(RawPtr, frontleft, frontright, center, lfe,
                surroundleft, surroundright, backleft, backright);
        }
        public Result SetMixLevelsInput(float[] levels, int numlevels)
        {
            return FMOD_ChannelGroup_SetMixLevelsInput(RawPtr, levels, numlevels);
        }
        public Result SetMixMatrix(float[] matrix, int outchannels, int inchannels, int inchannelHop)
        {
            return FMOD_ChannelGroup_SetMixMatrix(RawPtr, matrix, outchannels, inchannels, inchannelHop);
        }
        public Result GetMixMatrix(float[] matrix, out int outchannels, out int inchannels, int inchannelHop)
        {
            return FMOD_ChannelGroup_GetMixMatrix(RawPtr, matrix, out outchannels, out inchannels, inchannelHop);
        }

        // Clock based functionality.
        public Result GetDspClock(out ulong dspclock, out ulong parentclock)
        {
            return FMOD_ChannelGroup_GetDSPClock(RawPtr, out dspclock, out parentclock);
        }
        public Result SetDelay(ulong dspclockStart, ulong dspclockEnd, bool stopchannels)
        {
            return FMOD_ChannelGroup_SetDelay(RawPtr, dspclockStart, dspclockEnd, stopchannels);
        }
        public Result GetDelay(out ulong dspclockStart, out ulong dspclockEnd, out bool stopchannels)
        {
            return FMOD_ChannelGroup_GetDelay(RawPtr, out dspclockStart, out dspclockEnd, out stopchannels);
        }
        public Result AddFadePoint(ulong dspclock, float volume)
        {
            return FMOD_ChannelGroup_AddFadePoint(RawPtr, dspclock, volume);
        }
        public Result SetFadePointRamp(ulong dspclock, float volume)
        {
            return FMOD_ChannelGroup_SetFadePointRamp(RawPtr, dspclock, volume);
        }
        public Result RemoveFadePoints(ulong dspclockStart, ulong dspclockEnd)
        {
            return FMOD_ChannelGroup_RemoveFadePoints(RawPtr, dspclockStart, dspclockEnd);
        }
        public Result GetFadePoints(ref uint numpoints, ulong[] pointDspclock, float[] pointVolume)
        {
            return FMOD_ChannelGroup_GetFadePoints(RawPtr, ref numpoints, pointDspclock, pointVolume);
        }

        // DSP effects.
        public Result GetDsp(int index, out Dsp dsp)
        {
            dsp = null;

            IntPtr dspraw;
            Result result = FMOD_ChannelGroup_GetDSP(RawPtr, index, out dspraw);
            dsp = new Dsp(dspraw);

            return result;
        }
        public Result AddDsp(int index, Dsp dsp)
        {
            return FMOD_ChannelGroup_AddDSP(RawPtr, index, dsp.GetRaw());
        }
        public Result RemoveDsp(Dsp dsp)
        {
            return FMOD_ChannelGroup_RemoveDSP(RawPtr, dsp.GetRaw());
        }
        public Result GetNumDsPs(out int numdsps)
        {
            return FMOD_ChannelGroup_GetNumDSPs(RawPtr, out numdsps);
        }
        public Result SetDspIndex(Dsp dsp, int index)
        {
            return FMOD_ChannelGroup_SetDSPIndex(RawPtr, dsp.GetRaw(), index);
        }
        public Result GetDspIndex(Dsp dsp, out int index)
        {
            return FMOD_ChannelGroup_GetDSPIndex(RawPtr, dsp.GetRaw(), out index);
        }
        public Result OverridePanDsp(Dsp pan)
        {
            return FMOD_ChannelGroup_OverridePanDSP(RawPtr, pan.GetRaw());
        }

        // 3D functionality.
        public Result Set3DAttributes(ref Vector pos, ref Vector vel, ref Vector altPanPos)
        {
            return FMOD_ChannelGroup_Set3DAttributes(RawPtr, ref pos, ref vel, ref altPanPos);
        }
        public Result Get3DAttributes(out Vector pos, out Vector vel, out Vector altPanPos)
        {
            return FMOD_ChannelGroup_Get3DAttributes(RawPtr, out pos, out vel, out altPanPos);
        }
        public Result Set3DMinMaxDistance(float mindistance, float maxdistance)
        {
            return FMOD_ChannelGroup_Set3DMinMaxDistance(RawPtr, mindistance, maxdistance);
        }
        public Result Get3DMinMaxDistance(out float mindistance, out float maxdistance)
        {
            return FMOD_ChannelGroup_Get3DMinMaxDistance(RawPtr, out mindistance, out maxdistance);
        }
        public Result Set3DConeSettings(float insideconeangle, float outsideconeangle, float outsidevolume)
        {
            return FMOD_ChannelGroup_Set3DConeSettings(RawPtr, insideconeangle, outsideconeangle, outsidevolume);
        }
        public Result Get3DConeSettings(out float insideconeangle, out float outsideconeangle, out float outsidevolume)
        {
            return FMOD_ChannelGroup_Get3DConeSettings(RawPtr, out insideconeangle, out outsideconeangle, out outsidevolume);
        }
        public Result Set3DConeOrientation(ref Vector orientation)
        {
            return FMOD_ChannelGroup_Set3DConeOrientation(RawPtr, ref orientation);
        }
        public Result Get3DConeOrientation(out Vector orientation)
        {
            return FMOD_ChannelGroup_Get3DConeOrientation(RawPtr, out orientation);
        }
        public Result Set3DCustomRolloff(ref Vector points, int numpoints)
        {
            return FMOD_ChannelGroup_Set3DCustomRolloff(RawPtr, ref points, numpoints);
        }
        public Result Get3DCustomRolloff(out IntPtr points, out int numpoints)
        {
            return FMOD_ChannelGroup_Get3DCustomRolloff(RawPtr, out points, out numpoints);
        }
        public Result Set3DOcclusion(float directocclusion, float reverbocclusion)
        {
            return FMOD_ChannelGroup_Set3DOcclusion(RawPtr, directocclusion, reverbocclusion);
        }
        public Result Get3DOcclusion(out float directocclusion, out float reverbocclusion)
        {
            return FMOD_ChannelGroup_Get3DOcclusion(RawPtr, out directocclusion, out reverbocclusion);
        }
        public Result Set3DSpread(float angle)
        {
            return FMOD_ChannelGroup_Set3DSpread(RawPtr, angle);
        }
        public Result Get3DSpread(out float angle)
        {
            return FMOD_ChannelGroup_Get3DSpread(RawPtr, out angle);
        }
        public Result Set3DLevel(float level)
        {
            return FMOD_ChannelGroup_Set3DLevel(RawPtr, level);
        }
        public Result Get3DLevel(out float level)
        {
            return FMOD_ChannelGroup_Get3DLevel(RawPtr, out level);
        }
        public Result Set3DDopplerLevel(float level)
        {
            return FMOD_ChannelGroup_Set3DDopplerLevel(RawPtr, level);
        }
        public Result Get3DDopplerLevel(out float level)
        {
            return FMOD_ChannelGroup_Get3DDopplerLevel(RawPtr, out level);
        }
        public Result Set3DDistanceFilter(bool custom, float customLevel, float centerFreq)
        {
            return FMOD_ChannelGroup_Set3DDistanceFilter(RawPtr, custom, customLevel, centerFreq);
        }
        public Result Get3DDistanceFilter(out bool custom, out float customLevel, out float centerFreq)
        {
            return FMOD_ChannelGroup_Get3DDistanceFilter(RawPtr, out custom, out customLevel, out centerFreq);
        }

        // Userdata set/get.
        public Result SetUserData(IntPtr userdata)
        {
            return FMOD_ChannelGroup_SetUserData(RawPtr, userdata);
        }
        public Result GetUserData(out IntPtr userdata)
        {
            return FMOD_ChannelGroup_GetUserData(RawPtr, out userdata);
        }

        #region importfunctions

        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_Stop(IntPtr channelgroup);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_SetPaused(IntPtr channelgroup, bool paused);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_GetPaused(IntPtr channelgroup, out bool paused);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_GetVolume(IntPtr channelgroup, out float volume);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_SetVolumeRamp(IntPtr channelgroup, bool ramp);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_GetVolumeRamp(IntPtr channelgroup, out bool ramp);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_GetAudibility(IntPtr channelgroup, out float audibility);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_SetPitch(IntPtr channelgroup, float pitch);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_GetPitch(IntPtr channelgroup, out float pitch);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_SetMute(IntPtr channelgroup, bool mute);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_GetMute(IntPtr channelgroup, out bool mute);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_SetReverbProperties(IntPtr channelgroup, int instance, float wet);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_GetReverbProperties(IntPtr channelgroup, int instance, out float wet);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_SetLowPassGain(IntPtr channelgroup, float gain);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_GetLowPassGain(IntPtr channelgroup, out float gain);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_SetMode(IntPtr channelgroup, Mode mode);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_GetMode(IntPtr channelgroup, out Mode mode);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_SetCallback(IntPtr channelgroup, ChannelCallback callback);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_IsPlaying(IntPtr channelgroup, out bool isplaying);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_SetPan(IntPtr channelgroup, float pan);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_SetMixLevelsOutput(IntPtr channelgroup, float frontleft, float frontright, float center, float lfe, float surroundleft, float surroundright, float backleft, float backright);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_SetMixLevelsInput(IntPtr channelgroup, float[] levels, int numlevels);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_SetMixMatrix(IntPtr channelgroup, float[] matrix, int outchannels, int inchannels, int inchannelHop);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_GetMixMatrix(IntPtr channelgroup, float[] matrix, out int outchannels, out int inchannels, int inchannelHop);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_GetDSPClock(IntPtr channelgroup, out ulong dspclock, out ulong parentclock);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_SetDelay(IntPtr channelgroup, ulong dspclockStart, ulong dspclockEnd, bool stopchannels);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_GetDelay(IntPtr channelgroup, out ulong dspclockStart, out ulong dspclockEnd, out bool stopchannels);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_AddFadePoint(IntPtr channelgroup, ulong dspclock, float volume);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_SetFadePointRamp(IntPtr channelgroup, ulong dspclock, float volume);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_RemoveFadePoints(IntPtr channelgroup, ulong dspclockStart, ulong dspclockEnd);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_GetFadePoints(IntPtr channelgroup, ref uint numpoints, ulong[] pointDspclock, float[] pointVolume);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_Set3DAttributes(IntPtr channelgroup, ref Vector pos, ref Vector vel, ref Vector altPanPos);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_Get3DAttributes(IntPtr channelgroup, out Vector pos, out Vector vel, out Vector altPanPos);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_Set3DMinMaxDistance(IntPtr channelgroup, float mindistance, float maxdistance);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_Get3DMinMaxDistance(IntPtr channelgroup, out float mindistance, out float maxdistance);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_Set3DConeSettings(IntPtr channelgroup, float insideconeangle, float outsideconeangle, float outsidevolume);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_Get3DConeSettings(IntPtr channelgroup, out float insideconeangle, out float outsideconeangle, out float outsidevolume);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_Set3DConeOrientation(IntPtr channelgroup, ref Vector orientation);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_Get3DConeOrientation(IntPtr channelgroup, out Vector orientation);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_Set3DCustomRolloff(IntPtr channelgroup, ref Vector points, int numpoints);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_Get3DCustomRolloff(IntPtr channelgroup, out IntPtr points, out int numpoints);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_Set3DOcclusion(IntPtr channelgroup, float directocclusion, float reverbocclusion);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_Get3DOcclusion(IntPtr channelgroup, out float directocclusion, out float reverbocclusion);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_Set3DSpread(IntPtr channelgroup, float angle);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_Get3DSpread(IntPtr channelgroup, out float angle);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_Set3DLevel(IntPtr channelgroup, float level);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_Get3DLevel(IntPtr channelgroup, out float level);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_Set3DDopplerLevel(IntPtr channelgroup, float level);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_Get3DDopplerLevel(IntPtr channelgroup, out float level);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_Set3DDistanceFilter(IntPtr channelgroup, bool custom, float customLevel, float centerFreq);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_Get3DDistanceFilter(IntPtr channelgroup, out bool custom, out float customLevel, out float centerFreq);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_GetSystemObject(IntPtr channelgroup, out IntPtr system);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_SetVolume(IntPtr channelgroup, float volume);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_GetDSP(IntPtr channelgroup, int index, out IntPtr dsp);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_AddDSP(IntPtr channelgroup, int index, IntPtr dsp);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_RemoveDSP(IntPtr channelgroup, IntPtr dsp);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_GetNumDSPs(IntPtr channelgroup, out int numdsps);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_SetDSPIndex(IntPtr channelgroup, IntPtr dsp, int index);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_GetDSPIndex(IntPtr channelgroup, IntPtr dsp, out int index);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_OverridePanDSP(IntPtr channelgroup, IntPtr pan);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_SetUserData(IntPtr channelgroup, IntPtr userdata);
        [DllImport(FmodVersion.Dll)]
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
