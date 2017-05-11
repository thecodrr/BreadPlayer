using System;

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
    public enum Result
    {
        Ok,                        /* No errors. */
        ErrBadcommand,            /* Tried to call a function on a data type that does not allow this type of functionality (ie calling Sound::lock on a streaming sound). */
        ErrChannelAlloc,         /* Error trying to allocate a channel. */
        ErrChannelStolen,        /* The specified channel has been reused to play another sound. */
        ErrDma,                   /* DMA Failure.  See debug output for more information. */
        ErrDspConnection,        /* DSP connection error.  Connection possibly caused a cyclic dependency or connected dsps with incompatible buffer counts. */
        ErrDspDontprocess,       /* DSP return code from a DSP process query callback.  Tells mixer not to call the process callback and therefore not consume CPU.  Use this to optimize the DSP graph. */
        ErrDspFormat,            /* DSP Format error.  A DSP unit may have attempted to connect to this network with the wrong format, or a matrix may have been set with the wrong size if the target unit has a specified channel map. */
        ErrDspInuse,             /* DSP is already in the mixer's DSP network. It must be removed before being reinserted or released. */
        ErrDspNotfound,          /* DSP connection error.  Couldn't find the DSP unit specified. */
        ErrDspReserved,          /* DSP operation error.  Cannot perform operation on this DSP as it is reserved by the system. */
        ErrDspSilence,           /* DSP return code from a DSP process query callback.  Tells mixer silence would be produced from read, so go idle and not consume CPU.  Use this to optimize the DSP graph. */
        ErrDspType,              /* DSP operation cannot be performed on a DSP of this type. */
        ErrFileBad,              /* Error loading file. */
        ErrFileCouldnotseek,     /* Couldn't perform seek operation.  This is a limitation of the medium (ie netstreams) or the file format. */
        ErrFileDiskejected,      /* Media was ejected while reading. */
        ErrFileEof,              /* End of file unexpectedly reached while trying to read essential data (truncated?). */
        ErrFileEndofdata,        /* End of current chunk reached while trying to read data. */
        ErrFileNotfound,         /* File not found. */
        ErrFormat,                /* Unsupported file or audio format. */
        ErrHeaderMismatch,       /* There is a version mismatch between the BreadPlayer.Fmod header and either the BreadPlayer.Fmod Studio library or the BreadPlayer.Fmod Low Level library. */
        ErrHttp,                  /* A HTTP error occurred. This is a catch-all for HTTP errors not listed elsewhere. */
        ErrHttpAccess,           /* The specified resource requires authentication or is forbidden. */
        ErrHttpProxyAuth,       /* Proxy authentication is required to access the specified resource. */
        ErrHttpServerError,     /* A HTTP server error occurred. */
        ErrHttpTimeout,          /* The HTTP request timed out. */
        ErrInitialization,        /* BreadPlayer.Fmod was not initialized correctly to support this function. */
        ErrInitialized,           /* Cannot call this command after FMODSystem::init. */
        ErrInternal,              /* An error occurred that wasn't supposed to.  Contact support. */
        ErrInvalidFloat,         /* Value passed in was a NaN, Inf or denormalized float. */
        ErrInvalidHandle,        /* An invalid object handle was used. */
        ErrInvalidParam,         /* An invalid parameter was passed to this function. */
        ErrInvalidPosition,      /* An invalid seek position was passed to this function. */
        ErrInvalidSpeaker,       /* An invalid speaker was passed to this function based on the current speaker mode. */
        ErrInvalidSyncpoint,     /* The syncpoint did not come from this sound handle. */
        ErrInvalidThread,        /* Tried to call a function on a thread that is not supported. */
        ErrInvalidVector,        /* The vectors passed in are not unit length, or perpendicular. */
        ErrMaxaudible,            /* Reached maximum audible playback count for this sound's soundgroup. */
        ErrMemory,                /* Not enough memory or resources. */
        ErrMemoryCantpoint,      /* Can't use FMOD_OPENMEMORY_POINT on non PCM source data, or non mp3/xma/adpcm data if FMOD_CREATECOMPRESSEDSAMPLE was used. */
        ErrNeeds3D,               /* Tried to call a command on a 2d sound when the command was meant for 3d sound. */
        ErrNeedshardware,         /* Tried to use a feature that requires hardware support. */
        ErrNetConnect,           /* Couldn't connect to the specified host. */
        ErrNetSocketError,      /* A socket error occurred.  This is a catch-all for socket-related errors not listed elsewhere. */
        ErrNetUrl,               /* The specified URL couldn't be resolved. */
        ErrNetWouldBlock,       /* Operation on a non-blocking socket could not complete immediately. */
        ErrNotready,              /* Operation could not be performed because specified sound/DSP connection is not ready. */
        ErrOutputAllocated,      /* Error initializing output device, but more specifically, the output device is already in use and cannot be reused. */
        ErrOutputCreatebuffer,   /* Error creating hardware sound buffer. */
        ErrOutputDrivercall,     /* A call to a standard soundcard driver failed, which could possibly mean a bug in the driver or resources were missing or exhausted. */
        ErrOutputFormat,         /* Soundcard does not support the specified format. */
        ErrOutputInit,           /* Error initializing output device. */
        ErrOutputNodrivers,      /* The output device has no drivers installed.  If pre-init, FMOD_OUTPUT_NOSOUND is selected as the output mode.  If post-init, the function just fails. */
        ErrPlugin,                /* An unspecified error has been returned from a plugin. */
        ErrPluginMissing,        /* A requested output, dsp unit type or codec was not available. */
        ErrPluginResource,       /* A resource that the plugin requires cannot be found. (ie the DLS file for MIDI playback) */
        ErrPluginVersion,        /* A plugin was built with an unsupported SDK version. */
        ErrRecord,                /* An error occurred trying to initialize the recording device. */
        ErrReverbChannelgroup,   /* Reverb properties cannot be set on this channel because a parent channelgroup owns the reverb connection. */
        ErrReverbInstance,       /* Specified instance in FMOD_REVERB_PROPERTIES couldn't be set. Most likely because it is an invalid instance number or the reverb doesn't exist. */
        ErrSubsounds,             /* The error occurred because the sound referenced contains subsounds when it shouldn't have, or it doesn't contain subsounds when it should have.  The operation may also not be able to be performed on a parent sound. */
        ErrSubsoundAllocated,    /* This subsound is already being used by another sound, you cannot have more than one parent to a sound.  Null out the other parent's entry first. */
        ErrSubsoundCantmove,     /* Shared subsounds cannot be replaced or moved from their parent stream, such as when the parent stream is an FSB file. */
        ErrTagnotfound,           /* The specified tag could not be found or there are no tags. */
        ErrToomanychannels,       /* The sound created exceeds the allowable input channel count.  This can be increased using the 'maxinputchannels' parameter in FMODSystem::setSoftwareFormat. */
        ErrTruncated,             /* The retrieved string is too long to fit in the supplied buffer and has been truncated. */
        ErrUnimplemented,         /* Something in BreadPlayer.Fmod hasn't been implemented when it should be! contact support! */
        ErrUninitialized,         /* This command failed because FMODSystem::init or FMODSystem::setDriver was not called. */
        ErrUnsupported,           /* A command issued was not supported by this object.  Possibly a plugin without certain callbacks specified. */
        ErrVersion,               /* The version number of this file format is not supported. */
        ErrEventAlreadyLoaded,  /* The specified bank has already been loaded. */
        ErrEventLiveupdateBusy, /* The live update connection failed due to the game already being connected. */
        ErrEventLiveupdateMismatch, /* The live update connection failed due to the game data being out of sync with the tool. */
        ErrEventLiveupdateTimeout, /* The live update connection timed out. */
        ErrEventNotfound,        /* The requested event, bus or vca could not be found. */
        ErrStudioUninitialized,  /* The Studio::FMODSystem object is not yet initialized. */
        ErrStudioNotLoaded,     /* The specified resource is not loaded, so it can't be unloaded. */
        ErrInvalidString,        /* An invalid string was passed to this function. */
        ErrAlreadyLocked,        /* The specified resource is already locked. */
        ErrNotLocked,            /* The specified resource is not locked, so it can't be unlocked. */
        ErrRecordDisconnected,   /* The specified recording driver has been disconnected. */
        ErrToomanysamples        /* The length provided exceed the allowable limit. */
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
    public enum ChannelControlType
    {
        Channel,
        Channelgroup
    }
    /*
[ENUM]
[
   [DESCRIPTION]
   These output types are used with FMODSystem::setOutput / FMODSystem::getOutput, to choose which output method to use.

   [REMARKS]
   To pass information to the driver when initializing fmod use the *extradriverdata* parameter in FMODSystem::init for the following reasons.

   - FMOD_OUTPUTTYPE_WAVWRITER     - extradriverdata is a pointer to a char * file name that the wav writer will output to.
   - FMOD_OUTPUTTYPE_WAVWRITER_NRT - extradriverdata is a pointer to a char * file name that the wav writer will output to.
   - FMOD_OUTPUTTYPE_DSOUND        - extradriverdata is cast to a HWND type, so that BreadPlayer.Fmod can set the focus on the audio for a particular window.
   - FMOD_OUTPUTTYPE_PS3           - extradriverdata is a pointer to a FMOD_PS3_EXTRADRIVERDATA struct. This can be found in fmodps3.h.
   - FMOD_OUTPUTTYPE_XBOX360       - extradriverdata is a pointer to a FMOD_360_EXTRADRIVERDATA struct. This can be found in fmodxbox360.h.

   Currently these are the only BreadPlayer.Fmod drivers that take extra information.  Other unknown plugins may have different requirements.

   Note! If FMOD_OUTPUTTYPE_WAVWRITER_NRT or FMOD_OUTPUTTYPE_NOSOUND_NRT are used, and if the FMODSystem::update function is being called
   very quickly (ie for a non realtime decode) it may be being called too quickly for the BreadPlayer.Fmod streamer thread to respond to.
   The result will be a skipping/stuttering output in the captured audio.

   To remedy this, disable the BreadPlayer.Fmod streamer thread, and use FMOD_INIT_STREAM_FROM_UPDATE to avoid skipping in the output stream,
   as it will lock the mixer and the streamer together in the same thread.

   [SEE_ALSO]
       FMODSystem::setOutput
       FMODSystem::getOutput
       FMODSystem::setSoftwareFormat
       FMODSystem::getSoftwareFormat
       FMODSystem::init
       FMODSystem::update
       FMOD_INITFLAGS
]
*/
    public enum OutputType
    {
        Autodetect,      /* Picks the best output mode for the platform. This is the default. */

        Unknown,         /* All - 3rd party plugin, unknown. This is for use with FMODSystem::getOutput only. */
        Nosound,         /* All - Perform all mixing but discard the final output. */
        Wavwriter,       /* All - Writes output to a .wav file. */
        NosoundNrt,     /* All - Non-realtime version of FMOD_OUTPUTTYPE_NOSOUND. User can drive mixer with FMODSystem::update at whatever rate they want. */
        WavwriterNrt,   /* All - Non-realtime version of FMOD_OUTPUTTYPE_WAVWRITER. User can drive mixer with FMODSystem::update at whatever rate they want. */

        Dsound,          /* Win                  - Direct Sound.                        (Default on Windows XP and below) */
        Winmm,           /* Win                  - Windows Multimedia. */
        Wasapi,          /* Win/WinStore/XboxOne - Windows Audio Session API.           (Default on Windows Vista and above, Xbox One and Windows Store Applications) */
        Asio,            /* Win                  - Low latency ASIO 2.0. */
        Pulseaudio,      /* Linux                - Pulse Audio.                         (Default on Linux if available) */
        Alsa,            /* Linux                - Advanced Linux Sound Architecture.   (Default on Linux if PulseAudio isn't available) */
        Coreaudio,       /* Mac/iOS              - Core Audio.                          (Default on Mac and iOS) */
        Xaudio,          /* Xbox 360             - XAudio.                              (Default on Xbox 360) */
        Ps3,             /* PS3                  - Audio Out.                           (Default on PS3) */
        Audiotrack,      /* Android              - Java Audio Track.                    (Default on Android 2.2 and below) */
        Opensl,          /* Android              - OpenSL ES.                           (Default on Android 2.3 and above) */
        Wiiu,            /* Wii U                - AX.                                  (Default on Wii U) */
        Audioout,        /* PS4/PSVita           - Audio Out.                           (Default on PS4 and PS Vita) */
        Audio3D,         /* PS4                  - Audio3D. */
        Atmos,           /* Win                  - Dolby Atmos (WASAPI). */

        Max             /* Maximum number of output types supported. */
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
    public enum DebugMode
    {
        Tty,        /* Default log location per platform, i.e. Visual Studio output window, stderr, LogCat, etc */
        File,       /* Write log to specified file path */
        Callback   /* Call specified callback with log information */
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
        None = 0x00000000,   /* Disable all messages */
        Error = 0x00000001,   /* Enable only error messages. */
        Warning = 0x00000002,   /* Enable warning and error messages. */
        Log = 0x00000004,   /* Enable informational, warning and error messages (default). */

        TypeMemory = 0x00000100,   /* Verbose logging for memory operations, only use this if you are debugging a memory related issue. */
        TypeFile = 0x00000200,   /* Verbose logging for file access, only use this if you are debugging a file related issue. */
        TypeCodec = 0x00000400,   /* Verbose logging for codec initialization, only use this if you are debugging a codec related issue. */
        TypeTrace = 0x00000800,   /* Verbose logging for internal errors, use this for tracking the origin of error codes. */

        DisplayTimestamps = 0x00010000,   /* Display the time stamp of the log message in milliseconds. */
        DisplayLinenumbers = 0x00020000,   /* Display the source code file and line number for where the message originated. */
        DisplayThread = 0x00040000   /* Display the thread ID of the calling function that generated the message. */
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
        Normal = 0x00000000,       /* Standard memory. */
        StreamFile = 0x00000001,       /* Stream file buffer, size controllable with FMODSystem::setStreamBufferSize. */
        StreamDecode = 0x00000002,       /* Stream decode buffer, size controllable with FMOD_CREATESOUNDEXINFO::decodebuffersize. */
        Sampledata = 0x00000004,       /* Sample data buffer.  Raw audio data, usually PCM/MPEG/ADPCM/XMA data. */
        DspBuffer = 0x00000008,       /* DSP memory block allocated when more than 1 output exists on a DSP node. */
        Plugin = 0x00000010,       /* Memory allocated by a third party plugin. */
        Xbox360Physical = 0x00100000,       /* Requires XPhysicalAlloc / XPhysicalFree. */
        Persistent = 0x00200000,       /* Persistent memory. Memory will be freed when FMODSystem::release is called. */
        Secondary = 0x00400000,       /* Secondary memory. Allocation should be in secondary memory. For example RSX on the PS3. */
        All = 0xFFFFFFFF
    }

    /*
    [ENUM]
    [
        [DESCRIPTION]
        These are speaker types defined for use with the FMODSystem::setSoftwareFormat command.

        [REMARKS]
        Note below the phrase 'sound channels' is used.  These are the subchannels inside a sound, they are not related and
        have nothing to do with the BreadPlayer.Fmod class "Channel".<br>
        For example a mono sound has 1 sound channel, a stereo sound has 2 sound channels, and an AC3 or 6 channel wav file have 6 "sound channels".<br>
        <br>
        FMOD_SPEAKERMODE_RAW<br>
        ---------------------<br>
        This mode is for output devices that are not specifically mono/stereo/quad/surround/5.1 or 7.1, but are multichannel.<br>
        Use FMODSystem::setSoftwareFormat to specify the number of speakers you want to address, otherwise it will default to 2 (stereo).<br>
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
        FMODSystem::setSoftwareFormat
        FMODSystem::getSoftwareFormat
        DSP::setChannelFormat
    ]
    */
    public enum SpeakerMode
    {
        Default,          /* Default speaker mode based on operating system/output mode.  Windows = control panel setting, Xbox = 5.1, PS3 = 7.1 etc. */
        Raw,              /* There is no specific speakermode.  Sound channels are mapped in order of input to output.  Use FMODSystem::setSoftwareFormat to specify speaker count. See remarks for more information. */
        Mono,             /* The speakers are monaural. */
        Stereo,           /* The speakers are stereo. */
        Quad,             /* 4 speaker setup.  This includes front left, front right, surround left, surround right.  */
        Surround,         /* 5 speaker setup.  This includes front left, front right, center, surround left, surround right. */
        _5Point1,         /* 5.1 speaker setup.  This includes front left, front right, center, surround left, surround right and an LFE speaker. */
        _7Point1,         /* 7.1 speaker setup.  This includes front left, front right, center, surround left, surround right, back left, back right and an LFE speaker. */

        Max              /* Maximum number of speaker modes supported. */
    }

    /*
    [ENUM]
    [
        [DESCRIPTION]
        Assigns an enumeration for a speaker index.

        [REMARKS]

        [SEE_ALSO]
        FMODSystem::setSpeakerPosition
        FMODSystem::getSpeakerPosition
    ]
    */
    public enum Speaker
    {
        FrontLeft,
        FrontRight,
        FrontCenter,
        LowFrequency,
        SurroundLeft,
        SurroundRight,
        BackLeft,
        BackRight,

        Max               /* Maximum number of speaker types supported. */
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
        FrontLeft = 0x00000001,
        FrontRight = 0x00000002,
        FrontCenter = 0x00000004,
        LowFrequency = 0x00000008,
        SurroundLeft = 0x00000010,
        SurroundRight = 0x00000020,
        BackLeft = 0x00000040,
        BackRight = 0x00000080,
        BackCenter = 0x00000100,

        Mono = (FrontLeft),
        Stereo = (FrontLeft | FrontRight),
        Lrc = (FrontLeft | FrontRight | FrontCenter),
        Quad = (FrontLeft | FrontRight | SurroundLeft | SurroundRight),
        Surround = (FrontLeft | FrontRight | FrontCenter | SurroundLeft | SurroundRight),
        _5Point1 = (FrontLeft | FrontRight | FrontCenter | LowFrequency | SurroundLeft | SurroundRight),
        _5Point1Rears = (FrontLeft | FrontRight | FrontCenter | LowFrequency | BackLeft | BackRight),
        _7Point0 = (FrontLeft | FrontRight | FrontCenter | SurroundLeft | SurroundRight | BackLeft | BackRight),
        _7Point1 = (FrontLeft | FrontRight | FrontCenter | LowFrequency | SurroundLeft | SurroundRight | BackLeft | BackRight)
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
    public enum ChannelOrder
    {
        Default,              /* Left, Right, Center, LFE, Surround Left, Surround Right, Back Left, Back Right (see FMOD_SPEAKER enumeration)   */
        Waveformat,           /* Left, Right, Center, LFE, Back Left, Back Right, Surround Left, Surround Right (as per Microsoft .wav WAVEFORMAT structure master order) */
        Protools,             /* Left, Center, Right, Surround Left, Surround Right, LFE */
        Allmono,              /* Mono, Mono, Mono, Mono, Mono, Mono, ... (each channel all the way up to 32 channels are treated as if they were mono) */
        Allstereo,            /* Left, Right, Left, Right, Left, Right, ... (each pair of channels is treated as stereo all the way up to 32 channels) */
        Alsa,                 /* Left, Right, Surround Left, Surround Right, Center, LFE (as per Linux ALSA channel order) */

        Max                  /* Maximum number of channel orderings supported. */
    }

    /*
    [ENUM]
    [
        [DESCRIPTION]
        These are plugin types defined for use with the FMODSystem::getNumPlugins,
        FMODSystem::getPluginInfo and FMODSystem::unloadPlugin functions.

        [REMARKS]

        [SEE_ALSO]
        FMODSystem::getNumPlugins
        FMODSystem::getPluginInfo
        FMODSystem::unloadPlugin
    ]
    */
    public enum PluginType
    {
        Output,          /* The plugin type is an output module.  BreadPlayer.Fmod mixed audio will play through one of these devices */
        Codec,           /* The plugin type is a file format codec.  BreadPlayer.Fmod will use these codecs to load file formats for playback. */
        Dsp,             /* The plugin type is a DSP unit.  BreadPlayer.Fmod will use these plugins as part of its DSP network to apply effects to output or generate sound in realtime. */

        Max             /* Maximum number of plugin types supported. */
    }
    /*
[DEFINE]
[
    [NAME]
    FMOD_INITFLAGS

    [DESCRIPTION]
    Initialization flags.  Use them with FMODSystem::init in the *flags* parameter to change various behavior.

    [REMARKS]
    Use FMODSystem::setAdvancedSettings to adjust settings for some of the features that are enabled by these flags.

    [SEE_ALSO]
    FMODSystem::init
    FMODSystem::update
    FMODSystem::setAdvancedSettings
    Channel::set3DOcclusion
]
*/
    [Flags]
    public enum InitFlags : uint
    {
        Normal = 0x00000000, /* Initialize normally */
        StreamFromUpdate = 0x00000001, /* No stream thread is created internally.  Streams are driven from FMODSystem::update.  Mainly used with non-realtime outputs. */
        MixFromUpdate = 0x00000002, /* Win/Wii/PS3/Xbox/Xbox 360 Only - BreadPlayer.Fmod Mixer thread is woken up to do a mix when FMODSystem::update is called rather than waking periodically on its own timer. */
        _3DRighthanded = 0x00000004, /* BreadPlayer.Fmod will treat +X as right, +Y as up and +Z as backwards (towards you). */
        ChannelLowpass = 0x00000100, /* All FMOD_3D based voices will add a software lowpass filter effect into the DSP chain which is automatically used when Channel::set3DOcclusion is used or the geometry API.   This also causes sounds to sound duller when the sound goes behind the listener, as a fake HRTF style effect.  Use FMODSystem::setAdvancedSettings to disable or adjust cutoff frequency for this feature. */
        ChannelDistancefilter = 0x00000200, /* All FMOD_3D based voices will add a software lowpass and highpass filter effect into the DSP chain which will act as a distance-automated bandpass filter. Use FMODSystem::setAdvancedSettings to adjust the center frequency. */
        ProfileEnable = 0x00010000, /* Enable TCP/IP based host which allows BreadPlayer.Fmod Designer or BreadPlayer.Fmod Profiler to connect to it, and view memory, CPU and the DSP network graph in real-time. */
        Vol0BecomesVirtual = 0x00020000, /* Any sounds that are 0 volume will go virtual and not be processed except for having their positions updated virtually.  Use FMODSystem::setAdvancedSettings to adjust what volume besides zero to switch to virtual at. */
        GeometryUseclosest = 0x00040000, /* With the geometry engine, only process the closest polygon rather than accumulating all polygons the sound to listener line intersects. */
        PreferDolbyDownmix = 0x00080000, /* When using FMOD_SPEAKERMODE_5POINT1 with a stereo output device, use the Dolby Pro Logic II downmix algorithm instead of the SRS Circle Surround algorithm. */
        ThreadUnsafe = 0x00100000, /* Disables thread safety for API calls. Only use this if BreadPlayer.Fmod low level is being called from a single thread, and if Studio API is not being used! */
        ProfileMeterAll = 0x00200000  /* Slower, but adds level metering for every single DSP unit in the graph.  Use DSP::setMeteringEnabled to turn meters off individually. */
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
        Unknown,         /* 3rd party / unknown plugin format. */
        Aiff,            /* AIFF. */
        Asf,             /* Microsoft Advanced Systems Format (ie WMA/ASF/WMV). */
        Dls,             /* Sound font / downloadable sound bank. */
        Flac,            /* FLAC lossless codec. */
        Fsb,             /* BreadPlayer.Fmod Sample Bank. */
        It,              /* Impulse Tracker. */
        Midi,            /* MIDI. extracodecdata is a pointer to an FMOD_MIDI_EXTRACODECDATA structure. */
        Mod,             /* Protracker / Fasttracker MOD. */
        Mpeg,            /* MP2/MP3 MPEG. */
        Oggvorbis,       /* Ogg vorbis. */
        Playlist,        /* Information only from ASX/PLS/M3U/WAX playlists */
        Raw,             /* Raw PCM data. */
        S3M,             /* ScreamTracker 3. */
        User,            /* User created sound. */
        Wav,             /* Microsoft WAV. */
        Xm,              /* FastTracker 2 XM. */
        Xma,             /* Xbox360 XMA */
        Audioqueue,      /* iPhone hardware decoder, supports AAC, ALAC and MP3. extracodecdata is a pointer to an FMOD_AUDIOQUEUE_EXTRACODECDATA structure. */
        At9,             /* PS4 / PSVita ATRAC 9 format */
        Vorbis,          /* Vorbis */
        MediaFoundation,/* Windows Store Application built in system codecs */
        Mediacodec,      /* Android MediaCodec */
        Fadpcm,          /* BreadPlayer.Fmod Adaptive Differential Pulse Code Modulation */

        Max             /* Maximum number of sound types supported. */
    }

    /*
    [ENUM]
    [
        [DESCRIPTION]
        These definitions describe the native format of the hardware or software buffer that will be used.

        [REMARKS]
        This is the format the native hardware or software buffer will be or is created in.

        [SEE_ALSO]
        FMODSystem::createSoundEx
        Sound::getFormat
    ]
    */
    public enum SoundFormat
    {
        None,       /* Unitialized / unknown */
        Pcm8,       /* 8bit integer PCM data */
        Pcm16,      /* 16bit integer PCM data  */
        Pcm24,      /* 24bit integer PCM data  */
        Pcm32,      /* 32bit integer PCM data  */
        Pcmfloat,   /* 32bit floating point PCM data  */
        Bitstream,  /* Sound data is in its native compressed format. */

        Max         /* Maximum number of sound formats supported. */
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
        To have a sound stream instead, use FMOD_CREATESTREAM, or use the wrapper function FMODSystem::createStream.<br>
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
        FMODSystem::createSound
        FMODSystem::createStream
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
        Default = 0x00000000,  /* Default for all modes listed below. FMOD_LOOP_OFF, FMOD_2D, FMOD_3D_WORLDRELATIVE, FMOD_3D_INVERSEROLLOFF */
        LoopOff = 0x00000001,  /* For non looping sounds. (default).  Overrides FMOD_LOOP_NORMAL / FMOD_LOOP_BIDI. */
        LoopNormal = 0x00000002,  /* For forward looping sounds. */
        LoopBidi = 0x00000004,  /* For bidirectional looping sounds. (only works on software mixed static sounds). */
        _2D = 0x00000008,  /* Ignores any 3d processing. (default). */
        _3D = 0x00000010,  /* Makes the sound positionable in 3D.  Overrides FMOD_2D. */
        CreateStream = 0x00000080,  /* Decompress at runtime, streaming from the source provided (standard stream).  Overrides FMOD_CREATESAMPLE. */
        CreateSample = 0x00000100,  /* Decompress at loadtime, decompressing or decoding whole file into memory as the target sample format. (standard sample). */
        CreateCompressedSample = 0x00000200,  /* Load MP2, MP3, IMAADPCM or XMA into memory and leave it compressed.  During playback the BreadPlayer.Fmod software mixer will decode it in realtime as a 'compressed sample'.  Can only be used in combination with FMOD_SOFTWARE. */
        OpenUser = 0x00000400,  /* Opens a user created static sample or stream. Use FMOD_CREATESOUNDEXINFO to specify format and/or read callbacks.  If a user created 'sample' is created with no read callback, the sample will be empty.  Use FMOD_Sound_Lock and FMOD_Sound_Unlock to place sound data into the sound if this is the case. */
        OpenMemory = 0x00000800,  /* "name_or_data" will be interpreted as a pointer to memory instead of filename for creating sounds. */
        OpenMemoryPoint = 0x10000000,  /* "name_or_data" will be interpreted as a pointer to memory instead of filename for creating sounds.  Use FMOD_CREATESOUNDEXINFO to specify length.  This differs to FMOD_OPENMEMORY in that it uses the memory as is, without duplicating the memory into its own buffers.  Cannot be freed after open, only after Sound::release.   Will not work if the data is compressed and FMOD_CREATECOMPRESSEDSAMPLE is not used. */
        OpenRaw = 0x00001000,  /* Will ignore file format and treat as raw pcm.  User may need to declare if data is FMOD_SIGNED or FMOD_UNSIGNED */
        OpenOnly = 0x00002000,  /* Just open the file, dont prebuffer or read.  Good for fast opens for info, or when sound::readData is to be used. */
        AccurateTime = 0x00004000,  /* For FMOD_CreateSound - for accurate FMOD_Sound_GetLength / FMOD_Channel_SetPosition on VBR MP3, AAC and MOD/S3M/XM/IT/MIDI files.  Scans file first, so takes longer to open. FMOD_OPENONLY does not affect this. */
        MpegSearch = 0x00008000,  /* For corrupted / bad MP3 files.  This will search all the way through the file until it hits a valid MPEG header.  Normally only searches for 4k. */
        Nonblocking = 0x00010000,  /* For opening sounds and getting streamed subsounds (seeking) asyncronously.  Use Sound::getOpenState to poll the state of the sound as it opens or retrieves the subsound in the background. */
        Unique = 0x00020000,  /* Unique sound, can only be played one at a time */
        _3DHeadrelative = 0x00040000,  /* Make the sound's position, velocity and orientation relative to the listener. */
        _3DWorldrelative = 0x00080000,  /* Make the sound's position, velocity and orientation absolute (relative to the world). (DEFAULT) */
        _3DInverserolloff = 0x00100000,  /* This sound will follow the inverse rolloff model where mindistance = full volume, maxdistance = where sound stops attenuating, and rolloff is fixed according to the global rolloff factor.  (DEFAULT) */
        _3DLinearrolloff = 0x00200000,  /* This sound will follow a linear rolloff model where mindistance = full volume, maxdistance = silence.  */
        _3DLinearsquarerolloff = 0x00400000,  /* This sound will follow a linear-square rolloff model where mindistance = full volume, maxdistance = silence.  Rolloffscale is ignored. */
        _3DInversetaperedrolloff = 0x00800000,  /* This sound will follow the inverse rolloff model at distances close to mindistance and a linear-square rolloff close to maxdistance. */
        _3DCustomrolloff = 0x04000000,  /* This sound will follow a rolloff model defined by Sound::set3DCustomRolloff / Channel::set3DCustomRolloff.  */
        _3DIgnoregeometry = 0x40000000,  /* Is not affect by geometry occlusion.  If not specified in Sound::setMode, or Channel::setMode, the flag is cleared and it is affected by geometry again. */
        IgnoreTags = 0x02000000,  /* Skips id3v2/asf/etc tag checks when opening a sound, to reduce seek/read overhead when opening files (helps with CD performance). */
        LowMem = 0x08000000,  /* Removes some features from samples to give a lower memory overhead, like Sound::getName. */
        LoadSecondaryRam = 0x20000000,  /* Load sound into the secondary RAM of supported platform.  On PS3, sounds will be loaded into RSX/VRAM. */
        VirtualPlayFromStart = 0x80000000   /* For sounds that start virtual (due to being quiet or low importance), instead of swapping back to audible, and playing at the correct offset according to time, this flag makes the sound play from the start. */
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
    public enum OpenState
    {
        Ready = 0,       /* Opened and ready to play */
        Loading,         /* Initial load in progress */
        Error,           /* Failed to open - file not found, out of memory etc.  See return value of Sound::getOpenState for what happened. */
        Connecting,      /* Connecting to remote host (internet sounds only) */
        Buffering,       /* Buffering data */
        Seeking,         /* Seeking to subsound and re-flushing stream buffer. */
        Playing,         /* Ready and playing, but not possible to release at this time without stalling the main thread. */
        Setposition,     /* Seeking within a stream to a different position. */

        Max             /* Maximum number of open state types. */
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
    public enum SoundGroupBehavior
    {
        BehaviorFail,              /* Any sound played that puts the sound count over the SoundGroup::setMaxAudible setting, will simply fail during FMODSystem::playSound. */
        BehaviorMute,              /* Any sound played that puts the sound count over the SoundGroup::setMaxAudible setting, will be silent, then if another sound in the group stops the sound that was silent before becomes audible again. */
        BehaviorSteallowest,       /* Any sound played that puts the sound count over the SoundGroup::setMaxAudible setting, will steal the quietest / least important sound playing in the group. */

        Max               /* Maximum number of sound group behaviors. */
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
        <b>Note!</b>  Currently the user must call FMODSystem::update for these callbacks to trigger!

        [SEE_ALSO]
        Channel::setCallback
        ChannelGroup::setCallback
        FMOD_CHANNELCONTROL_CALLBACK
        FMODSystem::update
    ]
    */
    public enum ChannelControlCallbackType
    {
        End,                  /* Called when a sound ends. */
        Virtualvoice,         /* Called when a voice is swapped out or swapped in. */
        Syncpoint,            /* Called when a syncpoint is encountered.  Can be from wav file markers. */
        Occlusion,            /* Called when the channel has its geometry occlusion value calculated.  Can be used to clamp or change the value. */

        Max                  /* Maximum number of callback types supported. */
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
        public const int Head = -1;         /* Head of the DSP chain. */
        public const int Fader = -2;         /* Built in fader DSP. */
        public const int Panner = -3;         /* Built in panner DSP. */
        public const int Tail = -4;         /* Tail of the DSP chain. */
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
        None,
        System,
        Channel,
        Channelgroup,
        Channelcontrol,
        Sound,
        Soundgroup,
        Dsp,
        Dspconnection,
        Geometry,
        Reverb3D,
        StudioSystem,
        StudioEventdescription,
        StudioEventinstance,
        StudioParameterinstance,
        StudioBus,
        StudioVca,
        StudioBank,
        StudioCommandreplay
    }

    /*
   [DEFINE]
   [
       [NAME]
       FMOD_SYSTEM_CALLBACK_TYPE

       [DESCRIPTION]
       These callback types are used with FMODSystem::setCallback.

       [REMARKS]
       Each callback has commanddata parameters passed as void* unique to the type of callback.<br>
       See reference to FMOD_SYSTEM_CALLBACK to determine what they might mean for each type of callback.<br>
       <br>
       <b>Note!</b> Using FMOD_SYSTEM_CALLBACK_DEVICELISTCHANGED (on Mac only) requires the application to be running an event loop which will allow external changes to device list to be detected by BreadPlayer.Fmod.<br>
       <br>
       <b>Note!</b> The 'system' object pointer will be null for FMOD_SYSTEM_CALLBACK_THREADCREATED and FMOD_SYSTEM_CALLBACK_MEMORYALLOCATIONFAILED callbacks.

       [SEE_ALSO]
       FMODSystem::setCallback
       FMODSystem::update
       DSP::addInput
   ]
   */
    [Flags]
    public enum SystemCallbackType : uint
    {
        Devicelistchanged = 0x00000001,  /* Called from FMODSystem::update when the enumerated list of devices has changed. */
        Devicelost = 0x00000002,  /* Called from FMODSystem::update when an output device has been lost due to control panel parameter changes and BreadPlayer.Fmod cannot automatically recover. */
        Memoryallocationfailed = 0x00000004,  /* Called directly when a memory allocation fails somewhere in BreadPlayer.Fmod.  (NOTE - 'system' will be NULL in this callback type.)*/
        Threadcreated = 0x00000008,  /* Called directly when a thread is created. (NOTE - 'system' will be NULL in this callback type.) */
        Baddspconnection = 0x00000010,  /* Called when a bad connection was made with DSP::addInput. Usually called from mixer thread because that is where the connections are made.  */
        Premix = 0x00000020,  /* Called each tick before a mix update happens. */
        Postmix = 0x00000040,  /* Called each tick after a mix update happens. */
        Error = 0x00000080,  /* Called when each API function returns an error code, including delayed async functions. */
        Midmix = 0x00000100,  /* Called each tick in mix update after clocks have been updated before the main mix occurs. */
        Threaddestroyed = 0x00000200,  /* Called directly when a thread is destroyed. */
        Preupdate = 0x00000400,  /* Called at start of FMODSystem::update function. */
        Postupdate = 0x00000800,  /* Called at end of FMODSystem::update function. */
        Recordlistchanged = 0x00001000,  /* Called from FMODSystem::update when the enumerated list of recording devices has changed. */
        All = 0xFFFFFFFF  /* Pass this mask to FMODSystem::setCallback to receive all callback types.  */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        List of interpolation types that the BreadPlayer.Fmod Ex software mixer supports.

        [REMARKS]
        The default resampler type is FMOD_DSP_RESAMPLER_LINEAR.<br>
        Use FMODSystem::setSoftwareFormat to tell BreadPlayer.Fmod the resampling quality you require for FMOD_SOFTWARE based sounds.

        [SEE_ALSO]
        FMODSystem::setSoftwareFormat
        FMODSystem::getSoftwareFormat
    ]
    */
    public enum DspResampler
    {
        Default,         /* Default interpolation method.  Currently equal to FMOD_DSP_RESAMPLER_LINEAR. */
        Nointerp,        /* No interpolation.  High frequency aliasing hiss will be audible depending on the sample rate of the sound. */
        Linear,          /* Linear interpolation (default method).  Fast and good quality, causes very slight lowpass effect on low frequency sounds. */
        Cubic,           /* Cubic interpolation.  Slower than linear interpolation but better quality. */
        Spline,          /* 5 point spline interpolation.  Slowest resampling method but best quality. */

        Max             /* Maximum number of resample methods supported. */
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
    public enum DspConnectionType
    {
        Standard,          /* Default connection type.         Audio is mixed from the input to the output DSP's audible buffer.  */
        Sidechain,         /* Sidechain connection type.       Audio is mixed from the input to the output DSP's sidechain buffer.  */
        Send,              /* Send connection type.            Audio is mixed from the input to the output DSP's audible buffer, but the input is NOT executed, only copied from.  A standard connection or sidechain needs to make an input execute to generate data. */
        SendSidechain,    /* Send sidechain connection type.  Audio is mixed from the input to the output DSP's sidechain buffer, but the input is NOT executed, only copied from.  A standard connection or sidechain needs to make an input execute to generate data. */

        Max               /* Maximum number of DSP connection types supported. */
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
    public enum TagType
    {
        Unknown = 0,
        Id3V1,
        Id3V2,
        Vorbiscomment,
        Shoutcast,
        Icecast,
        Asf,
        Midi,
        Playlist,
        Fmod,
        User,

        Max                /* Maximum number of tag types supported. */
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
    public enum TagDataType
    {
        Binary = 0,
        Int,
        Float,
        String,
        StringUtf16,
        StringUtf16Be,
        StringUtf8,
        Cdtoc,

        Max                /* Maximum number of tag datatypes supported. */
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
            FMODSystem::getRecordDriverInfo
        ]
        */
    [Flags]
    public enum DriverState : uint
    {
        Connected = 0x00000001, /* Device is currently plugged in. */
        Default = 0x00000002 /* Device is the users preferred choice. */
    }

}
