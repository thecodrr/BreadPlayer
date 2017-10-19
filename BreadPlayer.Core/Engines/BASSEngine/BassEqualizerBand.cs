using BreadPlayer.Core.Engines.Interfaces;
using ManagedBass;
using ManagedBass.Fx;

namespace BreadPlayer.Core.Engines.BASSEngine
{
    public class BassEqualizerBand : ObservableObject, IEqualizerBand
    {
        private float _gain;
        private bool _isActive;
        private PeakEQParameters _eq;
        private int _fxEq;

        public BassEqualizerBand(int fxEqHandle, int bandNo, float centerValue, float gainValue, bool active)
        {
            _fxEq = fxEqHandle;
            _eq = new PeakEQParameters()
            {
                lBand = bandNo,
            };
            if (centerValue >= 1000)
            {
                BandCaption = string.Format("{0}KHz", (centerValue / 1000));
            }
            else
            {
                BandCaption = centerValue + "Hz";
            }
            _gain = gainValue;
            IsActive = active;
        }

        public string BandCaption { get; set; }

        /// <summary>
        /// Gain: Frequency Gain. 0.05 to 3.0. Default = 1.0
        /// </summary>
        public float Gain
        {
            get => _gain;
            set
            {
                Set(ref _gain, value);

                Bass.FXGetParameters(_fxEq, _eq);
                _eq.fGain = _gain;
                Bass.FXSetParameters(_fxEq, _eq);
            }
        }

        public bool IsActive
        {
            get => _isActive;
            set => Set(ref _isActive, value);
        }

        public void Remove()
        {
           // Gain = 0F;
        }
    }
}