using System;
using System.Runtime.InteropServices;
using System.Text;
using BreadPlayer.Fmod.Enums;
using static BreadPlayer.Fmod.Callbacks;

namespace BreadPlayer.Fmod.Structs
{
    /*
    [STRUCTURE]
    [
        [DESCRIPTION]
        Structure describing a point in 3D space.

        [REMARKS]
        BreadPlayer.Fmod uses a left handed co-ordinate system by default.
        To use a right handed co-ordinate system specify FMOD_INIT_3D_RIGHTHANDED from FMOD_INITFLAGS in FMODSystem::init.

        [SEE_ALSO]
        FMODSystem::set3DListenerAttributes
        FMODSystem::get3DListenerAttributes
        Channel::set3DAttributes
        Channel::get3DAttributes
        Geometry::addPolygon
        Geometry::setPolygonVertex
        Geometry::getPolygonVertex
        Geometry::setRotation
        Geometry::getRotation
        Geometry::setPosition
        Geometry::getPosition
        Geometry::setScale
        Geometry::getScale
        FMOD_INITFLAGS
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector
    {
        public float x;        /* X co-ordinate in 3D space. */
        public float y;        /* Y co-ordinate in 3D space. */
        public float z;        /* Z co-ordinate in 3D space. */
    }

    /*
    [STRUCTURE]
    [
        [DESCRIPTION]
        Structure describing a position, velocity and orientation.

        [REMARKS]

        [SEE_ALSO]
        FMOD_VECTOR
        FMOD_DSP_PARAMETER_3DATTRIBUTES
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct _3DAttributes
    {
        private Vector position;
        private Vector velocity;
        private Vector forward;
        private Vector up;
    }

    /*
    [STRUCTURE]
    [
        [DESCRIPTION]
        Structure that is passed into FMOD_FILE_ASYNCREAD_CALLBACK.  Use the information in this structure to perform

        [REMARKS]
        Members marked with [r] mean the variable is modified by BreadPlayer.Fmod and is for reading purposes only.  Do not change this value.<br>
        Members marked with [w] mean the variable can be written to.  The user can set the value.<br>
        <br>
        Instructions: write to 'buffer', and 'bytesread' <b>BEFORE</b> setting 'result'.<br>
        As soon as result is set, BreadPlayer.Fmod will asynchronously continue internally using the data provided in this structure.<br>
        <br>
        Set 'result' to the result expected from a normal file read callback.<br>
        If the read was successful, set it to FMOD_OK.<br>
        If it read some data but hit the end of the file, set it to FMOD_ERR_FILE_EOF.<br>
        If a bad error occurred, return FMOD_ERR_FILE_BAD<br>
        If a disk was ejected, return FMOD_ERR_FILE_DISKEJECTED.<br>

        [SEE_ALSO]
        FMOD_FILE_ASYNCREAD_CALLBACK
        FMOD_FILE_ASYNCCANCEL_CALLBACK
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct AsyncReadInfo
    {
        public IntPtr handle;                     /* [r] The file handle that was filled out in the open callback. */
        public uint offset;                     /* [r] Seek position, make sure you read from this file offset. */
        public uint sizebytes;                  /* [r] how many bytes requested for read. */
        public int priority;                   /* [r] 0 = low importance.  100 = extremely important (ie 'must read now or stuttering may occur') */

        public IntPtr userdata;                   /* [r] User data pointer. */
        public IntPtr buffer;                     /* [w] Buffer to read file data into. */
        public uint bytesread;                  /* [w] Fill this in before setting result code to tell BreadPlayer.Fmod how many bytes were read. */
        public AsyncreadinfoDoneCallback done;  /* [r] BreadPlayer.Fmod file system wake up function.  Call this when user file read is finished.  Pass result of file read as a parameter. */
    }

    /*
   [STRUCTURE]
   [
       [DESCRIPTION]
       Used to support lists of plugins within the one file.

       [REMARKS]
       The description field is either a pointer to FMOD_DSP_DESCRIPTION, FMOD_OUTPUT_DESCRIPTION, FMOD_CODEC_DESCRIPTION.

       This structure is returned from a plugin as a pointer to a list where the last entry has FMOD_PLUGINTYPE_MAX and
       a null description pointer.

       [SEE_ALSO]
       FMODSystem::getNumNestedPlugins
       FMODSystem::getNestedPlugin
   ]
   */
    [StructLayout(LayoutKind.Sequential)]
    public struct PluginList
    {
        private PluginType type;
        private IntPtr description;
    }

    /*
    [STRUCTURE]
    [
        [DESCRIPTION]
        Structure that is passed into FMOD_SYSTEM_CALLBACK for the FMOD_SYSTEM_CALLBACK_ERROR callback type.

        [REMARKS]
        The instance pointer will be a type corresponding to the instanceType enum.

        [SEE_ALSO]
        FMOD_ERRORCALLBACK_INSTANCETYPE
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct ErrorCallbackInfo
    {
        public Result result;                     /* Error code result */
        public ErrorCallbackInstancetype instancetype;               /* Type of instance the error occurred on */
        public IntPtr instance;                   /* Instance pointer */
        private IntPtr functionname_internal;      /* Function that the error occurred on */
        private IntPtr functionparams_internal;    /* Function parameters that the error ocurred on */

        public string Functionname => Marshal.PtrToStringAnsi(functionname_internal);
        public string Functionparams => Marshal.PtrToStringAnsi(functionparams_internal);
    }

    /*
    [STRUCTURE]
    [
        [DESCRIPTION]
        Structure describing a piece of tag data.

        [REMARKS]
        Members marked with [w] mean the user sets the value before passing it to the function.
        Members marked with [r] mean BreadPlayer.Fmod sets the value to be used after the function exits.

        [SEE_ALSO]
        Sound::getTag
        TAGTYPE
        TAGDATATYPE
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct Tag
    {
        public TagType type;         /* [r] The type of this tag. */
        public TagDataType datatype;     /* [r] The type of data that this tag contains */
        private IntPtr name_internal;/* [r] The name of this tag i.e. "TITLE", "ARTIST" etc. */
        public IntPtr data;         /* [r] Pointer to the tag data - its format is determined by the datatype member */
        public uint datalen;      /* [r] Length of the data contained in this tag */
        public bool updated;      /* [r] True if this tag has been updated since last being accessed with Sound::getTag */

        public string Name => Marshal.PtrToStringAnsi(name_internal);
    }
    /*
    [DEFINE]
    [
        [NAME]
        FMOD_TIMEUNIT

        [DESCRIPTION]
        List of time types that can be returned by Sound::getLength and used with Channel::setPosition or Channel::getPosition.

        [REMARKS]
        Do not combine flags except FMOD_TIMEUNIT_BUFFERED.

        [SEE_ALSO]
        Sound::getLength
        Channel::setPosition
        Channel::getPosition
    ]
    */
    [Flags]
    public enum TimeUnit : uint
    {
        Ms = 0x00000001,  /* Milliseconds. */
        Pcm = 0x00000002,  /* PCM Samples, related to milliseconds * samplerate / 1000. */
        Pcmbytes = 0x00000004,  /* Bytes, related to PCM samples * channels * datawidth (ie 16bit = 2 bytes). */
        Rawbytes = 0x00000008,  /* Raw file bytes of (compressed) sound data (does not include headers).  Only used by Sound::getLength and Channel::getPosition. */
        Pcmfraction = 0x00000010,  /* Fractions of 1 PCM sample.  Unsigned int range 0 to 0xFFFFFFFF.  Used for sub-sample granularity for DSP purposes. */
        Modorder = 0x00000100,  /* MOD/S3M/XM/IT.  Order in a sequenced module format.  Use Sound::getFormat to determine the format. */
        Modrow = 0x00000200,  /* MOD/S3M/XM/IT.  Current row in a sequenced module format.  Sound::getLength will return the number if rows in the currently playing or seeked to pattern. */
        Modpattern = 0x00000400,  /* MOD/S3M/XM/IT.  Current pattern in a sequenced module format.  Sound::getLength will return the number of patterns in the song and Channel::getPosition will return the currently playing pattern. */
        Buffered = 0x10000000  /* Time value as seen by buffered stream.  This is always ahead of audible time, and is only used for processing. */
    }
    /*
       [DEFINE]
       [
           [NAME]
           FMOD_PORT_INDEX

           [DESCRIPTION]

           [REMARKS]

           [SEE_ALSO]
           FMODSystem::AttachChannelGroupToPort
       ]
       */
    public struct PortIndex
    {
        public const ulong None = 0xFFFFFFFFFFFFFFFF;
    }

    /*
    [STRUCTURE]
    [
        [DESCRIPTION]
        Use this structure with FMODSystem::createSound when more control is needed over loading.
        The possible reasons to use this with FMODSystem::createSound are:

        - Loading a file from memory.
        - Loading a file from within another larger (possibly wad/pak) file, by giving the loader an offset and length.
        - To create a user created / non file based sound.
        - To specify a starting subsound to seek to within a multi-sample sounds (ie FSB/DLS) when created as a stream.
        - To specify which subsounds to load for multi-sample sounds (ie FSB/DLS) so that memory is saved and only a subset is actually loaded/read from disk.
        - To specify 'piggyback' read and seek callbacks for capture of sound data as fmod reads and decodes it.  Useful for ripping decoded PCM data from sounds as they are loaded / played.
        - To specify a MIDI DLS sample set file to load when opening a MIDI file.

        See below on what members to fill for each of the above types of sound you want to create.

        [REMARKS]
        This structure is optional!  Specify 0 or NULL in FMODSystem::createSound if you don't need it!

        <u>Loading a file from memory.</u>

        - Create the sound using the FMOD_OPENMEMORY flag.
        - Mandatory.  Specify 'length' for the size of the memory block in bytes.
        - Other flags are optional.

        <u>Loading a file from within another larger (possibly wad/pak) file, by giving the loader an offset and length.</u>

        - Mandatory.  Specify 'fileoffset' and 'length'.
        - Other flags are optional.

        <u>To create a user created / non file based sound.</u>

        - Create the sound using the FMOD_OPENUSER flag.
        - Mandatory.  Specify 'defaultfrequency, 'numchannels' and 'format'.
        - Other flags are optional.

        <u>To specify a starting subsound to seek to and flush with, within a multi-sample stream (ie FSB/DLS).</u>

        - Mandatory.  Specify 'initialsubsound'.

        <u>To specify which subsounds to load for multi-sample sounds (ie FSB/DLS) so that memory is saved and only a subset is actually loaded/read from disk.</u>

        - Mandatory.  Specify 'inclusionlist' and 'inclusionlistnum'.

        <u>To specify 'piggyback' read and seek callbacks for capture of sound data as fmod reads and decodes it.  Useful for ripping decoded PCM data from sounds as they are loaded / played.</u>

        - Mandatory.  Specify 'pcmreadcallback' and 'pcmseekcallback'.

        <u>To specify a MIDI DLS sample set file to load when opening a MIDI file.</u>

        - Mandatory.  Specify 'dlsname'.

        Setting the 'decodebuffersize' is for cpu intensive codecs that may be causing stuttering, not file intensive codecs (ie those from CD or netstreams) which are normally
        altered with FMODSystem::setStreamBufferSize.  As an example of cpu intensive codecs, an mp3 file will take more cpu to decode than a PCM wav file.

        If you have a stuttering effect, then it is using more cpu than the decode buffer playback rate can keep up with.  Increasing the decode buffersize will most likely solve this problem.

        FSB codec.  If inclusionlist and numsubsounds are used together, this will trigger a special mode where subsounds are shuffled down to save memory.  (useful for large FSB
        files where you only want to load 1 sound).  There will be no gaps, ie no null subsounds.  As an example, if there are 10,000 subsounds and there is an inclusionlist with only 1 entry,
        and numsubsounds = 1, then subsound 0 will be that entry, and there will only be the memory allocated for 1 subsound.  Previously there would still be 10,000 subsound pointers and other
        associated codec entries allocated along with it multiplied by 10,000.

        Members marked with [r] mean the variable is modified by BreadPlayer.Fmod and is for reading purposes only.  Do not change this value.<br>
        Members marked with [w] mean the variable can be written to.  The user can set the value.

        [SEE_ALSO]
        FMODSystem::createSound
        FMODSystem::setStreamBufferSize
        FMOD_MODE
        FMOD_SOUND_FORMAT
        FMOD_SOUND_TYPE
        FMOD_CHANNELMASK
        FMOD_CHANNELORDER
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct CreateSoundExInfo
    {
        public int cbsize;                 /* [w]   Size of this structure.  This is used so the structure can be expanded in the future and still work on older versions of BreadPlayer.Fmod Ex. */
        public uint length;                 /* [w]   Optional. Specify 0 to ignore. Size in bytes of file to load, or sound to create (in this case only if FMOD_OPENUSER is used).  Required if loading from memory.  If 0 is specified, then it will use the size of the file (unless loading from memory then an error will be returned). */
        public uint fileoffset;             /* [w]   Optional. Specify 0 to ignore. Offset from start of the file to start loading from.  This is useful for loading files from inside big data files. */
        public int numchannels;            /* [w]   Optional. Specify 0 to ignore. Number of channels in a sound specified only if OPENUSER is used. */
        public int defaultfrequency;       /* [w]   Optional. Specify 0 to ignore. Default frequency of sound in a sound specified only if OPENUSER is used.  Other formats use the frequency determined by the file format. */
        public SoundFormat format;                 /* [w]   Optional. Specify 0 or SOUND_FORMAT_NONE to ignore. Format of the sound specified only if OPENUSER is used.  Other formats use the format determined by the file format.   */
        public uint decodebuffersize;       /* [w]   Optional. Specify 0 to ignore. For streams.  This determines the size of the double buffer (in PCM samples) that a stream uses.  Use this for user created streams if you want to determine the size of the callback buffer passed to you.  Specify 0 to use BreadPlayer.Fmod's default size which is currently equivalent to 400ms of the sound format created/loaded. */
        public int initialsubsound;        /* [w]   Optional. Specify 0 to ignore. In a multi-sample file format such as .FSB/.DLS/.SF2, specify the initial subsound to seek to, only if CREATESTREAM is used. */
        public int numsubsounds;           /* [w]   Optional. Specify 0 to ignore or have no subsounds.  In a user created multi-sample sound, specify the number of subsounds within the sound that are accessable with Sound::getSubSound / SoundGetSubSound. */
        public IntPtr inclusionlist;          /* [w]   Optional. Specify 0 to ignore. In a multi-sample format such as .FSB/.DLS/.SF2 it may be desirable to specify only a subset of sounds to be loaded out of the whole file.  This is an array of subsound indicies to load into memory when created. */
        public int inclusionlistnum;       /* [w]   Optional. Specify 0 to ignore. This is the number of integers contained within the */
        public SoundPcmreadcallback pcmreadcallback;        /* [w]   Optional. Specify 0 to ignore. Callback to 'piggyback' on BreadPlayer.Fmod's read functions and accept or even write PCM data while BreadPlayer.Fmod is opening the sound.  Used for user sounds created with OPENUSER or for capturing decoded data as BreadPlayer.Fmod reads it. */
        public SoundPcmsetposcallback pcmsetposcallback;      /* [w]   Optional. Specify 0 to ignore. Callback for when the user calls a seeking function such as Channel::setPosition within a multi-sample sound, and for when it is opened.*/
        public SoundNonblockcallback nonblockcallback;       /* [w]   Optional. Specify 0 to ignore. Callback for successful completion, or error while loading a sound that used the FMOD_NONBLOCKING flag.*/
        public IntPtr dlsname;                /* [w]   Optional. Specify 0 to ignore. Filename for a DLS or SF2 sample set when loading a MIDI file.   If not specified, on windows it will attempt to open /windows/system32/drivers/gm.dls, otherwise the MIDI will fail to open.  */
        public IntPtr encryptionkey;          /* [w]   Optional. Specify 0 to ignore. Key for encrypted FSB file.  Without this key an encrypted FSB file will not load. */
        public int maxpolyphony;           /* [w]   Optional. Specify 0 to ingore. For sequenced formats with dynamic channel allocation such as .MID and .IT, this specifies the maximum voice count allowed while playing.  .IT defaults to 64.  .MID defaults to 32. */
        public IntPtr userdata;               /* [w]   Optional. Specify 0 to ignore. This is user data to be attached to the sound during creation.  Access via Sound::getUserData. */
        public SoundType suggestedsoundtype;     /* [w]   Optional. Specify 0 or FMOD_SOUND_TYPE_UNKNOWN to ignore.  Instead of scanning all codec types, use this to speed up loading by making it jump straight to this codec. */
        public FileOpencallback fileuseropen;           /* [w]   Optional. Specify 0 to ignore. Callback for opening this file. */
        public FileClosecallback fileuserclose;          /* [w]   Optional. Specify 0 to ignore. Callback for closing this file. */
        public FileReadcallback fileuserread;           /* [w]   Optional. Specify 0 to ignore. Callback for reading from this file. */
        public FileSeekcallback fileuserseek;           /* [w]   Optional. Specify 0 to ignore. Callback for seeking within this file. */
        public FileAsyncreadcallback fileuserasyncread;      /* [w]   Optional. Specify 0 to ignore. Callback for asyncronously reading from this file. */
        public FileAsynccancelcallback fileuserasynccancel;    /* [w]   Optional. Specify 0 to ignore. Callback for cancelling an asyncronous read. */
        public IntPtr fileuserdata;           /* [w]   Optional. Specify 0 to ignore. User data to be passed into the file callbacks. */
        public int filebuffersize;         /* [w]   Optional. Specify 0 to ignore. Buffer size for reading the file, -1 to disable buffering, or 0 for system default. */
        public ChannelOrder channelorder;           /* [w]   Optional. Specify 0 to ignore. Use this to differ the way fmod maps multichannel sounds to speakers.  See FMOD_CHANNELORDER for more. */
        public ChannelMask channelmask;            /* [w]   Optional. Specify 0 to ignore. Use this to differ the way fmod maps multichannel sounds to speakers.  See FMOD_CHANNELMASK for more. */
        public IntPtr initialsoundgroup;      /* [w]   Optional. Specify 0 to ignore. Specify a sound group if required, to put sound in as it is created. */
        public uint initialseekposition;    /* [w]   Optional. Specify 0 to ignore. For streams. Specify an initial position to seek the stream to. */
        public TimeUnit initialseekpostype;     /* [w]   Optional. Specify 0 to ignore. For streams. Specify the time unit for the position set in initialseekposition. */
        public int ignoresetfilesystem;    /* [w]   Optional. Specify 0 to ignore. Set to 1 to use fmod's built in file system. Ignores setFileSystem callbacks and also FMOD_CREATESOUNEXINFO file callbacks.  Useful for specific cases where you don't want to use your own file system but want to use fmod's file system (ie net streaming). */
        public uint audioqueuepolicy;       /* [w]   Optional. Specify 0 or FMOD_AUDIOQUEUE_CODECPOLICY_DEFAULT to ignore. Policy used to determine whether hardware or software is used for decoding, see FMOD_AUDIOQUEUE_CODECPOLICY for options (iOS >= 3.0 required, otherwise only hardware is available) */
        public uint minmidigranularity;     /* [w]   Optional. Specify 0 to ignore. Allows you to set a minimum desired MIDI mixer granularity. Values smaller than 512 give greater than default accuracy at the cost of more CPU and vise versa. Specify 0 for default (512 samples). */
        public int nonblockthreadid;       /* [w]   Optional. Specify 0 to ignore. Specifies a thread index to execute non blocking load on.  Allows for up to 5 threads to be used for loading at once.  This is to avoid one load blocking another.  Maximum value = 4. */
        public IntPtr fsbguid;                /* [r/w] Optional. Specify 0 to ignore. Allows you to provide the GUID lookup for cached FSB header info. Once loaded the GUID will be written back to the pointer. This is to avoid seeking and reading the FSB header. */
    }
    /*
    [STRUCTURE]
    [
        [DESCRIPTION]
        Structure defining a reverb environment for FMOD_SOFTWARE based sounds only.<br>

        [REMARKS]
        Note the default reverb properties are the same as the FMOD_PRESET_GENERIC preset.<br>
        Note that integer values that typically range from -10,000 to 1000 are represented in decibels,
        and are of a logarithmic scale, not linear, wheras float values are always linear.<br>
        <br>
        The numerical values listed below are the maximum, minimum and default values for each variable respectively.<br>
        <br>
        Hardware voice / Platform Specific reverb support.<br>
        WII   See FMODWII.H for hardware specific reverb functionality.<br>
        3DS   See FMOD3DS.H for hardware specific reverb functionality.<br>
        PSP   See FMODWII.H for hardware specific reverb functionality.<br>
        <br>
        Members marked with [r] mean the variable is modified by BreadPlayer.Fmod and is for reading purposes only.  Do not change this value.<br>
        Members marked with [w] mean the variable can be written to.  The user can set the value.<br>
        Members marked with [r/w] are either read or write depending on if you are using FMODSystem::setReverbProperties (w) or FMODSystem::getReverbProperties (r).

        [SEE_ALSO]
        FMODSystem::setReverbProperties
        FMODSystem::getReverbProperties
        FMOD_REVERB_PRESETS
    ]
    */
#pragma warning disable 414
    [StructLayout(LayoutKind.Sequential)]
    public struct ReverbProperties
    {                            /*        MIN     MAX    DEFAULT   DESCRIPTION */
        public float DecayTime;         /* [r/w]  0.0    20000.0 1500.0  Reverberation decay time in ms                                        */
        public float EarlyDelay;        /* [r/w]  0.0    300.0   7.0     Initial reflection delay time                                         */
        public float LateDelay;         /* [r/w]  0.0    100     11.0    Late reverberation delay time relative to initial reflection          */
        public float HFReference;       /* [r/w]  20.0   20000.0 5000    Reference high frequency (hz)                                         */
        public float HFDecayRatio;      /* [r/w]  10.0   100.0   50.0    High-frequency to mid-frequency decay time ratio                      */
        public float Diffusion;         /* [r/w]  0.0    100.0   100.0   Value that controls the echo density in the late reverberation decay. */
        public float Density;           /* [r/w]  0.0    100.0   100.0   Value that controls the modal density in the late reverberation decay */
        public float LowShelfFrequency; /* [r/w]  20.0   1000.0  250.0   Reference low frequency (hz)                                          */
        public float LowShelfGain;      /* [r/w]  -36.0  12.0    0.0     Relative room effect level at low frequencies                         */
        public float HighCut;           /* [r/w]  20.0   20000.0 20000.0 Relative room effect level at high frequencies                        */
        public float EarlyLateMix;      /* [r/w]  0.0    100.0   50.0    Early reflections level relative to room effect                       */
        public float WetLevel;          /* [r/w]  -80.0  20.0    -6.0    Room effect level (at mid frequencies)
                                  * */
        #region wrapperinternal
        public ReverbProperties(float decayTime, float earlyDelay, float lateDelay, float hfReference,
            float hfDecayRatio, float diffusion, float density, float lowShelfFrequency, float lowShelfGain,
            float highCut, float earlyLateMix, float wetLevel)
        {
            DecayTime = decayTime;
            EarlyDelay = earlyDelay;
            LateDelay = lateDelay;
            HFReference = hfReference;
            HFDecayRatio = hfDecayRatio;
            Diffusion = diffusion;
            Density = density;
            LowShelfFrequency = lowShelfFrequency;
            LowShelfGain = lowShelfGain;
            HighCut = highCut;
            EarlyLateMix = earlyLateMix;
            WetLevel = wetLevel;
        }
        #endregion
    }

    #region wrapperinternal
    [StructLayout(LayoutKind.Sequential)]
    public struct StringWrapper
    {
        private IntPtr nativeUtf8Ptr;

        public static implicit operator string(StringWrapper fstring)
        {
            if (fstring.nativeUtf8Ptr == IntPtr.Zero)
            {
                return "";
            }

            int strlen = 0;
            while (Marshal.ReadByte(fstring.nativeUtf8Ptr, strlen) != 0)
            {
                strlen++;
            }
            if (strlen > 0)
            {
                byte[] bytes = new byte[strlen];
                Marshal.Copy(fstring.nativeUtf8Ptr, bytes, 0, strlen);
                return Encoding.UTF8.GetString(bytes, 0, strlen);
            }
            return "";
        }
    }
    #endregion
    /*
        [STRUCTURE]
        [
            [DESCRIPTION]
            Settings for advanced features like configuring memory and cpu usage for the FMOD_CREATECOMPRESSEDSAMPLE feature.

            [REMARKS]
            maxMPEGCodecs / maxADPCMCodecs / maxXMACodecs will determine the maximum cpu usage of playing realtime samples.  Use this to lower potential excess cpu usage and also control memory usage.<br>

            [SEE_ALSO]
            FMODSystem::setAdvancedSettings
            FMODSystem::getAdvancedSettings
        ]
        */
    [StructLayout(LayoutKind.Sequential)]
    public struct AdvancedSettings
    {
        public int cbSize;                     /* [w]   Size of this structure.  Use sizeof(FMOD_ADVANCEDSETTINGS) */
        public int maxMPEGCodecs;              /* [r/w] Optional. Specify 0 to ignore. For use with FMOD_CREATECOMPRESSEDSAMPLE only.  MPEG   codecs consume 30,528 bytes per instance and this number will determine how many MPEG   channels can be played simultaneously. Default = 32. */
        public int maxADPCMCodecs;             /* [r/w] Optional. Specify 0 to ignore. For use with FMOD_CREATECOMPRESSEDSAMPLE only.  ADPCM  codecs consume  3,128 bytes per instance and this number will determine how many ADPCM  channels can be played simultaneously. Default = 32. */
        public int maxXMACodecs;               /* [r/w] Optional. Specify 0 to ignore. For use with FMOD_CREATECOMPRESSEDSAMPLE only.  XMA    codecs consume 14,836 bytes per instance and this number will determine how many XMA    channels can be played simultaneously. Default = 32. */
        public int maxVorbisCodecs;            /* [r/w] Optional. Specify 0 to ignore. For use with FMOD_CREATECOMPRESSEDSAMPLE only.  Vorbis codecs consume 23,256 bytes per instance and this number will determine how many Vorbis channels can be played simultaneously. Default = 32. */
        public int maxAT9Codecs;               /* [r/w] Optional. Specify 0 to ignore. For use with FMOD_CREATECOMPRESSEDSAMPLE only.  AT9    codecs consume  8,720 bytes per instance and this number will determine how many AT9    channels can be played simultaneously. Default = 32. */
        public int maxFADPCMCodecs;            /* [r/w] Optional. Specify 0 to ignore. For use with FMOD_CREATECOMPRESSEDSAMPLE only.  This number will determine how many FADPCM channels can be played simultaneously. Default = 32. */
        public int maxPCMCodecs;               /* [r/w] Optional. Specify 0 to ignore. For use with PS3 only.                          PCM    codecs consume 12,672 bytes per instance and this number will determine how many streams and PCM voices can be played simultaneously. Default = 16. */
        public int ASIONumChannels;            /* [r/w] Optional. Specify 0 to ignore. Number of channels available on the ASIO device. */
        public IntPtr ASIOChannelList;            /* [r/w] Optional. Specify 0 to ignore. Pointer to an array of strings (number of entries defined by ASIONumChannels) with ASIO channel names. */
        public IntPtr ASIOSpeakerList;            /* [r/w] Optional. Specify 0 to ignore. Pointer to a list of speakers that the ASIO channels map to.  This can be called after FMODSystem::init to remap ASIO output. */
        public float HRTFMinAngle;               /* [r/w] Optional.                      For use with FMOD_INIT_HRTF_LOWPASS.  The angle range (0-360) of a 3D sound in relation to the listener, at which the HRTF function begins to have an effect. 0 = in front of the listener. 180 = from 90 degrees to the left of the listener to 90 degrees to the right. 360 = behind the listener. Default = 180.0. */
        public float HRTFMaxAngle;               /* [r/w] Optional.                      For use with FMOD_INIT_HRTF_LOWPASS.  The angle range (0-360) of a 3D sound in relation to the listener, at which the HRTF function has maximum effect. 0 = front of the listener. 180 = from 90 degrees to the left of the listener to 90 degrees to the right. 360 = behind the listener. Default = 360.0. */
        public float HRTFFreq;                   /* [r/w] Optional. Specify 0 to ignore. For use with FMOD_INIT_HRTF_LOWPASS.  The cutoff frequency of the HRTF's lowpass filter function when at maximum effect. (i.e. at HRTFMaxAngle).  Default = 4000.0. */
        public float vol0virtualvol;             /* [r/w] Optional. Specify 0 to ignore. For use with FMOD_INIT_VOL0_BECOMES_VIRTUAL.  If this flag is used, and the volume is below this, then the sound will become virtual.  Use this value to raise the threshold to a different point where a sound goes virtual. */
        public uint defaultDecodeBufferSize;    /* [r/w] Optional. Specify 0 to ignore. For streams. This determines the default size of the double buffer (in milliseconds) that a stream uses.  Default = 400ms */
        public ushort profilePort;                /* [r/w] Optional. Specify 0 to ignore. For use with FMOD_INIT_PROFILE_ENABLE.  Specify the port to listen on for connections by the profiler application. */
        public uint geometryMaxFadeTime;        /* [r/w] Optional. Specify 0 to ignore. The maximum time in miliseconds it takes for a channel to fade to the new level when its occlusion changes. */
        public float distanceFilterCenterFreq;   /* [r/w] Optional. Specify 0 to ignore. For use with FMOD_INIT_DISTANCE_FILTERING.  The default center frequency in Hz for the distance filtering effect. Default = 1500.0. */
        public int reverb3Dinstance;           /* [r/w] Optional. Specify 0 to ignore. Out of 0 to 3, 3d reverb spheres will create a phyical reverb unit on this instance slot.  See FMOD_REVERB_PROPERTIES. */
        public int DSPBufferPoolSize;          /* [r/w] Optional. Specify 0 to ignore. Number of buffers in DSP buffer pool.  Each buffer will be DSPBlockSize * sizeof(float) * SpeakerModeChannelCount.  ie 7.1 @ 1024 DSP block size = 8 * 1024 * 4 = 32kb.  Default = 8. */
        public uint stackSizeStream;            /* [r/w] Optional. Specify 0 to ignore. Specify the stack size for the BreadPlayer.Fmod Stream thread in bytes.  Useful for custom codecs that use excess stack.  Default 49,152 (48kb) */
        public uint stackSizeNonBlocking;       /* [r/w] Optional. Specify 0 to ignore. Specify the stack size for the FMOD_NONBLOCKING loading thread.  Useful for custom codecs that use excess stack.  Default 65,536 (64kb) */
        public uint stackSizeMixer;             /* [r/w] Optional. Specify 0 to ignore. Specify the stack size for the BreadPlayer.Fmod mixer thread.  Useful for custom dsps that use excess stack.  Default 49,152 (48kb) */
        public DspResampler resamplerMethod;            /* [r/w] Optional. Specify 0 to ignore. Resampling method used with fmod's software mixer.  See FMOD_DSP_RESAMPLER for details on methods. */
        public uint commandQueueSize;           /* [r/w] Optional. Specify 0 to ignore. Specify the command queue size for thread safe processing.  Default 2048 (2kb) */
        public uint randomSeed;                 /* [r/w] Optional. Specify 0 to ignore. Seed value that BreadPlayer.Fmod will use to initialize its internal random number generators. */
    }
}
