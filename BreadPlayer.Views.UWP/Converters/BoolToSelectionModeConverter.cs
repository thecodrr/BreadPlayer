using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace BreadPlayer.Converters
{
    class BoolToSelectionModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool mode = (bool)value;
            if (value is bool)
            {
                if (mode == true)
                    return ListViewSelectionMode.Multiple;
                else
                    return ListViewSelectionMode.Extended;                
            }
            return ListViewSelectionMode.Extended;
        }
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            return string.Empty;
        }
    }
}
