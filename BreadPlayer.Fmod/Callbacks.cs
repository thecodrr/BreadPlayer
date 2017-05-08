using BreadPlayer.Fmod.Enums;
using BreadPlayer.Fmod.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace BreadPlayer.Fmod
{
    public class Callbacks
    {  
        /*
        BreadPlayer.Fmod Callbacks
        */
        public delegate Result ASYNCREADINFO_DONE_CALLBACK(IntPtr info, Result result);

        public delegate Result DEBUG_CALLBACK(DebugFlags flags, string file, int line, string func, string message);

        public delegate Result SYSTEM_CALLBACK(IntPtr systemraw, SystemCallbackType type, IntPtr commanddata1, IntPtr commanddata2, IntPtr userdata);

        public delegate Result CHANNEL_CALLBACK(IntPtr channelraw, ChannelControlType controltype, ChannelControlCallbackType type, IntPtr commanddata1, IntPtr commanddata2);

        public delegate Result SOUND_NONBLOCKCALLBACK(IntPtr soundraw, Result result);
        public delegate Result SOUND_PCMREADCALLBACK(IntPtr soundraw, IntPtr data, uint datalen);
        public delegate Result SOUND_PCMSETPOSCALLBACK(IntPtr soundraw, int subsound, uint position, TimeUnit postype);

        public delegate Result FILE_OPENCALLBACK(StringWrapper name, ref uint filesize, ref IntPtr handle, IntPtr userdata);
        public delegate Result FILE_CLOSECALLBACK(IntPtr handle, IntPtr userdata);
        public delegate Result FILE_READCALLBACK(IntPtr handle, IntPtr buffer, uint sizebytes, ref uint bytesread, IntPtr userdata);
        public delegate Result FILE_SEEKCALLBACK(IntPtr handle, uint pos, IntPtr userdata);
        public delegate Result FILE_ASYNCREADCALLBACK(IntPtr handle, IntPtr info, IntPtr userdata);
        public delegate Result FILE_ASYNCCANCELCALLBACK(IntPtr handle, IntPtr userdata);

        public delegate IntPtr MEMORY_ALLOC_CALLBACK(uint size, MemoryType type, StringWrapper sourcestr);
        public delegate IntPtr MEMORY_REALLOC_CALLBACK(IntPtr ptr, uint size, MemoryType type, StringWrapper sourcestr);
        public delegate void MEMORY_FREE_CALLBACK(IntPtr ptr, MemoryType type, StringWrapper sourcestr);

        public delegate float CB_3D_ROLLOFFCALLBACK(IntPtr channelraw, float distance);

    }
}
