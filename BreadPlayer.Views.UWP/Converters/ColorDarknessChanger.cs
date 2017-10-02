using BreadPlayer.Extensions;
using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace BreadPlayer.Converters
{
    internal class ColorDarknessChanger : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                var color = value.ToString().FromHexString();
                if (parameter != null && color != null)
                {
                    if (parameter.ToString().Contains("Color"))
                        return color.ChangeColorBrightness(System.Convert.ToSingle(parameter.ToString().Replace("Color", "")));
                    else
                        return new SolidColorBrush(color.ChangeColorBrightness(System.Convert.ToSingle(parameter)));
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            return string.Empty;
        }
    }
}