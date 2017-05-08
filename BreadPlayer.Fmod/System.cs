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
            'System' API.
        */
    public class System : HandleBase
    {
        public Result Release                ()
        {
            Result result = FMOD_System_Release(rawPtr);
            if (result == Result.OK)
            {
                rawPtr = IntPtr.Zero;
            }
            return result;
        }


        // Pre-init functions.
        public Result SetOutput              (OutputType output)
        {
            return FMOD_System_SetOutput(rawPtr, output);
        }
        public Result GetOutput              (out OutputType output)
        {
            return FMOD_System_GetOutput(rawPtr, out output);
        }
        public Result GetNumDrivers          (out int numdrivers)
        {
            return FMOD_System_GetNumDrivers(rawPtr, out numdrivers);
        }
        public Result GetDriverInfo          (int id, StringBuilder name, int namelen, out Guid guid, out int systemrate, out SpeakerMode speakermode, out int speakermodechannels)
        {
            IntPtr stringMem = Marshal.AllocHGlobal(name.Capacity);

            Result result = FMOD_System_GetDriverInfo(rawPtr, id, stringMem, namelen, out guid, out systemrate, out speakermode, out speakermodechannels);

            StringMarshalHelper.NativeToBuilder(name, stringMem);
            Marshal.FreeHGlobal(stringMem);

            return result;
        }
        public Result SetDriver              (int driver)
        {
            return FMOD_System_SetDriver(rawPtr, driver);
        }
        public Result GetDriver              (out int driver)
        {
            return FMOD_System_GetDriver(rawPtr, out driver);
        }
        public Result SetSoftwareChannels    (int numsoftwarechannels)
        {
            return FMOD_System_SetSoftwareChannels(rawPtr, numsoftwarechannels);
        }
        public Result GetSoftwareChannels    (out int numsoftwarechannels)
        {
            return FMOD_System_GetSoftwareChannels(rawPtr, out numsoftwarechannels);
        }
        public Result SetSoftwareFormat      (int samplerate, SpeakerMode speakermode, int numrawspeakers)
        {
            return FMOD_System_SetSoftwareFormat(rawPtr, samplerate, speakermode, numrawspeakers);
        }
        public Result GetSoftwareFormat      (out int samplerate, out SpeakerMode speakermode, out int numrawspeakers)
        {
            return FMOD_System_GetSoftwareFormat(rawPtr, out samplerate, out speakermode, out numrawspeakers);
        }
        public Result SetDSPBufferSize       (uint bufferlength, int numbuffers)
        {
            return FMOD_System_SetDSPBufferSize(rawPtr, bufferlength, numbuffers);
        }
        public Result GetDSPBufferSize       (out uint bufferlength, out int numbuffers)
        {
            return FMOD_System_GetDSPBufferSize(rawPtr, out bufferlength, out numbuffers);
        }
        public Result SetFileSystem          (FILE_OPENCALLBACK useropen, FILE_CLOSECALLBACK userclose, FILE_READCALLBACK userread, FILE_SEEKCALLBACK userseek, FILE_ASYNCREADCALLBACK userasyncread, FILE_ASYNCCANCELCALLBACK userasynccancel, int blockalign)
        {
            return FMOD_System_SetFileSystem(rawPtr, useropen, userclose, userread, userseek, userasyncread, userasynccancel, blockalign);
        }
        public Result AttachFileSystem       (FILE_OPENCALLBACK useropen, FILE_CLOSECALLBACK userclose, FILE_READCALLBACK userread, FILE_SEEKCALLBACK userseek)
        {
            return FMOD_System_AttachFileSystem(rawPtr, useropen, userclose, userread, userseek);
        }
        public Result SetAdvancedSettings    (ref AdvancedSettings settings)
        {
            settings.cbSize = Marshal.SizeOf(settings);
            return FMOD_System_SetAdvancedSettings(rawPtr, ref settings);
        }
        public Result GetAdvancedSettings    (ref AdvancedSettings settings)
        {
            settings.cbSize = Marshal.SizeOf(settings);
            return FMOD_System_GetAdvancedSettings(rawPtr, ref settings);
        }
        public Result SetCallback            (SYSTEM_CALLBACK callback, SystemCallbackType callbackmask)
        {
            return FMOD_System_SetCallback(rawPtr, callback, callbackmask);
        }

        // Plug-in support.
        public Result SetPluginPath          (string path)
        {
            return FMOD_System_SetPluginPath(rawPtr, Encoding.UTF8.GetBytes(path + Char.MinValue));
        }
        public Result LoadPlugin             (string filename, out uint handle, uint priority)
        {
            return FMOD_System_LoadPlugin(rawPtr, Encoding.UTF8.GetBytes(filename + Char.MinValue), out handle, priority);
        }
        public Result LoadPlugin             (string filename, out uint handle)
        {
            return LoadPlugin(filename, out handle, 0);
        }
        public Result UnloadPlugin           (uint handle)
        {
            return FMOD_System_UnloadPlugin(rawPtr, handle);
        }
        public Result GetNumNestedPlugins    (uint handle, out int count)
        {
            return FMOD_System_GetNumNestedPlugins(rawPtr, handle, out count);
        }
        public Result GetNestedPlugin        (uint handle, int index, out uint nestedhandle)
        {
            return FMOD_System_GetNestedPlugin(rawPtr, handle, index, out nestedhandle);
        }
        public Result GetNumPlugins          (PluginType plugintype, out int numplugins)
        {
            return FMOD_System_GetNumPlugins(rawPtr, plugintype, out numplugins);
        }
        public Result GetPluginHandle        (PluginType plugintype, int index, out uint handle)
        {
            return FMOD_System_GetPluginHandle(rawPtr, plugintype, index, out handle);
        }
        public Result GetPluginInfo          (uint handle, out PluginType plugintype, StringBuilder name, int namelen, out uint version)
        {
            IntPtr stringMem = Marshal.AllocHGlobal(name.Capacity);

            Result result = FMOD_System_GetPluginInfo(rawPtr, handle, out plugintype, stringMem, namelen, out version);

            StringMarshalHelper.NativeToBuilder(name, stringMem);
            Marshal.FreeHGlobal(stringMem);

            return result;
        }
        public Result SetOutputByPlugin      (uint handle)
        {
            return FMOD_System_SetOutputByPlugin(rawPtr, handle);
        }
        public Result GetOutputByPlugin      (out uint handle)
        {
            return FMOD_System_GetOutputByPlugin(rawPtr, out handle);
        }
        public Result CreateDSPByPlugin(uint handle, out DSP dsp)
        {
            dsp = null;

            IntPtr dspraw;
            Result result = FMOD_System_CreateDSPByPlugin(rawPtr, handle, out dspraw);
            dsp = new DSP(dspraw);

            return result;
        }
        public Result GetDSPInfoByPlugin(uint handle, out IntPtr description)
        {
            return FMOD_System_GetDSPInfoByPlugin(rawPtr, handle, out description);
        }
        /*
        public Result registerCodec(ref CODEC_DESCRIPTION description, out uint handle, uint priority)
        {
            return FMOD_System_RegisterCodec(rawPtr, ref description, out handle, priority);
        }
        */
        public Result RegisterDSP(ref DspDescription description, out uint handle)
        {
            return FMOD_System_RegisterDSP(rawPtr, ref description, out handle);
        }
        /*
        public Result registerOutput(ref OUTPUT_DESCRIPTION description, out uint handle)
        {
            return FMOD_System_RegisterOutput(rawPtr, ref description, out handle);
        }
        */

        // Init/Close.
        public Result Init                   (int maxchannels, InitFlags flags, IntPtr extradriverdata)
        {
            return FMOD_System_Init(rawPtr, maxchannels, flags, extradriverdata);
        }
        public Result Close                  ()
        {
            return FMOD_System_Close(rawPtr);
        }


        // General post-init system functions.
        public Result Update                 ()
        {
            return FMOD_System_Update(rawPtr);
        }

        public Result SetSpeakerPosition(Speaker speaker, float x, float y, bool active)
        {
            return FMOD_System_SetSpeakerPosition(rawPtr, speaker, x, y, active);
        }
        public Result GetSpeakerPosition(Speaker speaker, out float x, out float y, out bool active)
        {
            return FMOD_System_GetSpeakerPosition(rawPtr, speaker, out x, out y, out active);
        }
        public Result SetStreamBufferSize(uint filebuffersize, TimeUnit filebuffersizetype)
        {
            return FMOD_System_SetStreamBufferSize(rawPtr, filebuffersize, filebuffersizetype);
        }
        public Result GetStreamBufferSize(out uint filebuffersize, out TimeUnit filebuffersizetype)
        {
            return FMOD_System_GetStreamBufferSize(rawPtr, out filebuffersize, out filebuffersizetype);
        }
        public Result Set3DSettings          (float dopplerscale, float distancefactor, float rolloffscale)
        {
            return FMOD_System_Set3DSettings(rawPtr, dopplerscale, distancefactor, rolloffscale);
        }
        public Result Get3DSettings          (out float dopplerscale, out float distancefactor, out float rolloffscale)
        {
            return FMOD_System_Get3DSettings(rawPtr, out dopplerscale, out distancefactor, out rolloffscale);
        }
        public Result Set3DNumListeners      (int numlisteners)
        {
            return FMOD_System_Set3DNumListeners(rawPtr, numlisteners);
        }
        public Result Get3DNumListeners      (out int numlisteners)
        {
            return FMOD_System_Get3DNumListeners(rawPtr, out numlisteners);
        }
        public Result Set3DListenerAttributes(int listener, ref Vector pos, ref Vector vel, ref Vector forward, ref Vector up)
        {
            return FMOD_System_Set3DListenerAttributes(rawPtr, listener, ref pos, ref vel, ref forward, ref up);
        }
        public Result Get3DListenerAttributes(int listener, out Vector pos, out Vector vel, out Vector forward, out Vector up)
        {
            return FMOD_System_Get3DListenerAttributes(rawPtr, listener, out pos, out vel, out forward, out up);
        }
        public Result Set3DRolloffCallback   (CB_3D_ROLLOFFCALLBACK callback)
        {
            return FMOD_System_Set3DRolloffCallback   (rawPtr, callback);
        }
        public Result MixerSuspend           ()
        {
            return FMOD_System_MixerSuspend(rawPtr);
        }
        public Result MixerResume            ()
        {
            return FMOD_System_MixerResume(rawPtr);
        }
        public Result GetDefaultMixMatrix    (SpeakerMode sourcespeakermode, SpeakerMode targetspeakermode, float[] matrix, int matrixhop)
        {
            return FMOD_System_GetDefaultMixMatrix(rawPtr, sourcespeakermode, targetspeakermode, matrix, matrixhop);
        }
        public Result GetSpeakerModeChannels (SpeakerMode mode, out int channels)
        {
            return FMOD_System_GetSpeakerModeChannels(rawPtr, mode, out channels);
        }

        // System information functions.
        public Result GetVersion             (out uint version)
        {
            return FMOD_System_GetVersion(rawPtr, out version);
        }
        public Result GetOutputHandle        (out IntPtr handle)
        {
            return FMOD_System_GetOutputHandle(rawPtr, out handle);
        }
        public Result GetChannelsPlaying     (out int channels, out int realchannels)
        {
            return FMOD_System_GetChannelsPlaying(rawPtr, out channels, out realchannels);
        }
        public Result GetCPUUsage            (out float dsp, out float stream, out float geometry, out float update, out float total)
        {
            return FMOD_System_GetCPUUsage(rawPtr, out dsp, out stream, out geometry, out update, out total);
        }
        public Result GetFileUsage            (out Int64 sampleBytesRead, out Int64 streamBytesRead, out Int64 otherBytesRead)
        {
            return FMOD_System_GetFileUsage(rawPtr, out sampleBytesRead, out streamBytesRead, out otherBytesRead);
        }
        public Result GetSoundRAM            (out int currentalloced, out int maxalloced, out int total)
        {
            return FMOD_System_GetSoundRAM(rawPtr, out currentalloced, out maxalloced, out total);
        }

        // Sound/DSP/Channel/FX creation and retrieval.
        public Result CreateSound            (string name, Mode mode, ref CreateSoundExInfo exinfo, out Sound sound)
        {
            sound = null;

            byte[] stringData;
            stringData = Encoding.UTF8.GetBytes(name + Char.MinValue);
            
            exinfo.cbsize = Marshal.SizeOf(exinfo);

            IntPtr soundraw;
            Result result = FMOD_System_CreateSound(rawPtr, stringData, mode, ref exinfo, out soundraw);
            sound = new Sound(soundraw);

            return result;
        }
        public Result CreateSound            (byte[] data, Mode mode, ref CreateSoundExInfo exinfo, out Sound sound)
        {
            sound = null;

            exinfo.cbsize = Marshal.SizeOf(exinfo);

            IntPtr soundraw;
            Result result = FMOD_System_CreateSound(rawPtr, data, mode, ref exinfo, out soundraw);
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
            Result result = FMOD_System_CreateStream(rawPtr, stringData, mode, ref exinfo, out soundraw);
            sound = new Sound(soundraw);

            return result;
        }
        public Result CreateStream            (byte[] data, Mode mode, ref CreateSoundExInfo exinfo, out Sound sound)
        {
            sound = null;

            exinfo.cbsize = Marshal.SizeOf(exinfo);

            IntPtr soundraw;
            Result result = FMOD_System_CreateStream(rawPtr, data, mode, ref exinfo, out soundraw);
            sound = new Sound(soundraw);

            return result;
        }
        public Result CreateStream            (string name, Mode mode, out Sound sound)
        {
            CreateSoundExInfo exinfo = new CreateSoundExInfo();
            exinfo.cbsize = Marshal.SizeOf(exinfo);

            return CreateStream(name, mode, ref exinfo, out sound);
        }
        public Result CreateDSP              (ref DspDescription description, out DSP dsp)
        {
            dsp = null;

            IntPtr dspraw;
            Result result = FMOD_System_CreateDSP(rawPtr, ref description, out dspraw);
            dsp = new DSP(dspraw);

            return result;
        }
        public Result CreateDSPByType          (DspType type, out DSP dsp)
        {
            dsp = null;

            IntPtr dspraw;
            Result result = FMOD_System_CreateDSPByType(rawPtr, type, out dspraw);
            dsp = new DSP(dspraw);

            return result;
        }
        public Result CreateChannelGroup     (string name, out ChannelGroup channelgroup)
        {
            channelgroup = null;

            byte[] stringData = Encoding.UTF8.GetBytes(name + Char.MinValue);

            IntPtr channelgroupraw;
            Result result = FMOD_System_CreateChannelGroup(rawPtr, stringData, out channelgroupraw);
            channelgroup = new ChannelGroup(channelgroupraw);

            return result;
        }
        public Result CreateSoundGroup       (string name, out SoundGroup soundgroup)
        {
            soundgroup = null;

            byte[] stringData = Encoding.UTF8.GetBytes(name + Char.MinValue);

            IntPtr soundgroupraw;
            Result result = FMOD_System_CreateSoundGroup(rawPtr, stringData, out soundgroupraw);
            soundgroup = new SoundGroup(soundgroupraw);

            return result;
        }
        public Result CreateReverb3D         (out Reverb3D reverb)
        {
            IntPtr reverbraw;
            Result result = FMOD_System_CreateReverb3D(rawPtr, out reverbraw);
            reverb = new Reverb3D(reverbraw);

            return result;
        }
        public Result PlaySound              (Sound sound, ChannelGroup channelGroup, bool paused, out Channel channel)
        {
            channel = null;

            IntPtr channelGroupRaw = (channelGroup != null) ? channelGroup.getRaw() : IntPtr.Zero;

            IntPtr channelraw;
            Result result = FMOD_System_PlaySound(rawPtr, sound.getRaw(), channelGroupRaw, paused, out channelraw);
            channel = new Channel(channelraw);

            return result;
        }
        public Result PlayDSP                (DSP dsp, ChannelGroup channelGroup, bool paused, out Channel channel)
        {
            channel = null;

            IntPtr channelGroupRaw = (channelGroup != null) ? channelGroup.getRaw() : IntPtr.Zero;

            IntPtr channelraw;
            Result result = FMOD_System_PlayDSP(rawPtr, dsp.getRaw(), channelGroupRaw, paused, out channelraw);
            channel = new Channel(channelraw);

            return result;
        }
        public Result GetChannel             (int channelid, out Channel channel)
        {
            channel = null;

            IntPtr channelraw;
            Result result = FMOD_System_GetChannel(rawPtr, channelid, out channelraw);
            channel = new Channel(channelraw);

            return result;
        }
        public Result GetMasterChannelGroup  (out ChannelGroup channelgroup)
        {
            channelgroup = null;

            IntPtr channelgroupraw;
            Result result = FMOD_System_GetMasterChannelGroup(rawPtr, out channelgroupraw);
            channelgroup = new ChannelGroup(channelgroupraw);

            return result;
        }
        public Result GetMasterSoundGroup    (out SoundGroup soundgroup)
        {
            soundgroup = null;

            IntPtr soundgroupraw;
            Result result = FMOD_System_GetMasterSoundGroup(rawPtr, out soundgroupraw);
            soundgroup = new SoundGroup(soundgroupraw);

            return result;
        }

        // Routing to ports.
        public Result AttachChannelGroupToPort(uint portType, ulong portIndex, ChannelGroup channelgroup, bool passThru = false)
        {
            return FMOD_System_AttachChannelGroupToPort(rawPtr, portType, portIndex, channelgroup.getRaw(), passThru);
        }
        public Result DetachChannelGroupFromPort(ChannelGroup channelgroup)
        {
            return FMOD_System_DetachChannelGroupFromPort(rawPtr, channelgroup.getRaw());
        }

        // Reverb api.
        public Result SetReverbProperties    (int instance, ref ReverbProperties prop)
        {
            return FMOD_System_SetReverbProperties(rawPtr, instance, ref prop);
        }
        public Result GetReverbProperties    (int instance, out ReverbProperties prop)
        {
            return FMOD_System_GetReverbProperties(rawPtr, instance, out prop);
        }

        // System level DSP functionality.
        public Result LockDSP            ()
        {
            return FMOD_System_LockDSP(rawPtr);
        }
        public Result UnlockDSP          ()
        {
            return FMOD_System_UnlockDSP(rawPtr);
        }

        // Recording api
        public Result GetRecordNumDrivers    (out int numdrivers, out int numconnected)
        {
            return FMOD_System_GetRecordNumDrivers(rawPtr, out numdrivers, out numconnected);
        }
        public Result GetRecordDriverInfo(int id, StringBuilder name, int namelen, out Guid guid, out int systemrate, out SpeakerMode speakermode, out int speakermodechannels, out DriverState state)
        {
            IntPtr stringMem = Marshal.AllocHGlobal(name.Capacity);

            Result result = FMOD_System_GetRecordDriverInfo(rawPtr, id, stringMem, namelen, out guid, out systemrate, out speakermode, out speakermodechannels, out state);

            StringMarshalHelper.NativeToBuilder(name, stringMem);
            Marshal.FreeHGlobal(stringMem);

            return result;
        }
        public Result GetRecordPosition      (int id, out uint position)
        {
            return FMOD_System_GetRecordPosition(rawPtr, id, out position);
        }
        public Result RecordStart            (int id, Sound sound, bool loop)
        {
            return FMOD_System_RecordStart(rawPtr, id, sound.getRaw(), loop);
        }
        public Result RecordStop             (int id)
        {
            return FMOD_System_RecordStop(rawPtr, id);
        }
        public Result IsRecording            (int id, out bool recording)
        {
            return FMOD_System_IsRecording(rawPtr, id, out recording);
        }

        // Geometry api
        public Result CreateGeometry         (int maxpolygons, int maxvertices, out Geometry geometry)
        {
            geometry = null;

            IntPtr geometryraw;
            Result result = FMOD_System_CreateGeometry(rawPtr, maxpolygons, maxvertices, out geometryraw);
            geometry = new Geometry(geometryraw);

            return result;
        }
        public Result SetGeometrySettings    (float maxworldsize)
        {
            return FMOD_System_SetGeometrySettings(rawPtr, maxworldsize);
        }
        public Result GetGeometrySettings    (out float maxworldsize)
        {
            return FMOD_System_GetGeometrySettings(rawPtr, out maxworldsize);
        }
        public Result LoadGeometry(IntPtr data, int datasize, out Geometry geometry)
        {
            geometry = null;

            IntPtr geometryraw;
            Result result = FMOD_System_LoadGeometry(rawPtr, data, datasize, out geometryraw);
            geometry = new Geometry(geometryraw);

            return result;
        }
        public Result GetGeometryOcclusion    (ref Vector listener, ref Vector source, out float direct, out float reverb)
        {
            return FMOD_System_GetGeometryOcclusion(rawPtr, ref listener, ref source, out direct, out reverb);
        }

        // Network functions
        public Result SetNetworkProxy               (string proxy)
        {
            return FMOD_System_SetNetworkProxy(rawPtr, Encoding.UTF8.GetBytes(proxy + Char.MinValue));
        }
        public Result GetNetworkProxy               (StringBuilder proxy, int proxylen)
        {
            IntPtr stringMem = Marshal.AllocHGlobal(proxy.Capacity);

            Result result = FMOD_System_GetNetworkProxy(rawPtr, stringMem, proxylen);

            StringMarshalHelper.NativeToBuilder(proxy, stringMem);
            Marshal.FreeHGlobal(stringMem);

            return result;
        }
        public Result SetNetworkTimeout      (int timeout)
        {
            return FMOD_System_SetNetworkTimeout(rawPtr, timeout);
        }
        public Result GetNetworkTimeout(out int timeout)
        {
            return FMOD_System_GetNetworkTimeout(rawPtr, out timeout);
        }

        // Userdata set/get
        public Result SetUserData            (IntPtr userdata)
        {
            return FMOD_System_SetUserData(rawPtr, userdata);
        }
        public Result GetUserData            (out IntPtr userdata)
        {
            return FMOD_System_GetUserData(rawPtr, out userdata);
        }


        #region importfunctions
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_Release                (IntPtr system);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_SetOutput              (IntPtr system, OutputType output);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetOutput              (IntPtr system, out OutputType output);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetNumDrivers          (IntPtr system, out int numdrivers);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetDriverInfo          (IntPtr system, int id, IntPtr name, int namelen, out Guid guid, out int systemrate, out SpeakerMode speakermode, out int speakermodechannels);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_SetDriver              (IntPtr system, int driver);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetDriver              (IntPtr system, out int driver);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_SetSoftwareChannels    (IntPtr system, int numsoftwarechannels);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetSoftwareChannels    (IntPtr system, out int numsoftwarechannels);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_SetSoftwareFormat      (IntPtr system, int samplerate, SpeakerMode speakermode, int numrawspeakers);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetSoftwareFormat      (IntPtr system, out int samplerate, out SpeakerMode speakermode, out int numrawspeakers);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_SetDSPBufferSize       (IntPtr system, uint bufferlength, int numbuffers);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetDSPBufferSize       (IntPtr system, out uint bufferlength, out int numbuffers);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_SetFileSystem          (IntPtr system, FILE_OPENCALLBACK useropen, FILE_CLOSECALLBACK userclose, FILE_READCALLBACK userread, FILE_SEEKCALLBACK userseek, FILE_ASYNCREADCALLBACK userasyncread, FILE_ASYNCCANCELCALLBACK userasynccancel, int blockalign);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_AttachFileSystem       (IntPtr system, FILE_OPENCALLBACK useropen, FILE_CLOSECALLBACK userclose, FILE_READCALLBACK userread, FILE_SEEKCALLBACK userseek);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_SetPluginPath          (IntPtr system, byte[] path);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_LoadPlugin             (IntPtr system, byte[] filename, out uint handle, uint priority);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_UnloadPlugin           (IntPtr system, uint handle);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetNumNestedPlugins    (IntPtr system, uint handle, out int count);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetNestedPlugin        (IntPtr system, uint handle, int index, out uint nestedhandle);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetNumPlugins          (IntPtr system, PluginType plugintype, out int numplugins);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetPluginHandle        (IntPtr system, PluginType plugintype, int index, out uint handle);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetPluginInfo          (IntPtr system, uint handle, out PluginType plugintype, IntPtr name, int namelen, out uint version);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_CreateDSPByPlugin      (IntPtr system, uint handle, out IntPtr dsp);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_SetOutputByPlugin      (IntPtr system, uint handle);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetOutputByPlugin      (IntPtr system, out uint handle);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetDSPInfoByPlugin     (IntPtr system, uint handle, out IntPtr description);
        [DllImport(FMODVersion.DLL)]
        //private static extern Result FMOD_System_RegisterCodec          (IntPtr system, out CODEC_DESCRIPTION description, out uint handle, uint priority);
        //[DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_RegisterDSP            (IntPtr system, ref DspDescription description, out uint handle);
        [DllImport(FMODVersion.DLL)]
        //private static extern Result FMOD_System_RegisterOutput         (IntPtr system, ref OUTPUT_DESCRIPTION description, out uint handle);
        //[DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_Init                   (IntPtr system, int maxchannels, InitFlags flags, IntPtr extradriverdata);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_Close                  (IntPtr system);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_Update                 (IntPtr system);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_SetAdvancedSettings    (IntPtr system, ref AdvancedSettings settings);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetAdvancedSettings    (IntPtr system, ref AdvancedSettings settings);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_Set3DRolloffCallback   (IntPtr system, CB_3D_ROLLOFFCALLBACK callback);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_MixerSuspend           (IntPtr system);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_MixerResume            (IntPtr system);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetDefaultMixMatrix    (IntPtr system, SpeakerMode sourcespeakermode, SpeakerMode targetspeakermode, float[] matrix, int matrixhop);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetSpeakerModeChannels (IntPtr system, SpeakerMode mode, out int channels);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_SetCallback            (IntPtr system, SYSTEM_CALLBACK callback, SystemCallbackType callbackmask);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_SetSpeakerPosition     (IntPtr system, Speaker speaker, float x, float y, bool active);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetSpeakerPosition     (IntPtr system, Speaker speaker, out float x, out float y, out bool active);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_Set3DSettings          (IntPtr system, float dopplerscale, float distancefactor, float rolloffscale);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_Get3DSettings          (IntPtr system, out float dopplerscale, out float distancefactor, out float rolloffscale);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_Set3DNumListeners      (IntPtr system, int numlisteners);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_Get3DNumListeners      (IntPtr system, out int numlisteners);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_Set3DListenerAttributes(IntPtr system, int listener, ref Vector pos, ref Vector vel, ref Vector forward, ref Vector up);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_Get3DListenerAttributes(IntPtr system, int listener, out Vector pos, out Vector vel, out Vector forward, out Vector up);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_SetStreamBufferSize    (IntPtr system, uint filebuffersize, TimeUnit filebuffersizetype);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetStreamBufferSize    (IntPtr system, out uint filebuffersize, out TimeUnit filebuffersizetype);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetVersion             (IntPtr system, out uint version);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetOutputHandle        (IntPtr system, out IntPtr handle);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetChannelsPlaying     (IntPtr system, out int channels, out int realchannels);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetCPUUsage            (IntPtr system, out float dsp, out float stream, out float geometry, out float update, out float total);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetFileUsage            (IntPtr system, out Int64 sampleBytesRead, out Int64 streamBytesRead, out Int64 otherBytesRead);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetSoundRAM            (IntPtr system, out int currentalloced, out int maxalloced, out int total);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_CreateSound            (IntPtr system, byte[] name_or_data, Mode mode, ref CreateSoundExInfo exinfo, out IntPtr sound);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_CreateStream           (IntPtr system, byte[] name_or_data, Mode mode, ref CreateSoundExInfo exinfo, out IntPtr sound);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_CreateDSP              (IntPtr system, ref DspDescription description, out IntPtr dsp);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_CreateDSPByType        (IntPtr system, DspType type, out IntPtr dsp);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_CreateChannelGroup     (IntPtr system, byte[] name, out IntPtr channelgroup);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_CreateSoundGroup       (IntPtr system, byte[] name, out IntPtr soundgroup);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_CreateReverb3D         (IntPtr system, out IntPtr reverb);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_PlaySound              (IntPtr system, IntPtr sound, IntPtr channelGroup, bool paused, out IntPtr channel);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_PlayDSP                (IntPtr system, IntPtr dsp, IntPtr channelGroup, bool paused, out IntPtr channel);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetChannel             (IntPtr system, int channelid, out IntPtr channel);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetMasterChannelGroup  (IntPtr system, out IntPtr channelgroup);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetMasterSoundGroup    (IntPtr system, out IntPtr soundgroup);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_AttachChannelGroupToPort  (IntPtr system, uint portType, ulong portIndex, IntPtr channelgroup, bool passThru);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_DetachChannelGroupFromPort(IntPtr system, IntPtr channelgroup);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_SetReverbProperties    (IntPtr system, int instance, ref ReverbProperties prop);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetReverbProperties    (IntPtr system, int instance, out ReverbProperties prop);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_LockDSP                (IntPtr system);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_UnlockDSP              (IntPtr system);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetRecordNumDrivers    (IntPtr system, out int numdrivers, out int numconnected);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetRecordDriverInfo    (IntPtr system, int id, IntPtr name, int namelen, out Guid guid, out int systemrate, out SpeakerMode speakermode, out int speakermodechannels, out DriverState state);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetRecordPosition      (IntPtr system, int id, out uint position);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_RecordStart            (IntPtr system, int id, IntPtr sound, bool loop);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_RecordStop             (IntPtr system, int id);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_IsRecording            (IntPtr system, int id, out bool recording);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_CreateGeometry         (IntPtr system, int maxpolygons, int maxvertices, out IntPtr geometry);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_SetGeometrySettings    (IntPtr system, float maxworldsize);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetGeometrySettings    (IntPtr system, out float maxworldsize);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_LoadGeometry           (IntPtr system, IntPtr data, int datasize, out IntPtr geometry);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetGeometryOcclusion   (IntPtr system, ref Vector listener, ref Vector source, out float direct, out float reverb);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_SetNetworkProxy        (IntPtr system, byte[] proxy);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetNetworkProxy        (IntPtr system, IntPtr proxy, int proxylen);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_SetNetworkTimeout      (IntPtr system, int timeout);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetNetworkTimeout      (IntPtr system, out int timeout);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_SetUserData            (IntPtr system, IntPtr userdata);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_System_GetUserData            (IntPtr system, out IntPtr userdata);
        #endregion

        #region wrapperinternal

        public System(IntPtr raw)
            : base(raw)
        {
        }

        #endregion
    }
}
