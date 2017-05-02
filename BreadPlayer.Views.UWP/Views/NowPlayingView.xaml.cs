using BreadPlayer.Extensions;
using BreadPlayer.Helpers;
using BreadPlayer.ViewModels;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BreadPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NowPlayingView : Page
    {
        bool isPressed;
        ShellViewModel ShellVM;
        public NowPlayingView()
        {
            this.InitializeComponent();
            ShellVM = (extrasPanel.DataContext as ShellViewModel);

            //events for providing seeking ability to the positon slider.
            Window.Current.CoreWindow.PointerPressed += (sender, e) =>
            {
                if (positionSlider.GetBoundingRect().Contains(e.CurrentPoint.Position) && !positionSlider.IsDragging())
                {
                    isPressed = true;
                    ShellVM.DontUpdatePosition = true;
                }
            };
            Window.Current.CoreWindow.PointerReleased += (sender, e) => 
            {
                if (isPressed && !positionSlider.IsDragging())
                {
                    positionSlider.UpdatePosition(null, ShellVM, true);
                    isPressed = false;
                }
            };
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            SplitViewMenu.SplitViewMenu.SelectHome();
            ApplicationView.GetForCurrentView().ExitFullScreenMode();
            ShellVM.IsPlaybarHidden = false;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //initialize tap to seek ability in positionSlider.
            positionSlider.InitEvents(() => { positionSlider.UpdatePosition(null, ShellVM); }, () => { ShellVM.DontUpdatePosition = true; });
            Window.Current.SizeChanged += (evnt, args) =>
            {
                if (InitializeCore.IsMobile && NowPlayingGrid.Children.Contains(NowPlayingList))
                {
                    NowPlayingGrid.Children.Remove(NowPlayingList);
                    RootGrid.Children.Insert(RootGrid.Children.Count - 2, NowPlayingList);
                }
                else if (!InitializeCore.IsMobile && !NowPlayingGrid.Children.Contains(NowPlayingList))
                {
                    RootGrid.Children.Remove(NowPlayingList);
                    NowPlayingGrid.Children.Add(NowPlayingList);
                }
            };
        }

        private void ShowNowPlayingListBtn_Click(object sender, RoutedEventArgs e)
        {
            if (InitializeCore.IsMobile && NowPlayingGrid.Children.Contains(NowPlayingList))
            {
                NowPlayingGrid.Children.Remove(NowPlayingList);
                RootGrid.Children.Insert(RootGrid.Children.Count - 2, NowPlayingList);
            }            
        }
    }
}
