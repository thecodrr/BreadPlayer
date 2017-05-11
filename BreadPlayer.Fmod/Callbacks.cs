using System;
using BreadPlayer.Fmod.Enums;
using BreadPlayer.Fmod.Structs;

namespace BreadPlayer.Fmod
{
    public class Callbacks
    {  
        /*
        BreadPlayer.Fmod Callbacks
        */
        public delegate Result AsyncreadinfoDoneCallback(IntPtr info, Result result);

        public delegate Result DebugCallback(DebugFlags flags, string file, int line, string func, string message);

        public delegate Result SystemCallback(IntPtr systemraw, SystemCallbackType type, IntPtr commanddata1, IntPtr commanddata2, IntPtr userdata);

        public delegate Result ChannelCallback(IntPtr channelraw, ChannelControlType controltype, ChannelControlCallbackType type, IntPtr commanddata1, IntPtr commanddata2);

        public delegate Result SoundNonblockcallback(IntPtr soundraw, Result result);
        public delegate Result SoundPcmreadcallback(IntPtr soundraw, IntPtr data, uint datalen);
        public delegate Result SoundPcmsetposcallback(IntPtr soundraw, int subsound, uint position, TimeUnit postype);

        public delegate Result FileOpencallback(StringWrapper name, ref uint filesize, ref IntPtr handle, IntPtr userdata);
        public delegate Result FileClosecallback(IntPtr handle, IntPtr userdata);
        public delegate Result FileReadcallback(IntPtr handle, IntPtr buffer, uint sizebytes, ref uint bytesread, IntPtr userdata);
        public delegate Result FileSeekcallback(IntPtr handle, uint pos, IntPtr userdata);
        public delegate Result FileAsyncreadcallback(IntPtr handle, IntPtr info, IntPtr userdata);
        public delegate Result FileAsynccancelcallback(IntPtr handle, IntPtr userdata);

        public delegate IntPtr MemoryAllocCallback(uint size, MemoryType type, StringWrapper sourcestr);
        public delegate IntPtr MemoryReallocCallback(IntPtr ptr, uint size, MemoryType type, StringWrapper sourcestr);
        public delegate void MemoryFreeCallback(IntPtr ptr, MemoryType type, StringWrapper sourcestr);

        public delegate float Cb_3DRolloffcallback(IntPtr channelraw, float distance);

    }
}
