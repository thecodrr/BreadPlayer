using System;
using System.Collections.Generic;
using System.Text;

namespace BreadPlayer.Fmod.Enums
{
    /*
    [ENUM]
    [
        [DESCRIPTION]
        error codes.  Returned from every function.

        [REMARKS]

        [SEE_ALSO]
    ]
    */
    public enum Result : int
    {
        OK,                        /* No errors. */
        ERR_BADCOMMAND,            /* Tried to call a function on a data type that does not allow this type of functionality (ie calling Sound::lock on a streaming sound). */
        ERR_CHANNEL_ALLOC,         /* Error trying to allocate a channel. */
        ERR_CHANNEL_STOLEN,        /* The specified channel has been reused to play another sound. */
        ERR_DMA,                   /* DMA Failure.  See debug output for more information. */
        ERR_DSP_CONNECTION,        /* DSP connection error.  Connection possibly caused a cyclic dependency or connected dsps with incompatible buffer counts. */
        ERR_DSP_DONTPROCESS,       /* DSP return code from a DSP process query callback.  Tells mixer not to call the process callback and therefore not consume CPU.  Use this to optimize the DSP graph. */
        ERR_DSP_FORMAT,            /* DSP Format error.  A DSP unit may have attempted to connect to this network with the wrong format, or a matrix may have been set with the wrong size if the target unit has a specified channel map. */
        ERR_DSP_INUSE,             /* DSP is already in the mixer's DSP network. It must be removed before being reinserted or released. */
        ERR_DSP_NOTFOUND,          /* DSP connection error.  Couldn't find the DSP unit specified. */
        ERR_DSP_RESERVED,          /* DSP operation error.  Cannot perform operation on this DSP as it is reserved by the system. */
        ERR_DSP_SILENCE,           /* DSP return code from a DSP process query callback.  Tells mixer silence would be produced from read, so go idle and not consume CPU.  Use this to optimize the DSP graph. */
        ERR_DSP_TYPE,              /* DSP operation cannot be performed on a DSP of this type. */
        ERR_FILE_BAD,              /* Error loading file. */
        ERR_FILE_COULDNOTSEEK,     /* Couldn't perform seek operation.  This is a limitation of the medium (ie netstreams) or the file format. */
        ERR_FILE_DISKEJECTED,      /* Media was ejected while reading. */
        ERR_FILE_EOF,              /* End of file unexpectedly reached while trying to read essential data (truncated?). */
        ERR_FILE_ENDOFDATA,        /* End of current chunk reached while trying to read data. */
        ERR_FILE_NOTFOUND,         /* File not found. */
        ERR_FORMAT,                /* Unsupported file or audio format. */
        ERR_HEADER_MISMATCH,       /* There is a version mismatch between the BreadPlayer.Fmod header and either the BreadPlayer.Fmod Studio library or the BreadPlayer.Fmod Low Level library. */
        ERR_HTTP,                  /* A HTTP error occurred. This is a catch-all for HTTP errors not listed elsewhere. */
        ERR_HTTP_ACCESS,           /* The specified resource requires authentication or is forbidden. */
        ERR_HTTP_PROXY_AUTH,       /* Proxy authentication is required to access the specified resource. */
        ERR_HTTP_SERVER_ERROR,     /* A HTTP server error occurred. */
        ERR_HTTP_TIMEOUT,          /* The HTTP request timed out. */
        ERR_INITIALIZATION,        /* BreadPlayer.Fmod was not initialized correctly to support this function. */
        ERR_INITIALIZED,           /* Cannot call this command after System::init. */
        ERR_INTERNAL,              /* An error occurred that wasn't supposed to.  Contact support. */
        ERR_INVALID_FLOAT,         /* Value passed in was a NaN, Inf or denormalized float. */
        ERR_INVALID_HANDLE,        /* An invalid object handle was used. */
        ERR_INVALID_PARAM,         /* An invalid parameter was passed to this function. */
        ERR_INVALID_POSITION,      /* An invalid seek position was passed to this function. */
        ERR_INVALID_SPEAKER,       /* An invalid speaker was passed to this function based on the current speaker mode. */
        ERR_INVALID_SYNCPOINT,     /* The syncpoint did not come from this sound handle. */
        ERR_INVALID_THREAD,        /* Tried to call a function on a thread that is not supported. */
        ERR_INVALID_VECTOR,        /* The vectors passed in are not unit length, or perpendicular. */
        ERR_MAXAUDIBLE,            /* Reached maximum audible playback count for this sound's soundgroup. */
        ERR_MEMORY,                /* Not enough memory or resources. */
        ERR_MEMORY_CANTPOINT,      /* Can't use FMOD_OPENMEMORY_POINT on non PCM source data, or non mp3/xma/adpcm data if FMOD_CREATECOMPRESSEDSAMPLE was used. */
        ERR_NEEDS3D,               /* Tried to call a command on a 2d sound when the command was meant for 3d sound. */
        ERR_NEEDSHARDWARE,         /* Tried to use a feature that requires hardware support. */
        ERR_NET_CONNECT,           /* Couldn't connect to the specified host. */
        ERR_NET_SOCKET_ERROR,      /* A socket error occurred.  This is a catch-all for socket-related errors not listed elsewhere. */
        ERR_NET_URL,               /* The specified URL couldn't be resolved. */
        ERR_NET_WOULD_BLOCK,       /* Operation on a non-blocking socket could not complete immediately. */
        ERR_NOTREADY,              /* Operation could not be performed because specified sound/DSP connection is not ready. */
        ERR_OUTPUT_ALLOCATED,      /* Error initializing output device, but more specifically, the output device is already in use and cannot be reused. */
        ERR_OUTPUT_CREATEBUFFER,   /* Error creating hardware sound buffer. */
        ERR_OUTPUT_DRIVERCALL,     /* A call to a standard soundcard driver failed, which could possibly mean a bug in the driver or resources were missing or exhausted. */
        ERR_OUTPUT_FORMAT,         /* Soundcard does not support the specified format. */
        ERR_OUTPUT_INIT,           /* Error initializing output device. */
        ERR_OUTPUT_NODRIVERS,      /* The output device has no drivers installed.  If pre-init, FMOD_OUTPUT_NOSOUND is selected as the output mode.  If post-init, the function just fails. */
        ERR_PLUGIN,                /* An unspecified error has been returned from a plugin. */
        ERR_PLUGIN_MISSING,        /* A requested output, dsp unit type or codec was not available. */
        ERR_PLUGIN_RESOURCE,       /* A resource that the plugin requires cannot be found. (ie the DLS file for MIDI playback) */
        ERR_PLUGIN_VERSION,        /* A plugin was built with an unsupported SDK version. */
        ERR_RECORD,                /* An error occurred trying to initialize the recording device. */
        ERR_REVERB_CHANNELGROUP,   /* Reverb properties cannot be set on this channel because a parent channelgroup owns the reverb connection. */
        ERR_REVERB_INSTANCE,       /* Specified instance in FMOD_REVERB_PROPERTIES couldn't be set. Most likely because it is an invalid instance number or the reverb doesn't exist. */
        ERR_SUBSOUNDS,             /* The error occurred because the sound referenced contains subsounds when it shouldn't have, or it doesn't contain subsounds when it should have.  The operation may also not be able to be performed on a parent sound. */
        ERR_SUBSOUND_ALLOCATED,    /* This subsound is already being used by another sound, you cannot have more than one parent to a sound.  Null out the other parent's entry first. */
        ERR_SUBSOUND_CANTMOVE,     /* Shared subsounds cannot be replaced or moved from their parent stream, such as when the parent stream is an FSB file. */
        ERR_TAGNOTFOUND,           /* The specified tag could not be found or there are no tags. */
        ERR_TOOMANYCHANNELS,       /* The sound created exceeds the allowable input channel count.  This can be increased using the 'maxinputchannels' parameter in System::setSoftwareFormat. */
        ERR_TRUNCATED,             /* The retrieved string is too long to fit in the supplied buffer and has been truncated. */
        ERR_UNIMPLEMENTED,         /* Something in BreadPlayer.Fmod hasn't been implemented when it should be! contact support! */
        ERR_UNINITIALIZED,         /* This command failed because System::init or System::setDriver was not called. */
        ERR_UNSUPPORTED,           /* A command issued was not supported by this object.  Possibly a plugin without certain callbacks specified. */
        ERR_VERSION,               /* The version number of this file format is not supported. */
        ERR_EVENT_ALREADY_LOADED,  /* The specified bank has already been loaded. */
        ERR_EVENT_LIVEUPDATE_BUSY, /* The live update connection failed due to the game already being connected. */
        ERR_EVENT_LIVEUPDATE_MISMATCH, /* The live update connection failed due to the game data being out of sync with the tool. */
        ERR_EVENT_LIVEUPDATE_TIMEOUT, /* The live update connection timed out. */
        ERR_EVENT_NOTFOUND,        /* The requested event, bus or vca could not be found. */
        ERR_STUDIO_UNINITIALIZED,  /* The Studio::System object is not yet initialized. */
        ERR_STUDIO_NOT_LOADED,     /* The specified resource is not loaded, so it can't be unloaded. */
        ERR_INVALID_STRING,        /* An invalid string was passed to this function. */
        ERR_ALREADY_LOCKED,        /* The specified resource is already locked. */
        ERR_NOT_LOCKED,            /* The specified resource is not locked, so it can't be unlocked. */
        ERR_RECORD_DISCONNECTED,   /* The specified recording driver has been disconnected. */
        ERR_TOOMANYSAMPLES,        /* The length provided exceed the allowable limit. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Used to distinguish if a FMOD_CHANNELCONTROL parameter is actually a channel or a channelgroup.

        [REMARKS]
        Cast the FMOD_CHANNELCONTROL to an FMOD_CHANNEL/BreadPlayer.Fmod::Channel, or FMOD_CHANNELGROUP/BreadPlayer.Fmod::ChannelGroup if specific functionality is needed for either class.
        Otherwise use as FMOD_CHANNELCONTROL/BreadPlayer.Fmod::ChannelControl and use that API.

        [SEE_ALSO]
        Channel::setCallback
        ChannelGroup::setCallback
    ]
    */
    public enum ChannelControlType : int
    {
        CHANNEL,
        CHANNELGROUP
    }
    /*
[ENUM]
[
   [DESCRIPTION]
   These output types are used with System::setOutput / System::getOutput, to choose which output method to use.

   [REMARKS]
   To pass information to the driver when initializing fmod use the *extradriverdata* parameter in System::init for the following reasons.

   - FMOD_OUTPUTTYPE_WAVWRITER     - extradriverdata is a pointer to a char * file name that the wav writer will output to.
   - FMOD_OUTPUTTYPE_WAVWRITER_NRT - extradriverdata is a pointer to a char * file name that the wav writer will output to.
   - FMOD_OUTPUTTYPE_DSOUND        - extradriverdata is cast to a HWND type, so that BreadPlayer.Fmod can set the focus on the audio for a particular window.
   - FMOD_OUTPUTTYPE_PS3           - extradriverdata is a pointer to a FMOD_PS3_EXTRADRIVERDATA struct. This can be found in fmodps3.h.
   - FMOD_OUTPUTTYPE_XBOX360       - extradriverdata is a pointer to a FMOD_360_EXTRADRIVERDATA struct. This can be found in fmodxbox360.h.

   Currently these are the only BreadPlayer.Fmod drivers that take extra information.  Other unknown plugins may have different requirements.

   Note! If FMOD_OUTPUTTYPE_WAVWRITER_NRT or FMOD_OUTPUTTYPE_NOSOUND_NRT are used, and if the System::update function is being called
   very quickly (ie for a non realtime decode) it may be being called too quickly for the BreadPlayer.Fmod streamer thread to respond to.
   The result will be a skipping/stuttering output in the captured audio.

   To remedy this, disable the BreadPlayer.Fmod streamer thread, and use FMOD_INIT_STREAM_FROM_UPDATE to avoid skipping in the output stream,
   as it will lock the mixer and the streamer together in the same thread.

   [SEE_ALSO]
       System::setOutput
       System::getOutput
       System::setSoftwareFormat
       System::getSoftwareFormat
       System::init
       System::update
       FMOD_INITFLAGS
]
*/
    public enum OutputType : int
    {
        AUTODETECT,      /* Picks the best output mode for the platform. This is the default. */

        UNKNOWN,         /* All - 3rd party plugin, unknown. This is for use with System::getOutput only. */
        NOSOUND,         /* All - Perform all mixing but discard the final output. */
        WAVWRITER,       /* All - Writes output to a .wav file. */
        NOSOUND_NRT,     /* All - Non-realtime version of FMOD_OUTPUTTYPE_NOSOUND. User can drive mixer with System::update at whatever rate they want. */
        WAVWRITER_NRT,   /* All - Non-realtime version of FMOD_OUTPUTTYPE_WAVWRITER. User can drive mixer with System::update at whatever rate they want. */

        DSOUND,          /* Win                  - Direct Sound.                        (Default on Windows XP and below) */
        WINMM,           /* Win                  - Windows Multimedia. */
        WASAPI,          /* Win/WinStore/XboxOne - Windows Audio Session API.           (Default on Windows Vista and above, Xbox One and Windows Store Applications) */
        ASIO,            /* Win                  - Low latency ASIO 2.0. */
        PULSEAUDIO,      /* Linux                - Pulse Audio.                         (Default on Linux if available) */
        ALSA,            /* Linux                - Advanced Linux Sound Architecture.   (Default on Linux if PulseAudio isn't available) */
        COREAUDIO,       /* Mac/iOS              - Core Audio.                          (Default on Mac and iOS) */
        XAUDIO,          /* Xbox 360             - XAudio.                              (Default on Xbox 360) */
        PS3,             /* PS3                  - Audio Out.                           (Default on PS3) */
        AUDIOTRACK,      /* Android              - Java Audio Track.                    (Default on Android 2.2 and below) */
        OPENSL,          /* Android              - OpenSL ES.                           (Default on Android 2.3 and above) */
        WIIU,            /* Wii U                - AX.                                  (Default on Wii U) */
        AUDIOOUT,        /* PS4/PSVita           - Audio Out.                           (Default on PS4 and PS Vita) */
        AUDIO3D,         /* PS4                  - Audio3D. */
        ATMOS,           /* Win                  - Dolby Atmos (WASAPI). */

        MAX,             /* Maximum number of output types supported. */
    }

    /*
    [ENUM]
    [
        [DESCRIPTION]
        Specify the destination of log output when using the logging version of BreadPlayer.Fmod.

        [REMARKS]
        TTY destination can vary depending on platform, common examples include the
        Visual Studio / Xcode output window, stderr and LogCat.

        [SEE_ALSO]
        FMOD_Debug_Initialize
    ]
    */
    public enum DebugMode : int
    {
        TTY,        /* Default log location per platform, i.e. Visual Studio output window, stderr, LogCat, etc */
        FILE,       /* Write log to specified file path */
        CALLBACK,   /* Call specified callback with log information */
    }

    /*
    [DEFINE]
    [
        [NAME]
        FMOD_DEBUG_FLAGS

        [DESCRIPTION]
        Specify the requested information to be output when using the logging version of BreadPlayer.Fmod.

        [REMARKS]

        [SEE_ALSO]
        FMOD_Debug_Initialize
    ]
    */
    [Flags]
    public enum DebugFlags : uint
    {
        NONE = 0x00000000,   /* Disable all messages */
        ERROR = 0x00000001,   /* Enable only error messages. */
        WARNING = 0x00000002,   /* Enable warning and error messages. */
        LOG = 0x00000004,   /* Enable informational, warning and error messages (default). */

        TYPE_MEMORY = 0x00000100,   /* Verbose logging for memory operations, only use this if you are debugging a memory related issue. */
        TYPE_FILE = 0x00000200,   /* Verbose logging for file access, only use this if you are debugging a file related issue. */
        TYPE_CODEC = 0x00000400,   /* Verbose logging for codec initialization, only use this if you are debugging a codec related issue. */
        TYPE_TRACE = 0x00000800,   /* Verbose logging for internal errors, use this for tracking the origin of error codes. */

        DISPLAY_TIMESTAMPS = 0x00010000,   /* Display the time stamp of the log message in milliseconds. */
        DISPLAY_LINENUMBERS = 0x00020000,   /* Display the source code file and line number for where the message originated. */
        DISPLAY_THREAD = 0x00040000,   /* Display the thread ID of the calling function that generated the message. */
    }

    /*
    [DEFINE]
    [
        [NAME]
        FMOD_MEMORY_TYPE

        [DESCRIPTION]
        Bit fields for memory allocation type being passed into BreadPlayer.Fmod memory callbacks.

        [REMARKS]
        Remember this is a bitfield.  You may get more than 1 bit set (ie physical + persistent) so do not simply switch on the types!  You must check each bit individually or clear out the bits that you do not want within the callback.<br>
        Bits can be excluded if you want during Memory_Initialize so that you never get them.

        [SEE_ALSO]
        FMOD_MEMORY_ALLOC_CALLBACK
        FMOD_MEMORY_REALLOC_CALLBACK
        FMOD_MEMORY_FREE_CALLBACK
        Memory_Initialize
    ]
    */
    [Flags]
    public enum MemoryType : uint
    {
        NORMAL = 0x00000000,       /* Standard memory. */
        STREAM_FILE = 0x00000001,       /* Stream file buffer, size controllable with System::setStreamBufferSize. */
        STREAM_DECODE = 0x00000002,       /* Stream decode buffer, size controllable with FMOD_CREATESOUNDEXINFO::decodebuffersize. */
        SAMPLEDATA = 0x00000004,       /* Sample data buffer.  Raw audio data, usually PCM/MPEG/ADPCM/XMA data. */
        DSP_BUFFER = 0x00000008,       /* DSP memory block allocated when more than 1 output exists on a DSP node. */
        PLUGIN = 0x00000010,       /* Memory allocated by a third party plugin. */
        XBOX360_PHYSICAL = 0x00100000,       /* Requires XPhysicalAlloc / XPhysicalFree. */
        PERSISTENT = 0x00200000,       /* Persistent memory. Memory will be freed when System::release is called. */
        SECONDARY = 0x00400000,       /* Secondary memory. Allocation should be in secondary memory. For example RSX on the PS3. */
        ALL = 0xFFFFFFFF
    }

    /*
    [ENUM]
    [
        [DESCRIPTION]
        These are speaker types defined for use with the System::setSoftwareFormat command.

        [REMARKS]
        Note below the phrase 'sound channels' is used.  These are the subchannels inside a sound, they are not related and
        have nothing to do with the BreadPlayer.Fmod class "Channel".<br>
        For example a mono sound has 1 sound channel, a stereo sound has 2 sound channels, and an AC3 or 6 channel wav file have 6 "sound channels".<br>
        <br>
        FMOD_SPEAKERMODE_RAW<br>
        ---------------------<br>
        This mode is for output devices that are not specifically mono/stereo/quad/surround/5.1 or 7.1, but are multichannel.<br>
        Use System::setSoftwareFormat to specify the number of speakers you want to address, otherwise it will default to 2 (stereo).<br>
        Sound channels map to speakers sequentially, so a mono sound maps to output speaker 0, stereo sound maps to output speaker 0 & 1.<br>
        The user assumes knowledge of the speaker order.  FMOD_SPEAKER enumerations may not apply, so raw channel indices should be used.<br>
        Multichannel sounds map input channels to output channels 1:1. <br>
        Channel::setPan and Channel::setPanLevels do not work.<br>
        Speaker levels must be manually set with Channel::setPanMatrix.<br>
        <br>
        FMOD_SPEAKERMODE_MONO<br>
        ---------------------<br>
        This mode is for a 1 speaker arrangement.<br>
        Panning does not work in this speaker mode.<br>
        Mono, stereo and multichannel sounds have each sound channel played on the one speaker unity.<br>
        Mix behavior for multichannel sounds can be set with Channel::setPanMatrix.<br>
        Channel::setPanLevels does not work.<br>
        <br>
        FMOD_SPEAKERMODE_STEREO<br>
        -----------------------<br>
        This mode is for 2 speaker arrangements that have a left and right speaker.<br>
        <li>Mono sounds default to an even distribution between left and right.  They can be panned with Channel::setPan.<br>
        <li>Stereo sounds default to the middle, or full left in the left speaker and full right in the right speaker.
        <li>They can be cross faded with Channel::setPan.<br>
        <li>Multichannel sounds have each sound channel played on each speaker at unity.<br>
        <li>Mix behavior for multichannel sounds can be set with Channel::setPanMatrix.<br>
        <li>Channel::setPanLevels works but only front left and right parameters are used, the rest are ignored.<br>
        <br>
        FMOD_SPEAKERMODE_QUAD<br>
        ------------------------<br>
        This mode is for 4 speaker arrangements that have a front left, front right, surround left and a surround right speaker.<br>
        <li>Mono sounds default to an even distribution between front left and front right.  They can be panned with Channel::setPan.<br>
        <li>Stereo sounds default to the left sound channel played on the front left, and the right sound channel played on the front right.<br>
        <li>They can be cross faded with Channel::setPan.<br>
        <li>Multichannel sounds default to all of their sound channels being played on each speaker in order of input.<br>
        <li>Mix behavior for multichannel sounds can be set with Channel::setPanMatrix.<br>
        <li>Channel::setPanLevels works but rear left, rear right, center and lfe are ignored.<br>
        <br>
        FMOD_SPEAKERMODE_SURROUND<br>
        ------------------------<br>
        This mode is for 5 speaker arrangements that have a left/right/center/surround left/surround right.<br>
        <li>Mono sounds default to the center speaker.  They can be panned with Channel::setPan.<br>
        <li>Stereo sounds default to the left sound channel played on the front left, and the right sound channel played on the front right.
        <li>They can be cross faded with Channel::setPan.<br>
        <li>Multichannel sounds default to all of their sound channels being played on each speaker in order of input.
        <li>Mix behavior for multichannel sounds can be set with Channel::setPanMatrix.<br>
        <li>Channel::setPanLevels works but rear left / rear right are ignored.<br>
        <br>
        FMOD_SPEAKERMODE_5POINT1<br>
        ---------------------------------------------------------<br>
        This mode is for 5.1 speaker arrangements that have a left/right/center/surround left/surround right and a subwoofer speaker.<br>
        <li>Mono sounds default to the center speaker.  They can be panned with Channel::setPan.<br>
        <li>Stereo sounds default to the left sound channel played on the front left, and the right sound channel played on the front right.
        <li>They can be cross faded with Channel::setPan.<br>
        <li>Multichannel sounds default to all of their sound channels being played on each speaker in order of input.
        <li>Mix behavior for multichannel sounds can be set with Channel::setPanMatrix.<br>
        <li>Channel::setPanLevels works but rear left / rear right are ignored.<br>
        <br>
        FMOD_SPEAKERMODE_7POINT1<br>
        ------------------------<br>
        This mode is for 7.1 speaker arrangements that have a left/right/center/surround left/surround right/rear left/rear right
        and a subwoofer speaker.<br>
        <li>Mono sounds default to the center speaker.  They can be panned with Channel::setPan.<br>
        <li>Stereo sounds default to the left sound channel played on the front left, and the right sound channel played on the front right.
        <li>They can be cross faded with Channel::setPan.<br>
        <li>Multichannel sounds default to all of their sound channels being played on each speaker in order of input.
        <li>Mix behavior for multichannel sounds can be set with Channel::setPanMatrix.<br>
        <li>Channel::setPanLevels works and every parameter is used to set the balance of a sound in any speaker.<br>
        <br>

        [SEE_ALSO]
        System::setSoftwareFormat
        System::getSoftwareFormat
        DSP::setChannelFormat
    ]
    */
    public enum SpeakerMode : int
    {
        DEFAULT,          /* Default speaker mode based on operating system/output mode.  Windows = control panel setting, Xbox = 5.1, PS3 = 7.1 etc. */
        RAW,              /* There is no specific speakermode.  Sound channels are mapped in order of input to output.  Use System::setSoftwareFormat to specify speaker count. See remarks for more information. */
        MONO,             /* The speakers are monaural. */
        STEREO,           /* The speakers are stereo. */
        QUAD,             /* 4 speaker setup.  This includes front left, front right, surround left, surround right.  */
        SURROUND,         /* 5 speaker setup.  This includes front left, front right, center, surround left, surround right. */
        _5POINT1,         /* 5.1 speaker setup.  This includes front left, front right, center, surround left, surround right and an LFE speaker. */
        _7POINT1,         /* 7.1 speaker setup.  This includes front left, front right, center, surround left, surround right, back left, back right and an LFE speaker. */

        MAX,              /* Maximum number of speaker modes supported. */
    }

    /*
    [ENUM]
    [
        [DESCRIPTION]
        Assigns an enumeration for a speaker index.

        [REMARKS]

        [SEE_ALSO]
        System::setSpeakerPosition
        System::getSpeakerPosition
    ]
    */
    public enum Speaker : int
    {
        FRONT_LEFT,
        FRONT_RIGHT,
        FRONT_CENTER,
        LOW_FREQUENCY,
        SURROUND_LEFT,
        SURROUND_RIGHT,
        BACK_LEFT,
        BACK_RIGHT,

        MAX,               /* Maximum number of speaker types supported. */
    }

    /*
    [DEFINE]
    [
        [NAME]
        FMOD_CHANNELMASK

        [DESCRIPTION]
        These are bitfields to describe for a certain number of channels in a signal, which channels are being represented.<br>
        For example, a signal could be 1 channel, but contain the LFE channel only.<br>

        [REMARKS]
        FMOD_CHANNELMASK_BACK_CENTER is not represented as an output speaker in fmod - but it is encountered in input formats and is down or upmixed appropriately to the nearest speakers.<br>

        [SEE_ALSO]
        DSP::setChannelFormat
        DSP::getChannelFormat
        FMOD_SPEAKERMODE
    ]
    */
    [Flags]
    public enum ChannelMask : uint
    {
        FRONT_LEFT = 0x00000001,
        FRONT_RIGHT = 0x00000002,
        FRONT_CENTER = 0x00000004,
        LOW_FREQUENCY = 0x00000008,
        SURROUND_LEFT = 0x00000010,
        SURROUND_RIGHT = 0x00000020,
        BACK_LEFT = 0x00000040,
        BACK_RIGHT = 0x00000080,
        BACK_CENTER = 0x00000100,

        MONO = (FRONT_LEFT),
        STEREO = (FRONT_LEFT | FRONT_RIGHT),
        LRC = (FRONT_LEFT | FRONT_RIGHT | FRONT_CENTER),
        QUAD = (FRONT_LEFT | FRONT_RIGHT | SURROUND_LEFT | SURROUND_RIGHT),
        SURROUND = (FRONT_LEFT | FRONT_RIGHT | FRONT_CENTER | SURROUND_LEFT | SURROUND_RIGHT),
        _5POINT1 = (FRONT_LEFT | FRONT_RIGHT | FRONT_CENTER | LOW_FREQUENCY | SURROUND_LEFT | SURROUND_RIGHT),
        _5POINT1_REARS = (FRONT_LEFT | FRONT_RIGHT | FRONT_CENTER | LOW_FREQUENCY | BACK_LEFT | BACK_RIGHT),
        _7POINT0 = (FRONT_LEFT | FRONT_RIGHT | FRONT_CENTER | SURROUND_LEFT | SURROUND_RIGHT | BACK_LEFT | BACK_RIGHT),
        _7POINT1 = (FRONT_LEFT | FRONT_RIGHT | FRONT_CENTER | LOW_FREQUENCY | SURROUND_LEFT | SURROUND_RIGHT | BACK_LEFT | BACK_RIGHT)
    }

    /*
    [ENUM]
    [
        [DESCRIPTION]
        When creating a multichannel sound, BreadPlayer.Fmod will pan them to their default speaker locations, for example a 6 channel sound will default to one channel per 5.1 output speaker.<br>
        Another example is a stereo sound.  It will default to left = front left, right = front right.<br>
        <br>
        This is for sounds that are not 'default'.  For example you might have a sound that is 6 channels but actually made up of 3 stereo pairs, that should all be located in front left, front right only.

        [REMARKS]

        [SEE_ALSO]
        FMOD_CREATESOUNDEXINFO
    ]
    */
    public enum ChannelOrder : int
    {
        DEFAULT,              /* Left, Right, Center, LFE, Surround Left, Surround Right, Back Left, Back Right (see FMOD_SPEAKER enumeration)   */
        WAVEFORMAT,           /* Left, Right, Center, LFE, Back Left, Back Right, Surround Left, Surround Right (as per Microsoft .wav WAVEFORMAT structure master order) */
        PROTOOLS,             /* Left, Center, Right, Surround Left, Surround Right, LFE */
        ALLMONO,              /* Mono, Mono, Mono, Mono, Mono, Mono, ... (each channel all the way up to 32 channels are treated as if they were mono) */
        ALLSTEREO,            /* Left, Right, Left, Right, Left, Right, ... (each pair of channels is treated as stereo all the way up to 32 channels) */
        ALSA,                 /* Left, Right, Surround Left, Surround Right, Center, LFE (as per Linux ALSA channel order) */

        MAX,                  /* Maximum number of channel orderings supported. */
    }

    /*
    [ENUM]
    [
        [DESCRIPTION]
        These are plugin types defined for use with the System::getNumPlugins,
        System::getPluginInfo and System::unloadPlugin functions.

        [REMARKS]

        [SEE_ALSO]
        System::getNumPlugins
        System::getPluginInfo
        System::unloadPlugin
    ]
    */
    public enum PluginType : int
    {
        OUTPUT,          /* The plugin type is an output module.  BreadPlayer.Fmod mixed audio will play through one of these devices */
        CODEC,           /* The plugin type is a file format codec.  BreadPlayer.Fmod will use these codecs to load file formats for playback. */
        DSP,             /* The plugin type is a DSP unit.  BreadPlayer.Fmod will use these plugins as part of its DSP network to apply effects to output or generate sound in realtime. */

        MAX,             /* Maximum number of plugin types supported. */
    }
    /*
[DEFINE]
[
    [NAME]
    FMOD_INITFLAGS

    [DESCRIPTION]
    Initialization flags.  Use them with System::init in the *flags* parameter to change various behavior.

    [REMARKS]
    Use System::setAdvancedSettings to adjust settings for some of the features that are enabled by these flags.

    [SEE_ALSO]
    System::init
    System::update
    System::setAdvancedSettings
    Channel::set3DOcclusion
]
*/
    [Flags]
    public enum InitFlags : uint
    {
        NORMAL = 0x00000000, /* Initialize normally */
        STREAM_FROM_UPDATE = 0x00000001, /* No stream thread is created internally.  Streams are driven from System::update.  Mainly used with non-realtime outputs. */
        MIX_FROM_UPDATE = 0x00000002, /* Win/Wii/PS3/Xbox/Xbox 360 Only - BreadPlayer.Fmod Mixer thread is woken up to do a mix when System::update is called rather than waking periodically on its own timer. */
        _3D_RIGHTHANDED = 0x00000004, /* BreadPlayer.Fmod will treat +X as right, +Y as up and +Z as backwards (towards you). */
        CHANNEL_LOWPASS = 0x00000100, /* All FMOD_3D based voices will add a software lowpass filter effect into the DSP chain which is automatically used when Channel::set3DOcclusion is used or the geometry API.   This also causes sounds to sound duller when the sound goes behind the listener, as a fake HRTF style effect.  Use System::setAdvancedSettings to disable or adjust cutoff frequency for this feature. */
        CHANNEL_DISTANCEFILTER = 0x00000200, /* All FMOD_3D based voices will add a software lowpass and highpass filter effect into the DSP chain which will act as a distance-automated bandpass filter. Use System::setAdvancedSettings to adjust the center frequency. */
        PROFILE_ENABLE = 0x00010000, /* Enable TCP/IP based host which allows BreadPlayer.Fmod Designer or BreadPlayer.Fmod Profiler to connect to it, and view memory, CPU and the DSP network graph in real-time. */
        VOL0_BECOMES_VIRTUAL = 0x00020000, /* Any sounds that are 0 volume will go virtual and not be processed except for having their positions updated virtually.  Use System::setAdvancedSettings to adjust what volume besides zero to switch to virtual at. */
        GEOMETRY_USECLOSEST = 0x00040000, /* With the geometry engine, only process the closest polygon rather than accumulating all polygons the sound to listener line intersects. */
        PREFER_DOLBY_DOWNMIX = 0x00080000, /* When using FMOD_SPEAKERMODE_5POINT1 with a stereo output device, use the Dolby Pro Logic II downmix algorithm instead of the SRS Circle Surround algorithm. */
        THREAD_UNSAFE = 0x00100000, /* Disables thread safety for API calls. Only use this if BreadPlayer.Fmod low level is being called from a single thread, and if Studio API is not being used! */
        PROFILE_METER_ALL = 0x00200000  /* Slower, but adds level metering for every single DSP unit in the graph.  Use DSP::setMeteringEnabled to turn meters off individually. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        These definitions describe the type of song being played.

        [REMARKS]

        [SEE_ALSO]
        Sound::getFormat
    ]
    */
    public enum SoundType
    {
        UNKNOWN,         /* 3rd party / unknown plugin format. */
        AIFF,            /* AIFF. */
        ASF,             /* Microsoft Advanced Systems Format (ie WMA/ASF/WMV). */
        DLS,             /* Sound font / downloadable sound bank. */
        FLAC,            /* FLAC lossless codec. */
        FSB,             /* BreadPlayer.Fmod Sample Bank. */
        IT,              /* Impulse Tracker. */
        MIDI,            /* MIDI. extracodecdata is a pointer to an FMOD_MIDI_EXTRACODECDATA structure. */
        MOD,             /* Protracker / Fasttracker MOD. */
        MPEG,            /* MP2/MP3 MPEG. */
        OGGVORBIS,       /* Ogg vorbis. */
        PLAYLIST,        /* Information only from ASX/PLS/M3U/WAX playlists */
        RAW,             /* Raw PCM data. */
        S3M,             /* ScreamTracker 3. */
        USER,            /* User created sound. */
        WAV,             /* Microsoft WAV. */
        XM,              /* FastTracker 2 XM. */
        XMA,             /* Xbox360 XMA */
        AUDIOQUEUE,      /* iPhone hardware decoder, supports AAC, ALAC and MP3. extracodecdata is a pointer to an FMOD_AUDIOQUEUE_EXTRACODECDATA structure. */
        AT9,             /* PS4 / PSVita ATRAC 9 format */
        VORBIS,          /* Vorbis */
        MEDIA_FOUNDATION,/* Windows Store Application built in system codecs */
        MEDIACODEC,      /* Android MediaCodec */
        FADPCM,          /* BreadPlayer.Fmod Adaptive Differential Pulse Code Modulation */

        MAX,             /* Maximum number of sound types supported. */
    }

    /*
    [ENUM]
    [
        [DESCRIPTION]
        These definitions describe the native format of the hardware or software buffer that will be used.

        [REMARKS]
        This is the format the native hardware or software buffer will be or is created in.

        [SEE_ALSO]
        System::createSoundEx
        Sound::getFormat
    ]
    */
    public enum SoundFormat : int
    {
        NONE,       /* Unitialized / unknown */
        PCM8,       /* 8bit integer PCM data */
        PCM16,      /* 16bit integer PCM data  */
        PCM24,      /* 24bit integer PCM data  */
        PCM32,      /* 32bit integer PCM data  */
        PCMFLOAT,   /* 32bit floating point PCM data  */
        BITSTREAM,  /* Sound data is in its native compressed format. */

        MAX         /* Maximum number of sound formats supported. */
    }


    /*
    [DEFINE]
    [
        [NAME]
        FMOD_MODE

        [DESCRIPTION]
        Sound description bitfields, bitwise OR them together for loading and describing sounds.

        [REMARKS]
        By default a sound will open as a static sound that is decompressed fully into memory to PCM. (ie equivalent of FMOD_CREATESAMPLE)<br>
        To have a sound stream instead, use FMOD_CREATESTREAM, or use the wrapper function System::createStream.<br>
        Some opening modes (ie FMOD_OPENUSER, FMOD_OPENMEMORY, FMOD_OPENMEMORY_POINT, FMOD_OPENRAW) will need extra information.<br>
        This can be provided using the FMOD_CREATESOUNDEXINFO structure.
        <br>
        Specifying FMOD_OPENMEMORY_POINT will POINT to your memory rather allocating its own sound buffers and duplicating it internally.<br>
        <b><u>This means you cannot free the memory while BreadPlayer.Fmod is using it, until after Sound::release is called.</b></u>
        With FMOD_OPENMEMORY_POINT, for PCM formats, only WAV, FSB, and RAW are supported.  For compressed formats, only those formats supported by FMOD_CREATECOMPRESSEDSAMPLE are supported.<br>
        With FMOD_OPENMEMORY_POINT and FMOD_OPENRAW or PCM, if using them together, note that you must pad the data on each side by 16 bytes.  This is so fmod can modify the ends of the data for looping/interpolation/mixing purposes.  If a wav file, you will need to insert silence, and then reset loop points to stop the playback from playing that silence.<br>
        <br>
        <b>Xbox 360 memory</b> On Xbox 360 Specifying FMOD_OPENMEMORY_POINT to a virtual memory address will cause FMOD_ERR_INVALID_ADDRESS
        to be returned.  Use physical memory only for this functionality.<br>
        <br>
        FMOD_LOWMEM is used on a sound if you want to minimize the memory overhead, by having BreadPlayer.Fmod not allocate memory for certain
        features that are not likely to be used in a game environment.  These are :<br>
        1. Sound::getName functionality is removed.  256 bytes per sound is saved.<br>

        [SEE_ALSO]
        System::createSound
        System::createStream
        Sound::setMode
        Sound::getMode
        Channel::setMode
        Channel::getMode
        Sound::set3DCustomRolloff
        Channel::set3DCustomRolloff
        Sound::getOpenState
    ]
    */
    [Flags]
    public enum Mode : uint
    {
        DEFAULT = 0x00000000,  /* Default for all modes listed below. FMOD_LOOP_OFF, FMOD_2D, FMOD_3D_WORLDRELATIVE, FMOD_3D_INVERSEROLLOFF */
        LOOP_OFF = 0x00000001,  /* For non looping sounds. (default).  Overrides FMOD_LOOP_NORMAL / FMOD_LOOP_BIDI. */
        LOOP_NORMAL = 0x00000002,  /* For forward looping sounds. */
        LOOP_BIDI = 0x00000004,  /* For bidirectional looping sounds. (only works on software mixed static sounds). */
        _2D = 0x00000008,  /* Ignores any 3d processing. (default). */
        _3D = 0x00000010,  /* Makes the sound positionable in 3D.  Overrides FMOD_2D. */
        CREATESTREAM = 0x00000080,  /* Decompress at runtime, streaming from the source provided (standard stream).  Overrides FMOD_CREATESAMPLE. */
        CREATESAMPLE = 0x00000100,  /* Decompress at loadtime, decompressing or decoding whole file into memory as the target sample format. (standard sample). */
        CREATECOMPRESSEDSAMPLE = 0x00000200,  /* Load MP2, MP3, IMAADPCM or XMA into memory and leave it compressed.  During playback the BreadPlayer.Fmod software mixer will decode it in realtime as a 'compressed sample'.  Can only be used in combination with FMOD_SOFTWARE. */
        OPENUSER = 0x00000400,  /* Opens a user created static sample or stream. Use FMOD_CREATESOUNDEXINFO to specify format and/or read callbacks.  If a user created 'sample' is created with no read callback, the sample will be empty.  Use FMOD_Sound_Lock and FMOD_Sound_Unlock to place sound data into the sound if this is the case. */
        OPENMEMORY = 0x00000800,  /* "name_or_data" will be interpreted as a pointer to memory instead of filename for creating sounds. */
        OPENMEMORY_POINT = 0x10000000,  /* "name_or_data" will be interpreted as a pointer to memory instead of filename for creating sounds.  Use FMOD_CREATESOUNDEXINFO to specify length.  This differs to FMOD_OPENMEMORY in that it uses the memory as is, without duplicating the memory into its own buffers.  Cannot be freed after open, only after Sound::release.   Will not work if the data is compressed and FMOD_CREATECOMPRESSEDSAMPLE is not used. */
        OPENRAW = 0x00001000,  /* Will ignore file format and treat as raw pcm.  User may need to declare if data is FMOD_SIGNED or FMOD_UNSIGNED */
        OPENONLY = 0x00002000,  /* Just open the file, dont prebuffer or read.  Good for fast opens for info, or when sound::readData is to be used. */
        ACCURATETIME = 0x00004000,  /* For FMOD_CreateSound - for accurate FMOD_Sound_GetLength / FMOD_Channel_SetPosition on VBR MP3, AAC and MOD/S3M/XM/IT/MIDI files.  Scans file first, so takes longer to open. FMOD_OPENONLY does not affect this. */
        MPEGSEARCH = 0x00008000,  /* For corrupted / bad MP3 files.  This will search all the way through the file until it hits a valid MPEG header.  Normally only searches for 4k. */
        NONBLOCKING = 0x00010000,  /* For opening sounds and getting streamed subsounds (seeking) asyncronously.  Use Sound::getOpenState to poll the state of the sound as it opens or retrieves the subsound in the background. */
        UNIQUE = 0x00020000,  /* Unique sound, can only be played one at a time */
        _3D_HEADRELATIVE = 0x00040000,  /* Make the sound's position, velocity and orientation relative to the listener. */
        _3D_WORLDRELATIVE = 0x00080000,  /* Make the sound's position, velocity and orientation absolute (relative to the world). (DEFAULT) */
        _3D_INVERSEROLLOFF = 0x00100000,  /* This sound will follow the inverse rolloff model where mindistance = full volume, maxdistance = where sound stops attenuating, and rolloff is fixed according to the global rolloff factor.  (DEFAULT) */
        _3D_LINEARROLLOFF = 0x00200000,  /* This sound will follow a linear rolloff model where mindistance = full volume, maxdistance = silence.  */
        _3D_LINEARSQUAREROLLOFF = 0x00400000,  /* This sound will follow a linear-square rolloff model where mindistance = full volume, maxdistance = silence.  Rolloffscale is ignored. */
        _3D_INVERSETAPEREDROLLOFF = 0x00800000,  /* This sound will follow the inverse rolloff model at distances close to mindistance and a linear-square rolloff close to maxdistance. */
        _3D_CUSTOMROLLOFF = 0x04000000,  /* This sound will follow a rolloff model defined by Sound::set3DCustomRolloff / Channel::set3DCustomRolloff.  */
        _3D_IGNOREGEOMETRY = 0x40000000,  /* Is not affect by geometry occlusion.  If not specified in Sound::setMode, or Channel::setMode, the flag is cleared and it is affected by geometry again. */
        IGNORETAGS = 0x02000000,  /* Skips id3v2/asf/etc tag checks when opening a sound, to reduce seek/read overhead when opening files (helps with CD performance). */
        LOWMEM = 0x08000000,  /* Removes some features from samples to give a lower memory overhead, like Sound::getName. */
        LOADSECONDARYRAM = 0x20000000,  /* Load sound into the secondary RAM of supported platform.  On PS3, sounds will be loaded into RSX/VRAM. */
        VIRTUAL_PLAYFROMSTART = 0x80000000   /* For sounds that start virtual (due to being quiet or low importance), instead of swapping back to audible, and playing at the correct offset according to time, this flag makes the sound play from the start. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        These values describe what state a sound is in after FMOD_NONBLOCKING has been used to open it.

        [REMARKS]
        With streams, if you are using FMOD_NONBLOCKING, note that if the user calls Sound::getSubSound, a stream will go into FMOD_OPENSTATE_SEEKING state and sound related commands will return FMOD_ERR_NOTREADY.<br>
        With streams, if you are using FMOD_NONBLOCKING, note that if the user calls Channel::getPosition, a stream will go into FMOD_OPENSTATE_SETPOSITION state and sound related commands will return FMOD_ERR_NOTREADY.<br>

        [SEE_ALSO]
        Sound::getOpenState
        FMOD_MODE
    ]
    */
    public enum OpenState : int
    {
        READY = 0,       /* Opened and ready to play */
        LOADING,         /* Initial load in progress */
        ERROR,           /* Failed to open - file not found, out of memory etc.  See return value of Sound::getOpenState for what happened. */
        CONNECTING,      /* Connecting to remote host (internet sounds only) */
        BUFFERING,       /* Buffering data */
        SEEKING,         /* Seeking to subsound and re-flushing stream buffer. */
        PLAYING,         /* Ready and playing, but not possible to release at this time without stalling the main thread. */
        SETPOSITION,     /* Seeking within a stream to a different position. */

        MAX,             /* Maximum number of open state types. */
    }

    /*
    [ENUM]
    [
        [DESCRIPTION]
        These flags are used with SoundGroup::setMaxAudibleBehavior to determine what happens when more sounds
        are played than are specified with SoundGroup::setMaxAudible.

        [REMARKS]
        When using FMOD_SOUNDGROUP_BEHAVIOR_MUTE, SoundGroup::setMuteFadeSpeed can be used to stop a sudden transition.
        Instead, the time specified will be used to cross fade between the sounds that go silent and the ones that become audible.

        [SEE_ALSO]
        SoundGroup::setMaxAudibleBehavior
        SoundGroup::getMaxAudibleBehavior
        SoundGroup::setMaxAudible
        SoundGroup::getMaxAudible
        SoundGroup::setMuteFadeSpeed
        SoundGroup::getMuteFadeSpeed
    ]
    */
    public enum SoundGroupBehavior : int
    {
        BEHAVIOR_FAIL,              /* Any sound played that puts the sound count over the SoundGroup::setMaxAudible setting, will simply fail during System::playSound. */
        BEHAVIOR_MUTE,              /* Any sound played that puts the sound count over the SoundGroup::setMaxAudible setting, will be silent, then if another sound in the group stops the sound that was silent before becomes audible again. */
        BEHAVIOR_STEALLOWEST,       /* Any sound played that puts the sound count over the SoundGroup::setMaxAudible setting, will steal the quietest / least important sound playing in the group. */

        MAX,               /* Maximum number of sound group behaviors. */
    }

    /*
    [ENUM]
    [
        [DESCRIPTION]
        These callback types are used with Channel::setCallback.

        [REMARKS]
        Each callback has commanddata parameters passed as int unique to the type of callback.<br>
        See reference to FMOD_CHANNELCONTROL_CALLBACK to determine what they might mean for each type of callback.<br>
        <br>
        <b>Note!</b>  Currently the user must call System::update for these callbacks to trigger!

        [SEE_ALSO]
        Channel::setCallback
        ChannelGroup::setCallback
        FMOD_CHANNELCONTROL_CALLBACK
        System::update
    ]
    */
    public enum ChannelControlCallbackType : int
    {
        END,                  /* Called when a sound ends. */
        VIRTUALVOICE,         /* Called when a voice is swapped out or swapped in. */
        SYNCPOINT,            /* Called when a syncpoint is encountered.  Can be from wav file markers. */
        OCCLUSION,            /* Called when the channel has its geometry occlusion value calculated.  Can be used to clamp or change the value. */

        MAX,                  /* Maximum number of callback types supported. */
    }

    /*
    [ENUM]
    [
        [DESCRIPTION]
        These enums denote special types of node within a DSP chain.

        [REMARKS]

        [SEE_ALSO]
        Channel::getDSP
        ChannelGroup::getDSP
    ]
    */
    public struct ChannelControlDspIndex
    {
        public const int HEAD = -1;         /* Head of the DSP chain. */
        public const int FADER = -2;         /* Built in fader DSP. */
        public const int PANNER = -3;         /* Built in panner DSP. */
        public const int TAIL = -4;         /* Tail of the DSP chain. */
    }

    /*
    [ENUM]
    [
        [DESCRIPTION]
        Used to distinguish the instance type passed into FMOD_ERROR_CALLBACK.

        [REMARKS]
        Cast the instance of FMOD_ERROR_CALLBACK to the appropriate class indicated by this enum.

        [SEE_ALSO]
    ]
    */
    public enum ErrorCallbackInstancetype
    {
        NONE,
        SYSTEM,
        CHANNEL,
        CHANNELGROUP,
        CHANNELCONTROL,
        SOUND,
        SOUNDGROUP,
        DSP,
        DSPCONNECTION,
        GEOMETRY,
        REVERB3D,
        STUDIO_SYSTEM,
        STUDIO_EVENTDESCRIPTION,
        STUDIO_EVENTINSTANCE,
        STUDIO_PARAMETERINSTANCE,
        STUDIO_BUS,
        STUDIO_VCA,
        STUDIO_BANK,
        STUDIO_COMMANDREPLAY
    }

    /*
   [DEFINE]
   [
       [NAME]
       FMOD_SYSTEM_CALLBACK_TYPE

       [DESCRIPTION]
       These callback types are used with System::setCallback.

       [REMARKS]
       Each callback has commanddata parameters passed as void* unique to the type of callback.<br>
       See reference to FMOD_SYSTEM_CALLBACK to determine what they might mean for each type of callback.<br>
       <br>
       <b>Note!</b> Using FMOD_SYSTEM_CALLBACK_DEVICELISTCHANGED (on Mac only) requires the application to be running an event loop which will allow external changes to device list to be detected by BreadPlayer.Fmod.<br>
       <br>
       <b>Note!</b> The 'system' object pointer will be null for FMOD_SYSTEM_CALLBACK_THREADCREATED and FMOD_SYSTEM_CALLBACK_MEMORYALLOCATIONFAILED callbacks.

       [SEE_ALSO]
       System::setCallback
       System::update
       DSP::addInput
   ]
   */
    [Flags]
    public enum SystemCallbackType : uint
    {
        DEVICELISTCHANGED = 0x00000001,  /* Called from System::update when the enumerated list of devices has changed. */
        DEVICELOST = 0x00000002,  /* Called from System::update when an output device has been lost due to control panel parameter changes and BreadPlayer.Fmod cannot automatically recover. */
        MEMORYALLOCATIONFAILED = 0x00000004,  /* Called directly when a memory allocation fails somewhere in BreadPlayer.Fmod.  (NOTE - 'system' will be NULL in this callback type.)*/
        THREADCREATED = 0x00000008,  /* Called directly when a thread is created. (NOTE - 'system' will be NULL in this callback type.) */
        BADDSPCONNECTION = 0x00000010,  /* Called when a bad connection was made with DSP::addInput. Usually called from mixer thread because that is where the connections are made.  */
        PREMIX = 0x00000020,  /* Called each tick before a mix update happens. */
        POSTMIX = 0x00000040,  /* Called each tick after a mix update happens. */
        ERROR = 0x00000080,  /* Called when each API function returns an error code, including delayed async functions. */
        MIDMIX = 0x00000100,  /* Called each tick in mix update after clocks have been updated before the main mix occurs. */
        THREADDESTROYED = 0x00000200,  /* Called directly when a thread is destroyed. */
        PREUPDATE = 0x00000400,  /* Called at start of System::update function. */
        POSTUPDATE = 0x00000800,  /* Called at end of System::update function. */
        RECORDLISTCHANGED = 0x00001000,  /* Called from System::update when the enumerated list of recording devices has changed. */
        ALL = 0xFFFFFFFF,  /* Pass this mask to System::setCallback to receive all callback types.  */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        List of interpolation types that the BreadPlayer.Fmod Ex software mixer supports.

        [REMARKS]
        The default resampler type is FMOD_DSP_RESAMPLER_LINEAR.<br>
        Use System::setSoftwareFormat to tell BreadPlayer.Fmod the resampling quality you require for FMOD_SOFTWARE based sounds.

        [SEE_ALSO]
        System::setSoftwareFormat
        System::getSoftwareFormat
    ]
    */
    public enum DspResampler : int
    {
        DEFAULT,         /* Default interpolation method.  Currently equal to FMOD_DSP_RESAMPLER_LINEAR. */
        NOINTERP,        /* No interpolation.  High frequency aliasing hiss will be audible depending on the sample rate of the sound. */
        LINEAR,          /* Linear interpolation (default method).  Fast and good quality, causes very slight lowpass effect on low frequency sounds. */
        CUBIC,           /* Cubic interpolation.  Slower than linear interpolation but better quality. */
        SPLINE,          /* 5 point spline interpolation.  Slowest resampling method but best quality. */

        MAX,             /* Maximum number of resample methods supported. */
    }

    /*
    [ENUM]
    [
        [DESCRIPTION]
        List of connection types between 2 DSP nodes.

        [REMARKS]
        FMOD_DSP_CONNECTION_TYPE_STANDARD<br>
        ----------------------------------<br>
        Default DSPConnection type.  Audio is mixed from the input to the output DSP's audible buffer, meaning it will be part of the audible signal.  A standard connection will execute its input DSP if it has not been executed before.<br>
        <br>
        FMOD_DSP_CONNECTION_TYPE_SIDECHAIN<br>
        ----------------------------------<br>
        Sidechain DSPConnection type.  Audio is mixed from the input to the output DSP's sidechain buffer, meaning it will NOT be part of the audible signal.  A sidechain connection will execute its input DSP if it has not been executed before.<br>
        The purpose of the seperate sidechain buffer in a DSP, is so that the DSP effect can privately access for analysis purposes.  An example of use in this case, could be a compressor which analyzes the signal, to control its own effect parameters (ie a compression level or gain).<br>
        <br>
        For the effect developer, to accept sidechain data, the sidechain data will appear in the FMOD_DSP_STATE struct which is passed into the read callback of a DSP unit.<br>
        FMOD_DSP_STATE::sidechaindata and FMOD_DSP::sidechainchannels will hold the mixed result of any sidechain data flowing into it.<br>
        <br>
        FMOD_DSP_CONNECTION_TYPE_SEND<br>
        -----------------------------<br>
        Send DSPConnection type.  Audio is mixed from the input to the output DSP's audible buffer, meaning it will be part of the audible signal.  A send connection will NOT execute its input DSP if it has not been executed before.<br>
        A send connection will only read what exists at the input's buffer at the time of executing the output DSP unit (which can be considered the 'return')<br>
        <br>
        FMOD_DSP_CONNECTION_TYPE_SEND_SIDECHAIN<br>
        ---------------------------------------<br>
        Send sidechain DSPConnection type.  Audio is mixed from the input to the output DSP's sidechain buffer, meaning it will NOT be part of the audible signal.  A send sidechain connection will NOT execute its input DSP if it has not been executed before.<br>
        A send sidechain connection will only read what exists at the input's buffer at the time of executing the output DSP unit (which can be considered the 'sidechain return').
        <br>
        For the effect developer, to accept sidechain data, the sidechain data will appear in the FMOD_DSP_STATE struct which is passed into the read callback of a DSP unit.<br>
        FMOD_DSP_STATE::sidechaindata and FMOD_DSP::sidechainchannels will hold the mixed result of any sidechain data flowing into it.

        [SEE_ALSO]
        DSP::addInput
        DSPConnection::getType
    ]
    */
    public enum DspConnectionType : int
    {
        STANDARD,          /* Default connection type.         Audio is mixed from the input to the output DSP's audible buffer.  */
        SIDECHAIN,         /* Sidechain connection type.       Audio is mixed from the input to the output DSP's sidechain buffer.  */
        SEND,              /* Send connection type.            Audio is mixed from the input to the output DSP's audible buffer, but the input is NOT executed, only copied from.  A standard connection or sidechain needs to make an input execute to generate data. */
        SEND_SIDECHAIN,    /* Send sidechain connection type.  Audio is mixed from the input to the output DSP's sidechain buffer, but the input is NOT executed, only copied from.  A standard connection or sidechain needs to make an input execute to generate data. */

        MAX,               /* Maximum number of DSP connection types supported. */
    }

    /*
    [ENUM]
    [
        [DESCRIPTION]
        List of tag types that could be stored within a sound.  These include id3 tags, metadata from netstreams and vorbis/asf data.

        [REMARKS]

        [SEE_ALSO]
        Sound::getTag
    ]
    */
    public enum TagType : int
    {
        UNKNOWN = 0,
        ID3V1,
        ID3V2,
        VORBISCOMMENT,
        SHOUTCAST,
        ICECAST,
        ASF,
        MIDI,
        PLAYLIST,
        FMOD,
        USER,

        MAX                /* Maximum number of tag types supported. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        List of data types that can be returned by Sound::getTag

        [REMARKS]

        [SEE_ALSO]
        Sound::getTag
    ]
    */
    public enum TagDataType : int
    {
        BINARY = 0,
        INT,
        FLOAT,
        STRING,
        STRING_UTF16,
        STRING_UTF16BE,
        STRING_UTF8,
        CDTOC,

        MAX                /* Maximum number of tag datatypes supported. */
    }
    /*
        [DEFINE]
        [
            [NAME]
            FMOD_DRIVER_STATE

            [DESCRIPTION]
            Flags that provide additional information about a particular driver.

            [REMARKS]

            [SEE_ALSO]
            System::getRecordDriverInfo
        ]
        */
    [Flags]
    public enum DriverState : uint
    {
        CONNECTED = 0x00000001, /* Device is currently plugged in. */
        DEFAULT = 0x00000002, /* Device is the users preferred choice. */
    }

}
