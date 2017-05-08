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
            'Channel' API
        */
    public class Channel : ChannelControl
    {
        // Channel specific control functionality.
        public Result setFrequency          (float frequency)
        {
            return FMOD_Channel_SetFrequency(getRaw(), frequency);
        }
        public Result getFrequency          (out float frequency)
        {
            return FMOD_Channel_GetFrequency(getRaw(), out frequency);
        }
        public Result setPriority           (int priority)
        {
            return FMOD_Channel_SetPriority(getRaw(), priority);
        }
        public Result getPriority           (out int priority)
        {
            return FMOD_Channel_GetPriority(getRaw(), out priority);
        }
        public Result setPosition           (uint position, TimeUnit postype)
        {
            return FMOD_Channel_SetPosition(getRaw(), position, postype);
        }
        public Result getPosition           (out uint position, TimeUnit postype)
        {
            return FMOD_Channel_GetPosition(getRaw(), out position, postype);
        }
        public Result setChannelGroup       (ChannelGroup channelgroup)
        {
            return FMOD_Channel_SetChannelGroup(getRaw(), channelgroup.getRaw());
        }
        public Result getChannelGroup       (out ChannelGroup channelgroup)
        {
            channelgroup = null;

            IntPtr channelgroupraw;
            Result result = FMOD_Channel_GetChannelGroup(getRaw(), out channelgroupraw);
            channelgroup = new ChannelGroup(channelgroupraw);

            return result;
        }
        public Result setLoopCount(int loopcount)
        {
            return FMOD_Channel_SetLoopCount(getRaw(), loopcount);
        }
        public Result getLoopCount(out int loopcount)
        {
            return FMOD_Channel_GetLoopCount(getRaw(), out loopcount);
        }
        public Result setLoopPoints(uint loopstart, TimeUnit loopstarttype, uint loopend, TimeUnit loopendtype)
        {
            return FMOD_Channel_SetLoopPoints(getRaw(), loopstart, loopstarttype, loopend, loopendtype);
        }
        public Result getLoopPoints(out uint loopstart, TimeUnit loopstarttype, out uint loopend, TimeUnit loopendtype)
        {
            return FMOD_Channel_GetLoopPoints(getRaw(), out loopstart, loopstarttype, out loopend, loopendtype);
        }

        // Information only functions.
        public Result isVirtual             (out bool isvirtual)
        {
            return FMOD_Channel_IsVirtual(getRaw(), out isvirtual);
        }
        public Result getCurrentSound       (out Sound sound)
        {
            sound = null;

            IntPtr soundraw;
            Result result = FMOD_Channel_GetCurrentSound(getRaw(), out soundraw);
            sound = new Sound(soundraw);

            return result;
        }
        public Result getIndex              (out int index)
        {
            return FMOD_Channel_GetIndex(getRaw(), out index);
        }

        #region importfunctions

        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Channel_SetFrequency          (IntPtr channel, float frequency);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Channel_GetFrequency          (IntPtr channel, out float frequency);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Channel_SetPriority           (IntPtr channel, int priority);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Channel_GetPriority           (IntPtr channel, out int priority);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Channel_SetChannelGroup       (IntPtr channel, IntPtr channelgroup);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Channel_GetChannelGroup       (IntPtr channel, out IntPtr channelgroup);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Channel_IsVirtual             (IntPtr channel, out bool isvirtual);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Channel_GetCurrentSound       (IntPtr channel, out IntPtr sound);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Channel_GetIndex              (IntPtr channel, out int index);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Channel_SetPosition           (IntPtr channel, uint position, TimeUnit postype);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Channel_GetPosition           (IntPtr channel, out uint position, TimeUnit postype);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Channel_SetMode               (IntPtr channel, Mode mode);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Channel_GetMode               (IntPtr channel, out Mode mode);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Channel_SetLoopCount          (IntPtr channel, int loopcount);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Channel_GetLoopCount          (IntPtr channel, out int loopcount);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Channel_SetLoopPoints         (IntPtr channel, uint  loopstart, TimeUnit loopstarttype, uint  loopend, TimeUnit loopendtype);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Channel_GetLoopPoints         (IntPtr channel, out uint loopstart, TimeUnit loopstarttype, out uint loopend, TimeUnit loopendtype);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Channel_SetUserData           (IntPtr channel, IntPtr userdata);
        [DllImport(FMODVersion.DLL)]
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
