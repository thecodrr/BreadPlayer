using System.Collections.Generic;

namespace BreadPlayer.Interfaces
{
    public interface IEqualizerSettings
    {
        Dictionary<string, float> GainValues { get; set; }
        bool IsEnabled { get; set; }
        string Name { get; set; }
    }
}