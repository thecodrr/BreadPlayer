using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BreadPlayer.Core;
using BreadPlayer.Extensions;
using BreadPlayer.Helpers;
using BreadPlayer.ViewModels;
using Windows.UI.Text;
using System;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using BreadPlayer.Services;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using System.Collections.Generic;
using Windows.Storage.Streams;
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

            (Resources["NowPlayingVM"] as NowPlayingViewModel).LyricActivated += NowPlayingView_LyricActivated;

            _shellVm = Application.Current.Resources["ShellVM"] as ShellViewModel;

            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += (s, args) => 
                {
                    args.Handled = true;
                    NavigationService.Instance.RegisterEvents();
                    _shellVm.IsPlaybarHidden = false;
                };
            }
        }

        private async void NowPlayingView_LyricActivated(object sender, EventArgs e)
        {
            try
            {
                await lyricsList.ScrollToItem(sender);
            }
            catch { }
        }
        bool isMaximized = false;
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
        public static Uri GetApplicationLink(string sharePageName)
        {
            return new Uri("ms-sdk-sharesourcecs:navigate?page=" + sharePageName);
        }
    private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataTransferManager.IsSupported())
            {
                DataTransferManager manager = DataTransferManager.GetForCurrentView();
                manager.DataRequested += async (s, args) =>
                {
                    var currentlyPlaying = SharedLogic.Player.CurrentlyPlayingFile;
                    DataRequest dataRequest = args.Request;
                    dataRequest.Data.Properties.Title = $"{currentlyPlaying.Title} by {currentlyPlaying.LeadArtist}";
                    dataRequest.Data.Properties.Description = "Now Playing toast from Bread Player";
                    dataRequest.Data.SetApplicationLink(GetApplicationLink(GetType().Name));
                    dataRequest.Data.Properties.ContentSourceApplicationLink = GetApplicationLink(GetType().Name);

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
        }
       
    }
}
