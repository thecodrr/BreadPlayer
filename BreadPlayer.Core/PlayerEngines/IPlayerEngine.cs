using BreadPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Core.PlayerEngines
{
    public interface IPlayerEngine : IDisposable
    {
        //METHODS
        Task Init(bool isMobile);
        Task<bool> Load(Mediafile mediaFile);
        Task Pause();
        Task Stop();
        Task Play();

        //PROPERTIES
        bool IsVolumeMuted { get; set; }
        Effects Effect { get; set; }
        IEqualizer Equalizer { get; set; }
        double Volume { get; set; }
        double Position { get; set; }
        double Length { get; set; }
        PlayerState PlayerState { get; set; }
        Mediafile CurrentlyPlayingFile { get; set; }
        bool IgnoreErrors { get; set; }

        //EVENTS
        event OnMediaStateChanged MediaStateChanged;
        event OnMediaEnded MediaEnded;
        event OnMediaAboutToEnd MediaAboutToEnd;
        event OnMediaChanging MediaChanging;
    }
}
