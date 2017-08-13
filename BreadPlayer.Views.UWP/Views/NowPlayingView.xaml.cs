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

            //events for providing seeking ability to the positon slider.
            Window.Current.CoreWindow.PointerPressed += (sender, e) =>
            {
                if (positionSlider.GetBoundingRect().Contains(e.CurrentPoint.Position) && !positionSlider.IsDragging())
                {
                    _isPressed = true;
                    _shellVm.DontUpdatePosition = true;
                }
            };
            Window.Current.CoreWindow.PointerReleased += (sender, e) =>
            {
                if (_isPressed && !positionSlider.IsDragging())
                {
                    positionSlider.UpdatePosition(null, _shellVm, true);
                    _isPressed = false;
                }
            };
        }

        private async void NowPlayingView_LyricActivated(object sender, EventArgs e)
        {
            try
            {
                await lyricsList.ScrollToItem(sender);
            }
            catch { }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SplitViewMenu.SplitViewMenu.SelectPrevious();
            //we don't want to exit fullscreen mode in mobile phones
            if(!InitializeCore.IsMobile)
            {
                ApplicationView.GetForCurrentView().ExitFullScreenMode();
            }
            _shellVm.IsPlaybarHidden = false;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
          
        }
       
    }
}
