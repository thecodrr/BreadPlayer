using BreadPlayer.Core.Models;
using System;
using System.IO;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace BreadPlayer.Converters
{
    internal class PathToImageSourceConverter : IValueConverter
    {
        private async void SetSourceAsync(BitmapImage image, Stream stream)
        {
            await image.SetSourceAsync(stream.AsRandomAccessStream());
        }
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Mediafile mediaFile)
            {
                BitmapImage image = new BitmapImage();
                if (mediaFile.AttachedPictureBytes != null)
                {
                    using (MemoryStream stream = new MemoryStream(mediaFile.AttachedPictureBytes))
                    {
                        SetSourceAsync(image, stream);
                    }
                }
                else if (!string.IsNullOrEmpty(mediaFile.AttachedPicture))
                {
                    // string def = Windows.UI.Xaml.Application.Current.RequestedTheme == Windows.UI.Xaml.ApplicationTheme.Light ? "ms-appx:///Assets/albumart.png" : "ms-appx:///Assets/albumart_black.png";
                    if (parameter == null)
                    {
                        image.DecodePixelHeight = 200;
                        image.DecodePixelWidth = 200;
                    }
                    if (value is string stringValue && !string.IsNullOrEmpty(stringValue))
                    {
                        image.UriSource = new Uri(stringValue, UriKind.RelativeOrAbsolute);
                    }
                }
                return image ?? null;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }    
}