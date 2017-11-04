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
        private void SetSource(BitmapImage bitmapImage, string uriSource, object para)
        {
            if (!string.IsNullOrEmpty(uriSource))
            {
                // string def = Windows.UI.Xaml.Application.Current.RequestedTheme == Windows.UI.Xaml.ApplicationTheme.Light ? "ms-appx:///Assets/albumart.png" : "ms-appx:///Assets/albumart_black.png";
                if (para == null)
                {
                    bitmapImage.DecodePixelHeight = 200;
                    bitmapImage.DecodePixelWidth = 200;
                }
                bitmapImage.UriSource = new Uri(uriSource, UriKind.RelativeOrAbsolute);
            }
        }
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            BitmapImage image = new BitmapImage();
            if (value is Mediafile mediaFile)
            {
                if (mediaFile.AttachedPictureBytes != null)
                {
                    using (MemoryStream stream = new MemoryStream(mediaFile.AttachedPictureBytes))
                    {
                        SetSourceAsync(image, stream);
                    }
                }
                else if (!string.IsNullOrEmpty(mediaFile.AttachedPicture))
                {
                    SetSource(image, mediaFile.AttachedPicture, parameter);
                }
                return image ?? null;
            }
            else if (value is string stringValue)
            {
                SetSource(image, stringValue, parameter);
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