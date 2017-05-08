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
    public class DSPConnection : HandleBase
    {
        public Result getInput              (out DSP input)
        {
            input = null;

            IntPtr dspraw;
            Result result = FMOD_DSPConnection_GetInput(rawPtr, out dspraw);
            input = new DSP(dspraw);

            return result;
        }
        public Result getOutput             (out DSP output)
        {
            output = null;

            IntPtr dspraw;
            Result result = FMOD_DSPConnection_GetOutput(rawPtr, out dspraw);
            output = new DSP(dspraw);

            return result;
        }
        public Result setMix                (float volume)
        {
            return FMOD_DSPConnection_SetMix(rawPtr, volume);
        }
        public Result getMix                (out float volume)
        {
            return FMOD_DSPConnection_GetMix(rawPtr, out volume);
        }
        public Result setMixMatrix(float[] matrix, int outchannels, int inchannels, int inchannel_hop)
        {
            return FMOD_DSPConnection_SetMixMatrix(rawPtr, matrix, outchannels, inchannels, inchannel_hop);
        }
        public Result getMixMatrix(float[] matrix, out int outchannels, out int inchannels, int inchannel_hop)
        {
            return FMOD_DSPConnection_GetMixMatrix(rawPtr, matrix, out outchannels, out inchannels, inchannel_hop);
        }
        public Result getType(out DspConnectionType type)
        {
            return FMOD_DSPConnection_GetType(rawPtr, out type);
        }

        // Userdata set/get.
        public Result setUserData(IntPtr userdata)
        {
            return FMOD_DSPConnection_SetUserData(rawPtr, userdata);
        }
        public Result getUserData(out IntPtr userdata)
        {
            return FMOD_DSPConnection_GetUserData(rawPtr, out userdata);
        }

        #region importfunctions
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSPConnection_GetInput        (IntPtr dspconnection, out IntPtr input);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSPConnection_GetOutput       (IntPtr dspconnection, out IntPtr output);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSPConnection_SetMix          (IntPtr dspconnection, float volume);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSPConnection_GetMix          (IntPtr dspconnection, out float volume);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSPConnection_SetMixMatrix    (IntPtr dspconnection, float[] matrix, int outchannels, int inchannels, int inchannel_hop);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSPConnection_GetMixMatrix    (IntPtr dspconnection, float[] matrix, out int outchannels, out int inchannels, int inchannel_hop);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSPConnection_GetType         (IntPtr dspconnection, out DspConnectionType type);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSPConnection_SetUserData     (IntPtr dspconnection, IntPtr userdata);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSPConnection_GetUserData     (IntPtr dspconnection, out IntPtr userdata);
        #endregion

        #region wrapperinternal

        public DSPConnection(IntPtr raw)
            : base(raw)
        {
        }

        #endregion
    }
}
