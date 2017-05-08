using BreadPlayer.Fmod.Enums;
using BreadPlayer.Fmod.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace BreadPlayer.Fmod
{
    public static class Helpers
    {
        public static Result SetFadePoint(this Channel FMODChannel, float fromVolume, float toVolume, ulong sampleRange)
        {
            Result result;
            result = FMODChannel.getDSPClock(out ulong clockDSP, out ulong parentclock);
            result = FMODChannel.addFadePoint(parentclock, fromVolume);
            result = FMODChannel.addFadePoint(parentclock + sampleRange, toVolume);
            return result;
        }
        public static uint GetTotalSamplesLeft(this Channel FMODChannel, Sound FMODSound)
        {
            FMODChannel.getPosition(out uint currentPosition, TimeUnit.PCM);
            FMODSound.getLength(out uint pcmLength, TimeUnit.PCM);
            return pcmLength - currentPosition;
        }
        public static ulong ConvertSecondsToPCM(this Sound FMODSound, double seconds)
        {
            FMODSound.getDefaults(out float frequency, out int priority);
            return (ulong)(frequency * seconds);
        }
    }
}
