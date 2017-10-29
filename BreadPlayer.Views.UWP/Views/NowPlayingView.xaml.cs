using BreadPlayer.Core;
using BreadPlayer.Extensions;
using BreadPlayer.Helpers;
using BreadPlayer.Services;
using BreadPlayer.ViewModels;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BreadPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NowPlayingView : Page
    {
        private bool _isPressed;
        private ShellViewModel _shellVm;

        public NowPlayingView()
        {
            InitializeComponent();
        }     

        private bool isMaximized = false;

        private void OnMaximizeToFullScreen(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, isMaximized ? "MinimizeState" : "MaximizeState", false);
            isMaximized = !isMaximized;
        }

        private void OnShareSong(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        private void RemoveLyricsList(Grid removeFrom, Grid removeTo, string foregroundColor)
        {
            removeFrom.Children.Remove(lyricsList);
            removeTo.Children.Add(lyricsList);
            ((SolidColorBrush)Resources["LyricsForeground"]).Color = ((SolidColorBrush)Application.Current.Resources[foregroundColor]).Color;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _shellVm = Application.Current.Resources["ShellVM"] as ShellViewModel;

            if (DataTransferManager.IsSupported())
            {
                DataTransferManager manager = DataTransferManager.GetForCurrentView();
                manager.DataRequested += async (s, args) =>
                {
                    var currentlyPlaying = SharedLogic.Instance.Player.CurrentlyPlayingFile;
                    DataRequest dataRequest = args.Request;
                    dataRequest.Data.Properties.Title = $"{currentlyPlaying.Title} by {currentlyPlaying.LeadArtist}";
                    dataRequest.Data.Properties.Description = "Now baking toast from BreadPlayer";

                    if (!string.IsNullOrEmpty(currentlyPlaying.AttachedPicture))
                    {
                        var albumArt = await StorageFile.GetFileFromPathAsync(currentlyPlaying.AttachedPicture);
                        List<IStorageItem> imageItems = new List<IStorageItem>();
                        imageItems.Add(albumArt);
                        dataRequest.Data.SetStorageItems(imageItems);

                        RandomAccessStreamReference imageStreamRef = RandomAccessStreamReference.CreateFromFile(albumArt);
                        dataRequest.Data.Properties.Thumbnail = imageStreamRef;
                        dataRequest.Data.SetBitmap(imageStreamRef);
                    }
                    dataRequest.Data.SetHtmlFormat($"Now listening to {currentlyPlaying.Title} by {currentlyPlaying.LeadArtist}.\r\n\r\nGet BreadPlayer for your device: http://bit.ly/2wIqkHX");
                    dataRequest.Data.SetText($"Now listening to {currentlyPlaying.Title} by {currentlyPlaying.LeadArtist}.\r\n\r\nGet BreadPlayer for your device: http://bit.ly/2wIqkHX");
                    dataRequest.Data.SetWebLink(new Uri("http://bit.ly/2wIqkHX"));
                };
            }
            //events for providing seeking ability to the positon slider.
            Window.Current.CoreWindow.PointerPressed += (s, args) =>
            {
                if (positionSlider.GetBoundingRect().Contains(args.CurrentPoint.Position) && !positionSlider.IsDragging())
                {
                    _isPressed = true;
                    _shellVm.DontUpdatePosition = true;
                }
            };
            Window.Current.CoreWindow.PointerReleased += (s, args) =>
            {
                if (_isPressed && !positionSlider.IsDragging())
                {
                    positionSlider.UpdatePosition(_shellVm, true);
                    _isPressed = false;
                }
            };
            await ((NowPlayingViewModel)Resources["NowPlayingVM"]).Init().ConfigureAwait(false);
        }
    }
}