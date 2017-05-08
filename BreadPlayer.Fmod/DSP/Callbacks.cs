using BreadPlayer.Fmod.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BreadPlayer.Fmod.CoreDSP
{
    public class Callbacks
    {
        /*
        DSP callbacks
        */
        public delegate Result DSP_CREATECALLBACK(ref DspState dsp_state);
        public delegate Result DSP_RELEASECALLBACK(ref DspState dsp_state);
        public delegate Result DSP_RESETCALLBACK(ref DspState dsp_state);
        public delegate Result DSP_SETPOSITIONCALLBACK(ref DspState dsp_state, uint pos);
        public delegate Result DSP_READCALLBACK(ref DspState dsp_state, IntPtr inbuffer, IntPtr outbuffer, uint length, int inchannels, ref int outchannels);
        public delegate Result DSP_SHOULDIPROCESS_CALLBACK(ref DspState dsp_state, bool inputsidle, uint length, ChannelMask inmask, int inchannels, SpeakerMode speakermode);
        public delegate Result DSP_PROCESS_CALLBACK(ref DspState dsp_state, uint length, ref DspBufferArray inbufferarray, ref DspBufferArray outbufferarray, bool inputsidle, DspProcessOperation op);

        public delegate Result DSP_SETPARAM_FLOAT_CALLBACK(ref DspState dsp_state, int index, float value);
        public delegate Result DSP_SETPARAM_INT_CALLBACK(ref DspState dsp_state, int index, int value);
        public delegate Result DSP_SETPARAM_BOOL_CALLBACK(ref DspState dsp_state, int index, bool value);
        public delegate Result DSP_SETPARAM_DATA_CALLBACK(ref DspState dsp_state, int index, IntPtr data, uint length);
        public delegate Result DSP_GETPARAM_FLOAT_CALLBACK(ref DspState dsp_state, int index, ref float value, IntPtr valuestr);
        public delegate Result DSP_GETPARAM_INT_CALLBACK(ref DspState dsp_state, int index, ref int value, IntPtr valuestr);
        public delegate Result DSP_GETPARAM_BOOL_CALLBACK(ref DspState dsp_state, int index, ref bool value, IntPtr valuestr);
        public delegate Result DSP_GETPARAM_DATA_CALLBACK(ref DspState dsp_state, int index, ref IntPtr data, ref uint length, IntPtr valuestr);

        public delegate Result DSP_SYSTEM_REGISTER_CALLBACK(ref DspState dsp_state);
        public delegate Result DSP_SYSTEM_DEREGISTER_CALLBACK(ref DspState dsp_state);
        public delegate Result DSP_SYSTEM_MIX_CALLBACK(ref DspState dsp_state, int stage);

        public delegate Result DSP_SYSTEM_GETSAMPLERATE(ref DspState dsp_state, ref int rate);
        public delegate Result DSP_SYSTEM_GETBLOCKSIZE(ref DspState dsp_state, ref uint blocksize);
        public delegate Result DSP_SYSTEM_GETSPEAKERMODE(ref DspState dsp_state, ref int speakermode_mixer, ref int speakermode_output);

        public delegate Result DSP_DFT_FFTREAL(ref DspState dsp_state, int size, IntPtr signal, IntPtr dft, IntPtr window, int signalhop);
        public delegate Result DSP_DFT_IFFTREAL(ref DspState dsp_state, int size, IntPtr dft, IntPtr signal, IntPtr window, int signalhop);

        public delegate Result DSP_PAN_SUM_MONO_MATRIX(ref DspState dsp_state, int sourceSpeakerMode, float lowFrequencyGain, float overallGain, IntPtr matrix);
        public delegate Result DSP_PAN_SUM_STEREO_MATRIX(ref DspState dsp_state, int sourceSpeakerMode, float pan, float lowFrequencyGain, float overallGain, int matrixHop, IntPtr matrix);
        public delegate Result DSP_PAN_SUM_SURROUND_MATRIX(ref DspState dsp_state, int sourceSpeakerMode, int targetSpeakerMode, float direction, float extent, float rotation, float lowFrequencyGain, float overallGain, int matrixHop, IntPtr matrix, DspPanSurroundFlags flags);
        public delegate Result DSP_PAN_SUM_MONO_TO_SURROUND_MATRIX(ref DspState dsp_state, int targetSpeakerMode, float direction, float extent, float lowFrequencyGain, float overallGain, int matrixHop, IntPtr matrix);
        public delegate Result DSP_PAN_SUM_STEREO_TO_SURROUND_MATRIX(ref DspState dsp_state, int targetSpeakerMode, float direction, float extent, float rotation, float lowFrequencyGain, float overallGain, int matrixHop, IntPtr matrix);
        public delegate Result DSP_PAN_3D_GET_ROLLOFF_GAIN(ref DspState dsp_state, DspPan3DRollOffType rolloff, float distance, float mindistance, float maxdistance, out float gain);
        public delegate Result FMOD_DSP_STATE_GETCLOCK(ref DspState dsp_state, out ulong clock, out uint offset, out uint length);
    }
}
