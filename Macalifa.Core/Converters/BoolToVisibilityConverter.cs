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
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool flag = false;
            if (value is bool)
            {
                flag = (bool)value;
            }
            else if (value is bool?)
            {
                bool? nullable = (bool?)value;
                flag = nullable.HasValue ? nullable.Value : false;
            }
            return (flag ? Visibility.Visible : Visibility.Collapsed);
        }
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
