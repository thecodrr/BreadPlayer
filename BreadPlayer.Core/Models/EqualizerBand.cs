using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Core.Models
{
    public class EqualizerBand : ObservableObject
    {
        string FormatNumber(float num)
        {
            if (num >= 100000)
                return FormatNumber(num / 1000) + "K";
            if (num >= 1000)
            {
                return (num / 1000D).ToString("0.#") + "K";
            }
            return num.ToString("#0");
        }
        public string CenterTitle
        {
            get { return FormatNumber(Center) + "Hz"; }
        }
        float center;
        public float Center
        {
            get { return center; }
            set { Set(ref center, value); }
        }
        float gain;
        public float Gain
        {
            get { return gain; }
            set { Set(ref gain, value); }
        }
    }
}
