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
            'Channel' API
        */
    public class Channel : ChannelControl
    {
        // Channel specific control functionality.
        public Result SetFrequency          (float frequency)
        {
            return FMOD_Channel_SetFrequency(GetRaw(), frequency);
        }
        public Result GetFrequency          (out float frequency)
        {
            return FMOD_Channel_GetFrequency(GetRaw(), out frequency);
        }
        public Result SetPriority           (int priority)
        {
            return FMOD_Channel_SetPriority(GetRaw(), priority);
        }
        public Result GetPriority           (out int priority)
        {
            return FMOD_Channel_GetPriority(GetRaw(), out priority);
        }
        public Result SetPosition           (uint position, TimeUnit postype)
        {
            return FMOD_Channel_SetPosition(GetRaw(), position, postype);
        }
        public Result GetPosition           (out uint position, TimeUnit postype)
        {
            return FMOD_Channel_GetPosition(GetRaw(), out position, postype);
        }
        public Result SetChannelGroup       (ChannelGroup channelgroup)
        {
            return FMOD_Channel_SetChannelGroup(GetRaw(), channelgroup.GetRaw());
        }
        public Result GetChannelGroup       (out ChannelGroup channelgroup)
        {
            channelgroup = null;

            IntPtr channelgroupraw;
            Result result = FMOD_Channel_GetChannelGroup(GetRaw(), out channelgroupraw);
            channelgroup = new ChannelGroup(channelgroupraw);

            return result;
        }
        public Result SetLoopCount(int loopcount)
        {
            return FMOD_Channel_SetLoopCount(GetRaw(), loopcount);
        }
        public Result GetLoopCount(out int loopcount)
        {
            return FMOD_Channel_GetLoopCount(GetRaw(), out loopcount);
        }
        public Result SetLoopPoints(uint loopstart, TimeUnit loopstarttype, uint loopend, TimeUnit loopendtype)
        {
            return FMOD_Channel_SetLoopPoints(GetRaw(), loopstart, loopstarttype, loopend, loopendtype);
        }
        public Result GetLoopPoints(out uint loopstart, TimeUnit loopstarttype, out uint loopend, TimeUnit loopendtype)
        {
            return FMOD_Channel_GetLoopPoints(GetRaw(), out loopstart, loopstarttype, out loopend, loopendtype);
        }

        // Information only functions.
        public Result IsVirtual             (out bool isvirtual)
        {
            return FMOD_Channel_IsVirtual(GetRaw(), out isvirtual);
        }
        public Result GetCurrentSound       (out Sound sound)
        {
            sound = null;

            IntPtr soundraw;
            Result result = FMOD_Channel_GetCurrentSound(GetRaw(), out soundraw);
            sound = new Sound(soundraw);

            return result;
        }
        public Result GetIndex              (out int index)
        {
            return FMOD_Channel_GetIndex(GetRaw(), out index);
        }

        #region importfunctions

        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Channel_SetFrequency          (IntPtr channel, float frequency);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Channel_GetFrequency          (IntPtr channel, out float frequency);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Channel_SetPriority           (IntPtr channel, int priority);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Channel_GetPriority           (IntPtr channel, out int priority);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Channel_SetChannelGroup       (IntPtr channel, IntPtr channelgroup);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Channel_GetChannelGroup       (IntPtr channel, out IntPtr channelgroup);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Channel_IsVirtual             (IntPtr channel, out bool isvirtual);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Channel_GetCurrentSound       (IntPtr channel, out IntPtr sound);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Channel_GetIndex              (IntPtr channel, out int index);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Channel_SetPosition           (IntPtr channel, uint position, TimeUnit postype);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Channel_GetPosition           (IntPtr channel, out uint position, TimeUnit postype);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Channel_SetMode               (IntPtr channel, Mode mode);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Channel_GetMode               (IntPtr channel, out Mode mode);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Channel_SetLoopCount          (IntPtr channel, int loopcount);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Channel_GetLoopCount          (IntPtr channel, out int loopcount);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Channel_SetLoopPoints         (IntPtr channel, uint  loopstart, TimeUnit loopstarttype, uint  loopend, TimeUnit loopendtype);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Channel_GetLoopPoints         (IntPtr channel, out uint loopstart, TimeUnit loopstarttype, out uint loopend, TimeUnit loopendtype);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Channel_SetUserData           (IntPtr channel, IntPtr userdata);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Channel_GetUserData           (IntPtr channel, out IntPtr userdata);
        #endregion

        #region wrapperinternal

        public Channel(IntPtr raw)
            : base(raw)
        {
        }

        #endregion
    }
}
