using System;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

namespace BreadPlayer.Controls
{
    partial class ImageEx
    {
        private bool _isHttpSource;

        #region Source

        public object Source
        {
            get => GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        private static void SourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ImageEx;
            control.SetSource(e.NewValue);
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(object), typeof(ImageEx), new PropertyMetadata(null, SourceChanged));

        #endregion Source

        #region SetSource

        private void SetSource(object source)
        {
            _isHttpSource = false;

            if (source == null) return;

            switch (source)
            {
                case string url:
                    SetSourceString(url);
                    break;

                case BitmapImage bitmap:
                    SetImage(bitmap);
                    break;

                case Uri uri:
                    SetSourceUri(uri);
                    break;

                case ImageSource imageSource:
                    SetImage(imageSource);
                    break;

                default:
                    ClearImage();
                    break;
            }
        }

        private void SetSourceString(string url)
        {
            Uri uri = null;
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                SetSourceUri(uri);
            }
            else if (Uri.IsWellFormedUriString(url, UriKind.Relative)
                && Uri.TryCreate("ms-appx:///" + url.TrimStart('/'), UriKind.Absolute, out uri))
            {
                SetSourceUri(uri);
            }
            else
            {
                ClearImage();
            }
        }

        private Uri _currentUri;

        private async void SetSourceUri(Uri uri)
        {
            _currentUri = uri;
            try
            {
                if (uri.IsAbsoluteUri)
                {
                    var cachedUri = uri;
                    if (uri.Scheme == "http" || uri.Scheme == "https")
                    {
                        SetProgress();
                        _isHttpSource = true;
                        if (!DesignMode.DesignModeEnabled)
                        {
                            cachedUri = await BitmapCache.GetImageUriAsync(uri, (int)_currentSize.Width, (int)_currentSize.Height);
                            if (cachedUri == null)
                            {
                                ClearProgress();
                                ClearImage();
                                return;
                            }
                        }
                    }
                    SetImage(new BitmapImage(cachedUri));
                }
                else
                {
                    ClearImage();
                }
            }
            catch
            {
                // Invalid Uri
                ClearImage();
            }
        }

        #endregion SetSource

        private async void RefreshSourceUri(Uri uri)
        {
            try
            {
                if (!DesignMode.DesignModeEnabled)
                {
                    uri = await BitmapCache.GetImageUriAsync(uri, (int)_currentSize.Width, (int)_currentSize.Height);
                }
                if (uri != null)
                {
                    SetImage(new BitmapImage(uri));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("RefreshSourceUri. {0}", ex.Message);
            }
        }

        private static int _progressCount;
        private object _progressCountLock = new object();

        private void SetProgress()
        {
            if (Progress != null)
            {
                return;
            }

            bool available = false;

            lock (_progressCountLock)
            {
                if (_progressCount < 100)
                {
                    _progressCount++;
                    available = true;
                }
            }

            if (available)
            {
                var progress = new ProgressRing
                {
                    IsActive = true
                };
                progress.SetBinding(BackgroundProperty, new Binding { Source = this, Path = new PropertyPath("Background") });
                progress.SetBinding(ForegroundProperty, new Binding { Source = this, Path = new PropertyPath("Foreground") });
                Content = progress;
            }
        }

        private void SetImage(ImageSource imageSource)
        {
            ClearProgress();

            var image = Image;
            if (image == null)
            {
                image = new Image();
                image.SetBinding(Image.StretchProperty, new Binding { Source = this, Path = new PropertyPath("Stretch") });
                image.SetBinding(HorizontalAlignmentProperty, new Binding { Source = this, Path = new PropertyPath("HorizontalAlignment") });
                image.SetBinding(VerticalAlignmentProperty, new Binding { Source = this, Path = new PropertyPath("VerticalAlignment") });
                image.SetBinding(Image.NineGridProperty, new Binding { Source = this, Path = new PropertyPath("NineGrid") });
                Content = image;
            }
            if (imageSource != null)
            {
                if ((_oldImageSource as BitmapImage)?.UriSource != (imageSource as BitmapImage).UriSource)
                {
                    StartAnimation(1, 0, .5).Completed += (sender, e) => { image.Source = imageSource; StartAnimation(0, 1, 1); };
                    if (imageSource != null)
                    {
                        _oldImageSource = imageSource;
                    }
                }
                else
                {
                    image.Source = _oldImageSource;
                }
            }
            else
            {
                image.Source = _oldImageSource;
            }
        }

        private Storyboard StartAnimation(int from, int to, double seconds)
        {
            Storyboard animationBoard = new Storyboard();
            DoubleAnimation opacityAnimation = new DoubleAnimation()
            {
                To = to,
                Duration = TimeSpan.FromSeconds(seconds),
                From = from
            };
            Storyboard.SetTarget(opacityAnimation, Image);
            Storyboard.SetTargetProperty(opacityAnimation, "Opacity");
            animationBoard.Children.Add(opacityAnimation);
            animationBoard.Begin();
            return animationBoard;
        }

        private ImageSource _oldImageSource;

        private void ClearProgress()
        {
            if (Progress != null)
            {
                Progress.IsActive = false;
                Content = null;
                lock (_progressCountLock)
                {
                    _progressCount--;
                }
            }
        }

        private void ClearImage()
        {
            if (Image != null)
            {
                Image.Source = null;
                Content = null;
            }
        }
    }
}