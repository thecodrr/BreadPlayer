using BreadPlayer.Core.Engines.Interfaces;
using BreadPlayer.Fmod;
using BreadPlayer.Fmod.CoreDSP;

namespace BreadPlayer.Core.Engines
{
    public class FmodEqualizerBand : ObservableObject, IEqualizerBand
    {
        private Dsp _dspEq;
        private float _gain;
        private bool _isActive;
        private Channel _fChannel;

        public FmodEqualizerBand(Channel fmodChannel, Dsp dspParamEq, float centerValue, float gainValue, bool active)
        {
            _fChannel = fmodChannel;
            _dspEq = dspParamEq;
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

        public void Remove()
        {
            if (_dspEq != null)
            {
                var result = _fChannel?.RemoveDsp(_dspEq);
                result = _dspEq.Release();                
                _dspEq = null;
            }
            IsActive = false;
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
                if (_dspEq != null)
                {
                    var result = _dspEq.SetActive(false);

                    result = _dspEq.SetParameterFloat((int)DspParamEq.Gain, value);

                    result = _dspEq.SetActive(true);
                }
            }
        }

        public bool IsActive
        {
            get => _isActive;
            set => Set(ref _isActive, value);
        }
    }
}
