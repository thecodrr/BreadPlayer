/* ========================================================================================== */
/*                                                                                            */
/* BreadPlayer.Fmod Studio - C# Wrapper . Copyright (c), Firelight Technologies Pty, Ltd. 2004-2016.      */
/*                                                                                            */
/* ========================================================================================== */

using System;
using System.Text;
using System.Runtime.InteropServices;
using BreadPlayer.Fmod.Enums;
using BreadPlayer.Fmod.Structs;
using static BreadPlayer.Fmod.Callbacks;
using BreadPlayer.Fmod.CoreDSP;

namespace BreadPlayer.Fmod
{
    /*
            'FMODSystem' API.
        */
    public class FmodSystem : HandleBase
    {
        public Result Release()
        {
            Result result = FMOD_System_Release(RawPtr);
            if (result == Result.Ok)
            {
                RawPtr = IntPtr.Zero;
            }
            return result;
        }


        // Pre-init functions.
        public OutputType Output
        {
            get { FMOD_System_GetOutput(RawPtr, out OutputType output); return output; }
            set => FMOD_System_SetOutput(RawPtr, value);
        }
        public int NumDrivers
        {
            get { FMOD_System_GetNumDrivers(RawPtr, out int numdrivers); return numdrivers; }
        }

        public Result GetDriverInfo(int id, StringBuilder name, int namelen, out Guid guid, out int systemrate, out SpeakerMode speakermode, out int speakermodechannels)
        {
            IntPtr stringMem = Marshal.AllocHGlobal(name.Capacity);

            Result result = FMOD_System_GetDriverInfo(RawPtr, id, stringMem, namelen, out guid, out systemrate, out speakermode, out speakermodechannels);

            StringMarshalHelper.NativeToBuilder(name, stringMem);
            Marshal.FreeHGlobal(stringMem);

            return result;
        }
        public int Driver
        {
            get { FMOD_System_GetDriver(RawPtr, out int driver); return driver; }
            set => FMOD_System_SetDriver(RawPtr, value);
        }
        public int SoftwareChannels
        {
            get { FMOD_System_GetSoftwareChannels(RawPtr, out int numsoftwarechannels); return numsoftwarechannels; }
            set => FMOD_System_SetSoftwareChannels(RawPtr, value);
        }
        public Result SetSoftwareFormat(int samplerate, SpeakerMode speakermode, int numrawspeakers)
        {
            return FMOD_System_SetSoftwareFormat(RawPtr, samplerate, speakermode, numrawspeakers);
        }
        public Result GetSoftwareFormat(out int samplerate, out SpeakerMode speakermode, out int numrawspeakers)
        {
            return FMOD_System_GetSoftwareFormat(RawPtr, out samplerate, out speakermode, out numrawspeakers);
        }
        public Result SetDspBufferSize(uint bufferlength, int numbuffers)
        {
            return FMOD_System_SetDSPBufferSize(RawPtr, bufferlength, numbuffers);
        }
        public Result GetDspBufferSize(out uint bufferlength, out int numbuffers)
        {
            return FMOD_System_GetDSPBufferSize(RawPtr, out bufferlength, out numbuffers);
        }
        public Result SetFileSystem(FileOpencallback useropen, FileClosecallback userclose, FileReadcallback userread, FileSeekcallback userseek, FileAsyncreadcallback userasyncread, FileAsynccancelcallback userasynccancel, int blockalign)
        {
            return FMOD_System_SetFileSystem(RawPtr, useropen, userclose, userread, userseek, userasyncread, userasynccancel, blockalign);
        }
        public Result AttachFileSystem(FileOpencallback useropen, FileClosecallback userclose, FileReadcallback userread, FileSeekcallback userseek)
        {
            return FMOD_System_AttachFileSystem(RawPtr, useropen, userclose, userread, userseek);
        }
        public Result SetAdvancedSettings(ref AdvancedSettings settings)
        {
            settings.cbSize = Marshal.SizeOf(settings);
            return FMOD_System_SetAdvancedSettings(RawPtr, ref settings);
        }
        public Result GetAdvancedSettings(ref AdvancedSettings settings)
        {
            settings.cbSize = Marshal.SizeOf(settings);
            return FMOD_System_GetAdvancedSettings(RawPtr, ref settings);
        }
        public Result SetCallback(SystemCallback callback, SystemCallbackType callbackmask)
        {
            return FMOD_System_SetCallback(RawPtr, callback, callbackmask);
        }

        // Plug-in support.
        public Result SetPluginPath(string path)
        {
            return FMOD_System_SetPluginPath(RawPtr, Encoding.UTF8.GetBytes(path + Char.MinValue));
        }
        public Result LoadPlugin(string filename, out uint handle, uint priority)
        {
            return FMOD_System_LoadPlugin(RawPtr, Encoding.UTF8.GetBytes(filename + Char.MinValue), out handle, priority);
        }
        public Result LoadPlugin(string filename, out uint handle)
        {
            return LoadPlugin(filename, out handle, 0);
        }
        public Result UnloadPlugin(uint handle)
        {
            return FMOD_System_UnloadPlugin(RawPtr, handle);
        }
        public Result GetNumNestedPlugins(uint handle, out int count)
        {
            return FMOD_System_GetNumNestedPlugins(RawPtr, handle, out count);
        }
        public Result GetNestedPlugin(uint handle, int index, out uint nestedhandle)
        {
            return FMOD_System_GetNestedPlugin(RawPtr, handle, index, out nestedhandle);
        }
        public Result GetNumPlugins(PluginType plugintype, out int numplugins)
        {
            return FMOD_System_GetNumPlugins(RawPtr, plugintype, out numplugins);
        }
        public Result GetPluginHandle(PluginType plugintype, int index, out uint handle)
        {
            return FMOD_System_GetPluginHandle(RawPtr, plugintype, index, out handle);
        }
        public Result GetPluginInfo(uint handle, out PluginType plugintype, StringBuilder name, int namelen, out uint version)
        {
            IntPtr stringMem = Marshal.AllocHGlobal(name.Capacity);

            Result result = FMOD_System_GetPluginInfo(RawPtr, handle, out plugintype, stringMem, namelen, out version);

            StringMarshalHelper.NativeToBuilder(name, stringMem);
            Marshal.FreeHGlobal(stringMem);

            return result;
        }
        public Result SetOutputByPlugin(uint handle)
        {
            return FMOD_System_SetOutputByPlugin(RawPtr, handle);
        }
        public Result GetOutputByPlugin(out uint handle)
        {
            return FMOD_System_GetOutputByPlugin(RawPtr, out handle);
        }
        public Result CreateDspByPlugin(uint handle, out Dsp dsp)
        {
            dsp = null;

            IntPtr dspraw;
            Result result = FMOD_System_CreateDSPByPlugin(RawPtr, handle, out dspraw);
            dsp = new Dsp(dspraw);

            return result;
        }
        public Result GetDspInfoByPlugin(uint handle, out IntPtr description)
        {
            return FMOD_System_GetDSPInfoByPlugin(RawPtr, handle, out description);
        }
        /*
        public Result registerCodec(ref CODEC_DESCRIPTION description, out uint handle, uint priority)
        {
            return FMOD_System_RegisterCodec(rawPtr, ref description, out handle, priority);
        }
        */
        public Result RegisterDsp(ref DspDescription description, out uint handle)
        {
            return FMOD_System_RegisterDSP(RawPtr, ref description, out handle);
        }
        /*
        public Result registerOutput(ref OUTPUT_DESCRIPTION description, out uint handle)
        {
            return FMOD_System_RegisterOutput(rawPtr, ref description, out handle);
        }
        */

        // Init/Close.
        public Result Init(int maxchannels, InitFlags flags, IntPtr extradriverdata)
        {
            return FMOD_System_Init(RawPtr, maxchannels, flags, extradriverdata);
        }
        public Result Close()
        {
            return FMOD_System_Close(RawPtr);
        }


        // General post-init system functions.
        public Result Update()
        {
            return FMOD_System_Update(RawPtr);
        }

        public Result SetSpeakerPosition(Speaker speaker, float x, float y, bool active)
        {
            return FMOD_System_SetSpeakerPosition(RawPtr, speaker, x, y, active);
        }
        public Result GetSpeakerPosition(Speaker speaker, out float x, out float y, out bool active)
        {
            return FMOD_System_GetSpeakerPosition(RawPtr, speaker, out x, out y, out active);
        }
        public Result SetStreamBufferSize(uint filebuffersize, TimeUnit filebuffersizetype)
        {
            return FMOD_System_SetStreamBufferSize(RawPtr, filebuffersize, filebuffersizetype);
        }
        public Result GetStreamBufferSize(out uint filebuffersize, out TimeUnit filebuffersizetype)
        {
            return FMOD_System_GetStreamBufferSize(RawPtr, out filebuffersize, out filebuffersizetype);
        }
        public Result Set3DSettings(float dopplerscale, float distancefactor, float rolloffscale)
        {
            return FMOD_System_Set3DSettings(RawPtr, dopplerscale, distancefactor, rolloffscale);
        }
        public Result Get3DSettings(out float dopplerscale, out float distancefactor, out float rolloffscale)
        {
            return FMOD_System_Get3DSettings(RawPtr, out dopplerscale, out distancefactor, out rolloffscale);
        }
        public int _3DNumListeners
        {
            get
            {
                FMOD_System_Get3DNumListeners(RawPtr, out int numlisteners);
                return numlisteners;
            }
            set => FMOD_System_Set3DNumListeners(RawPtr, value);
        }       
        public Result Set3DListenerAttributes(int listener, ref Vector pos, ref Vector vel, ref Vector forward, ref Vector up)
        {
            return FMOD_System_Set3DListenerAttributes(RawPtr, listener, ref pos, ref vel, ref forward, ref up);
        }
        public Result Get3DListenerAttributes(int listener, out Vector pos, out Vector vel, out Vector forward, out Vector up)
        {
            return FMOD_System_Get3DListenerAttributes(RawPtr, listener, out pos, out vel, out forward, out up);
        }
        public Result Set3DRolloffCallback   (Cb_3DRolloffcallback callback)
        {
            return FMOD_System_Set3DRolloffCallback   (RawPtr, callback);
        }
        public Result MixerSuspend           ()
        {
            return FMOD_System_MixerSuspend(RawPtr);
        }
        public Result MixerResume            ()
        {
            return FMOD_System_MixerResume(RawPtr);
        }
        public Result GetDefaultMixMatrix    (SpeakerMode sourcespeakermode, SpeakerMode targetspeakermode, float[] matrix, int matrixhop)
        {
            return FMOD_System_GetDefaultMixMatrix(RawPtr, sourcespeakermode, targetspeakermode, matrix, matrixhop);
        }
        public Result GetSpeakerModeChannels (SpeakerMode mode, out int channels)
        {
            return FMOD_System_GetSpeakerModeChannels(RawPtr, mode, out channels);
        }


        // FMODSystem information functions.
        public uint Version
        {
            get
            {
                FMOD_System_GetVersion(RawPtr, out uint version);
                return version;
            }
        }
        public IntPtr OutputHandle
        {
            get
            {
                FMOD_System_GetOutputHandle(RawPtr, out IntPtr handle);
                return handle;
            }
        }       
        public Result GetChannelsPlaying     (out int channels, out int realchannels)
        {
            return FMOD_System_GetChannelsPlaying(RawPtr, out channels, out realchannels);
        }
        public Result GetCpuUsage            (out float dsp, out float stream, out float geometry, out float update, out float total)
        {
            return FMOD_System_GetCPUUsage(RawPtr, out dsp, out stream, out geometry, out update, out total);
        }
        public Result GetFileUsage            (out Int64 sampleBytesRead, out Int64 streamBytesRead, out Int64 otherBytesRead)
        {
            return FMOD_System_GetFileUsage(RawPtr, out sampleBytesRead, out streamBytesRead, out otherBytesRead);
        }
        public Result GetSoundRam            (out int currentalloced, out int maxalloced, out int total)
        {
            return FMOD_System_GetSoundRAM(RawPtr, out currentalloced, out maxalloced, out total);
        }

        // Sound/DSP/Channel/FX creation and retrieval.
        public Result CreateSound            (string name, Mode mode, ref CreateSoundExInfo exinfo, out Sound sound)
        {
            sound = null;

            byte[] stringData;
            stringData = Encoding.UTF8.GetBytes(name + Char.MinValue);
            
            exinfo.cbsize = Marshal.SizeOf(exinfo);

            IntPtr soundraw;
            Result result = FMOD_System_CreateSound(RawPtr, stringData, mode, ref exinfo, out soundraw);
            sound = new Sound(soundraw);

            return result;
        }
        public Result CreateSound            (byte[] data, Mode mode, ref CreateSoundExInfo exinfo, out Sound sound)
        {
            sound = null;

            exinfo.cbsize = Marshal.SizeOf(exinfo);

            IntPtr soundraw;
            Result result = FMOD_System_CreateSound(RawPtr, data, mode, ref exinfo, out soundraw);
            sound = new Sound(soundraw);

            return result;
        }
        public Result CreateSound            (string name, Mode mode, out Sound sound)
        {
            CreateSoundExInfo exinfo = new CreateSoundExInfo();
            exinfo.cbsize = Marshal.SizeOf(exinfo);

            return CreateSound(name, mode, ref exinfo, out sound);
        }
        public Result CreateStream            (string name, Mode mode, ref CreateSoundExInfo exinfo, out Sound sound)
        {
            sound = null;

            byte[] stringData;
            stringData = Encoding.UTF8.GetBytes(name + Char.MinValue);
            
            exinfo.cbsize = Marshal.SizeOf(exinfo);

            IntPtr soundraw;
            Result result = FMOD_System_CreateStream(RawPtr, stringData, mode, ref exinfo, out soundraw);
            sound = new Sound(soundraw);

            return result;
        }
        public Result CreateStream            (byte[] data, Mode mode, ref CreateSoundExInfo exinfo, out Sound sound)
        {
            sound = null;

            exinfo.cbsize = Marshal.SizeOf(exinfo);

            IntPtr soundraw;
            Result result = FMOD_System_CreateStream(RawPtr, data, mode, ref exinfo, out soundraw);
            sound = new Sound(soundraw);

            return result;
        }
        public Result CreateStream            (string name, Mode mode, out Sound sound)
        {
            CreateSoundExInfo exinfo = new CreateSoundExInfo();
            exinfo.cbsize = Marshal.SizeOf(exinfo);

            return CreateStream(name, mode, ref exinfo, out sound);
        }
        public Result CreateDsp              (ref DspDescription description, out Dsp dsp)
        {
            dsp = null;

            IntPtr dspraw;
            Result result = FMOD_System_CreateDSP(RawPtr, ref description, out dspraw);
            dsp = new Dsp(dspraw);

            return result;
        }
        public Result CreateDspByType          (DspType type, out Dsp dsp)
        {
            dsp = null;

            IntPtr dspraw;
            Result result = FMOD_System_CreateDSPByType(RawPtr, type, out dspraw);
            dsp = new Dsp(dspraw);

            return result;
        }
        public Result CreateChannelGroup     (string name, out ChannelGroup channelgroup)
        {
            channelgroup = null;

            byte[] stringData = Encoding.UTF8.GetBytes(name + Char.MinValue);

            IntPtr channelgroupraw;
            Result result = FMOD_System_CreateChannelGroup(RawPtr, stringData, out channelgroupraw);
            channelgroup = new ChannelGroup(channelgroupraw);

            return result;
        }
        public Result CreateSoundGroup       (string name, out SoundGroup soundgroup)
        {
            soundgroup = null;

            byte[] stringData = Encoding.UTF8.GetBytes(name + Char.MinValue);

            IntPtr soundgroupraw;
            Result result = FMOD_System_CreateSoundGroup(RawPtr, stringData, out soundgroupraw);
            soundgroup = new SoundGroup(soundgroupraw);

            return result;
        }
        public Result CreateReverb3D         (out Reverb3D reverb)
        {
            IntPtr reverbraw;
            Result result = FMOD_System_CreateReverb3D(RawPtr, out reverbraw);
            reverb = new Reverb3D(reverbraw);

            return result;
        }
        public Result PlaySound              (Sound sound, ChannelGroup channelGroup, bool paused, out Channel channel)
        {
            channel = null;

            IntPtr channelGroupRaw = (channelGroup != null) ? channelGroup.GetRaw() : IntPtr.Zero;

            IntPtr channelraw;
            Result result = FMOD_System_PlaySound(RawPtr, sound.GetRaw(), channelGroupRaw, paused, out channelraw);
            channel = new Channel(channelraw);

            return result;
        }
        public Result PlayDsp                (Dsp dsp, ChannelGroup channelGroup, bool paused, out Channel channel)
        {
            channel = null;

            IntPtr channelGroupRaw = (channelGroup != null) ? channelGroup.GetRaw() : IntPtr.Zero;

            IntPtr channelraw;
            Result result = FMOD_System_PlayDSP(RawPtr, dsp.GetRaw(), channelGroupRaw, paused, out channelraw);
            channel = new Channel(channelraw);

            return result;
        }
        public Result GetChannel             (int channelid, out Channel channel)
        {
            channel = null;

            IntPtr channelraw;
            Result result = FMOD_System_GetChannel(RawPtr, channelid, out channelraw);
            channel = new Channel(channelraw);

            return result;
        }
        public ChannelGroup MasterChannelGroup
        {
            get
            {
                IntPtr channelgroupraw;
                Result result = FMOD_System_GetMasterChannelGroup(RawPtr, out channelgroupraw);
                return new ChannelGroup(channelgroupraw);
            }
        }
        public SoundGroup MasterSoundGroup
        {
            get
            {
                IntPtr soundgroupraw;
                Result result = FMOD_System_GetMasterSoundGroup(RawPtr, out soundgroupraw);
                return new SoundGroup(soundgroupraw);
            }
        }        

        // Routing to ports.
        public Result AttachChannelGroupToPort(uint portType, ulong portIndex, ChannelGroup channelgroup, bool passThru = false)
        {
            return FMOD_System_AttachChannelGroupToPort(RawPtr, portType, portIndex, channelgroup.GetRaw(), passThru);
        }
        public Result DetachChannelGroupFromPort(ChannelGroup channelgroup)
        {
            return FMOD_System_DetachChannelGroupFromPort(RawPtr, channelgroup.GetRaw());
        }

        // Reverb api.
        public Result SetReverbProperties    (int instance, ref ReverbProperties prop)
        {
            return FMOD_System_SetReverbProperties(RawPtr, instance, ref prop);
        }
        public Result GetReverbProperties    (int instance, out ReverbProperties prop)
        {
            return FMOD_System_GetReverbProperties(RawPtr, instance, out prop);
        }

        // FMODSystem level DSP functionality.
        public Result LockDsp            ()
        {
            return FMOD_System_LockDSP(RawPtr);
        }
        public Result UnlockDsp          ()
        {
            return FMOD_System_UnlockDSP(RawPtr);
        }

        // Recording api
        public Result GetRecordNumDrivers    (out int numdrivers, out int numconnected)
        {
            return FMOD_System_GetRecordNumDrivers(RawPtr, out numdrivers, out numconnected);
        }
        public Result GetRecordDriverInfo(int id, StringBuilder name, int namelen, out Guid guid, out int systemrate, out SpeakerMode speakermode, out int speakermodechannels, out DriverState state)
        {
            IntPtr stringMem = Marshal.AllocHGlobal(name.Capacity);

            Result result = FMOD_System_GetRecordDriverInfo(RawPtr, id, stringMem, namelen, out guid, out systemrate, out speakermode, out speakermodechannels, out state);

            StringMarshalHelper.NativeToBuilder(name, stringMem);
            Marshal.FreeHGlobal(stringMem);

            return result;
        }
        public Result GetRecordPosition      (int id, out uint position)
        {
            return FMOD_System_GetRecordPosition(RawPtr, id, out position);
        }
        public Result RecordStart            (int id, Sound sound, bool loop)
        {
            return FMOD_System_RecordStart(RawPtr, id, sound.GetRaw(), loop);
        }
        public Result RecordStop             (int id)
        {
            return FMOD_System_RecordStop(RawPtr, id);
        }
        public Result IsRecording            (int id, out bool recording)
        {
            return FMOD_System_IsRecording(RawPtr, id, out recording);
        }

        // Geometry api
        public Result CreateGeometry         (int maxpolygons, int maxvertices, out Geometry geometry)
        {
            geometry = null;

            IntPtr geometryraw;
            Result result = FMOD_System_CreateGeometry(RawPtr, maxpolygons, maxvertices, out geometryraw);
            geometry = new Geometry(geometryraw);

            return result;
        }
        public Result SetGeometrySettings    (float maxworldsize)
        {
            return FMOD_System_SetGeometrySettings(RawPtr, maxworldsize);
        }
        public Result GetGeometrySettings    (out float maxworldsize)
        {
            return FMOD_System_GetGeometrySettings(RawPtr, out maxworldsize);
        }
        public Result LoadGeometry(IntPtr data, int datasize, out Geometry geometry)
        {
            geometry = null;

            IntPtr geometryraw;
            Result result = FMOD_System_LoadGeometry(RawPtr, data, datasize, out geometryraw);
            geometry = new Geometry(geometryraw);

            return result;
        }
        public Result GetGeometryOcclusion    (ref Vector listener, ref Vector source, out float direct, out float reverb)
        {
            return FMOD_System_GetGeometryOcclusion(RawPtr, ref listener, ref source, out direct, out reverb);
        }

        // Network functions
        public string NetworkProxy
        {         
            set => FMOD_System_SetNetworkProxy(RawPtr, Encoding.UTF8.GetBytes(value + Char.MinValue));
        }
        public Result GetNetworkProxy               (StringBuilder proxy, int proxylen)
        {
            IntPtr stringMem = Marshal.AllocHGlobal(proxy.Capacity);

            Result result = FMOD_System_GetNetworkProxy(RawPtr, stringMem, proxylen);

            StringMarshalHelper.NativeToBuilder(proxy, stringMem);
            Marshal.FreeHGlobal(stringMem);

            return result;
        }
        public int NetworkTimeout
        {
            get
            {
                FMOD_System_GetNetworkTimeout(RawPtr, out int timeout);
                return timeout;
            }
            set => FMOD_System_SetNetworkTimeout(RawPtr, value);
        }

        // Userdata set/get
        public IntPtr UserData
        {
            get
            {
                FMOD_System_GetUserData(RawPtr, out IntPtr userdata);
                return userdata;
            }
            set => FMOD_System_SetUserData(RawPtr, value);
        } 

        #region importfunctions
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_Release                (IntPtr system);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_SetOutput              (IntPtr system, OutputType output);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetOutput              (IntPtr system, out OutputType output);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetNumDrivers          (IntPtr system, out int numdrivers);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetDriverInfo          (IntPtr system, int id, IntPtr name, int namelen, out Guid guid, out int systemrate, out SpeakerMode speakermode, out int speakermodechannels);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_SetDriver              (IntPtr system, int driver);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetDriver              (IntPtr system, out int driver);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_SetSoftwareChannels    (IntPtr system, int numsoftwarechannels);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetSoftwareChannels    (IntPtr system, out int numsoftwarechannels);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_SetSoftwareFormat      (IntPtr system, int samplerate, SpeakerMode speakermode, int numrawspeakers);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetSoftwareFormat      (IntPtr system, out int samplerate, out SpeakerMode speakermode, out int numrawspeakers);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_SetDSPBufferSize       (IntPtr system, uint bufferlength, int numbuffers);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetDSPBufferSize       (IntPtr system, out uint bufferlength, out int numbuffers);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_SetFileSystem          (IntPtr system, FileOpencallback useropen, FileClosecallback userclose, FileReadcallback userread, FileSeekcallback userseek, FileAsyncreadcallback userasyncread, FileAsynccancelcallback userasynccancel, int blockalign);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_AttachFileSystem       (IntPtr system, FileOpencallback useropen, FileClosecallback userclose, FileReadcallback userread, FileSeekcallback userseek);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_SetPluginPath          (IntPtr system, byte[] path);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_LoadPlugin             (IntPtr system, byte[] filename, out uint handle, uint priority);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_UnloadPlugin           (IntPtr system, uint handle);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetNumNestedPlugins    (IntPtr system, uint handle, out int count);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetNestedPlugin        (IntPtr system, uint handle, int index, out uint nestedhandle);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetNumPlugins          (IntPtr system, PluginType plugintype, out int numplugins);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetPluginHandle        (IntPtr system, PluginType plugintype, int index, out uint handle);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetPluginInfo          (IntPtr system, uint handle, out PluginType plugintype, IntPtr name, int namelen, out uint version);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_CreateDSPByPlugin      (IntPtr system, uint handle, out IntPtr dsp);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_SetOutputByPlugin      (IntPtr system, uint handle);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetOutputByPlugin      (IntPtr system, out uint handle);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetDSPInfoByPlugin     (IntPtr system, uint handle, out IntPtr description);
        [DllImport(FmodVersion.Dll)]
        //private static extern Result FMOD_System_RegisterCodec          (IntPtr system, out CODEC_DESCRIPTION description, out uint handle, uint priority);
        //[DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_RegisterDSP            (IntPtr system, ref DspDescription description, out uint handle);
        [DllImport(FmodVersion.Dll)]
        //private static extern Result FMOD_System_RegisterOutput         (IntPtr system, ref OUTPUT_DESCRIPTION description, out uint handle);
        //[DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_Init                   (IntPtr system, int maxchannels, InitFlags flags, IntPtr extradriverdata);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_Close                  (IntPtr system);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_Update                 (IntPtr system);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_SetAdvancedSettings    (IntPtr system, ref AdvancedSettings settings);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetAdvancedSettings    (IntPtr system, ref AdvancedSettings settings);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_Set3DRolloffCallback   (IntPtr system, Cb_3DRolloffcallback callback);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_MixerSuspend           (IntPtr system);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_MixerResume            (IntPtr system);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetDefaultMixMatrix    (IntPtr system, SpeakerMode sourcespeakermode, SpeakerMode targetspeakermode, float[] matrix, int matrixhop);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetSpeakerModeChannels (IntPtr system, SpeakerMode mode, out int channels);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_SetCallback            (IntPtr system, SystemCallback callback, SystemCallbackType callbackmask);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_SetSpeakerPosition     (IntPtr system, Speaker speaker, float x, float y, bool active);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetSpeakerPosition     (IntPtr system, Speaker speaker, out float x, out float y, out bool active);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_Set3DSettings          (IntPtr system, float dopplerscale, float distancefactor, float rolloffscale);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_Get3DSettings          (IntPtr system, out float dopplerscale, out float distancefactor, out float rolloffscale);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_Set3DNumListeners      (IntPtr system, int numlisteners);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_Get3DNumListeners      (IntPtr system, out int numlisteners);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_Set3DListenerAttributes(IntPtr system, int listener, ref Vector pos, ref Vector vel, ref Vector forward, ref Vector up);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_Get3DListenerAttributes(IntPtr system, int listener, out Vector pos, out Vector vel, out Vector forward, out Vector up);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_SetStreamBufferSize    (IntPtr system, uint filebuffersize, TimeUnit filebuffersizetype);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetStreamBufferSize    (IntPtr system, out uint filebuffersize, out TimeUnit filebuffersizetype);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetVersion             (IntPtr system, out uint version);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetOutputHandle        (IntPtr system, out IntPtr handle);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetChannelsPlaying     (IntPtr system, out int channels, out int realchannels);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetCPUUsage            (IntPtr system, out float dsp, out float stream, out float geometry, out float update, out float total);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetFileUsage            (IntPtr system, out Int64 sampleBytesRead, out Int64 streamBytesRead, out Int64 otherBytesRead);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetSoundRAM            (IntPtr system, out int currentalloced, out int maxalloced, out int total);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_CreateSound            (IntPtr system, byte[] nameOrData, Mode mode, ref CreateSoundExInfo exinfo, out IntPtr sound);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_CreateStream           (IntPtr system, byte[] nameOrData, Mode mode, ref CreateSoundExInfo exinfo, out IntPtr sound);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_CreateDSP              (IntPtr system, ref DspDescription description, out IntPtr dsp);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_CreateDSPByType        (IntPtr system, DspType type, out IntPtr dsp);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_CreateChannelGroup     (IntPtr system, byte[] name, out IntPtr channelgroup);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_CreateSoundGroup       (IntPtr system, byte[] name, out IntPtr soundgroup);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_CreateReverb3D         (IntPtr system, out IntPtr reverb);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_PlaySound              (IntPtr system, IntPtr sound, IntPtr channelGroup, bool paused, out IntPtr channel);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_PlayDSP                (IntPtr system, IntPtr dsp, IntPtr channelGroup, bool paused, out IntPtr channel);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetChannel             (IntPtr system, int channelid, out IntPtr channel);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetMasterChannelGroup  (IntPtr system, out IntPtr channelgroup);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetMasterSoundGroup    (IntPtr system, out IntPtr soundgroup);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_AttachChannelGroupToPort  (IntPtr system, uint portType, ulong portIndex, IntPtr channelgroup, bool passThru);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_DetachChannelGroupFromPort(IntPtr system, IntPtr channelgroup);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_SetReverbProperties    (IntPtr system, int instance, ref ReverbProperties prop);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetReverbProperties    (IntPtr system, int instance, out ReverbProperties prop);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_LockDSP                (IntPtr system);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_UnlockDSP              (IntPtr system);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetRecordNumDrivers    (IntPtr system, out int numdrivers, out int numconnected);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetRecordDriverInfo    (IntPtr system, int id, IntPtr name, int namelen, out Guid guid, out int systemrate, out SpeakerMode speakermode, out int speakermodechannels, out DriverState state);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetRecordPosition      (IntPtr system, int id, out uint position);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_RecordStart            (IntPtr system, int id, IntPtr sound, bool loop);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_RecordStop             (IntPtr system, int id);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_IsRecording            (IntPtr system, int id, out bool recording);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_CreateGeometry         (IntPtr system, int maxpolygons, int maxvertices, out IntPtr geometry);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_SetGeometrySettings    (IntPtr system, float maxworldsize);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetGeometrySettings    (IntPtr system, out float maxworldsize);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_LoadGeometry           (IntPtr system, IntPtr data, int datasize, out IntPtr geometry);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetGeometryOcclusion   (IntPtr system, ref Vector listener, ref Vector source, out float direct, out float reverb);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_SetNetworkProxy        (IntPtr system, byte[] proxy);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetNetworkProxy        (IntPtr system, IntPtr proxy, int proxylen);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_SetNetworkTimeout      (IntPtr system, int timeout);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetNetworkTimeout      (IntPtr system, out int timeout);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_SetUserData            (IntPtr system, IntPtr userdata);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_System_GetUserData            (IntPtr system, out IntPtr userdata);
        #endregion

        #region wrapperinternal

        public FmodSystem(IntPtr raw)
            : base(raw)
        {
        }

        #endregion
    }
}
