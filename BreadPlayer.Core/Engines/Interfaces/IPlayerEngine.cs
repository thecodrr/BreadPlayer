using BreadPlayer.Core.Common;
using BreadPlayer.Core.Engines.BASSEngine;
using BreadPlayer.Core.Models;
using System;
using System.Threading.Tasks;

namespace BreadPlayer.Core.Engines.Interfaces
{
    public interface IPlayerEngine : IDisposable
    {
        //METHODS
        Task Init(bool isMobile);
        Task<bool> LoadLocalFileAsync(Mediafile mediaFile);
        Task<bool> LoadURLAsync(Mediafile mediafile, string uri);
        Task<bool> LoadStreamAsync(Mediafile mediafile, byte[] array);
        Task Pause();

        Task Stop();

        Task Play();

        Task ChangeDevice(string deviceName);
        //PROPERTIES
        bool CrossfadeEnabled { get; set; }
        double DeviceBufferSize { get; set; }
        bool IsVolumeMuted { get; set; }
        Equalizer Equalizer { get; set; }
        double Volume { get; set; }
        double Position { get; set; }
        double Length { get; set; }
        bool IsSeekable { get; set; }
        bool IsLoopingEnabled { get; set; }
        PlayerState PlayerState { get; set; }
        Mediafile CurrentlyPlayingFile { get; set; }

        //EVENTS
        event OnMediaStateChanged MediaStateChanged;

        event OnMediaEnded MediaEnded;

        event OnMediaAboutToEnd MediaAboutToEnd;

        event OnMediaChanging MediaChanging;

        event OnMediaChanging MediaChanged;
    }
}