using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Core.PlayerEngines
{
    public class EqualizerSettings
    {
        public string Name { get; set; }
        public Dictionary<string, float> GainValues { get; set; }
        public bool IsEnabled { get; set; }
    }
}
