using System;
using System.Collections.Generic;
using System.Text;

namespace BreadPlayer.Fmod.CoreDSP
{
    /*
     [ENUM]
     [
         [DESCRIPTION]
         Operation type for FMOD_DSP_PROCESS_CALLBACK.

         [REMARKS]

         [SEE_ALSO]
         FMOD_DSP_DESCRIPTION
     ]
     */
    public enum DspProcessOperation
    {
        PROCESS_PERFORM = 0,               /* Process the incoming audio in 'inbufferarray' and output to 'outbufferarray'. */
        PROCESS_QUERY                      /* The DSP is being queried for the expected output format and whether it needs to process audio or should be bypassed.  The function should return any value other than FMOD_OK if audio can pass through unprocessed. If audio is to be processed, 'outbufferarray' must be filled with the expected output format, channel count and mask. */
    }
    /*
        [ENUM]
        [
            [DESCRIPTION]
            Flags for the FMOD_DSP_PAN_SUM_SURROUND_MATRIX callback.

            [REMARKS]
            This functionality is experimental, please contact support@fmod.org for more information.

            [SEE_ALSO]
            FMOD_DSP_STATE_PAN_CALLBACKS
        ]
        */
    public enum DspPanSurroundFlags
    {
        DEFAULT = 0,
        ROTATION_NOT_BIASED = 1,
    }
    
    /*
    [ENUM]
    [
        [DESCRIPTION]
        These definitions can be used for creating BreadPlayer.Fmod defined special effects or DSP units.

        [REMARKS]
        To get them to be active, first create the unit, then add it somewhere into the DSP network, either at the front of the network near the soundcard unit to affect the global output (by using System::getDSPHead), or on a single channel (using Channel::getDSPHead).

        [SEE_ALSO]
        System::createDSPByType
    ]
    */
    public enum DspType : int
    {
        UNKNOWN,            /* This unit was created via a non BreadPlayer.Fmod plugin so has an unknown purpose. */
        MIXER,              /* This unit does nothing but take inputs and mix them together then feed the result to the soundcard unit. */
        OSCILLATOR,         /* This unit generates sine/square/saw/triangle or noise tones. */
        LOWPASS,            /* This unit filters sound using a high quality, resonant lowpass filter algorithm but consumes more CPU time. */
        ITLOWPASS,          /* This unit filters sound using a resonant lowpass filter algorithm that is used in Impulse Tracker, but with limited cutoff range (0 to 8060hz). */
        HIGHPASS,           /* This unit filters sound using a resonant highpass filter algorithm. */
        ECHO,               /* This unit produces an echo on the sound and fades out at the desired rate. */
        FADER,              /* This unit pans and scales the volume of a unit. */
        FLANGE,             /* This unit produces a flange effect on the sound. */
        DISTORTION,         /* This unit distorts the sound. */
        NORMALIZE,          /* This unit normalizes or amplifies the sound to a certain level. */
        LIMITER,            /* This unit limits the sound to a certain level. */
        PARAMEQ,            /* This unit attenuates or amplifies a selected frequency range. */
        PITCHSHIFT,         /* This unit bends the pitch of a sound without changing the speed of playback. */
        CHORUS,             /* This unit produces a chorus effect on the sound. */
        VSTPLUGIN,          /* This unit allows the use of Steinberg VST plugins */
        WINAMPPLUGIN,       /* This unit allows the use of Nullsoft Winamp plugins */
        ITECHO,             /* This unit produces an echo on the sound and fades out at the desired rate as is used in Impulse Tracker. */
        COMPRESSOR,         /* This unit implements dynamic compression (linked multichannel, wideband) */
        SFXREVERB,          /* This unit implements SFX reverb */
        LOWPASS_SIMPLE,     /* This unit filters sound using a simple lowpass with no resonance, but has flexible cutoff and is fast. */
        DELAY,              /* This unit produces different delays on individual channels of the sound. */
        TREMOLO,            /* This unit produces a tremolo / chopper effect on the sound. */
        LADSPAPLUGIN,       /* This unit allows the use of LADSPA standard plugins. */
        SEND,               /* This unit sends a copy of the signal to a return DSP anywhere in the DSP tree. */
        RETURN,             /* This unit receives signals from a number of send DSPs. */
        HIGHPASS_SIMPLE,    /* This unit filters sound using a simple highpass with no resonance, but has flexible cutoff and is fast. */
        PAN,                /* This unit pans the signal, possibly upmixing or downmixing as well. */
        THREE_EQ,           /* This unit is a three-band equalizer. */
        FFT,                /* This unit simply analyzes the signal and provides spectrum information back through getParameter. */
        LOUDNESS_METER,     /* This unit analyzes the loudness and true peak of the signal. */
        ENVELOPEFOLLOWER,   /* This unit tracks the envelope of the input/sidechain signal */
        CONVOLUTIONREVERB,  /* This unit implements convolution reverb. */
        CHANNELMIX,         /* This unit provides per signal channel gain, and output channel mapping to allow 1 multichannel signal made up of many groups of signals to map to a single output signal. */
        TRANSCEIVER,        /* This unit 'sends' and 'receives' from a selection of up to 32 different slots.  It is like a send/return but it uses global slots rather than returns as the destination.  It also has other features.  Multiple transceivers can receive from a single channel, or multiple transceivers can send to a single channel, or a combination of both. */
        OBJECTPAN,          /* This unit sends the signal to a 3d object encoder like Dolby Atmos. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        DSP parameter types.

        [REMARKS]

        [SEE_ALSO]
        FMOD_DSP_PARAMETER_DESC
    ]
    */
    public enum DspParameterType
    {
        FLOAT = 0,
        INT,
        BOOL,
        DATA,
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        DSP float parameter mappings. These determine how values are mapped across dials and automation curves.

        [REMARKS]
        FMOD_DSP_PARAMETER_FLOAT_MAPPING_TYPE_AUTO generates a mapping based on range and units. For example, if the units are in Hertz and the range is with-in the audio spectrum, a Bark scale will be chosen. Logarithmic scales may also be generated for ranges above zero spanning several orders of magnitude.

        [SEE_ALSO]
        FMOD_DSP_PARAMETER_FLOAT_MAPPING
    ]
    */
    public enum DspParameterFloatMappingType
    {
        DSP_PARAMETER_FLOAT_MAPPING_TYPE_LINEAR = 0,          /* Values mapped linearly across range. */
        DSP_PARAMETER_FLOAT_MAPPING_TYPE_AUTO,                /* A mapping is automatically chosen based on range and units.  See remarks. */
        DSP_PARAMETER_FLOAT_MAPPING_TYPE_PIECEWISE_LINEAR,    /* Values mapped in a piecewise linear fashion defined by FMOD_DSP_PARAMETER_DESC_FLOAT::mapping.piecewiselinearmapping. */
    }
    
    /*
    [ENUM]
    [
        [DESCRIPTION]
        Built-in types for the 'datatype' member of FMOD_DSP_PARAMETER_DESC_DATA.  Data parameters of type other than FMOD_DSP_PARAMETER_DATA_TYPE_USER will be treated specially by the system.

        [REMARKS]

        [SEE_ALSO]
        FMOD_DSP_PARAMETER_DESC_DATA
        FMOD_DSP_PARAMETER_OVERALLGAIN
        FMOD_DSP_PARAMETER_3DATTRIBUTES
        FMOD_DSP_PARAMETER_3DATTRIBUTES_MULTI
        FMOD_DSP_PARAMETER_SIDECHAIN
    ]
    */
    public enum DspParameterDataType
    {
        DSP_PARAMETER_DATA_TYPE_USER = 0,              /* The default data type.  All user data types should be 0 or above. */
        DSP_PARAMETER_DATA_TYPE_OVERALLGAIN = -1,      /* The data type for FMOD_DSP_PARAMETER_OVERALLGAIN parameters.  There should a maximum of one per DSP. */
        DSP_PARAMETER_DATA_TYPE_3DATTRIBUTES = -2,     /* The data type for FMOD_DSP_PARAMETER_3DATTRIBUTES parameters.  There should a maximum of one per DSP. */
        DSP_PARAMETER_DATA_TYPE_SIDECHAIN = -3,        /* The data type for FMOD_DSP_PARAMETER_SIDECHAIN parameters.  There should a maximum of one per DSP. */
        DSP_PARAMETER_DATA_TYPE_FFT = -4,              /* The data type for FMOD_DSP_PARAMETER_FFT parameters.  There should a maximum of one per DSP. */
        DSP_PARAMETER_DATA_TYPE_3DATTRIBUTES_MULTI = -5, /* The data type for FMOD_DSP_PARAMETER_3DATTRIBUTES_MULTI parameters.  There should a maximum of one per DSP. */
    }
    /*
       [ENUM]
       [
           [DESCRIPTION]
           Parameter types for the FMOD_DSP_TYPE_OSCILLATOR filter.

           [REMARKS]

           [SEE_ALSO]
           DSP::setParameter
           DSP::getParameter
           FMOD_DSP_TYPE
       ]
       */
    /*
     ==============================================================================================================

     BreadPlayer.Fmod built in effect parameters.
     Use DSP::setParameter with these enums for the 'index' parameter.

     ==============================================================================================================
 */
    public enum DspOscillator
    {
        TYPE,   /* Waveform type.  0 = sine.  1 = square. 2 = sawup. 3 = sawdown. 4 = triangle. 5 = noise.  */
        RATE    /* Frequency of the sinewave in hz.  1.0 to 22000.0.  Default = 220.0. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter types for the FMOD_DSP_TYPE_LOWPASS filter.

        [REMARKS]

        [SEE_ALSO]
        DSP::setParameter
        DSP::getParameter
        FMOD_DSP_TYPE
    ]
    */
    public enum DspLowpass
    {
        CUTOFF,    /* Lowpass cutoff frequency in hz.   1.0 to 22000.0.  Default = 5000.0. */
        RESONANCE  /* Lowpass resonance Q value. 1.0 to 10.0.  Default = 1.0. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter types for the FMOD_DSP_TYPE_ITLOWPASS filter.
        This is different to the default FMOD_DSP_TYPE_ITLOWPASS filter in that it uses a different quality algorithm and is
        the filter used to produce the correct sounding playback in .IT files.
        BreadPlayer.Fmod Ex's .IT playback uses this filter.

        [REMARKS]
        Note! This filter actually has a limited cutoff frequency below the specified maximum, due to its limited design,
        so for a more  open range filter use FMOD_DSP_LOWPASS or if you don't mind not having resonance,
        FMOD_DSP_LOWPASS_SIMPLE.
        The effective maximum cutoff is about 8060hz.

        [SEE_ALSO]
        DSP::setParameter
        DSP::getParameter
        FMOD_DSP_TYPE
    ]
    */
    public enum DspITlowpass
    {
        CUTOFF,    /* Lowpass cutoff frequency in hz.  1.0 to 22000.0.  Default = 5000.0/ */
        RESONANCE  /* Lowpass resonance Q value.  0.0 to 127.0.  Default = 1.0. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter types for the FMOD_DSP_TYPE_HIGHPASS filter.

        [REMARKS]

        [SEE_ALSO]
        DSP::setParameter
        DSP::getParameter
        FMOD_DSP_TYPE
    ]
    */
    public enum DspHighpass
    {
        CUTOFF,    /* (Type:float) - Highpass cutoff frequency in hz.  1.0 to output 22000.0.  Default = 5000.0. */
        RESONANCE  /* (Type:float) - Highpass resonance Q value.  1.0 to 10.0.  Default = 1.0. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter types for the FMOD_DSP_TYPE_ECHO filter.

        [REMARKS]
        Note.  Every time the delay is changed, the plugin re-allocates the echo buffer.  This means the echo will dissapear at that time while it refills its new buffer.
        Larger echo delays result in larger amounts of memory allocated.

        [SEE_ALSO]
        DSP::setParameterFloat
        DSP::getParameterFloat
        FMOD_DSP_TYPE
    ]
    */
    public enum DspEcho
    {
        DELAY,       /* (Type:float) - Echo delay in ms.  10  to 5000.  Default = 500. */
        FEEDBACK,    /* (Type:float) - Echo decay per delay.  0 to 100.  100.0 = No decay, 0.0 = total decay (ie simple 1 line delay).  Default = 50.0. */
        DRYLEVEL,    /* (Type:float) - Original sound volume in dB.  -80.0 to 10.0.  Default = 0. */
        WETLEVEL     /* (Type:float) - Volume of echo signal to pass to output in dB.  -80.0 to 10.0.  Default = 0. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter types for the FMOD_DSP_TYPE_DELAY filter.

        [REMARKS]
        Note.  Every time MaxDelay is changed, the plugin re-allocates the delay buffer.  This means the delay will dissapear at that time while it refills its new buffer.
        A larger MaxDelay results in larger amounts of memory allocated.
        Channel delays above MaxDelay will be clipped to MaxDelay and the delay buffer will not be resized.

        [SEE_ALSO]
        DSP::setParameterFloat
        DSP::getParameterFloat
        FMOD_DSP_TYPE
    ]
    */
    public enum DspDelay
    {
        CH0,      /* Channel #0 Delay in ms.   0  to 10000.  Default = 0.  */
        CH1,      /* Channel #1 Delay in ms.   0  to 10000.  Default = 0.  */
        CH2,      /* Channel #2 Delay in ms.   0  to 10000.  Default = 0.  */
        CH3,      /* Channel #3 Delay in ms.   0  to 10000.  Default = 0.  */
        CH4,      /* Channel #4 Delay in ms.   0  to 10000.  Default = 0.  */
        CH5,      /* Channel #5 Delay in ms.   0  to 10000.  Default = 0.  */
        CH6,      /* Channel #6 Delay in ms.   0  to 10000.  Default = 0.  */
        CH7,      /* Channel #7 Delay in ms.   0  to 10000.  Default = 0.  */
        CH8,      /* Channel #8 Delay in ms.   0  to 10000.  Default = 0.  */
        CH9,      /* Channel #9 Delay in ms.   0  to 10000.  Default = 0.  */
        CH10,     /* Channel #10 Delay in ms.  0  to 10000.  Default = 0.  */
        CH11,     /* Channel #11 Delay in ms.  0  to 10000.  Default = 0.  */
        CH12,     /* Channel #12 Delay in ms.  0  to 10000.  Default = 0.  */
        CH13,     /* Channel #13 Delay in ms.  0  to 10000.  Default = 0.  */
        CH14,     /* Channel #14 Delay in ms.  0  to 10000.  Default = 0.  */
        CH15,     /* Channel #15 Delay in ms.  0  to 10000.  Default = 0.  */
        MAXDELAY, /* Maximum delay in ms.      0  to 1000.   Default = 10. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter types for the FMOD_DSP_TYPE_FLANGE filter.

        [REMARKS]
        Flange is an effect where the signal is played twice at the same time, and one copy slides back and forth creating a whooshing or flanging effect.
        As there are 2 copies of the same signal, by default each signal is given 50% mix, so that the total is not louder than the original unaffected signal.
        
        Flange depth is a percentage of a 10ms shift from the original signal.  Anything above 10ms is not considered flange because to the ear it begins to 'echo' so 10ms is the highest value possible.

        [SEE_ALSO]
        DSP::setParameterFloat
        DSP::getParameterFloat
        FMOD_DSP_TYPE
    ]
    */
    public enum DspFlange
    {
        MIX,         /* (Type:float) - Percentage of wet signal in mix.  0 to 100. Default = 50. */
        DEPTH,       /* (Type:float) - Flange depth (percentage of 40ms delay).  0.01 to 1.0.  Default = 1.0. */
        RATE         /* (Type:float) - Flange speed in hz.  0.0 to 20.0.  Default = 0.1. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter types for the FMOD_DSP_TYPE_TREMOLO filter.

        [REMARKS]
        The tremolo effect varies the amplitude of a sound. Depending on the settings, this unit can produce a tremolo, chopper or auto-pan effect.
        
        The shape of the LFO (low freq. oscillator) can morphed between sine, triangle and sawtooth waves using the FMOD_DSP_TREMOLO_SHAPE and FMOD_DSP_TREMOLO_SKEW parameters.
        FMOD_DSP_TREMOLO_DUTY and FMOD_DSP_TREMOLO_SQUARE are useful for a chopper-type effect where the first controls the on-time duration and second controls the flatness of the envelope.
        FMOD_DSP_TREMOLO_SPREAD varies the LFO phase between channels to get an auto-pan effect. This works best with a sine shape LFO.
        The LFO can be synchronized using the FMOD_DSP_TREMOLO_PHASE parameter which sets its instantaneous phase.

        [SEE_ALSO]
        DSP::setParameterFloat
        DSP::getParameter
        FMOD_DSP_TYPE
    ]
    */
    public enum DspTremolo
    {
        FREQUENCY,     /* LFO frequency in Hz.  0.1 to 20.  Default = 4. */
        DEPTH,         /* Tremolo depth.  0 to 1.  Default = 0. */
        SHAPE,         /* LFO shape morph between triangle and sine.  0 to 1.  Default = 0. */
        SKEW,          /* Time-skewing of LFO cycle.  -1 to 1.  Default = 0. */
        DUTY,          /* LFO on-time.  0 to 1.  Default = 0.5. */
        SQUARE,        /* Flatness of the LFO shape.  0 to 1.  Default = 0. */
        PHASE,         /* Instantaneous LFO phase.  0 to 1.  Default = 0. */
        SPREAD         /* Rotation / auto-pan effect.  -1 to 1.  Default = 0. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter types for the FMOD_DSP_TYPE_DISTORTION filter.

        [REMARKS]

        [SEE_ALSO]
        DSP::setParameterFloat
        DSP::getParameterFloat
        FMOD_DSP_TYPE
    ]
    */
    public enum DspDistortion
    {
        LEVEL    /* Distortion value.  0.0 to 1.0.  Default = 0.5. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter types for the FMOD_DSP_TYPE_NORMALIZE filter.

        [REMARKS]
        Normalize amplifies the sound based on the maximum peaks within the signal.
        For example if the maximum peaks in the signal were 50% of the bandwidth, it would scale the whole sound by 2.
        The lower threshold value makes the normalizer ignores peaks below a certain point, to avoid over-amplification if a loud signal suddenly came in, and also to avoid amplifying to maximum things like background hiss.
        
        Because BreadPlayer.Fmod is a realtime audio processor, it doesn't have the luxury of knowing the peak for the whole sound (ie it can't see into the future), so it has to process data as it comes in.
        To avoid very sudden changes in volume level based on small samples of new data, fmod fades towards the desired amplification which makes for smooth gain control.  The fadetime parameter can control this.

        [SEE_ALSO]
        DSP::setParameterFloat
        DSP::getParameter
        FMOD_DSP_TYPE
    ]
    */
    public enum DspNormalize
    {
        FADETIME,    /* Time to ramp the silence to full in ms.  0.0 to 20000.0. Default = 5000.0. */
        THRESHHOLD,  /* Lower volume range threshold to ignore.  0.0 to 1.0.  Default = 0.1.  Raise higher to stop amplification of very quiet signals. */
        MAXAMP       /* Maximum amplification allowed.  1.0 to 100000.0.  Default = 20.0.  1.0 = no amplifaction, higher values allow more boost. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter types for the FMOD_DSP_TYPE_LIMITER filter.

        [REMARKS]

        [SEE_ALSO]
        DSP::setParameterFloat
        DSP::getParameterFloat
        FMOD_DSP_TYPE
    ]
    */
    public enum DspLimiter
    {
        RELEASETIME,   /* (Type:float) - Time to ramp the silence to full in ms.  1.0 to 1000.0. Default = 10.0. */
        CEILING,       /* (Type:float) - Maximum level of the output signal in dB.  -12.0 to 0.0.  Default = 0.0. */
        MAXIMIZERGAIN, /* (Type:float) - Maximum amplification allowed in dB.  0.0 to 12.0.  Default = 0.0. 0.0 = no amplifaction, higher values allow more boost. */
        Mode,          /* (Type:float) - Channel processing mode. 0 or 1. Default = 0. 0 = Independent (limiter per channel), 1 = Linked. */
    }

    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter types for the FMOD_DSP_TYPE_PARAMEQ filter.

        [REMARKS]
        Parametric EQ is a bandpass filter that attenuates or amplifies a selected frequency and its neighbouring frequencies.
        
        To create a multi-band EQ create multiple FMOD_DSP_TYPE_PARAMEQ units and set each unit to different frequencies, for example 1000hz, 2000hz, 4000hz, 8000hz, 16000hz with a range of 1 octave each.
        
        When a frequency has its gain set to 1.0, the sound will be unaffected and represents the original signal exactly.

        [SEE_ALSO]
        DSP::setParameterFloat
        DSP::getParameterFloat
        FMOD_DSP_TYPE
    ]
    */
    public enum DspParamEQ
    {
        CENTER,     /* Frequency center.  20.0 to 22000.0.  Default = 8000.0. */
        BANDWIDTH,  /* Octave range around the center frequency to filter.  0.2 to 5.0.  Default = 1.0. */
        GAIN        /* Frequency Gain.  0.05 to 3.0.  Default = 1.0.  */
    }



    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter types for the FMOD_DSP_TYPE_PITCHSHIFT filter.

        [REMARKS]
        This pitch shifting unit can be used to change the pitch of a sound without speeding it up or slowing it down.
        It can also be used for time stretching or scaling, for example if the pitch was doubled, and the frequency of the sound was halved, the pitch of the sound would sound correct but it would be twice as slow.
        
        Warning! This filter is very computationally expensive!  Similar to a vocoder, it requires several overlapping FFT and IFFT's to produce smooth output, and can require around 440mhz for 1 stereo 48khz signal using the default settings.
        Reducing the signal to mono will half the cpu usage, as will the overlap count.
        Reducing this will lower audio quality, but what settings to use are largely dependant on the sound being played.  A noisy polyphonic signal will need higher overlap and fft size compared to a speaking voice for example.
        
        This pitch shifter is based on the pitch shifter code at http://www.dspdimension.com, written by Stephan M. Bernsee.
        The original code is COPYRIGHT 1999-2003 Stephan M. Bernsee <smb@dspdimension.com>.
        
        'maxchannels' dictates the amount of memory allocated.  By default, the maxchannels value is 0.  If BreadPlayer.Fmod is set to stereo, the pitch shift unit will allocate enough memory for 2 channels.  If it is 5.1, it will allocate enough memory for a 6 channel pitch shift, etc.
        If the pitch shift effect is only ever applied to the global mix (ie it was added with System::addDSP), then 0 is the value to set as it will be enough to handle all speaker modes.
        When the pitch shift is added to a channel (ie Channel::addDSP) then the channel count that comes in could be anything from 1 to 8 possibly.  It is only in this case where you might want to increase the channel count above the output's channel count.
        If a channel pitch shift is set to a lower number than the sound's channel count that is coming in, it will not pitch shift the sound.

        [SEE_ALSO]
        DSP::setParameterFloat
        DSP::getParameterFloat
        FMOD_DSP_TYPE
    ]
    */
    public enum DspPitchShift
    {
        PITCH,       /* Pitch value.  0.5 to 2.0.  Default = 1.0. 0.5 = one octave down, 2.0 = one octave up.  1.0 does not change the pitch. */
        FFTSIZE,     /* FFT window size.  256, 512, 1024, 2048, 4096.  Default = 1024.  Increase this to reduce 'smearing'.  This effect is a warbling sound similar to when an mp3 is encoded at very low bitrates. */
        OVERLAP,     /* Window overlap.  1 to 32.  Default = 4.  Increase this to reduce 'tremolo' effect.  Increasing it by a factor of 2 doubles the CPU usage. */
        MAXCHANNELS  /* Maximum channels supported.  0 to 16.  0 = same as fmod's default output polyphony, 1 = mono, 2 = stereo etc.  See remarks for more.  Default = 0.  It is suggested to leave at 0! */
    }



    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter types for the FMOD_DSP_TYPE_CHORUS filter.

        [REMARKS]
        Chorous is an effect where the sound is more 'spacious' due to 1 to 3 versions of the sound being played along side the original signal but with the pitch of each copy modulating on a sine wave.
        This is a highly configurable chorus unit.  It supports 3 taps, small and large delay times and also feedback.
        This unit also could be used to do a simple echo, or a flange effect.

        [SEE_ALSO]
        DSP::setParameterFloat
        DSP::getParameterFloat
        FMOD_DSP_TYPE
    ]
    */
    public enum DspChorus
    {
        MIX,      /* (Type:float) - Volume of original signal to pass to output.  0.0 to 100.0. Default = 50.0. */
        RATE,     /* (Type:float) - Chorus modulation rate in Hz.  0.0 to 20.0.  Default = 0.8 Hz. */
        DEPTH,    /* (Type:float) - Chorus modulation depth.  0.0 to 100.0.  Default = 3.0. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter types for the FMOD_DSP_TYPE_ITECHO filter.
        This is effectively a software based echo filter that emulates the DirectX DMO echo effect.  Impulse tracker files can support this, and BreadPlayer.Fmod will produce the effect on ANY platform, not just those that support DirectX effects!

        [REMARKS]
        Note.  Every time the delay is changed, the plugin re-allocates the echo buffer.  This means the echo will dissapear at that time while it refills its new buffer.
        Larger echo delays result in larger amounts of memory allocated.
        
        As this is a stereo filter made mainly for IT playback, it is targeted for stereo signals.
        With mono signals only the FMOD_DSP_ITECHO_LEFTDELAY is used.
        For multichannel signals (>2) there will be no echo on those channels.

        [SEE_ALSO]
        DSP::setParameterFloat
        DSP::getParameterFloat
        FMOD_DSP_TYPE
        System::addDSP
    ]
    */
    public enum DspITecho
    {
        WETDRYMIX,      /* (Type:float) - Ratio of wet (processed) signal to dry (unprocessed) signal. Must be in the range from 0.0 through 100.0 (all wet).  Default = 50. */
        FEEDBACK,       /* (Type:float) - Percentage of output fed back into input, in the range from 0.0 through 100.0.  Default = 50. */
        LEFTDELAY,      /* (Type:float) - Delay for left channel, in milliseconds, in the range from 1.0 through 2000.0.  Default = 500 ms. */
        RIGHTDELAY,     /* (Type:float) - Delay for right channel, in milliseconds, in the range from 1.0 through 2000.0.  Default = 500 ms. */
        PANDELAY        /* (Type:float) - Value that specifies whether to swap left and right delays with each successive echo.  Ranges from 0.0 (equivalent to FALSE) to 1.0 (equivalent to TRUE), meaning no swap.  Default = 0.  CURRENTLY NOT SUPPORTED. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter types for the FMOD_DSP_TYPE_COMPRESSOR unit.
        This is a multichannel software limiter that is uniform across the whole spectrum.

        [REMARKS]
        The limiter is not guaranteed to catch every peak above the threshold level,
        because it cannot apply gain reduction instantaneously - the time delay is
        determined by the attack time. However setting the attack time too short will
        distort the sound, so it is a compromise. High level peaks can be avoided by
        using a short attack time - but not too short, and setting the threshold a few
        decibels below the critical level.
        
        [SEE_ALSO]
        DSP::setParameterFloat
        DSP::getParameterFloat
        DSP::setParameterBool
        DSP::getParameterBool
        FMOD_DSP_TYPE
    ]
    */
    public enum DspCompressor
    {
        THRESHOLD,   /* (Type:float) - Threshold level (dB) in the range from -80 through 0. The default value is 0. */
        RATIO,       /* (Type:float) - Compression Ratio (dB/dB) in the range from 1 to 50. The default value is 2.5. */
        ATTACK,      /* (Type:float) - Attack time (milliseconds), in the range from 0.1 through 1000. The default value is 20. */
        RELEASE,     /* (Type:float) - Release time (milliseconds), in the range from 10 through 5000. The default value is 100 */
        GAINMAKEUP,  /* (Type:float) - Make-up gain (dB) applied after limiting, in the range from 0 through 30. The default value is 0. */
        USESIDECHAIN,/* (Type:bool)  - Whether to analyse the sidechain signal instead of the input signal. The default value is false */
        LINKED       /* (Type:bool)  - FALSE = Independent (compressor per channel), TRUE = Linked.  The default value is TRUE. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter types for the FMOD_DSP_TYPE_SFXREVERB unit.

        [REMARKS]
        This is a high quality I3DL2 based reverb.
        On top of the I3DL2 property set, "Dry Level" is also included to allow the dry mix to be changed.
        
        These properties can be set with presets in FMOD_REVERB_PRESETS.

        [SEE_ALSO]
        DSP::setParameterFloat
        DSP::getParameterFloat
        FMOD_DSP_TYPE
        FMOD_REVERB_PRESETS
    ]
    */
    public enum DspSFXreverb
    {
        DECAYTIME,           /* (Type:float) - Decay Time       : Reverberation decay time at low-frequencies in milliseconds.  Ranges from 100.0 to 20000.0. Default is 1500. */
        EARLYDELAY,          /* (Type:float) - Early Delay      : Delay time of first reflection in milliseconds.  Ranges from 0.0 to 300.0.  Default is 20. */
        LATEDELAY,           /* (Type:float) - Reverb Delay     : Late reverberation delay time relative to first reflection in milliseconds.  Ranges from 0.0 to 100.0.  Default is 40. */
        HFREFERENCE,         /* (Type:float) - HF Reference     : Reference frequency for high-frequency decay in Hz.  Ranges from 20.0 to 20000.0. Default is 5000. */
        HFDECAYRATIO,        /* (Type:float) - Decay HF Ratio   : High-frequency decay time relative to decay time in percent.  Ranges from 10.0 to 100.0. Default is 50. */
        DIFFUSION,           /* (Type:float) - Diffusion        : Reverberation diffusion (echo density) in percent.  Ranges from 0.0 to 100.0.  Default is 100. */
        DENSITY,             /* (Type:float) - Density          : Reverberation density (modal density) in percent.  Ranges from 0.0 to 100.0.  Default is 100. */
        LOWSHELFFREQUENCY,   /* (Type:float) - Low Shelf Frequency : Transition frequency of low-shelf filter in Hz.  Ranges from 20.0 to 1000.0. Default is 250. */
        LOWSHELFGAIN,        /* (Type:float) - Low Shelf Gain   : Gain of low-shelf filter in dB.  Ranges from -36.0 to 12.0.  Default is 0. */
        HIGHCUT,             /* (Type:float) - High Cut         : Cutoff frequency of low-pass filter in Hz.  Ranges from 20.0 to 20000.0. Default is 20000. */
        EARLYLATEMIX,        /* (Type:float) - Early/Late Mix   : Blend ratio of late reverb to early reflections in percent.  Ranges from 0.0 to 100.0.  Default is 50. */
        WETLEVEL,            /* (Type:float) - Wet Level        : Reverb signal level in dB.  Ranges from -80.0 to 20.0.  Default is -6. */
        DRYLEVEL             /* (Type:float) - Dry Level        : Dry signal level in dB.  Ranges from -80.0 to 20.0.  Default is 0. */
    }

    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter types for the FMOD_DSP_TYPE_LOWPASS_SIMPLE filter.
        This is a very simple low pass filter, based on two single-pole RC time-constant modules.
        The emphasis is on speed rather than accuracy, so this should not be used for task requiring critical filtering.

        [REMARKS]

        [SEE_ALSO]
        DSP::setParameterFloat
        DSP::getParameterFloat
        FMOD_DSP_TYPE
    ]
    */
    public enum DspLowpassSimple
    {
        CUTOFF     /* Lowpass cutoff frequency in hz.  10.0 to 22000.0.  Default = 5000.0 */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter types for the FMOD_DSP_TYPE_SEND DSP.

        [REMARKS]

        [SEE_ALSO]
        DSP::setParameterInt
        DSP::getParameterInt
        DSP::setParameterFloat
        DSP::getParameterFloat
        FMOD_DSP_TYPE
    ]
    */
    public enum DspSend
    {
        RETURNID,     /* (Type:int) - ID of the Return DSP this send is connected to (integer values only). -1 indicates no connected Return DSP. Default = -1. */
        LEVEL,        /* (Type:float) - Send level. 0.0 to 1.0. Default = 1.0 */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter types for the FMOD_DSP_TYPE_RETURN DSP.

        [REMARKS]

        [SEE_ALSO]
        DSP::setParameterInt
        DSP::getParameterInt
        FMOD_DSP_TYPE
    ]
    */
    public enum DspReturn
    {
        ID,                 /* (Type:int) - ID of this Return DSP. Read-only.  Default = -1. */
        INPUT_SPEAKER_MODE  /* (Type:int) - Input speaker mode of this return.  Default = FMOD_SPEAKERMODE_DEFAULT. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter types for the FMOD_DSP_TYPE_HIGHPASS_SIMPLE filter.
        This is a very simple single-order high pass filter.
        The emphasis is on speed rather than accuracy, so this should not be used for task requiring critical filtering. 

        [REMARKS]

        [SEE_ALSO]
        DSP::setParameterFloat
        DSP::getParameterFloat
        FMOD_DSP_TYPE
    ]
    */
    public enum DspHighpassSimple
    {
        CUTOFF     /* (Type:float) - Highpass cutoff frequency in hz.  10.0 to 22000.0.  Default = 1000.0 */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter values for the FMOD_DSP_PAN_SURROUND_FROM_STEREO_MODE parameter of the FMOD_DSP_TYPE_PAN DSP.

        [REMARKS]

        [SEE_ALSO]
        FMOD_DSP_PAN
    ]
    */
    public enum DspPanSurroundFromStereoModeType
    {
        DISTRIBUTED,
        DISCRETE
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter values for the FMOD_DSP_PAN_MODE parameter of the FMOD_DSP_TYPE_PAN DSP.

        [REMARKS]

        [SEE_ALSO]
        FMOD_DSP_PAN
    ]
    */
    public enum DspPanModeType
    {
        MONO,
        STEREO,
        SURROUND
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter values for the FMOD_DSP_PAN_3D_ROLLOFF parameter of the FMOD_DSP_TYPE_PAN DSP.

        [REMARKS]

        [SEE_ALSO]
        FMOD_DSP_PAN
    ]
    */
    public enum DspPan3DRollOffType
    {
        LINEARSQUARED,
        LINEAR,
        INVERSE,
        INVERSETAPERED,
        CUSTOM
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter values for the FMOD_DSP_PAN_3D_EXTENT_MODE parameter of the FMOD_DSP_TYPE_PAN DSP.

        [REMARKS]

        [SEE_ALSO]
        FMOD_DSP_PAN
    ]
    */
    public enum DspPan3DExtentModeType
    {
        AUTO,
        USER,
        OFF
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter types for the FMOD_DSP_TYPE_PAN DSP.

        [REMARKS]

        [SEE_ALSO]
        DSP::setParameterFloat
        DSP::getParameterFloat
        DSP::setParameterInt
        DSP::getParameterInt
        DSP::setParameterData
        DSP::getParameterData
        FMOD_DSP_TYPE
    ]
    */
    public enum DspPan
    {
        Mode,                           /* (Type:int)   - Panner mode.              FMOD_DSP_PAN_MODE_MONO for mono down-mix, FMOD_DSP_PAN_MODE_STEREO for stereo panning or FMOD_DSP_PAN_MODE_SURROUND for surround panning.  Default = FMOD_DSP_PAN_MODE_SURROUND */
        STEREO_POSITION,                /* (Type:float) - Stereo pan position       STEREO_POSITION_MIN to STEREO_POSITION_MAX.  Default = 0.0. */
        SURROUND_DIRECTION,             /* (Type:float) - Surround pan direction    ROTATION_MIN to ROTATION_MAX.  Default = 0.0. */
        SURROUND_EXTENT,                /* (Type:float) - Surround pan extent       EXTENT_MIN to EXTENT_MAX.  Default = 360.0. */
        SURROUND_ROTATION,              /* (Type:float) - Surround pan rotation     ROTATION_MIN to ROTATION_MAX.  Default = 0.0. */
        SURROUND_LFE_LEVEL,             /* (Type:float) - Surround pan LFE level    SURROUND_LFE_LEVEL_MIN to SURROUND_LFE_LEVEL_MAX.  Default = 0.0. */
        SURROUND_FROM_STEREO_MODE,      /* (Type:int)   - Stereo-To-Surround Mode   FMOD_DSP_PAN_SURROUND_FROM_STEREO_MODE_DISTRIBUTED to FMOD_DSP_PAN_SURROUND_FROM_STEREO_MODE_DISCRETE.  Default = FMOD_DSP_PAN_SURROUND_FROM_STEREO_MODE_DISCRETE. */
        SURROUND_STEREO_SEPARATION,     /* (Type:float) - Stereo-To-Surround Stereo Separation. ROTATION_MIN to ROTATION_MAX.  Default = 60.0. */
        SURROUND_STEREO_AXIS,           /* (Type:float) - Stereo-To-Surround Stereo Axis. ROTATION_MIN to ROTATION_MAX.  Default = 0.0. */
        ENABLED_SURROUND_SPEAKERS,      /* (Type:int)   - Surround Speakers Enabled. 0 to 0xFFF.  Default = 0xFFF.  */
        _3D_POSITION,                   /* (Type:data)  - 3D Position               data of type FMOD_DSP_PARAMETER_DATA_TYPE_3DPOS */
        _3D_ROLLOFF,                    /* (Type:int)   - 3D Rolloff                FMOD_DSP_PAN_3D_ROLLOFF_LINEARSQUARED to FMOD_DSP_PAN_3D_ROLLOFF_CUSTOM.  Default = FMOD_DSP_PAN_3D_ROLLOFF_LINEARSQUARED. */
        _3D_MIN_DISTANCE,               /* (Type:float) - 3D Min Distance           0.0 to GAME_UNITS_MAX.  Default = 1.0. */
        _3D_MAX_DISTANCE,               /* (Type:float) - 3D Max Distance           0.0 to GAME_UNITS_MAX.  Default = 20.0. */
        _3D_EXTENT_MODE,                /* (Type:int)   - 3D Extent Mode            FMOD_DSP_PAN_3D_EXTENT_MODE_AUTO to FMOD_DSP_PAN_3D_EXTENT_MODE_OFF.  Default = FMOD_DSP_PAN_3D_EXTENT_MODE_AUTO. */
        _3D_SOUND_SIZE,                 /* (Type:float) - 3D Sound Size             0.0 to GAME_UNITS_MAX.  Default = 0.0. */
        _3D_MIN_EXTENT,                 /* (Type:float) - 3D Min Extent             EXTENT_MIN to EXTENT_MAX.  Default = 0.0. */
        _3D_PAN_BLEND,                  /* (Type:float) - 3D Pan Blend              PAN_BLEND_MIN to PAN_BLEND_MAX.  Default = 0.0. */
        LFE_UPMIX_ENABLED,              /* (Type:int)   - LFE Upmix Enabled         0 to 1.  Default = 0. */
        OVERALL_GAIN,                   /* (Type:data)  - Overall gain.             For information only, not set by user.  Data of type FMOD_DSP_PARAMETER_DATA_TYPE_OVERALLGAIN to provide to BreadPlayer.Fmod, to allow BreadPlayer.Fmod to know the DSP is scaling the signal for virtualization purposes. */
        SURROUND_SPEAKER_MODE           /* (Type:int)   - Surround speaker mode.    Target speaker mode for surround panning.  Default = FMOD_SPEAKERMODE_DEFAULT. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter values for the FMOD_DSP_THREE_EQ_CROSSOVERSLOPE parameter of the FMOD_DSP_TYPE_THREE_EQ DSP.

        [REMARKS]

        [SEE_ALSO]
        FMOD_DSP_THREE_EQ
    ]
    */
    public enum DspThreeEQCrossoverSlopeType
    {
        _12DB,
        _24DB,
        _48DB
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter types for the FMOD_DSP_TYPE_THREE_EQ filter.

        [REMARKS]

        [SEE_ALSO]
        DSP::setParameterFloat
        DSP::getParameterFloat
        DSP::setParameterInt
        DSP::getParameterInt
        FMOD_DSP_TYPE
        FMOD_DSP_THREE_EQ_CROSSOVERSLOPE_TYPE
    ]
    */
    public enum DspThreeEQ
    {
        LOWGAIN,       /* (Type:float) - Low frequency gain in dB.  -80.0 to 10.0.  Default = 0. */
        MIDGAIN,       /* (Type:float) - Mid frequency gain in dB.  -80.0 to 10.0.  Default = 0. */
        HIGHGAIN,      /* (Type:float) - High frequency gain in dB.  -80.0 to 10.0.  Default = 0. */
        LOWCROSSOVER,  /* (Type:float) - Low-to-mid crossover frequency in Hz.  10.0 to 22000.0.  Default = 400.0. */
        HIGHCROSSOVER, /* (Type:float) - Mid-to-high crossover frequency in Hz.  10.0 to 22000.0.  Default = 4000.0. */
        CROSSOVERSLOPE /* (Type:int)   - Crossover Slope.  0 = 12dB/Octave, 1 = 24dB/Octave, 2 = 48dB/Octave.  Default = 1 (24dB/Octave). */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        List of windowing methods for the FMOD_DSP_TYPE_FFT unit.  Used in spectrum analysis to reduce leakage / transient signals intefering with the analysis.
        This is a problem with analysis of continuous signals that only have a small portion of the signal sample (the fft window size).
        Windowing the signal with a curve or triangle tapers the sides of the fft window to help alleviate this problem.

        [REMARKS]
        Cyclic signals such as a sine wave that repeat their cycle in a multiple of the window size do not need windowing.
        I.e. If the sine wave repeats every 1024, 512, 256 etc samples and the BreadPlayer.Fmod fft window is 1024, then the signal would not need windowing.
        Not windowing is the same as FMOD_DSP_FFT_WINDOW_RECT, which is the default.
        If the cycle of the signal (ie the sine wave) is not a multiple of the window size, it will cause frequency abnormalities, so a different windowing method is needed.
        
        FMOD_DSP_FFT_WINDOW_RECT.
        <img src="..\static\overview\rectangle.gif"></img>
        
        FMOD_DSP_FFT_WINDOW_TRIANGLE.
        <img src="..\static\overview\triangle.gif"></img>
        
        FMOD_DSP_FFT_WINDOW_HAMMING.
        <img src="..\static\overview\hamming.gif"></img>
        
        FMOD_DSP_FFT_WINDOW_HANNING.
        <img src="..\static\overview\hanning.gif"></img>
        
        FMOD_DSP_FFT_WINDOW_BLACKMAN.
        <img src="..\static\overview\blackman.gif"></img>
        
        FMOD_DSP_FFT_WINDOW_BLACKMANHARRIS.
        <img src="..\static\overview\blackmanharris.gif"></img>
    
        [SEE_ALSO]
        FMOD_DSP_FFT
    ]
    */
    public enum DspFFTWindow
    {
        RECT,            /* w[n] = 1.0                                                                                            */
        TRIANGLE,        /* w[n] = TRI(2n/N)                                                                                      */
        HAMMING,         /* w[n] = 0.54 - (0.46 * COS(n/N) )                                                                      */
        HANNING,         /* w[n] = 0.5 *  (1.0  - COS(n/N) )                                                                      */
        BLACKMAN,        /* w[n] = 0.42 - (0.5  * COS(n/N) ) + (0.08 * COS(2.0 * n/N) )                                           */
        BLACKMANHARRIS   /* w[n] = 0.35875 - (0.48829 * COS(1.0 * n/N)) + (0.14128 * COS(2.0 * n/N)) - (0.01168 * COS(3.0 * n/N)) */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter types for the FMOD_DSP_TYPE_FFT dsp effect.

        [REMARKS]
        Set the attributes for the spectrum analysis with FMOD_DSP_FFT_WINDOWSIZE and FMOD_DSP_FFT_WINDOWTYPE, and retrieve the results with FMOD_DSP_FFT_SPECTRUM and FMOD_DSP_FFT_DOMINANT_FREQ.
        FMOD_DSP_FFT_SPECTRUM stores its data in the FMOD_DSP_PARAMETER_DATA_TYPE_FFT.  You will need to cast to this structure to get the right data.

        [SEE_ALSO]
        DSP::setParameterFloat
        DSP::getParameterFloat
        DSP::setParameterInt
        DSP::getParameterInt
        DSP::setParameterData
        DSP::getParameterData
        FMOD_DSP_TYPE
        FMOD_DSP_FFT_WINDOW
    ]
    */
    public enum DspFFT
    {
        WINDOWSIZE,            /*  (Type:int)   - [r/w] Must be a power of 2 between 128 and 16384.  128, 256, 512, 1024, 2048, 4096, 8192, 16384 are accepted.  Default = 2048. */
        WINDOWTYPE,            /*  (Type:int)   - [r/w] Refer to FMOD_DSP_FFT_WINDOW enumeration.  Default = FMOD_DSP_FFT_WINDOW_HAMMING. */
        SPECTRUMDATA,          /*  (Type:data)  - [r]   Returns the current spectrum values between 0 and 1 for each 'fft bin'.  Cast data to FMOD_DSP_PARAMETER_DATA_TYPE_FFT.  Divide the niquist rate by the window size to get the hz value per entry. */
        DOMINANT_FREQ          /*  (Type:float) - [r]   Returns the dominant frequencies for each channel. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter types for the FMOD_DSP_TYPE_ENVELOPEFOLLOWER unit.
        This is a simple envelope follower for tracking the signal level.

        [REMARKS]
        This unit does not affect the incoming signal
        
        [SEE_ALSO]
        DSP::setParameterFloat
        DSP::getParameterFloat
        DSP::setParameterBool
        DSP::getParameterBool
        FMOD_DSP_TYPE
    ]
    */
    public enum DspEnvelopeFollower
    {
        ATTACK,      /* (Type:float) - Attack time (milliseconds), in the range from 0.1 through 1000. The default value is 20. */
        RELEASE,     /* (Type:float) - Release time (milliseconds), in the range from 10 through 5000. The default value is 100 */
        ENVELOPE,    /* (Type:float) - Current value of the envelope, in the range 0 to 1. Read-only. */
        USESIDECHAIN /* (Type:bool)  - Whether to analyse the sidechain signal instead of the input signal. The default value is false */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter types for the FMOD_DSP_TYPE_CHORUS filter.

        [REMARKS]
        Convolution Reverb reverb IR.

        [SEE_ALSO]
        DSP::setParameterFloat
        DSP::getParameterFloat
        DSP::setParameterData
        DSP::getParameterData
        FMOD_DSP_TYPE
    ]
    */
    public enum DspConvolutionReverb
    {
        IR,       /* (Type:data)  - [w]   16-bit reverb IR (short*) with an extra sample prepended to the start which specifies the number of channels. */
        WET,      /* (Type:float) - [r/w] Volume of echo signal to pass to output in dB.  -80.0 to 10.0.  Default = 0. */
        DRY       /* (Type:float) - [r/w] Original sound volume in dB.  -80.0 to 10.0.  Default = 0. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter types for the FMOD_DSP_CHANNELMIX_OUTPUTGROUPING parameter for FMOD_DSP_TYPE_CHANNELMIX effect.

        [REMARKS]

        [SEE_ALSO]
        DSP::setParameterInt
        DSP::getParameterInt
        FMOD_DSP_TYPE
    ]
    */
    public enum DspChannelMixOutput
    {
        DEFAULT,      /*  Output channel count = input channel count.  Mapping: See FMOD_SPEAKER enumeration. */
        ALLMONO,      /*  Output channel count = 1.  Mapping: Mono, Mono, Mono, Mono, Mono, Mono, ... (each channel all the way up to FMOD_MAX_CHANNEL_WIDTH channels are treated as if they were mono) */
        ALLSTEREO,    /*  Output channel count = 2.  Mapping: Left, Right, Left, Right, Left, Right, ... (each pair of channels is treated as stereo all the way up to FMOD_MAX_CHANNEL_WIDTH channels) */
        ALLQUAD,      /*  Output channel count = 4.  Mapping: Repeating pattern of Front Left, Front Right, Surround Left, Surround Right. */
        ALL5POINT1,   /*  Output channel count = 6.  Mapping: Repeating pattern of Front Left, Front Right, Center, LFE, Surround Left, Surround Right. */
        ALL7POINT1,   /*  Output channel count = 8.  Mapping: Repeating pattern of Front Left, Front Right, Center, LFE, Surround Left, Surround Right, Back Left, Back Right.  */
        ALLLFE        /*  Output channel count = 6.  Mapping: Repeating pattern of LFE in a 5.1 output signal.  */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter types for the FMOD_DSP_TYPE_CHANNELMIX filter.

        [REMARKS]
        For FMOD_DSP_CHANNELMIX_OUTPUTGROUPING, this value will set the output speaker format for the DSP, and also map the incoming channels to the 
        outgoing channels in a round-robin fashion.  Use this for example play a 32 channel input signal as if it were a repeating group of output signals.
        Ie.
        FMOD_DSP_CHANNELMIX_OUTPUT_ALLMONO    = all incoming channels are mixed to a mono output.
        FMOD_DSP_CHANNELMIX_OUTPUT_ALLSTEREO  = all incoming channels are mixed to a stereo output, ie even incoming channels 0,2,4,6,etc are mixed to left, and odd incoming channels 1,3,5,7,etc are mixed to right.
        FMOD_DSP_CHANNELMIX_OUTPUT_ALL5POINT1 = all incoming channels are mixed to a 5.1 output.  If there are less than 6 coming in, it will just fill the first n channels in the 6 output channels.   
                                                 If there are more, then it will repeat the input pattern to the output like it did with the stereo case, ie 12 incoming channels are mapped as 0-5 mixed to the 
                                                 5.1 output and 6 to 11 mapped to the 5.1 output.
        FMOD_DSP_CHANNELMIX_OUTPUT_ALLLFE     = all incoming channels are mixed to a 5.1 output but via the LFE channel only.


        [SEE_ALSO]
        DSP::setParameterInt
        DSP::getParameterInt
        DSP::setParameterFloat
        DSP::getParameterFloat
        FMOD_DSP_TYPE
    ]
    */
    public enum DspChannelMix
    {
        OUTPUTGROUPING,     /* (Type:int)   - Refer to FMOD_DSP_CHANNELMIX_OUTPUT enumeration.  Default = FMOD_DSP_CHANNELMIX_OUTPUT_DEFAULT.  See remarks. */
        GAIN_CH0,           /* (Type:float) - Channel  #0 gain in dB.  -80.0 to 10.0.  Default = 0. */
        GAIN_CH1,           /* (Type:float) - Channel  #1 gain in dB.  -80.0 to 10.0.  Default = 0. */
        GAIN_CH2,           /* (Type:float) - Channel  #2 gain in dB.  -80.0 to 10.0.  Default = 0. */
        GAIN_CH3,           /* (Type:float) - Channel  #3 gain in dB.  -80.0 to 10.0.  Default = 0. */
        GAIN_CH4,           /* (Type:float) - Channel  #4 gain in dB.  -80.0 to 10.0.  Default = 0. */
        GAIN_CH5,           /* (Type:float) - Channel  #5 gain in dB.  -80.0 to 10.0.  Default = 0. */
        GAIN_CH6,           /* (Type:float) - Channel  #6 gain in dB.  -80.0 to 10.0.  Default = 0. */
        GAIN_CH7,           /* (Type:float) - Channel  #7 gain in dB.  -80.0 to 10.0.  Default = 0. */
        GAIN_CH8,           /* (Type:float) - Channel  #8 gain in dB.  -80.0 to 10.0.  Default = 0. */
        GAIN_CH9,           /* (Type:float) - Channel  #9 gain in dB.  -80.0 to 10.0.  Default = 0. */
        GAIN_CH10,          /* (Type:float) - Channel #10 gain in dB.  -80.0 to 10.0.  Default = 0. */
        GAIN_CH11,          /* (Type:float) - Channel #11 gain in dB.  -80.0 to 10.0.  Default = 0. */
        GAIN_CH12,          /* (Type:float) - Channel #12 gain in dB.  -80.0 to 10.0.  Default = 0. */
        GAIN_CH13,          /* (Type:float) - Channel #13 gain in dB.  -80.0 to 10.0.  Default = 0. */
        GAIN_CH14,          /* (Type:float) - Channel #14 gain in dB.  -80.0 to 10.0.  Default = 0. */
        GAIN_CH15,          /* (Type:float) - Channel #15 gain in dB.  -80.0 to 10.0.  Default = 0. */
        GAIN_CH16,          /* (Type:float) - Channel #16 gain in dB.  -80.0 to 10.0.  Default = 0. */
        GAIN_CH17,          /* (Type:float) - Channel #17 gain in dB.  -80.0 to 10.0.  Default = 0. */
        GAIN_CH18,          /* (Type:float) - Channel #18 gain in dB.  -80.0 to 10.0.  Default = 0. */
        GAIN_CH19,          /* (Type:float) - Channel #19 gain in dB.  -80.0 to 10.0.  Default = 0. */
        GAIN_CH20,          /* (Type:float) - Channel #20 gain in dB.  -80.0 to 10.0.  Default = 0. */
        GAIN_CH21,          /* (Type:float) - Channel #21 gain in dB.  -80.0 to 10.0.  Default = 0. */
        GAIN_CH22,          /* (Type:float) - Channel #22 gain in dB.  -80.0 to 10.0.  Default = 0. */
        GAIN_CH23,          /* (Type:float) - Channel #23 gain in dB.  -80.0 to 10.0.  Default = 0. */
        GAIN_CH24,          /* (Type:float) - Channel #24 gain in dB.  -80.0 to 10.0.  Default = 0. */
        GAIN_CH25,          /* (Type:float) - Channel #25 gain in dB.  -80.0 to 10.0.  Default = 0. */
        GAIN_CH26,          /* (Type:float) - Channel #26 gain in dB.  -80.0 to 10.0.  Default = 0. */
        GAIN_CH27,          /* (Type:float) - Channel #27 gain in dB.  -80.0 to 10.0.  Default = 0. */
        GAIN_CH28,          /* (Type:float) - Channel #28 gain in dB.  -80.0 to 10.0.  Default = 0. */
        GAIN_CH29,          /* (Type:float) - Channel #29 gain in dB.  -80.0 to 10.0.  Default = 0. */
        GAIN_CH30,          /* (Type:float) - Channel #30 gain in dB.  -80.0 to 10.0.  Default = 0. */
        GAIN_CH31           /* (Type:float) - Channel #31 gain in dB.  -80.0 to 10.0.  Default = 0. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter types for the FMOD_DSP_TRANSCEIVER_SPEAKERMODE parameter for FMOD_DSP_TYPE_TRANSCEIVER effect.

        [REMARKS]
        The speaker mode of a transceiver buffer (of which there are up to 32 of) is determined automatically depending on the signal flowing through the transceiver effect, or it can be forced.
        Use a smaller fixed speaker mode buffer to save memory.

        Only relevant for transmitter dsps, as they control the format of the transceiver channel's buffer.

        If multiple transceivers transmit to a single buffer in different speaker modes, it will allocate memory for each speaker mode.   This uses more memory than a single speaker mode.
        If there are multiple receivers reading from a channel with multiple speaker modes, it will read them all and mix them together.

        If the system's speaker mode is stereo or mono, it will not create a 3rd buffer, it will just use the mono/stereo speaker mode buffer.

        [SEE_ALSO]
        DSP::setParameterInt
        DSP::getParameterInt
        FMOD_DSP_TYPE
    ]
    */
    public enum DspTransceiverSpeakerMode
    {
        AUTO = -1,     /* A transmitter will use whatever signal channel count coming in to the transmitter, to determine which speaker mode is allocated for the transceiver channel. */
        MONO = 0,      /* A transmitter will always downmix to a mono channel buffer. */
        STEREO,        /* A transmitter will always upmix or downmix to a stereo channel buffer. */
        SURROUND,      /* A transmitter will always upmix or downmix to a surround channel buffer.   Surround is the speaker mode of the system above stereo, so could be quad/surround/5.1/7.1. */
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter types for the FMOD_DSP_TYPE_TRANSCEIVER filter.

        [REMARKS]
        The transceiver only transmits and receives to a global array of 32 channels.   The transceiver can be set to receiver mode (like a return) and can receive the signal at a variable gain (FMOD_DSP_TRANSCEIVER_GAIN).
        The transceiver can also be set to transmit to a chnnel (like a send) and can transmit the signal with a variable gain (FMOD_DSP_TRANSCEIVER_GAIN).
    
        The FMOD_DSP_TRANSCEIVER_TRANSMITSPEAKERMODE is only applicable to the transmission format, not the receive format.   This means this parameter is ignored in 'receive mode'.  This allows receivers to receive at
        the speaker mode of the user's choice.   Receiving from a mono channel, is cheaper than receiving from a surround channel for example.
        The 3 speaker modes FMOD_DSP_TRANSCEIVER_SPEAKERMODE_MONO, FMOD_DSP_TRANSCEIVER_SPEAKERMODE_STEREO, FMOD_DSP_TRANSCEIVER_SPEAKERMODE_SURROUND are stored as seperate buffers in memory for a tranmitter channel.
        To save memory, use 1 common speaker mode for a transmitter.

        The transceiver is double buffered to avoid desyncing of transmitters and receivers.   This means there will be a 1 block delay on a receiver, compared to the data sent from a transmitter.

        Multiple transmitters sending to the same channel will be mixed together.

        [SEE_ALSO]
        DSP::setParameterFloat
        DSP::getParameterFloat
        DSP::setParameterInt
        DSP::getParameterInt
        DSP::setParameterBool
        DSP::getParameterBool
        FMOD_DSP_TYPE
    ]
    */
    public enum DspTransceiver
    {
        TRANSMIT,            /* (Type:bool)  - [r/w] - FALSE = Transceiver is a 'receiver' (like a return) and accepts data from a channel.  TRUE = Transceiver is a 'transmitter' (like a send).  Default = FALSE. */
        GAIN,                /* (Type:float) - [r/w] - Gain to receive or transmit at in dB.  -80.0 to 10.0.  Default = 0. */
        CHANNEL,             /* (Type:int)   - [r/w] - Integer to select current global slot, shared by all Transceivers, that can be transmitted to or received from.  0 to 31.  Default = 0.*/
        TRANSMITSPEAKERMODE  /* (Type:int)   - [r/w] - Speaker mode (transmitter mode only).  Specifies either 0 (Auto) Default = 0.*/
    }


    /*
    [ENUM]
    [
        [DESCRIPTION]
        Parameter types for the FMOD_DSP_TYPE_OBJECTPAN DSP.  3D Object panners are meant for hardware 3d object systems like Dolby Atmos or Sony Morpheus.
        These object panners take input in, and send it to the 7.1 bed, but do not send the signal further down the DSP chain (the output of the dsp is silence).

        [REMARKS]

        [SEE_ALSO]
        DSP::setParameterFloat
        DSP::getParameterFloat
        DSP::setParameterInt
        DSP::getParameterInt
        DSP::setParameterData
        DSP::getParameterData
        FMOD_DSP_TYPE
    ]
    */
    public enum DspObjectPan
    {
        _3D_POSITION,        /* (Type:data)  - 3D Position.               data of type FMOD_DSP_PARAMETER_3DATTRIBUTES_MULTI */
        _3D_ROLLOFF,         /* (Type:int)   - 3D Rolloff.                FMOD_DSP_PAN_3D_ROLLOFF_LINEARSQUARED to FMOD_DSP_PAN_3D_ROLLOFF_CUSTOM.  Default = FMOD_DSP_PAN_3D_ROLLOFF_LINEARSQUARED. */
        _3D_MIN_DISTANCE,    /* (Type:float) - 3D Min Distance.           0.0 to 1e+19f.  Default = 1.0. */
        _3D_MAX_DISTANCE,    /* (Type:float) - 3D Max Distance.           0.0 to 1e+19f.  Default = 20.0. */
        _3D_EXTENT_MODE,     /* (Type:int)   - 3D Extent Mode.            FMOD_DSP_PAN_3D_EXTENT_MODE_AUTO to FMOD_DSP_PAN_3D_EXTENT_MODE_OFF.  Default = FMOD_DSP_PAN_3D_EXTENT_MODE_AUTO. */
        _3D_SOUND_SIZE,      /* (Type:float) - 3D Sound Size.             0.0 to 1e+19f.  Default = 0.0. */
        _3D_MIN_EXTENT,      /* (Type:float) - 3D Min Extent.             0.0 (degrees) to 360.0 (degrees).  Default = 0.0. */
        OVERALL_GAIN,        /* (Type:data)  - Overall gain.              For information only, not set by user.  Data of type FMOD_DSP_PARAMETER_DATA_TYPE_OVERALLGAIN to provide to BreadPlayer.Fmod, to allow BreadPlayer.Fmod to know the DSP is scaling the signal for virtualization purposes. */
        OUTPUTGAIN           /* (Type:float) - Output gain level.         0.0 to 1.0 linear scale.  For the user to scale the output of the object panner's signal. */
    }
}
