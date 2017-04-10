using BreadPlayer.ViewModels;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BreadPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NowPlayingView : Page
    {
        public NowPlayingView()
        {
            this.InitializeComponent();
            (App.Current.Resources["ShellVM"] as ShellViewModel).IsPlaybarHidden = true;
        }
    }
}
