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
            //if (isMaximized)
            //{
            //    RemoveLyricsList(lyricsGrid, trackControlPanel, "TextBrush");
            //}
            //else
            //{
            //    RemoveLyricsList(trackControlPanel, lyricsGrid, "ThemeForeground");
            //}
        }
        private void RemoveLyricsList(Grid removeFrom, Grid removeTo, string foregroundColor)
        {
            removeFrom.Children.Remove(lyricsList);
            removeTo.Children.Add(lyricsList);
            ((SolidColorBrush)Resources["LyricsForeground"]).Color = ((SolidColorBrush)Application.Current.Resources[foregroundColor]).Color;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
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
