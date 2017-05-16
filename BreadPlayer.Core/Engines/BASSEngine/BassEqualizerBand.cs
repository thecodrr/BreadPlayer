using BreadPlayer.Core.Engines.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Core.Engines.BASSEngine
{
    public class BassEqualizerBand : IEqualizerBand
    {
        public string BandCaption { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public float Gain { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsActive { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Remove()
        {
            throw new NotImplementedException();
        }
    }
}
