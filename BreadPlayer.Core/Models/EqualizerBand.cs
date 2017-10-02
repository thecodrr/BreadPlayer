namespace BreadPlayer.Core.Models
{
    public class EqualizerBand : ObservableObject
    {
        private string FormatNumber(float num)
        {
            if (num >= 100000)
            {
                return FormatNumber(num / 1000) + "K";
            }

            if (num >= 1000)
            {
                return (num / 1000D).ToString("0.#") + "K";
            }
            return num.ToString("#0");
        }

        public string CenterTitle => FormatNumber(Center) + "Hz";

        private float _center;

        public float Center
        {
            get => _center;
            set => Set(ref _center, value);
        }

        private float _gain;

        public float Gain
        {
            get => _gain;
            set => Set(ref _gain, value);
        }
    }
}