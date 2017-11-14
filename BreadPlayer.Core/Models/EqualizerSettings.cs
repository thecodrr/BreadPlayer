using BreadPlayer.Interfaces;
using System.Collections.Generic;

namespace BreadPlayer.Core.Models
{
    public class EqualizerSettings : IEqualizerSettings
    {
        public string Name { get; set; }
        public Dictionary<string, float> GainValues { get; set; }
        public bool IsEnabled { get; set; }
    }
}