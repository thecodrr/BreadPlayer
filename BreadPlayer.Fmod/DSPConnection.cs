/* ========================================================================================== */
/*                                                                                            */
/* BreadPlayer.Fmod Studio - C# Wrapper . Copyright (c), Firelight Technologies Pty, Ltd. 2004-2016.      */
/*                                                                                            */
/* ========================================================================================== */

using BreadPlayer.Fmod.Enums;
using System;
using System.Runtime.InteropServices;

namespace BreadPlayer.Fmod
{
    /*
            'DSPConnection' API
        */
    public class DspConnection : HandleBase
    {
        public Result GetInput              (out Dsp input)
        {
            input = null;

            IntPtr dspraw;
            Result result = FMOD_DSPConnection_GetInput(RawPtr, out dspraw);
            input = new Dsp(dspraw);

            return result;
        }
        public Result GetOutput             (out Dsp output)
        {
            output = null;

            IntPtr dspraw;
            Result result = FMOD_DSPConnection_GetOutput(RawPtr, out dspraw);
            output = new Dsp(dspraw);

            return result;
        }
        public Result SetMix                (float volume)
        {
            return FMOD_DSPConnection_SetMix(RawPtr, volume);
        }
        public Result GetMix                (out float volume)
        {
            return FMOD_DSPConnection_GetMix(RawPtr, out volume);
        }
        public Result SetMixMatrix(float[] matrix, int outchannels, int inchannels, int inchannelHop)
        {
            return FMOD_DSPConnection_SetMixMatrix(RawPtr, matrix, outchannels, inchannels, inchannelHop);
        }
        public Result GetMixMatrix(float[] matrix, out int outchannels, out int inchannels, int inchannelHop)
        {
            return FMOD_DSPConnection_GetMixMatrix(RawPtr, matrix, out outchannels, out inchannels, inchannelHop);
        }
        public Result getType(out DspConnectionType type)
        {
            return FMOD_DSPConnection_GetType(RawPtr, out type);
        }

        // Userdata set/get.
        public Result SetUserData(IntPtr userdata)
        {
            return FMOD_DSPConnection_SetUserData(RawPtr, userdata);
        }
        public Result GetUserData(out IntPtr userdata)
        {
            return FMOD_DSPConnection_GetUserData(RawPtr, out userdata);
        }

        #region importfunctions
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSPConnection_GetInput        (IntPtr dspconnection, out IntPtr input);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSPConnection_GetOutput       (IntPtr dspconnection, out IntPtr output);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSPConnection_SetMix          (IntPtr dspconnection, float volume);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSPConnection_GetMix          (IntPtr dspconnection, out float volume);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSPConnection_SetMixMatrix    (IntPtr dspconnection, float[] matrix, int outchannels, int inchannels, int inchannelHop);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSPConnection_GetMixMatrix    (IntPtr dspconnection, float[] matrix, out int outchannels, out int inchannels, int inchannelHop);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSPConnection_GetType         (IntPtr dspconnection, out DspConnectionType type);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSPConnection_SetUserData     (IntPtr dspconnection, IntPtr userdata);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSPConnection_GetUserData     (IntPtr dspconnection, out IntPtr userdata);
        #endregion

        #region wrapperinternal

        public DspConnection(IntPtr raw)
            : base(raw)
        {
        }

        #endregion
    }
}
