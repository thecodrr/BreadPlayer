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
   public class TrueToFalseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Windows.UI.Xaml.Controls.SplitViewDisplayMode mode = (Windows.UI.Xaml.Controls.SplitViewDisplayMode)value;
            if (mode == Windows.UI.Xaml.Controls.SplitViewDisplayMode.CompactInline)
                return true;
            else
                return false;         
        }
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class BrushToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Windows.UI.Xaml.Visibility mode = (Windows.UI.Xaml.Visibility)value;

            return mode.ToString();
        }
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
