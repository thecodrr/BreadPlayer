using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using System.Globalization;
namespace Macalifa.Converters
{
    public class DoubleToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string FormattedTime = "00:00";
            if (value is double)
            {
                double time = (double)value;
                FormattedTime = TimeSpan.FromSeconds(time).ToString(@"mm\:ss");                
            }
            return FormattedTime;
        }
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
