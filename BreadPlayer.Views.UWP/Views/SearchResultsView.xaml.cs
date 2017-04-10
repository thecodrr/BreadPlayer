using BreadPlayer.Models;
using BreadPlayer.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BreadPlayer.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchResultsView : Page
    {
        public SearchResultsView()
        {
            this.InitializeComponent();
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            SearchResultsViewModel ViewModel = new SearchResultsViewModel();
            await ViewModel.GetAlbumsAndTracks((e.Parameter as Query).QueryWord);
            this.DataContext = ViewModel;
        }
    }
}
