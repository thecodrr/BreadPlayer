using BreadPlayer.Fmod.Enums;
using BreadPlayer.Fmod.Structs;

namespace BreadPlayer.Fmod
{
    public static class Helpers
    {
        public static Result SetFadePoint(this Channel fmodChannel, float fromVolume, float toVolume, ulong sampleRange)
        {
            Result result;
            result = fmodChannel.GetDspClock(out ulong clockDsp, out ulong parentclock);
            result = fmodChannel.AddFadePoint(parentclock, fromVolume);
            result = fmodChannel.AddFadePoint(parentclock + sampleRange, toVolume);
            return result;
        }
        public static uint GetTotalSamplesLeft(this Channel fmodChannel, Sound fmodSound)
        {
            fmodChannel.GetPosition(out uint currentPosition, TimeUnit.Pcm);
            fmodSound.GetLength(out uint pcmLength, TimeUnit.Pcm);
            return pcmLength - currentPosition;
        }
        public static ulong ConvertSecondsToPcm(this Sound fmodSound, double seconds)
        {
            fmodSound.GetDefaults(out float frequency, out int priority);
            return (ulong)(frequency * seconds);
        }
    }
}
