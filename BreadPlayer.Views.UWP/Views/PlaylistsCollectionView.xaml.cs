using BreadPlayer.Controls;
using BreadPlayer.Messengers;
using BreadPlayer.Services;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BreadPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlaylistsCollectionView : Page
    {
        public PlaylistsCollectionView()
        {
            this.InitializeComponent();
        }

        private void OnPlaylistClicked(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem != null)
            {
                SplitViewMenu.UnSelectAll();
                Messenger.Instance.NotifyColleagues(MessageTypes.MsgNavigate, new { pageType = typeof(PlaylistView), parameter = e.ClickedItem });
            }
        }
    }
}