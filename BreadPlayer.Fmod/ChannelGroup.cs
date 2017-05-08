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
            'ChannelGroup' API
        */
    public class ChannelGroup : ChannelControl
    {
        public Result release                ()
        {
            Result result = FMOD_ChannelGroup_Release(getRaw());
            if (result == Result.OK)
            {
                rawPtr = IntPtr.Zero;
            }
            return result;
        }

        // Nested channel groups.
        public Result addGroup               (ChannelGroup group, bool propagatedspclock, out DSPConnection connection)
        {
			connection = null;
			
			IntPtr connectionRaw;
            Result result = FMOD_ChannelGroup_AddGroup(getRaw(), group.getRaw(), propagatedspclock, out connectionRaw);
			connection = new DSPConnection(connectionRaw);
			
			return result;
        }
        public Result getNumGroups           (out int numgroups)
        {
            return FMOD_ChannelGroup_GetNumGroups(getRaw(), out numgroups);
        }
        public Result getGroup               (int index, out ChannelGroup group)
        {
            group = null;

            IntPtr groupraw;
            Result result = FMOD_ChannelGroup_GetGroup(getRaw(), index, out groupraw);
            group = new ChannelGroup(groupraw);

            return result;
        }
        public Result getParentGroup         (out ChannelGroup group)
        {
            group = null;

            IntPtr groupraw;
            Result result = FMOD_ChannelGroup_GetParentGroup(getRaw(), out groupraw);
            group = new ChannelGroup(groupraw);

            return result;
        }

        // Information only functions.
        public Result getName                (StringBuilder name, int namelen)
        {
            IntPtr stringMem = Marshal.AllocHGlobal(name.Capacity);

            Result result = FMOD_ChannelGroup_GetName(getRaw(), stringMem, namelen);

            StringMarshalHelper.NativeToBuilder(name, stringMem);
            Marshal.FreeHGlobal(stringMem);

            return result;
        }
        public Result getNumChannels         (out int numchannels)
        {
            return FMOD_ChannelGroup_GetNumChannels(getRaw(), out numchannels);
        }
        public Result getChannel             (int index, out Channel channel)
        {
            channel = null;

            IntPtr channelraw;
            Result result = FMOD_ChannelGroup_GetChannel(getRaw(), index, out channelraw);
            channel = new Channel(channelraw);

            return result;
        }

        #region importfunctions
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_Release          (IntPtr channelgroup);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_AddGroup         (IntPtr channelgroup, IntPtr group, bool propagatedspclock, out IntPtr connection);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_GetNumGroups     (IntPtr channelgroup, out int numgroups);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_GetGroup         (IntPtr channelgroup, int index, out IntPtr group);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_GetParentGroup   (IntPtr channelgroup, out IntPtr group);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_GetName          (IntPtr channelgroup, IntPtr name, int namelen);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_GetNumChannels   (IntPtr channelgroup, out int numchannels);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_ChannelGroup_GetChannel       (IntPtr channelgroup, int index, out IntPtr channel);
        #endregion

        #region wrapperinternal

        public ChannelGroup(IntPtr raw)
            : base(raw)
        {
        }

        #endregion
    }
}
