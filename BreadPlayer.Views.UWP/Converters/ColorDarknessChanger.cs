using BreadPlayer.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace BreadPlayer.Converters
{
    class ColorDarknessChanger : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var color = value.ToString().FromHexString();
            if(parameter != null && color != null)
            {
                if (parameter.ToString().Contains("Color"))
                    return color.ChangeColorBrightness(System.Convert.ToSingle(parameter.ToString().Replace("Color", "")));
                else
                   return new SolidColorBrush(color.ChangeColorBrightness(System.Convert.ToSingle( parameter)));
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
