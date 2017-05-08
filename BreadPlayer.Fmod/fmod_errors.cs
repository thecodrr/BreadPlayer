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
                case Result.OK:                            return "No errors.";
                case Result.ERR_BADCOMMAND:                return "Tried to call a function on a data type that does not allow this type of functionality (ie calling Sound::lock on a streaming sound).";
                case Result.ERR_CHANNEL_ALLOC:             return "Error trying to allocate a channel.";
                case Result.ERR_CHANNEL_STOLEN:            return "The specified channel has been reused to play another sound.";
                case Result.ERR_DMA:                       return "DMA Failure.  See debug output for more information.";
                case Result.ERR_DSP_CONNECTION:            return "DSP connection error.  Connection possibly caused a cyclic dependency or connected dsps with incompatible buffer counts.";
                case Result.ERR_DSP_DONTPROCESS:           return "DSP return code from a DSP process query callback.  Tells mixer not to call the process callback and therefore not consume CPU.  Use this to optimize the DSP graph.";
                case Result.ERR_DSP_FORMAT:                return "DSP Format error.  A DSP unit may have attempted to connect to this network with the wrong format, or a matrix may have been set with the wrong size if the target unit has a specified channel map.";
                case Result.ERR_DSP_INUSE:                 return "DSP is already in the mixer's DSP network. It must be removed before being reinserted or released.";
                case Result.ERR_DSP_NOTFOUND:              return "DSP connection error.  Couldn't find the DSP unit specified.";
                case Result.ERR_DSP_RESERVED:              return "DSP operation error.  Cannot perform operation on this DSP as it is reserved by the system.";
                case Result.ERR_DSP_SILENCE:               return "DSP return code from a DSP process query callback.  Tells mixer silence would be produced from read, so go idle and not consume CPU.  Use this to optimize the DSP graph.";
                case Result.ERR_DSP_TYPE:                  return "DSP operation cannot be performed on a DSP of this type.";
                case Result.ERR_FILE_BAD:                  return "Error loading file.";
                case Result.ERR_FILE_COULDNOTSEEK:         return "Couldn't perform seek operation.  This is a limitation of the medium (ie netstreams) or the file format.";
                case Result.ERR_FILE_DISKEJECTED:          return "Media was ejected while reading.";
                case Result.ERR_FILE_EOF:                  return "End of file unexpectedly reached while trying to read essential data (truncated?).";
                case Result.ERR_FILE_ENDOFDATA:            return "End of current chunk reached while trying to read data.";
                case Result.ERR_FILE_NOTFOUND:             return "File not found.";
                case Result.ERR_FORMAT:                    return "Unsupported file or audio format.";
                case Result.ERR_HEADER_MISMATCH:           return "There is a version mismatch between the BreadPlayer.Fmod header and either the BreadPlayer.Fmod Studio library or the BreadPlayer.Fmod Low Level library.";
                case Result.ERR_HTTP:                      return "A HTTP error occurred. This is a catch-all for HTTP errors not listed elsewhere.";
                case Result.ERR_HTTP_ACCESS:               return "The specified resource requires authentication or is forbidden.";
                case Result.ERR_HTTP_PROXY_AUTH:           return "Proxy authentication is required to access the specified resource.";
                case Result.ERR_HTTP_SERVER_ERROR:         return "A HTTP server error occurred.";
                case Result.ERR_HTTP_TIMEOUT:              return "The HTTP request timed out.";
                case Result.ERR_INITIALIZATION:            return "BreadPlayer.Fmod was not initialized correctly to support this function.";
                case Result.ERR_INITIALIZED:               return "Cannot call this command after System::init.";
                case Result.ERR_INTERNAL:                  return "An error occurred that wasn't supposed to.  Contact support.";
                case Result.ERR_INVALID_FLOAT:             return "Value passed in was a NaN, Inf or denormalized float.";
                case Result.ERR_INVALID_HANDLE:            return "An invalid object handle was used.";
                case Result.ERR_INVALID_PARAM:             return "An invalid parameter was passed to this function.";
                case Result.ERR_INVALID_POSITION:          return "An invalid seek position was passed to this function.";
                case Result.ERR_INVALID_SPEAKER:           return "An invalid speaker was passed to this function based on the current speaker mode.";
                case Result.ERR_INVALID_SYNCPOINT:         return "The syncpoint did not come from this sound handle.";
                case Result.ERR_INVALID_THREAD:            return "Tried to call a function on a thread that is not supported.";
                case Result.ERR_INVALID_VECTOR:            return "The vectors passed in are not unit length, or perpendicular.";
                case Result.ERR_MAXAUDIBLE:                return "Reached maximum audible playback count for this sound's soundgroup.";
                case Result.ERR_MEMORY:                    return "Not enough memory or resources.";
                case Result.ERR_MEMORY_CANTPOINT:          return "Can't use FMOD_OPENMEMORY_POINT on non PCM source data, or non mp3/xma/adpcm data if FMOD_CREATECOMPRESSEDSAMPLE was used.";
                case Result.ERR_NEEDS3D:                   return "Tried to call a command on a 2d sound when the command was meant for 3d sound.";
                case Result.ERR_NEEDSHARDWARE:             return "Tried to use a feature that requires hardware support.";
                case Result.ERR_NET_CONNECT:               return "Couldn't connect to the specified host.";
                case Result.ERR_NET_SOCKET_ERROR:          return "A socket error occurred.  This is a catch-all for socket-related errors not listed elsewhere.";
                case Result.ERR_NET_URL:                   return "The specified URL couldn't be resolved.";
                case Result.ERR_NET_WOULD_BLOCK:           return "Operation on a non-blocking socket could not complete immediately.";
                case Result.ERR_NOTREADY:                  return "Operation could not be performed because specified sound/DSP connection is not ready.";
                case Result.ERR_OUTPUT_ALLOCATED:          return "Error initializing output device, but more specifically, the output device is already in use and cannot be reused.";
                case Result.ERR_OUTPUT_CREATEBUFFER:       return "Error creating hardware sound buffer.";
                case Result.ERR_OUTPUT_DRIVERCALL:         return "A call to a standard soundcard driver failed, which could possibly mean a bug in the driver or resources were missing or exhausted.";
                case Result.ERR_OUTPUT_FORMAT:             return "Soundcard does not support the specified format.";
                case Result.ERR_OUTPUT_INIT:               return "Error initializing output device.";
                case Result.ERR_OUTPUT_NODRIVERS:          return "The output device has no drivers installed.  If pre-init, FMOD_OUTPUT_NOSOUND is selected as the output mode.  If post-init, the function just fails.";
                case Result.ERR_PLUGIN:                    return "An unspecified error has been returned from a plugin.";
                case Result.ERR_PLUGIN_MISSING:            return "A requested output, dsp unit type or codec was not available.";
                case Result.ERR_PLUGIN_RESOURCE:           return "A resource that the plugin requires cannot be found. (ie the DLS file for MIDI playback)";
                case Result.ERR_PLUGIN_VERSION:            return "A plugin was built with an unsupported SDK version.";
                case Result.ERR_RECORD:                    return "An error occurred trying to initialize the recording device.";
                case Result.ERR_REVERB_CHANNELGROUP:       return "Reverb properties cannot be set on this channel because a parent channelgroup owns the reverb connection.";
                case Result.ERR_REVERB_INSTANCE:           return "Specified instance in FMOD_REVERB_PROPERTIES couldn't be set. Most likely because it is an invalid instance number or the reverb doesn't exist.";
                case Result.ERR_SUBSOUNDS:                 return "The error occurred because the sound referenced contains subsounds when it shouldn't have, or it doesn't contain subsounds when it should have.  The operation may also not be able to be performed on a parent sound.";
                case Result.ERR_SUBSOUND_ALLOCATED:        return "This subsound is already being used by another sound, you cannot have more than one parent to a sound.  Null out the other parent's entry first.";
                case Result.ERR_SUBSOUND_CANTMOVE:         return "Shared subsounds cannot be replaced or moved from their parent stream, such as when the parent stream is an FSB file.";
                case Result.ERR_TAGNOTFOUND:               return "The specified tag could not be found or there are no tags.";
                case Result.ERR_TOOMANYCHANNELS:           return "The sound created exceeds the allowable input channel count.  This can be increased using the 'maxinputchannels' parameter in System::setSoftwareFormat.";
                case Result.ERR_TRUNCATED:                 return "The retrieved string is too long to fit in the supplied buffer and has been truncated.";
                case Result.ERR_UNIMPLEMENTED:             return "Something in BreadPlayer.Fmod hasn't been implemented when it should be! contact support!";
                case Result.ERR_UNINITIALIZED:             return "This command failed because System::init or System::setDriver was not called.";
                case Result.ERR_UNSUPPORTED:               return "A command issued was not supported by this object.  Possibly a plugin without certain callbacks specified.";
                case Result.ERR_VERSION:                   return "The version number of this file format is not supported.";
                case Result.ERR_EVENT_ALREADY_LOADED:      return "The specified bank has already been loaded.";
                case Result.ERR_EVENT_LIVEUPDATE_BUSY:     return "The live update connection failed due to the game already being connected.";
                case Result.ERR_EVENT_LIVEUPDATE_MISMATCH: return "The live update connection failed due to the game data being out of sync with the tool.";
                case Result.ERR_EVENT_LIVEUPDATE_TIMEOUT:  return "The live update connection timed out.";
                case Result.ERR_EVENT_NOTFOUND:            return "The requested event, bus or vca could not be found.";
                case Result.ERR_STUDIO_UNINITIALIZED:      return "The Studio::System object is not yet initialized.";
                case Result.ERR_STUDIO_NOT_LOADED:         return "The specified resource is not loaded, so it can't be unloaded.";
                case Result.ERR_INVALID_STRING:            return "An invalid string was passed to this function.";
                case Result.ERR_ALREADY_LOCKED:            return "The specified resource is already locked.";
                case Result.ERR_NOT_LOCKED:                return "The specified resource is not locked, so it can't be unlocked.";
				case Result.ERR_RECORD_DISCONNECTED:       return "The specified recording driver has been disconnected.";
				case Result.ERR_TOOMANYSAMPLES:            return "The length provided exceed the allowable limit.";
                default:                                        return "Unknown error.";
            }
        }
    }
}
