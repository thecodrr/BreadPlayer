using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace BreadPlayer.Converters
{
    class PathToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            BitmapImage image = new BitmapImage();
            image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            string def = Windows.UI.Xaml.Application.Current.RequestedTheme == Windows.UI.Xaml.ApplicationTheme.Light ? "ms-appx:///Assets/albumart.png" : "ms-appx:///Assets/albumart_black.png";
            if (parameter == null)
            {
                image.DecodePixelHeight = 160;
                image.DecodePixelWidth = 160;
            }
            if (value is string && !string.IsNullOrEmpty(value.ToString()))
            {
                image.UriSource = new Uri(value.ToString(), UriKind.RelativeOrAbsolute);
            }
            if(value == null)
            {
                image.DecodePixelHeight = 170;
                image.DecodePixelWidth = 160;
                image.UriSource = parameter == null ? new Uri(def, UriKind.RelativeOrAbsolute) : null;
            }
           
            return image;
        }
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

