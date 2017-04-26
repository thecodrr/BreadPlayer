using System;
using System.IO;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media.Animation;

namespace BreadPlayer.Controls
{
    partial class ImageEx
    {
        private bool _isHttpSource = false;

        #region Source
        public object Source
        {
            get { return (object)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        private static void SourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ImageEx;
            control.SetSource(e.NewValue);            
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(object), typeof(ImageEx), new PropertyMetadata(null, SourceChanged));
        #endregion

        #region SetSource
        private void SetSource(object source)
        {
            _isHttpSource = false;
            if (source != null)
            {
                string url = source as String;
                if (url != null)
                {
                    SetSourceString(url);
                }
                else if(source is BitmapImage bitmap)
                {
                    SetImage(bitmap);
                }
                else
                {
                    Uri uri = source as Uri;
                    if (uri != null)
                    {
                        SetSourceUri(uri);
                    }
                    else
                    {
                        ImageSource imageSource = source as ImageSource;
                        if (imageSource != null)
                        {
                            SetImage(imageSource);
                        }
                        else
                        {
                            ClearImage();
                        }
                    }
                }
            }           
        }

        private void SetSourceString(string url)
        {
            Uri uri = null;
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                SetSourceUri(uri);
            }
            else if (Uri.IsWellFormedUriString(url, UriKind.Relative))
            {
                if (Uri.TryCreate("ms-appx:///" + url.TrimStart('/'), UriKind.Absolute, out uri))
                {
                    SetSourceUri(uri);
                }
                else
                {
                    ClearImage();
                }
            }
            else
            {
                ClearImage();
            }
        }

        private Uri _currentUri = null;

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
                        if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
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
                    this.SetImage(new BitmapImage(cachedUri));
                    
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
        #endregion

        private async void RefreshSourceUri(Uri uri)
        {
            try
            {
                if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                {
                    uri = await BitmapCache.GetImageUriAsync(uri, (int)_currentSize.Width, (int)_currentSize.Height);
                }
                if (uri != null)
                {
                    this.SetImage(new BitmapImage(uri));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("RefreshSourceUri. {0}", ex.Message);
            }
        }

        private static int _progressCount = 0;
        private object _progressCountLock = new object();

        private void SetProgress()
        {
            if (this.Progress != null)
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
                progress.SetBinding(ProgressRing.BackgroundProperty, new Binding { Source = this, Path = new PropertyPath("Background") });
                progress.SetBinding(ProgressRing.ForegroundProperty, new Binding { Source = this, Path = new PropertyPath("Foreground") });
                this.Content = progress;
            }
        }

        private void SetImage(ImageSource imageSource)
        {
            ClearProgress();

            var image = this.Image;
            if (image == null)
            {
                image = new Image();
                image.SetBinding(Image.StretchProperty, new Binding { Source = this, Path = new PropertyPath("Stretch") });
                image.SetBinding(Image.HorizontalAlignmentProperty, new Binding { Source = this, Path = new PropertyPath("HorizontalAlignment") });
                image.SetBinding(Image.VerticalAlignmentProperty, new Binding { Source = this, Path = new PropertyPath("VerticalAlignment") });
                image.SetBinding(Image.NineGridProperty, new Binding { Source = this, Path = new PropertyPath("NineGrid") });
                this.Content = image;
            }
            if (imageSource != null)
            {
                if ((oldImageSource as BitmapImage)?.UriSource != (imageSource as BitmapImage).UriSource)
                {
                    StartAnimation(1, 0, 1.5).Completed += (sender, e) => { image.Source = imageSource; StartAnimation(0, 1, 3); };
                    if (imageSource != null)
                        oldImageSource = imageSource;
                }
                else
                    image.Source = oldImageSource;
            }
            else
                image.Source = oldImageSource;
        }
        private Storyboard StartAnimation(int from, int to, double seconds)
        {
            Storyboard animationBoard = new Storyboard();
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.To = to;
            opacityAnimation.Duration = TimeSpan.FromSeconds(seconds);
            opacityAnimation.From = from;
            Storyboard.SetTarget(opacityAnimation, this.Image);
            Storyboard.SetTargetProperty(opacityAnimation, "Opacity");
            animationBoard.Children.Add(opacityAnimation);
            animationBoard.Begin();
            return animationBoard;
        }
        ImageSource oldImageSource;
        private void ClearProgress()
        {
            if (this.Progress != null)
            {
                this.Progress.IsActive = false;
                this.Content = null;
                lock (_progressCountLock)
                {
                    _progressCount--;
                }
            }
        }

        private void ClearImage()
        {
            if (this.Image != null)
            {
                this.Image.Source = null;
                this.Content = null;
            }
        }      
    }
}
