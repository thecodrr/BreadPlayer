using System;
using BreadPlayer.Fmod.Enums;

namespace BreadPlayer.Fmod.CoreDSP
{
    public class Callbacks
    {
        /*
        DSP callbacks
        */
        public delegate Result DspCreatecallback(ref DspState dspState);
        public delegate Result DspReleasecallback(ref DspState dspState);
        public delegate Result DspResetcallback(ref DspState dspState);
        public delegate Result DspSetpositioncallback(ref DspState dspState, uint pos);
        public delegate Result DspReadcallback(ref DspState dspState, IntPtr inbuffer, IntPtr outbuffer, uint length, int inchannels, ref int outchannels);
        public delegate Result DspShouldiprocessCallback(ref DspState dspState, bool inputsidle, uint length, ChannelMask inmask, int inchannels, SpeakerMode speakermode);
        public delegate Result DspProcessCallback(ref DspState dspState, uint length, ref DspBufferArray inbufferarray, ref DspBufferArray outbufferarray, bool inputsidle, DspProcessOperation op);

        public delegate Result DspSetparamFloatCallback(ref DspState dspState, int index, float value);
        public delegate Result DspSetparamIntCallback(ref DspState dspState, int index, int value);
        public delegate Result DspSetparamBoolCallback(ref DspState dspState, int index, bool value);
        public delegate Result DspSetparamDataCallback(ref DspState dspState, int index, IntPtr data, uint length);
        public delegate Result DspGetparamFloatCallback(ref DspState dspState, int index, ref float value, IntPtr valuestr);
        public delegate Result DspGetparamIntCallback(ref DspState dspState, int index, ref int value, IntPtr valuestr);
        public delegate Result DspGetparamBoolCallback(ref DspState dspState, int index, ref bool value, IntPtr valuestr);
        public delegate Result DspGetparamDataCallback(ref DspState dspState, int index, ref IntPtr data, ref uint length, IntPtr valuestr);

        public delegate Result DspSystemRegisterCallback(ref DspState dspState);
        public delegate Result DspSystemDeregisterCallback(ref DspState dspState);
        public delegate Result DspSystemMixCallback(ref DspState dspState, int stage);

        public delegate Result DspSystemGetsamplerate(ref DspState dspState, ref int rate);
        public delegate Result DspSystemGetblocksize(ref DspState dspState, ref uint blocksize);
        public delegate Result DspSystemGetspeakermode(ref DspState dspState, ref int speakermodeMixer, ref int speakermodeOutput);

        public delegate Result DspDftFftreal(ref DspState dspState, int size, IntPtr signal, IntPtr dft, IntPtr window, int signalhop);
        public delegate Result DspDftIfftreal(ref DspState dspState, int size, IntPtr dft, IntPtr signal, IntPtr window, int signalhop);

        public delegate Result DspPanSumMonoMatrix(ref DspState dspState, int sourceSpeakerMode, float lowFrequencyGain, float overallGain, IntPtr matrix);
        public delegate Result DspPanSumStereoMatrix(ref DspState dspState, int sourceSpeakerMode, float pan, float lowFrequencyGain, float overallGain, int matrixHop, IntPtr matrix);
        public delegate Result DspPanSumSurroundMatrix(ref DspState dspState, int sourceSpeakerMode, int targetSpeakerMode, float direction, float extent, float rotation, float lowFrequencyGain, float overallGain, int matrixHop, IntPtr matrix, DspPanSurroundFlags flags);
        public delegate Result DspPanSumMonoToSurroundMatrix(ref DspState dspState, int targetSpeakerMode, float direction, float extent, float lowFrequencyGain, float overallGain, int matrixHop, IntPtr matrix);
        public delegate Result DspPanSumStereoToSurroundMatrix(ref DspState dspState, int targetSpeakerMode, float direction, float extent, float rotation, float lowFrequencyGain, float overallGain, int matrixHop, IntPtr matrix);
        public delegate Result DspPan_3DGetRolloffGain(ref DspState dspState, DspPan3DRollOffType rolloff, float distance, float mindistance, float maxdistance, out float gain);
        public delegate Result FmodDspStateGetclock(ref DspState dspState, out ulong clock, out uint offset, out uint length);
    }
}
