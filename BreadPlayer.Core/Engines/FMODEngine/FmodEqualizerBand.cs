using BreadPlayer.Fmod;
using BreadPlayer.Fmod.CoreDSP;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Core.PlayerEngines
{
    public class FmodEqualizerBand : ObservableObject, IEqualizerBand
    {
        private DSP dspEQ;
        private float gain;
        private bool isActive;

        public FmodEqualizerBand(DSP dspParamEq, float centerValue, float gainValue, bool active)
        {
            this.dspEQ = dspParamEq;
            if (centerValue >= 1000)
            {
                this.BandCaption = string.Format("{0}K", (centerValue / 1000));
            }
            else
            {
                this.BandCaption = centerValue.ToString(CultureInfo.InvariantCulture);
            }
            this.gain = gainValue;
            this.IsActive = active;
        }  

        public void Remove()
        {
            if (this.dspEQ != null)
            {
                var result = this.dspEQ.release();
                this.dspEQ = null;
            }
            this.IsActive = false;
        }

        public string BandCaption { get; set; }

        /// <summary>
        /// Gain: Frequency Gain. 0.05 to 3.0. Default = 1.0
        /// </summary>
        public float Gain
        {
            get { return this.gain; }
            set
            {
                Set(ref gain, value);
                if (this.dspEQ != null)
                {
                    var result = this.dspEQ.setActive(false);

                    result = this.dspEQ.setParameterFloat((int)DspParamEQ.GAIN, value);

                    result = this.dspEQ.setActive(true);
                }
            }
        }

        public bool IsActive
        {
            get { return this.isActive; }
            set
            {
                Set(ref isActive, value);
            }
        }
    }
}
