/* =================================================================================================== */
/* BreadPlayer.Fmod Studio - Error string header file. Copyright (c), Firelight Technologies Pty, Ltd. 2004-2016.  */
/*                                                                                                     */
/* Use this header if you want to store or display a string version / english explanation of           */
/* the BreadPlayer.Fmod error codes.                                                                               */
/*                                                                                                     */
/* =================================================================================================== */

using BreadPlayer.Fmod.Enums;

namespace BreadPlayer.Fmod
{
    public class Error
    {
        public static string String(Result errcode)
        {
            switch (errcode)
            {
                case Result.Ok:                            return "No errors.";
                case Result.ErrBadcommand:                return "Tried to call a function on a data type that does not allow this type of functionality (ie calling Sound::lock on a streaming sound).";
                case Result.ErrChannelAlloc:             return "Error trying to allocate a channel.";
                case Result.ErrChannelStolen:            return "The specified channel has been reused to play another sound.";
                case Result.ErrDma:                       return "DMA Failure.  See debug output for more information.";
                case Result.ErrDspConnection:            return "DSP connection error.  Connection possibly caused a cyclic dependency or connected dsps with incompatible buffer counts.";
                case Result.ErrDspDontprocess:           return "DSP return code from a DSP process query callback.  Tells mixer not to call the process callback and therefore not consume CPU.  Use this to optimize the DSP graph.";
                case Result.ErrDspFormat:                return "DSP Format error.  A DSP unit may have attempted to connect to this network with the wrong format, or a matrix may have been set with the wrong size if the target unit has a specified channel map.";
                case Result.ErrDspInuse:                 return "DSP is already in the mixer's DSP network. It must be removed before being reinserted or released.";
                case Result.ErrDspNotfound:              return "DSP connection error.  Couldn't find the DSP unit specified.";
                case Result.ErrDspReserved:              return "DSP operation error.  Cannot perform operation on this DSP as it is reserved by the system.";
                case Result.ErrDspSilence:               return "DSP return code from a DSP process query callback.  Tells mixer silence would be produced from read, so go idle and not consume CPU.  Use this to optimize the DSP graph.";
                case Result.ErrDspType:                  return "DSP operation cannot be performed on a DSP of this type.";
                case Result.ErrFileBad:                  return "Error loading file.";
                case Result.ErrFileCouldnotseek:         return "Couldn't perform seek operation.  This is a limitation of the medium (ie netstreams) or the file format.";
                case Result.ErrFileDiskejected:          return "Media was ejected while reading.";
                case Result.ErrFileEof:                  return "End of file unexpectedly reached while trying to read essential data (truncated?).";
                case Result.ErrFileEndofdata:            return "End of current chunk reached while trying to read data.";
                case Result.ErrFileNotfound:             return "File not found.";
                case Result.ErrFormat:                    return "Unsupported file or audio format.";
                case Result.ErrHeaderMismatch:           return "There is a version mismatch between the BreadPlayer.Fmod header and either the BreadPlayer.Fmod Studio library or the BreadPlayer.Fmod Low Level library.";
                case Result.ErrHttp:                      return "A HTTP error occurred. This is a catch-all for HTTP errors not listed elsewhere.";
                case Result.ErrHttpAccess:               return "The specified resource requires authentication or is forbidden.";
                case Result.ErrHttpProxyAuth:           return "Proxy authentication is required to access the specified resource.";
                case Result.ErrHttpServerError:         return "A HTTP server error occurred.";
                case Result.ErrHttpTimeout:              return "The HTTP request timed out.";
                case Result.ErrInitialization:            return "BreadPlayer.Fmod was not initialized correctly to support this function.";
                case Result.ErrInitialized:               return "Cannot call this command after FMODSystem::init.";
                case Result.ErrInternal:                  return "An error occurred that wasn't supposed to.  Contact support.";
                case Result.ErrInvalidFloat:             return "Value passed in was a NaN, Inf or denormalized float.";
                case Result.ErrInvalidHandle:            return "An invalid object handle was used.";
                case Result.ErrInvalidParam:             return "An invalid parameter was passed to this function.";
                case Result.ErrInvalidPosition:          return "An invalid seek position was passed to this function.";
                case Result.ErrInvalidSpeaker:           return "An invalid speaker was passed to this function based on the current speaker mode.";
                case Result.ErrInvalidSyncpoint:         return "The syncpoint did not come from this sound handle.";
                case Result.ErrInvalidThread:            return "Tried to call a function on a thread that is not supported.";
                case Result.ErrInvalidVector:            return "The vectors passed in are not unit length, or perpendicular.";
                case Result.ErrMaxaudible:                return "Reached maximum audible playback count for this sound's soundgroup.";
                case Result.ErrMemory:                    return "Not enough memory or resources.";
                case Result.ErrMemoryCantpoint:          return "Can't use FMOD_OPENMEMORY_POINT on non PCM source data, or non mp3/xma/adpcm data if FMOD_CREATECOMPRESSEDSAMPLE was used.";
                case Result.ErrNeeds3D:                   return "Tried to call a command on a 2d sound when the command was meant for 3d sound.";
                case Result.ErrNeedshardware:             return "Tried to use a feature that requires hardware support.";
                case Result.ErrNetConnect:               return "Couldn't connect to the specified host.";
                case Result.ErrNetSocketError:          return "A socket error occurred.  This is a catch-all for socket-related errors not listed elsewhere.";
                case Result.ErrNetUrl:                   return "The specified URL couldn't be resolved.";
                case Result.ErrNetWouldBlock:           return "Operation on a non-blocking socket could not complete immediately.";
                case Result.ErrNotready:                  return "Operation could not be performed because specified sound/DSP connection is not ready.";
                case Result.ErrOutputAllocated:          return "Error initializing output device, but more specifically, the output device is already in use and cannot be reused.";
                case Result.ErrOutputCreatebuffer:       return "Error creating hardware sound buffer.";
                case Result.ErrOutputDrivercall:         return "A call to a standard soundcard driver failed, which could possibly mean a bug in the driver or resources were missing or exhausted.";
                case Result.ErrOutputFormat:             return "Soundcard does not support the specified format.";
                case Result.ErrOutputInit:               return "Error initializing output device.";
                case Result.ErrOutputNodrivers:          return "The output device has no drivers installed.  If pre-init, FMOD_OUTPUT_NOSOUND is selected as the output mode.  If post-init, the function just fails.";
                case Result.ErrPlugin:                    return "An unspecified error has been returned from a plugin.";
                case Result.ErrPluginMissing:            return "A requested output, dsp unit type or codec was not available.";
                case Result.ErrPluginResource:           return "A resource that the plugin requires cannot be found. (ie the DLS file for MIDI playback)";
                case Result.ErrPluginVersion:            return "A plugin was built with an unsupported SDK version.";
                case Result.ErrRecord:                    return "An error occurred trying to initialize the recording device.";
                case Result.ErrReverbChannelgroup:       return "Reverb properties cannot be set on this channel because a parent channelgroup owns the reverb connection.";
                case Result.ErrReverbInstance:           return "Specified instance in FMOD_REVERB_PROPERTIES couldn't be set. Most likely because it is an invalid instance number or the reverb doesn't exist.";
                case Result.ErrSubsounds:                 return "The error occurred because the sound referenced contains subsounds when it shouldn't have, or it doesn't contain subsounds when it should have.  The operation may also not be able to be performed on a parent sound.";
                case Result.ErrSubsoundAllocated:        return "This subsound is already being used by another sound, you cannot have more than one parent to a sound.  Null out the other parent's entry first.";
                case Result.ErrSubsoundCantmove:         return "Shared subsounds cannot be replaced or moved from their parent stream, such as when the parent stream is an FSB file.";
                case Result.ErrTagnotfound:               return "The specified tag could not be found or there are no tags.";
                case Result.ErrToomanychannels:           return "The sound created exceeds the allowable input channel count.  This can be increased using the 'maxinputchannels' parameter in FMODSystem::setSoftwareFormat.";
                case Result.ErrTruncated:                 return "The retrieved string is too long to fit in the supplied buffer and has been truncated.";
                case Result.ErrUnimplemented:             return "Something in BreadPlayer.Fmod hasn't been implemented when it should be! contact support!";
                case Result.ErrUninitialized:             return "This command failed because FMODSystem::init or FMODSystem::setDriver was not called.";
                case Result.ErrUnsupported:               return "A command issued was not supported by this object.  Possibly a plugin without certain callbacks specified.";
                case Result.ErrVersion:                   return "The version number of this file format is not supported.";
                case Result.ErrEventAlreadyLoaded:      return "The specified bank has already been loaded.";
                case Result.ErrEventLiveupdateBusy:     return "The live update connection failed due to the game already being connected.";
                case Result.ErrEventLiveupdateMismatch: return "The live update connection failed due to the game data being out of sync with the tool.";
                case Result.ErrEventLiveupdateTimeout:  return "The live update connection timed out.";
                case Result.ErrEventNotfound:            return "The requested event, bus or vca could not be found.";
                case Result.ErrStudioUninitialized:      return "The Studio::FMODSystem object is not yet initialized.";
                case Result.ErrStudioNotLoaded:         return "The specified resource is not loaded, so it can't be unloaded.";
                case Result.ErrInvalidString:            return "An invalid string was passed to this function.";
                case Result.ErrAlreadyLocked:            return "The specified resource is already locked.";
                case Result.ErrNotLocked:                return "The specified resource is not locked, so it can't be unlocked.";
				case Result.ErrRecordDisconnected:       return "The specified recording driver has been disconnected.";
				case Result.ErrToomanysamples:            return "The length provided exceed the allowable limit.";
                default:                                        return "Unknown error.";
            }
        }
    }
}
