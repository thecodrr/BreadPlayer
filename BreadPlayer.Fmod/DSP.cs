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
    public class DSP : HandleBase
    {
        public Result release                   ()
        {
            Result result = FMOD_DSP_Release(getRaw());
            if (result == Result.OK)
            {
                rawPtr = IntPtr.Zero;
            }
            return result;
        }
        public Result getSystemObject           (out FMODSystem system)
        {
            system = null;

            IntPtr systemraw;
            Result result = FMOD_DSP_GetSystemObject(rawPtr, out systemraw);
            system = new FMODSystem(systemraw);

            return result;
        }

        // Connection / disconnection / input and output enumeration.
        public Result addInput(DSP target, out DSPConnection connection, DspConnectionType type)
        {
            connection = null;

            IntPtr dspconnectionraw;
            Result result = FMOD_DSP_AddInput(rawPtr, target.getRaw(), out dspconnectionraw, type);
            connection = new DSPConnection(dspconnectionraw);

            return result;
        }
        public Result disconnectFrom            (DSP target, DSPConnection connection)
        {
            return FMOD_DSP_DisconnectFrom(rawPtr, target.getRaw(), connection.getRaw());
        }
        public Result disconnectAll             (bool inputs, bool outputs)
        {
            return FMOD_DSP_DisconnectAll(rawPtr, inputs, outputs);
        }
        public Result getNumInputs              (out int numinputs)
        {
            return FMOD_DSP_GetNumInputs(rawPtr, out numinputs);
        }
        public Result getNumOutputs             (out int numoutputs)
        {
            return FMOD_DSP_GetNumOutputs(rawPtr, out numoutputs);
        }
        public Result getInput                  (int index, out DSP input, out DSPConnection inputconnection)
        {
            input = null;
            inputconnection = null;

            IntPtr dspinputraw;
            IntPtr dspconnectionraw;
            Result result = FMOD_DSP_GetInput(rawPtr, index, out dspinputraw, out dspconnectionraw);
            input = new DSP(dspinputraw);
            inputconnection = new DSPConnection(dspconnectionraw);

            return result;
        }
        public Result getOutput                 (int index, out DSP output, out DSPConnection outputconnection)
        {
            output = null;
            outputconnection = null;

            IntPtr dspoutputraw;
            IntPtr dspconnectionraw;
            Result result = FMOD_DSP_GetOutput(rawPtr, index, out dspoutputraw, out dspconnectionraw);
            output = new DSP(dspoutputraw);
            outputconnection = new DSPConnection(dspconnectionraw);

            return result;
        }

        // DSP unit control.
        public Result setActive                 (bool active)
        {
            return FMOD_DSP_SetActive(rawPtr, active);
        }
        public Result getActive                 (out bool active)
        {
            return FMOD_DSP_GetActive(rawPtr, out active);
        }
        public Result setBypass(bool bypass)
        {
            return FMOD_DSP_SetBypass(rawPtr, bypass);
        }
        public Result getBypass(out bool bypass)
        {
            return FMOD_DSP_GetBypass(rawPtr, out bypass);
        }
        public Result setWetDryMix(float prewet, float postwet, float dry)
        {
            return FMOD_DSP_SetWetDryMix(rawPtr, prewet, postwet, dry);
        }
        public Result getWetDryMix(out float prewet, out float postwet, out float dry)
        {
            return FMOD_DSP_GetWetDryMix(rawPtr, out prewet, out postwet, out dry);
        }
        public Result setChannelFormat(ChannelMask channelmask, int numchannels, SpeakerMode source_speakermode)
        {
            return FMOD_DSP_SetChannelFormat(rawPtr, channelmask, numchannels, source_speakermode);
        }
        public Result getChannelFormat(out ChannelMask channelmask, out int numchannels, out SpeakerMode source_speakermode)
        {
            return FMOD_DSP_GetChannelFormat(rawPtr, out channelmask, out numchannels, out source_speakermode);
        }
        public Result getOutputChannelFormat(ChannelMask inmask, int inchannels, SpeakerMode inspeakermode, out ChannelMask outmask, out int outchannels, out SpeakerMode outspeakermode)
        {
            return FMOD_DSP_GetOutputChannelFormat(rawPtr, inmask, inchannels, inspeakermode, out outmask, out outchannels, out outspeakermode);
        }
        public Result reset                     ()
        {
            return FMOD_DSP_Reset(rawPtr);
        }

        // DSP parameter control.
        public Result setParameterFloat(int index, float value)
        {
            return FMOD_DSP_SetParameterFloat(rawPtr, index, value);
        }
        public Result setParameterInt(int index, int value)
        {
            return FMOD_DSP_SetParameterInt(rawPtr, index, value);
        }
        public Result setParameterBool(int index, bool value)
        {
            return FMOD_DSP_SetParameterBool(rawPtr, index, value);
        }
        public Result setParameterData(int index, byte[] data)
        {
            return FMOD_DSP_SetParameterData(rawPtr, index, Marshal.UnsafeAddrOfPinnedArrayElement(data, 0), (uint)data.Length);
        }
        public Result getParameterFloat(int index, out float value)
        {
            IntPtr valuestr = IntPtr.Zero;
            return FMOD_DSP_GetParameterFloat(rawPtr, index, out value, valuestr, 0);
        }
        public Result getParameterInt(int index, out int value)
        {
            IntPtr valuestr = IntPtr.Zero;
            return FMOD_DSP_GetParameterInt(rawPtr, index, out value, valuestr, 0);
        }
        public Result getParameterBool(int index, out bool value)
        {
            return FMOD_DSP_GetParameterBool(rawPtr, index, out value, IntPtr.Zero, 0);
        }
        public Result getParameterData(int index, out IntPtr data, out uint length)
        {
            return FMOD_DSP_GetParameterData(rawPtr, index, out data, out length, IntPtr.Zero, 0);
        }
        public Result getNumParameters          (out int numparams)
        {
            return FMOD_DSP_GetNumParameters(rawPtr, out numparams);
        }
        public Result getParameterInfo          (int index, out DspParameterDesc desc)
        {
            IntPtr descPtr;
            Result result = FMOD_DSP_GetParameterInfo(rawPtr, index, out descPtr);
            if (result == Result.OK)
            {
                desc = (DspParameterDesc)Marshal.PtrToStructure<DspParameterDesc>(descPtr);
            }
            else
            {
                desc = new DspParameterDesc();
            }
            return result;
        }
        public Result getDataParameterIndex(int datatype, out int index)
        {
            return FMOD_DSP_GetDataParameterIndex     (rawPtr, datatype, out index);
        }
        public Result showConfigDialog          (IntPtr hwnd, bool show)
        {
            return FMOD_DSP_ShowConfigDialog          (rawPtr, hwnd, show);
        }

        //  DSP attributes.
        public Result getInfo                   (StringBuilder name, out uint version, out int channels, out int configwidth, out int configheight)
        {
            IntPtr nameMem = Marshal.AllocHGlobal(32);
            Result result = FMOD_DSP_GetInfo(rawPtr, nameMem, out version, out channels, out configwidth, out configheight);
            StringMarshalHelper.NativeToBuilder(name, nameMem);
            Marshal.FreeHGlobal(nameMem);
            return result;
        }
        public Result GetType                   (out DspType type)
        {
            return FMOD_DSP_GetType(rawPtr, out type);
        }
        public Result getIdle                   (out bool idle)
        {
            return FMOD_DSP_GetIdle(rawPtr, out idle);
        }

        // Userdata set/get.
        public Result setUserData               (IntPtr userdata)
        {
            return FMOD_DSP_SetUserData(rawPtr, userdata);
        }
        public Result getUserData               (out IntPtr userdata)
        {
            return FMOD_DSP_GetUserData(rawPtr, out userdata);
        }

        // Metering.
        public Result setMeteringEnabled(bool inputEnabled, bool outputEnabled)
        {
            return FMOD_DSP_SetMeteringEnabled(rawPtr, inputEnabled, outputEnabled);
        }
        public Result getMeteringEnabled(out bool inputEnabled, out bool outputEnabled)
        {
            return FMOD_DSP_GetMeteringEnabled(rawPtr, out inputEnabled, out outputEnabled);
        }

        public Result GetMeteringInfo(DspMeteringInfo inputInfo, DspMeteringInfo outputInfo)
        {
            return FMOD_DSP_GetMeteringInfo(rawPtr, inputInfo, outputInfo);
        }

        #region importfunctions

        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_Release                   (IntPtr dsp);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_GetSystemObject           (IntPtr dsp, out IntPtr system);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_AddInput                  (IntPtr dsp, IntPtr target, out IntPtr connection, DspConnectionType type);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_DisconnectFrom            (IntPtr dsp, IntPtr target, IntPtr connection);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_DisconnectAll             (IntPtr dsp, bool inputs, bool outputs);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_GetNumInputs              (IntPtr dsp, out int numinputs);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_GetNumOutputs             (IntPtr dsp, out int numoutputs);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_GetInput                  (IntPtr dsp, int index, out IntPtr input, out IntPtr inputconnection);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_GetOutput                 (IntPtr dsp, int index, out IntPtr output, out IntPtr outputconnection);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_SetActive                 (IntPtr dsp, bool active);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_GetActive                 (IntPtr dsp, out bool active);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_SetBypass                 (IntPtr dsp, bool bypass);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_GetBypass                 (IntPtr dsp, out bool bypass);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_SetWetDryMix              (IntPtr dsp, float prewet, float postwet, float dry);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_GetWetDryMix              (IntPtr dsp, out float prewet, out float postwet, out float dry);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_SetChannelFormat          (IntPtr dsp, ChannelMask channelmask, int numchannels, SpeakerMode source_speakermode);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_GetChannelFormat          (IntPtr dsp, out ChannelMask channelmask, out int numchannels, out SpeakerMode source_speakermode);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_GetOutputChannelFormat    (IntPtr dsp, ChannelMask inmask, int inchannels, SpeakerMode inspeakermode, out ChannelMask outmask, out int outchannels, out SpeakerMode outspeakermode);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_Reset                     (IntPtr dsp);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_SetParameterFloat         (IntPtr dsp, int index, float value);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_SetParameterInt           (IntPtr dsp, int index, int value);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_SetParameterBool          (IntPtr dsp, int index, bool value);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_SetParameterData          (IntPtr dsp, int index, IntPtr data, uint length);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_GetParameterFloat         (IntPtr dsp, int index, out float value, IntPtr valuestr, int valuestrlen);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_GetParameterInt           (IntPtr dsp, int index, out int value, IntPtr valuestr, int valuestrlen);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_GetParameterBool          (IntPtr dsp, int index, out bool value, IntPtr valuestr, int valuestrlen);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_GetParameterData          (IntPtr dsp, int index, out IntPtr data, out uint length, IntPtr valuestr, int valuestrlen);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_GetNumParameters          (IntPtr dsp, out int numparams);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_GetParameterInfo          (IntPtr dsp, int index, out IntPtr desc);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_GetDataParameterIndex     (IntPtr dsp, int datatype, out int index);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_ShowConfigDialog          (IntPtr dsp, IntPtr hwnd, bool show);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_GetInfo                   (IntPtr dsp, IntPtr name, out uint version, out int channels, out int configwidth, out int configheight);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_GetType                   (IntPtr dsp, out DspType type);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_GetIdle                   (IntPtr dsp, out bool idle);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_SetUserData               (IntPtr dsp, IntPtr userdata);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_DSP_GetUserData               (IntPtr dsp, out IntPtr userdata);
        [DllImport(FMODVersion.DLL)]
        public static extern Result FMOD_DSP_SetMeteringEnabled         (IntPtr dsp, bool inputEnabled, bool outputEnabled);
        [DllImport(FMODVersion.DLL)]
        public static extern Result FMOD_DSP_GetMeteringEnabled         (IntPtr dsp, out bool inputEnabled, out bool outputEnabled);
        [DllImport(FMODVersion.DLL)]
        public static extern Result FMOD_DSP_GetMeteringInfo            (IntPtr dsp, [Out] DspMeteringInfo inputInfo, [Out] DspMeteringInfo outputInfo);
        #endregion

        #region wrapperinternal

        public DSP(IntPtr raw)
            : base(raw)
        {
        }

        #endregion
    }
}
