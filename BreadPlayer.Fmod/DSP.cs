/* ========================================================================================== */
/*                                                                                            */
/* BreadPlayer.Fmod Studio - C# Wrapper . Copyright (c), Firelight Technologies Pty, Ltd. 2004-2016.      */
/*                                                                                            */
/* ========================================================================================== */

using System;
using System.Text;
using System.Runtime.InteropServices;
using BreadPlayer.Fmod.Enums;
using BreadPlayer.Fmod.CoreDSP;

namespace BreadPlayer.Fmod
{
    /*
            'DSP' API
        */
    public class Dsp : HandleBase
    {
        public Result Release                   ()
        {
            Result result = FMOD_DSP_Release(GetRaw());
            if (result == Result.Ok)
            {
                RawPtr = IntPtr.Zero;
            }
            return result;
        }
        public Result GetSystemObject           (out FmodSystem system)
        {
            system = null;

            IntPtr systemraw;
            Result result = FMOD_DSP_GetSystemObject(RawPtr, out systemraw);
            system = new FmodSystem(systemraw);

            return result;
        }

        // Connection / disconnection / input and output enumeration.
        public Result AddInput(Dsp target, out DspConnection connection, DspConnectionType type)
        {
            connection = null;

            IntPtr dspconnectionraw;
            Result result = FMOD_DSP_AddInput(RawPtr, target.GetRaw(), out dspconnectionraw, type);
            connection = new DspConnection(dspconnectionraw);

            return result;
        }
        public Result DisconnectFrom            (Dsp target, DspConnection connection)
        {
            return FMOD_DSP_DisconnectFrom(RawPtr, target.GetRaw(), connection.GetRaw());
        }
        public Result DisconnectAll             (bool inputs, bool outputs)
        {
            return FMOD_DSP_DisconnectAll(RawPtr, inputs, outputs);
        }
        public Result GetNumInputs              (out int numinputs)
        {
            return FMOD_DSP_GetNumInputs(RawPtr, out numinputs);
        }
        public Result GetNumOutputs             (out int numoutputs)
        {
            return FMOD_DSP_GetNumOutputs(RawPtr, out numoutputs);
        }
        public Result GetInput                  (int index, out Dsp input, out DspConnection inputconnection)
        {
            input = null;
            inputconnection = null;

            IntPtr dspinputraw;
            IntPtr dspconnectionraw;
            Result result = FMOD_DSP_GetInput(RawPtr, index, out dspinputraw, out dspconnectionraw);
            input = new Dsp(dspinputraw);
            inputconnection = new DspConnection(dspconnectionraw);

            return result;
        }
        public Result GetOutput                 (int index, out Dsp output, out DspConnection outputconnection)
        {
            output = null;
            outputconnection = null;

            IntPtr dspoutputraw;
            IntPtr dspconnectionraw;
            Result result = FMOD_DSP_GetOutput(RawPtr, index, out dspoutputraw, out dspconnectionraw);
            output = new Dsp(dspoutputraw);
            outputconnection = new DspConnection(dspconnectionraw);

            return result;
        }

        // DSP unit control.
        public Result SetActive                 (bool active)
        {
            return FMOD_DSP_SetActive(RawPtr, active);
        }
        public Result GetActive                 (out bool active)
        {
            return FMOD_DSP_GetActive(RawPtr, out active);
        }
        public Result SetBypass(bool bypass)
        {
            return FMOD_DSP_SetBypass(RawPtr, bypass);
        }
        public Result GetBypass(out bool bypass)
        {
            return FMOD_DSP_GetBypass(RawPtr, out bypass);
        }
        public Result SetWetDryMix(float prewet, float postwet, float dry)
        {
            return FMOD_DSP_SetWetDryMix(RawPtr, prewet, postwet, dry);
        }
        public Result GetWetDryMix(out float prewet, out float postwet, out float dry)
        {
            return FMOD_DSP_GetWetDryMix(RawPtr, out prewet, out postwet, out dry);
        }
        public Result SetChannelFormat(ChannelMask channelmask, int numchannels, SpeakerMode sourceSpeakermode)
        {
            return FMOD_DSP_SetChannelFormat(RawPtr, channelmask, numchannels, sourceSpeakermode);
        }
        public Result GetChannelFormat(out ChannelMask channelmask, out int numchannels, out SpeakerMode sourceSpeakermode)
        {
            return FMOD_DSP_GetChannelFormat(RawPtr, out channelmask, out numchannels, out sourceSpeakermode);
        }
        public Result GetOutputChannelFormat(ChannelMask inmask, int inchannels, SpeakerMode inspeakermode, out ChannelMask outmask, out int outchannels, out SpeakerMode outspeakermode)
        {
            return FMOD_DSP_GetOutputChannelFormat(RawPtr, inmask, inchannels, inspeakermode, out outmask, out outchannels, out outspeakermode);
        }
        public Result Reset                     ()
        {
            return FMOD_DSP_Reset(RawPtr);
        }

        // DSP parameter control.
        public Result SetParameterFloat(int index, float value)
        {
            return FMOD_DSP_SetParameterFloat(RawPtr, index, value);
        }
        public Result SetParameterInt(int index, int value)
        {
            return FMOD_DSP_SetParameterInt(RawPtr, index, value);
        }
        public Result SetParameterBool(int index, bool value)
        {
            return FMOD_DSP_SetParameterBool(RawPtr, index, value);
        }
        public Result SetParameterData(int index, byte[] data)
        {
            return FMOD_DSP_SetParameterData(RawPtr, index, Marshal.UnsafeAddrOfPinnedArrayElement(data, 0), (uint)data.Length);
        }
        public Result GetParameterFloat(int index, out float value)
        {
            IntPtr valuestr = IntPtr.Zero;
            return FMOD_DSP_GetParameterFloat(RawPtr, index, out value, valuestr, 0);
        }
        public Result GetParameterInt(int index, out int value)
        {
            IntPtr valuestr = IntPtr.Zero;
            return FMOD_DSP_GetParameterInt(RawPtr, index, out value, valuestr, 0);
        }
        public Result GetParameterBool(int index, out bool value)
        {
            return FMOD_DSP_GetParameterBool(RawPtr, index, out value, IntPtr.Zero, 0);
        }
        public Result GetParameterData(int index, out IntPtr data, out uint length)
        {
            return FMOD_DSP_GetParameterData(RawPtr, index, out data, out length, IntPtr.Zero, 0);
        }
        public Result GetNumParameters          (out int numparams)
        {
            return FMOD_DSP_GetNumParameters(RawPtr, out numparams);
        }
        public Result GetParameterInfo          (int index, out DspParameterDesc desc)
        {
            IntPtr descPtr;
            Result result = FMOD_DSP_GetParameterInfo(RawPtr, index, out descPtr);
            if (result == Result.Ok)
            {
                desc = (DspParameterDesc)Marshal.PtrToStructure<DspParameterDesc>(descPtr);
            }
            else
            {
                desc = new DspParameterDesc();
            }
            return result;
        }
        public Result GetDataParameterIndex(int datatype, out int index)
        {
            return FMOD_DSP_GetDataParameterIndex     (RawPtr, datatype, out index);
        }
        public Result ShowConfigDialog          (IntPtr hwnd, bool show)
        {
            return FMOD_DSP_ShowConfigDialog          (RawPtr, hwnd, show);
        }

        //  DSP attributes.
        public Result GetInfo                   (StringBuilder name, out uint version, out int channels, out int configwidth, out int configheight)
        {
            IntPtr nameMem = Marshal.AllocHGlobal(32);
            Result result = FMOD_DSP_GetInfo(RawPtr, nameMem, out version, out channels, out configwidth, out configheight);
            StringMarshalHelper.NativeToBuilder(name, nameMem);
            Marshal.FreeHGlobal(nameMem);
            return result;
        }
        public Result GetType                   (out DspType type)
        {
            return FMOD_DSP_GetType(RawPtr, out type);
        }
        public Result GetIdle                   (out bool idle)
        {
            return FMOD_DSP_GetIdle(RawPtr, out idle);
        }

        // Userdata set/get.
        public Result SetUserData               (IntPtr userdata)
        {
            return FMOD_DSP_SetUserData(RawPtr, userdata);
        }
        public Result GetUserData               (out IntPtr userdata)
        {
            return FMOD_DSP_GetUserData(RawPtr, out userdata);
        }

        // Metering.
        public Result SetMeteringEnabled(bool inputEnabled, bool outputEnabled)
        {
            return FMOD_DSP_SetMeteringEnabled(RawPtr, inputEnabled, outputEnabled);
        }
        public Result GetMeteringEnabled(out bool inputEnabled, out bool outputEnabled)
        {
            return FMOD_DSP_GetMeteringEnabled(RawPtr, out inputEnabled, out outputEnabled);
        }

        public Result GetMeteringInfo(DspMeteringInfo inputInfo, DspMeteringInfo outputInfo)
        {
            return FMOD_DSP_GetMeteringInfo(RawPtr, inputInfo, outputInfo);
        }

        #region importfunctions

        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_Release                   (IntPtr dsp);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_GetSystemObject           (IntPtr dsp, out IntPtr system);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_AddInput                  (IntPtr dsp, IntPtr target, out IntPtr connection, DspConnectionType type);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_DisconnectFrom            (IntPtr dsp, IntPtr target, IntPtr connection);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_DisconnectAll             (IntPtr dsp, bool inputs, bool outputs);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_GetNumInputs              (IntPtr dsp, out int numinputs);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_GetNumOutputs             (IntPtr dsp, out int numoutputs);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_GetInput                  (IntPtr dsp, int index, out IntPtr input, out IntPtr inputconnection);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_GetOutput                 (IntPtr dsp, int index, out IntPtr output, out IntPtr outputconnection);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_SetActive                 (IntPtr dsp, bool active);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_GetActive                 (IntPtr dsp, out bool active);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_SetBypass                 (IntPtr dsp, bool bypass);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_GetBypass                 (IntPtr dsp, out bool bypass);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_SetWetDryMix              (IntPtr dsp, float prewet, float postwet, float dry);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_GetWetDryMix              (IntPtr dsp, out float prewet, out float postwet, out float dry);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_SetChannelFormat          (IntPtr dsp, ChannelMask channelmask, int numchannels, SpeakerMode sourceSpeakermode);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_GetChannelFormat          (IntPtr dsp, out ChannelMask channelmask, out int numchannels, out SpeakerMode sourceSpeakermode);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_GetOutputChannelFormat    (IntPtr dsp, ChannelMask inmask, int inchannels, SpeakerMode inspeakermode, out ChannelMask outmask, out int outchannels, out SpeakerMode outspeakermode);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_Reset                     (IntPtr dsp);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_SetParameterFloat         (IntPtr dsp, int index, float value);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_SetParameterInt           (IntPtr dsp, int index, int value);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_SetParameterBool          (IntPtr dsp, int index, bool value);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_SetParameterData          (IntPtr dsp, int index, IntPtr data, uint length);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_GetParameterFloat         (IntPtr dsp, int index, out float value, IntPtr valuestr, int valuestrlen);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_GetParameterInt           (IntPtr dsp, int index, out int value, IntPtr valuestr, int valuestrlen);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_GetParameterBool          (IntPtr dsp, int index, out bool value, IntPtr valuestr, int valuestrlen);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_GetParameterData          (IntPtr dsp, int index, out IntPtr data, out uint length, IntPtr valuestr, int valuestrlen);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_GetNumParameters          (IntPtr dsp, out int numparams);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_GetParameterInfo          (IntPtr dsp, int index, out IntPtr desc);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_GetDataParameterIndex     (IntPtr dsp, int datatype, out int index);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_ShowConfigDialog          (IntPtr dsp, IntPtr hwnd, bool show);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_GetInfo                   (IntPtr dsp, IntPtr name, out uint version, out int channels, out int configwidth, out int configheight);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_GetType                   (IntPtr dsp, out DspType type);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_GetIdle                   (IntPtr dsp, out bool idle);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_SetUserData               (IntPtr dsp, IntPtr userdata);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_DSP_GetUserData               (IntPtr dsp, out IntPtr userdata);
        [DllImport(FmodVersion.Dll)]
        public static extern Result FMOD_DSP_SetMeteringEnabled         (IntPtr dsp, bool inputEnabled, bool outputEnabled);
        [DllImport(FmodVersion.Dll)]
        public static extern Result FMOD_DSP_GetMeteringEnabled         (IntPtr dsp, out bool inputEnabled, out bool outputEnabled);
        [DllImport(FmodVersion.Dll)]
        public static extern Result FMOD_DSP_GetMeteringInfo            (IntPtr dsp, [Out] DspMeteringInfo inputInfo, [Out] DspMeteringInfo outputInfo);
        #endregion

        #region wrapperinternal

        public Dsp(IntPtr raw)
            : base(raw)
        {
        }

        #endregion
    }
}
