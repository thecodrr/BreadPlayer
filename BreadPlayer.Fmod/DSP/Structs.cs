using System;
using System.Runtime.InteropServices;
using BreadPlayer.Fmod.Enums;
using BreadPlayer.Fmod.Structs;
using static BreadPlayer.Fmod.Callbacks;
using static BreadPlayer.Fmod.CoreDSP.Callbacks;

namespace BreadPlayer.Fmod.CoreDSP
{
    /*
     [STRUCTURE]
     [
         [DESCRIPTION]
         Structure for FMOD_DSP_PROCESS_CALLBACK input and output buffers.

         [REMARKS]
         Members marked with [r] mean the variable is modified by BreadPlayer.Fmod and is for reading purposes only.  Do not change this value.
         Members marked with [w] mean the variable can be written to.  The user can set the value.

         [SEE_ALSO]
         FMOD_DSP_DESCRIPTION
     ]
     */
    [StructLayout(LayoutKind.Sequential)]
    public struct DspBufferArray
    {
        public int numbuffers;              /* [r/w] number of buffers */
        public int[] buffernumchannels;       /* [r/w] array of number of channels for each buffer */
        public ChannelMask[] bufferchannelmask;       /* [r/w] array of channel masks for each buffer */
        public IntPtr[] buffers;                 /* [r/w] array of buffers */
        public SpeakerMode speakermode;             /* [r/w] speaker mode for all buffers in the array */
    }




    /*
    [STRUCTURE] 
    [
        [DESCRIPTION]
        Complex number structure used for holding FFT frequency domain-data for FMOD_FFTREAL and FMOD_IFFTREAL DSP callbacks.

        [REMARKS]

        [SEE_ALSO]    
        FMOD_DSP_STATE_SYSTEMCALLBACKS
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct Complex
    {
        public float real; /* Real component */
        public float imag; /* Imaginary component */
    }
    /*
   [STRUCTURE] 
   [
       [DESCRIPTION]
       Structure to define a piecewise linear mapping.

       [REMARKS]
       Members marked with [r] mean the variable is modified by BreadPlayer.Fmod and is for reading purposes only.  Do not change this value.
       Members marked with [w] mean the variable can be written to.  The user can set the value.

       [SEE_ALSO]    
       FMOD_DSP_PARAMETER_FLOAT_MAPPING_TYPE
       FMOD_DSP_PARAMETER_FLOAT_MAPPING
   ]
   */
    [StructLayout(LayoutKind.Sequential)]
    public struct DspParameterFloatMappingPiecewiseLinear
    {
        public int numpoints;                       /* [w] The number of <position, value> pairs in the piecewise mapping (at least 2). */
        public IntPtr pointparamvalues;             /* [w] The values in the parameter's units for each point */
        public IntPtr pointpositions;               /* [w] The positions along the control's scale (e.g. dial angle) corresponding to each parameter value.  The range of this scale is arbitrary and all positions will be relative to the minimum and maximum values (e.g. [0,1,3] is equivalent to [1,2,4] and [2,4,8]).  If this array is zero, pointparamvalues will be distributed with equal spacing. */
    }

    /*
    [STRUCTURE]
    [
        [DESCRIPTION]
        Structure to define a mapping for a DSP unit's float parameter.

        [REMARKS]
        Members marked with [r] mean the variable is modified by BreadPlayer.Fmod and is for reading purposes only.  Do not change this value.
        Members marked with [w] mean the variable can be written to.  The user can set the value.

        [SEE_ALSO]
        FMOD_DSP_PARAMETER_FLOAT_MAPPING_TYPE
        FMOD_DSP_PARAMETER_DESC_FLOAT
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct DspParameterFloatMapping
    {
        public DspParameterFloatMappingType type;
        public DspParameterFloatMappingPiecewiseLinear piecewiselinearmapping;    /* [w] Only required for FMOD_DSP_PARAMETER_FLOAT_MAPPING_TYPE_PIECEWISE_LINEAR type mapping. */
    }


    /*
    [STRUCTURE]
    [
        [DESCRIPTION]
        Structure to define a float parameter for a DSP unit.

        [REMARKS]
        Members marked with [r] mean the variable is modified by BreadPlayer.Fmod and is for reading purposes only.  Do not change this value.
        Members marked with [w] mean the variable can be written to.  The user can set the value.

        [SEE_ALSO]
        FMODSystem::createDSP
        DSP::setParameterFloat
        DSP::getParameterFloat
        FMOD_DSP_PARAMETER_DESC
        FMOD_DSP_PARAMETER_FLOAT_MAPPING
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct DspParameterDescFloat
    {
        public float min;                      /* [w] Minimum parameter value. */
        public float max;                      /* [w] Maximum parameter value. */
        public float defaultval;               /* [w] Default parameter value. */
        public DspParameterFloatMappingType mapping;           /* [w] How the values are distributed across dials and automation curves (e.g. linearly, exponentially etc). */
    }


    /*
    [STRUCTURE]
    [
        [DESCRIPTION]
        Structure to define a int parameter for a DSP unit.

        [REMARKS]
        Members marked with [r] mean the variable is modified by BreadPlayer.Fmod and is for reading purposes only.  Do not change this value.
        Members marked with [w] mean the variable can be written to.  The user can set the value.

        [SEE_ALSO]
        FMODSystem::createDSP
        DSP::setParameterInt
        DSP::getParameterInt
        FMOD_DSP_PARAMETER_DESC
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct DspParameterDescInt
    {
        public int min;                      /* [w] Minimum parameter value. */
        public int max;                      /* [w] Maximum parameter value. */
        public int defaultval;               /* [w] Default parameter value. */
        public bool goestoinf;                /* [w] Whether the last value represents infiniy. */
        public IntPtr valuenames;               /* [w] Names for each value.  There should be as many strings as there are possible values (max - min + 1).  Optional. */
    }


    /*
    [STRUCTURE]
    [
        [DESCRIPTION]
        Structure to define a boolean parameter for a DSP unit.

        [REMARKS]
        Members marked with [r] mean the variable is modified by BreadPlayer.Fmod and is for reading purposes only.  Do not change this value.
        Members marked with [w] mean the variable can be written to.  The user can set the value.

        [SEE_ALSO]
        FMODSystem::createDSP
        DSP::setParameterBool
        DSP::getParameterBool
        FMOD_DSP_PARAMETER_DESC
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct DspParameterDescBool
    {
        public bool defaultval;               /* [w] Default parameter value. */
        public IntPtr valuenames;               /* [w] Names for false and true, respectively.  There should be two strings.  Optional. */
    }


    /*
    [STRUCTURE]
    [
        [DESCRIPTION]
        Structure to define a data parameter for a DSP unit.  Use 0 or above for custom types.  This parameter will be treated specially by the system if set to one of the FMOD_DSP_PARAMETER_DATA_TYPE values.

        [REMARKS]
        Members marked with [r] mean the variable is modified by BreadPlayer.Fmod and is for reading purposes only.  Do not change this value.
        Members marked with [w] mean the variable can be written to.  The user can set the value.

        [SEE_ALSO]
        FMODSystem::createDSP
        DSP::setParameterData
        DSP::getParameterData
        FMOD_DSP_PARAMETER_DATA_TYPE
        FMOD_DSP_PARAMETER_DESC
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct DspParameterDescData
    {
        public int datatype;                 /* [w] The type of data for this parameter.  Use 0 or above for custom types or set to one of the FMOD_DSP_PARAMETER_DATA_TYPE values. */
    }


    /*
    [STRUCTURE]
    [
        [DESCRIPTION]

        [REMARKS]
        Members marked with [w] mean the user sets the value before passing it to the function.
        Members marked with [r] mean BreadPlayer.Fmod sets the value to be used after the function exits.
        
        The step parameter tells the gui or application that the parameter has a certain granularity.
        For example in the example of cutoff frequency with a range from 100.0 to 22050.0 you might only want the selection to be in 10hz increments.  For this you would simply use 10.0 as the step value.
        For a boolean, you can use min = 0.0, max = 1.0, step = 1.0.  This way the only possible values are 0.0 and 1.0.
        Some applications may detect min = 0.0, max = 1.0, step = 1.0 and replace a graphical slider bar with a checkbox instead.
        A step value of 1.0 would simulate integer values only.
        A step value of 0.0 would mean the full floating point range is accessable.

        [SEE_ALSO]
        FMODSystem::createDSP
        FMODSystem::getDSP
    ]
    */
    [StructLayout(LayoutKind.Explicit)]
    public struct DspParameterDescUnion
    {
        [FieldOffset(0)]
        public DspParameterDescFloat floatdesc;  /* [w] Struct containing information about the parameter in floating point format.  Use when type is FMOD_DSP_PARAMETER_TYPE_FLOAT. */
        [FieldOffset(0)]
        public DspParameterDescInt intdesc;    /* [w] Struct containing information about the parameter in integer format.  Use when type is FMOD_DSP_PARAMETER_TYPE_INT. */
        [FieldOffset(0)]
        public DspParameterDescBool booldesc;   /* [w] Struct containing information about the parameter in boolean format.  Use when type is FMOD_DSP_PARAMETER_TYPE_BOOL. */
        [FieldOffset(0)]
        public DspParameterDescData datadesc;   /* [w] Struct containing information about the parameter in data format.  Use when type is FMOD_DSP_PARAMETER_TYPE_DATA. */
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DspParameterDesc
    {
        public DspParameterType type;            /* [w] Type of this parameter. */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public char[] name;            /* [w] Name of the parameter to be displayed (ie "Cutoff frequency"). */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public char[] label;           /* [w] Short string to be put next to value to denote the unit type (ie "hz"). */
        public string description;     /* [w] Description of the parameter to be displayed as a help item / tooltip for this parameter. */

        public DspParameterDescUnion desc;
    }



    /*
    [STRUCTURE] 
    [
        [DESCRIPTION]
        Structure for data parameters of type FMOD_DSP_PARAMETER_DATA_TYPE_OVERALLGAIN.
        A parameter of this type is used in effects that affect the overgain of the signal in a predictable way.
        This parameter is read by the system to determine the effect's gain for voice virtualization.
    
        [REMARKS]
        Members marked with [r] mean the variable is modified by BreadPlayer.Fmod and is for reading purposes only.  Do not change this value.
        Members marked with [w] mean the variable can be written to.  The user can set the value.
    
        [SEE_ALSO]    
        FMOD_DSP_PARAMETER_DATA_TYPE
        FMOD_DSP_PARAMETER_DESC
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct DspParameterOverallgain
    {
        public float linear_gain;                                  /* [r] The overall linear gain of the effect on the direct signal path */
        public float linear_gain_additive;                         /* [r] Additive gain, for parallel signal paths */
    }


    /*
    [STRUCTURE] 
    [
        [DESCRIPTION]
        Structure for data parameters of type FMOD_DSP_PARAMETER_DATA_TYPE_3DATTRIBUTES.
        A parameter of this type is used in effects that respond to a sound's 3D position.
        The system will set this parameter automatically if a sound's position changes.
    
        [REMARKS]
        Members marked with [r] mean the variable is modified by BreadPlayer.Fmod and is for reading purposes only.  Do not change this value.
        Members marked with [w] mean the variable can be written to.  The user can set the value.
    
        [SEE_ALSO]    
        FMOD_DSP_PARAMETER_DATA_TYPE
        FMOD_DSP_PARAMETER_DESC
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct DspParameter3DAttributes
    {
        public _3DAttributes relative;                        /* [w] The position of the sound relative to the listener. */
        public _3DAttributes absolute;                        /* [w] The position of the sound in world coordinates. */
    }

    /*
    [STRUCTURE] 
    [
        [DESCRIPTION]
        Structure for data parameters of type FMOD_DSP_PARAMETER_DATA_TYPE_3DATTRIBUTES.
        A parameter of this type is used in effects that respond to a sound's 3D position.
        The system will set this parameter automatically if a sound's position changes.
    
        [REMARKS]
        Members marked with [r] mean the variable is modified by BreadPlayer.Fmod and is for reading purposes only.  Do not change this value.
        Members marked with [w] mean the variable can be written to.  The user can set the value.
    
        [SEE_ALSO]    
        FMOD_DSP_PARAMETER_DATA_TYPE
        FMOD_DSP_PARAMETER_DESC
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct DspParameter3DAttributesMulti
    {
        public int numlisteners;                    /* [w] The number of listeners. */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public _3DAttributes[] relative;                      /* [w] The position of the sound relative to the listeners. */
        public _3DAttributes absolute;                        /* [w] The position of the sound in world coordinates. */
    }

    /*
    [STRUCTURE] 
    [
        [DESCRIPTION]
        Structure for data parameters of type FMOD_DSP_PARAMETER_DATA_TYPE_SIDECHAIN.
        A parameter of this type is declared for effects which support sidechaining.
    
        [REMARKS]
        Members marked with [r] mean the variable is modified by BreadPlayer.Fmod and is for reading purposes only.  Do not change this value.
        Members marked with [w] mean the variable can be written to.  The user can set the value.
    
        [SEE_ALSO]    
        FMOD_DSP_PARAMETER_DATA_TYPE
        FMOD_DSP_PARAMETER_DESC
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct DspParameterSidechain
    {
        public int sidechainenable;                               /* [r/w] Whether sidechains are enabled. */
    }


    /*
    [STRUCTURE] 
    [
        [DESCRIPTION]
        Structure for data parameters of type FMOD_DSP_PARAMETER_DATA_TYPE_FFT.
        A parameter of this type is declared for the FMOD_DSP_TYPE_FFT effect.
    
        [REMARKS]
        Members marked with [r] mean the variable is modified by BreadPlayer.Fmod and is for reading purposes only.  Do not change this value.
        Members marked with [w] mean the variable can be written to.  The user can set the value.
        
        Notes on the spectrum data member.  Values inside the float buffer are typically between 0 and 1.0.
        Each top level array represents one PCM channel of data.
        Address data as spectrum[channel][bin].  A bin is 1 fft window entry.
        Only read/display half of the buffer typically for analysis as the 2nd half is usually the same data reversed due to the nature of the way FFT works.
    
        [SEE_ALSO]    
        FMOD_DSP_PARAMETER_DATA_TYPE
        FMOD_DSP_PARAMETER_DESC
        FMOD_DSP_PARAMETER_DATA_TYPE_FFT
        FMOD_DSP_TYPE
        FMOD_DSP_FFT
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct DspParameterFft
    {
        public int length;                                    /* [r] Number of entries in this spectrum window.   Divide this by the output rate to get the hz per entry. */
        public int numchannels;                               /* [r] Number of channels in spectrum. */

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        private IntPtr[] spectrum_internal;                           /* [r] Per channel spectrum arrays.  See remarks for more. */

        public float[][] Spectrum
        {
            get
            {
                var buffer = new float[numchannels][];

                for (int i = 0; i < numchannels; ++i)
                {
                    buffer[i] = new float[length];
                    Marshal.Copy(spectrum_internal[i], buffer[i], 0, length);
                }

                return buffer;
            }
        }
    }

    /*
    [STRUCTURE]
    [
        [DESCRIPTION]
        When creating a DSP unit, declare one of these and provide the relevant callbacks and name for BreadPlayer.Fmod to use when it creates and uses a DSP unit of this type.

        [REMARKS]
        Members marked with [r] mean the variable is modified by BreadPlayer.Fmod and is for reading purposes only.  Do not change this value.
        Members marked with [w] mean the variable can be written to.  The user can set the value.
        
        There are 2 different ways to change a parameter in this architecture.
        One is to use DSP::setParameterFloat / DSP::setParameterInt / DSP::setParameterBool / DSP::setParameterData.  This is platform independant and is dynamic, so new unknown plugins can have their parameters enumerated and used.
        The other is to use DSP::showConfigDialog.  This is platform specific and requires a GUI, and will display a dialog box to configure the plugin.

        [SEE_ALSO]    
        FMODSystem::createDSP
        DSP::setParameterFloat
        DSP::setParameterInt
        DSP::setParameterBool
        DSP::setParameterData
        FMOD_DSP_STATE
        FMOD_DSP_CREATE_CALLBACK
        FMOD_DSP_RELEASE_CALLBACK
        FMOD_DSP_RESET_CALLBACK
        FMOD_DSP_READ_CALLBACK
        FMOD_DSP_PROCESS_CALLBACK
        FMOD_DSP_SETPOSITION_CALLBACK
        FMOD_DSP_PARAMETER_DESC
        FMOD_DSP_SETPARAM_FLOAT_CALLBACK
        FMOD_DSP_SETPARAM_INT_CALLBACK
        FMOD_DSP_SETPARAM_BOOL_CALLBACK
        FMOD_DSP_SETPARAM_DATA_CALLBACK
        FMOD_DSP_GETPARAM_FLOAT_CALLBACK
        FMOD_DSP_GETPARAM_INT_CALLBACK
        FMOD_DSP_GETPARAM_BOOL_CALLBACK
        FMOD_DSP_GETPARAM_DATA_CALLBACK
        FMOD_DSP_SHOULDIPROCESS_CALLBACK
        FMOD_DSP_SYSTEM_REGISTER_CALLBACK
        FMOD_DSP_SYSTEM_DEREGISTER_CALLBACK
        FMOD_DSP_SYSTEM_MIX_CALLBACK
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct DspDescription
    {
        public uint pluginsdkversion;   /* [w] The plugin SDK version this plugin is built for.  set to this to FMOD_PLUGIN_SDK_VERSION defined above. */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public char[] name;               /* [w] Name of the unit to be displayed in the network. */
        public uint version;            /* [w] Plugin writer's version number. */
        public int numinputbuffers;    /* [w] Number of input buffers to process.  Use 0 for DSPs that only generate sound and 1 for effects that process incoming sound. */
        public int numoutputbuffers;   /* [w] Number of audio output buffers.  Only one output buffer is currently supported. */
        public DspCreatecallback create;             /* [w] Create callback.  This is called when DSP unit is created.  Can be null. */
        public DspReleasecallback release;            /* [w] Release callback.  This is called just before the unit is freed so the user can do any cleanup needed for the unit.  Can be null. */
        public DspResetcallback reset;              /* [w] Reset callback.  This is called by the user to reset any history buffers that may need resetting for a filter, when it is to be used or re-used for the first time to its initial clean state.  Use to avoid clicks or artifacts. */
        public DspReadcallback read;               /* [w] Read callback.  Processing is done here.  Can be null. */
        public DspProcessCallback process;            /* [w] Process callback.  Can be specified instead of the read callback if any channel format changes occur between input and output.  This also replaces shouldiprocess and should return an error if the effect is to be bypassed.  Can be null. */
        public DspSetpositioncallback setposition;        /* [w] Setposition callback.  This is called if the unit wants to update its position info but not process data.  Can be null. */

        public int numparameters;      /* [w] Number of parameters used in this filter.  The user finds this with DSP::getNumParameters */
        public IntPtr paramdesc;          /* [w] Variable number of parameter structures. */
        public DspSetparamFloatCallback setparameterfloat;  /* [w] This is called when the user calls DSP.setParameterFloat. Can be null. */
        public DspSetparamIntCallback setparameterint;    /* [w] This is called when the user calls DSP.setParameterInt.   Can be null. */
        public DspSetparamBoolCallback setparameterbool;   /* [w] This is called when the user calls DSP.setParameterBool.  Can be null. */
        public DspSetparamDataCallback setparameterdata;   /* [w] This is called when the user calls DSP.setParameterData.  Can be null. */
        public DspGetparamFloatCallback getparameterfloat;  /* [w] This is called when the user calls DSP.getParameterFloat. Can be null. */
        public DspGetparamIntCallback getparameterint;    /* [w] This is called when the user calls DSP.getParameterInt.   Can be null. */
        public DspGetparamBoolCallback getparameterbool;   /* [w] This is called when the user calls DSP.getParameterBool.  Can be null. */
        public DspGetparamDataCallback getparameterdata;   /* [w] This is called when the user calls DSP.getParameterData.  Can be null. */
        public DspShouldiprocessCallback shouldiprocess;     /* [w] This is called before processing.  You can detect if inputs are idle and return FMOD_OK to process, or any other error code to avoid processing the effect.  Use a count down timer to allow effect tails to process before idling! */
        public IntPtr userdata;           /* [w] Optional. Specify 0 to ignore. This is user data to be attached to the DSP unit during creation.  Access via DSP::getUserData. */

        public DspSystemRegisterCallback sys_register;       /* [w] Register callback.  This is called when DSP unit is loaded/registered.  Useful for 'global'/per system object init for plugin.  Can be null. */
        public DspSystemDeregisterCallback sys_deregister;     /* [w] Deregister callback.  This is called when DSP unit is unloaded/deregistered.  Useful as 'global'/per system object shutdown for plugin.  Can be null. */
        public DspSystemMixCallback sys_mix;            /* [w] FMODSystem mix stage callback.  This is called when the mixer starts to execute or is just finishing executing.  Useful for 'global'/per system object once a mix update calls for a plugin.  Can be null. */
    }

    /*
    [STRUCTURE] 
    [
        [DESCRIPTION]
        Struct containing DFT callbacks for plugins, to enable a plugin to perform optimized time-frequency domain conversion.

        [REMARKS]
        Members marked with [r] mean the variable is modified by BreadPlayer.Fmod and is for reading purposes only.  Do not change this value.
        Members marked with [w] mean the variable can be written to.  The user can set the value.

        [SEE_ALSO]
        FMOD_DSP_STATE_SYSTEMCALLBACKS
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct DspStateDftcallbacks
    {
        public DspDftFftreal fftreal;        /* [r] Callback for performing an FFT on a real signal. */
        public DspDftIfftreal inversefftreal; /* [r] Callback for performing an inverse FFT to get a real signal. */
    }

    /*
    [STRUCTURE] 
    [
        [DESCRIPTION]
        Struct containing panning helper callbacks for plugins.

        [REMARKS]
        These are experimental, please contact support@fmod.org for more information.

        [SEE_ALSO]
        FMOD_DSP_STATE_SYSTEMCALLBACKS
        FMOD_PAN_SURROUND_FLAGS
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct DspStatePanCallbacks
    {
        public DspPanSumMonoMatrix summonomatrix;
        public DspPanSumStereoMatrix sumstereomatrix;
        public DspPanSumSurroundMatrix sumsurroundmatrix;
        public DspPanSumMonoToSurroundMatrix summonotosurroundmatrix;
        public DspPanSumStereoToSurroundMatrix sumstereotosurroundmatrix;
        public DspPan_3DGetRolloffGain getrolloffgain;
    }

    /*
    [STRUCTURE] 
    [
        [DESCRIPTION]
        Struct containing FMODSystem level callbacks for plugins, to enable a plugin to query information about the system or allocate memory using BreadPlayer.Fmod's (and therefore possibly the game's) allocators.

        [REMARKS]
        Members marked with [r] mean the variable is modified by BreadPlayer.Fmod and is for reading purposes only.  Do not change this value.
        Members marked with [w] mean the variable can be written to.  The user can set the value.

        [SEE_ALSO]
        FMOD_DSP_STATE
        FMOD_DSP_STATE_DFTCALLBACKS  
        FMOD_DSP_STATE_PAN_CALLBACKS     
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct DspStateSystemcallbacks
    {
        private MemoryAllocCallback alloc;          /* [r] Memory allocation callback. Use this for all dynamic memory allocation within the plugin. */
        private MemoryReallocCallback realloc;        /* [r] Memory reallocation callback. */
        private MemoryFreeCallback free;           /* [r] Memory free callback. */
        private DspSystemGetsamplerate getsamplerate;  /* [r] Callback for getting the system samplerate. */
        private DspSystemGetblocksize getblocksize;   /* [r] Callback for getting the system's block size.  DSPs will be requested to process blocks of varying length up to this size.*/
        private IntPtr dft;            /* [r] Struct containing callbacks for performing FFTs and inverse FFTs. */
        private IntPtr pancallbacks;   /* [r] Pointer to a structure of callbacks for calculating pan, up-mix and down-mix matrices. */
        private DspSystemGetspeakermode getspeakermode; /* [r] Callback for getting the system's speaker modes.  One is the mixer's default speaker mode, the other is the output mode the system is downmixing or upmixing to.*/
        private FmodDspStateGetclock getclock;       /* [r] Callback for getting the clock of the current DSP, as well as the subset of the input buffer that contains the signal */
    }

    /*
    [STRUCTURE] 
    [
        [DESCRIPTION]
        DSP plugin structure that is passed into each callback.

        [REMARKS]
        Members marked with [r] mean the variable is modified by BreadPlayer.Fmod and is for reading purposes only.  Do not change this value.
        Members marked with [w] mean the variable can be written to.  The user can set the value.
        
        'systemobject' is an integer that relates to the FMODSystem object that created the DSP or registered the DSP plugin.  If only 1 FMODSystem object is created then it should be 0.  A second object would be 1 and so on.
        FMOD_DSP_STATE_SYSTEMCALLBACKS::getsamplerate and FMOD_DSP_STATE_SYSTEMCALLBACKS::getblocksize could return different results so it could be relevant to plugin developers to monitor which object is being used.

        [SEE_ALSO]
        FMOD_DSP_DESCRIPTION
        FMOD_DSP_STATE_SYSTEMCALLBACKS
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct DspState
    {
        public IntPtr instance;            /* [r] Handle to the DSP hand the user created.  Not to be modified.  C++ users cast to BreadPlayer.Fmod::DSP to use.  */
        public IntPtr plugindata;          /* [r/w] Plugin writer created data the output author wants to attach to this object. */
        public uint channelmask;         /* [r] Specifies which speakers the DSP effect is active on */
        public int source_speakermode;  /* [r] Specifies which speaker mode the signal originated for information purposes, ie in case panning needs to be done differently. */
        public IntPtr sidechaindata;       /* [r] The mixed result of all incoming sidechains is stored at this pointer address. */
        public int sidechainchannels;   /* [r] The number of channels of pcm data stored within the sidechain buffer. */
        public IntPtr callbacks;           /* [r] Struct containing callbacks for system level functionality. */
        public int systemobject;        /* [r] BreadPlayer.Fmod::FMODSystem object index, relating to the FMODSystem object that created this DSP. */
    }

    /*
    [STRUCTURE] 
    [
        [DESCRIPTION]
        DSP metering info used for retrieving metering info

        [REMARKS]
        Members marked with [r] mean the variable is modified by BreadPlayer.Fmod and is for reading purposes only.  Do not change this value.
        Members marked with [w] mean the variable can be written to.  The user can set the value.

        [SEE_ALSO]
        FMOD_SPEAKER
    ]
    */
    [StructLayout(LayoutKind.Sequential)]
    public class DspMeteringInfo
    {
        public int numsamples;        /* [r] The number of samples considered for this metering info. */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public float[] peaklevel;       /* [r] The peak level per channel. */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public float[] rmslevel;        /* [r] The rms level per channel. */
        public short numchannels;       /* [r] Number of channels. */
    }

}
