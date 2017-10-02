using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace BreadPlayer.Converters
{
    public class RecordTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var valueType = value.GetType().Name;
            if (valueType.ToLower() == parameter.ToString().ToLower())
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}