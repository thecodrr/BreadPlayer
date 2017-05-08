using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Core.PlayerEngines
{
    public interface IEqualizerBand
    {
        void Remove();

        string BandCaption { get; set; }

        /// <summary>
        /// Gain: Frequency Gain. -30 to +30. Default = 0
        /// </summary>
        float Gain { get; set; }
        bool IsActive { get; set; }
    }
}
