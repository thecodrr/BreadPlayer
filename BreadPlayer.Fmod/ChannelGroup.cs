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
        public Result Release                ()
        {
            Result result = FMOD_ChannelGroup_Release(GetRaw());
            if (result == Result.Ok)
            {
                RawPtr = IntPtr.Zero;
            }
            return result;
        }

        // Nested channel groups.
        public Result AddGroup               (ChannelGroup group, bool propagatedspclock, out DspConnection connection)
        {
			connection = null;
			
			IntPtr connectionRaw;
            Result result = FMOD_ChannelGroup_AddGroup(GetRaw(), group.GetRaw(), propagatedspclock, out connectionRaw);
			connection = new DspConnection(connectionRaw);
			
			return result;
        }
        public Result GetNumGroups           (out int numgroups)
        {
            return FMOD_ChannelGroup_GetNumGroups(GetRaw(), out numgroups);
        }
        public Result GetGroup               (int index, out ChannelGroup group)
        {
            group = null;

            IntPtr groupraw;
            Result result = FMOD_ChannelGroup_GetGroup(GetRaw(), index, out groupraw);
            group = new ChannelGroup(groupraw);

            return result;
        }
        public Result GetParentGroup         (out ChannelGroup group)
        {
            group = null;

            IntPtr groupraw;
            Result result = FMOD_ChannelGroup_GetParentGroup(GetRaw(), out groupraw);
            group = new ChannelGroup(groupraw);

            return result;
        }

        // Information only functions.
        public Result GetName                (StringBuilder name, int namelen)
        {
            IntPtr stringMem = Marshal.AllocHGlobal(name.Capacity);

            Result result = FMOD_ChannelGroup_GetName(GetRaw(), stringMem, namelen);

            StringMarshalHelper.NativeToBuilder(name, stringMem);
            Marshal.FreeHGlobal(stringMem);

            return result;
        }
        public Result GetNumChannels         (out int numchannels)
        {
            return FMOD_ChannelGroup_GetNumChannels(GetRaw(), out numchannels);
        }
        public Result GetChannel             (int index, out Channel channel)
        {
            channel = null;

            IntPtr channelraw;
            Result result = FMOD_ChannelGroup_GetChannel(GetRaw(), index, out channelraw);
            channel = new Channel(channelraw);

            return result;
        }

        #region importfunctions
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_Release          (IntPtr channelgroup);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_AddGroup         (IntPtr channelgroup, IntPtr group, bool propagatedspclock, out IntPtr connection);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_GetNumGroups     (IntPtr channelgroup, out int numgroups);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_GetGroup         (IntPtr channelgroup, int index, out IntPtr group);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_GetParentGroup   (IntPtr channelgroup, out IntPtr group);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_GetName          (IntPtr channelgroup, IntPtr name, int namelen);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_ChannelGroup_GetNumChannels   (IntPtr channelgroup, out int numchannels);
        [DllImport(FmodVersion.Dll)]
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
