using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Macalifa.Converters
{
    class PathToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string def = App.Current.RequestedTheme == Windows.UI.Xaml.ApplicationTheme.Light ? "ms-appx:///Assets/albumart.png" : "ms-appx:///Assets/albumart_black.png";
            if (value is string && value != null)
            {
                BitmapImage image = new BitmapImage(new Uri(value.ToString() ?? def, UriKind.RelativeOrAbsolute));
                return image;
            }
            if(parameter != null)
                return null;
            return new BitmapImage(new Uri(def, UriKind.RelativeOrAbsolute));
        }
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

