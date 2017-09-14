using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using BreadPlayer.Core.Models;
using BreadPlayer.ViewModels;
using BreadPlayer.Services;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BreadPlayer.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchResultsView : Page
    {
        Query CurrentQuery;
        public SearchResultsView()
        {
            InitializeComponent();
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            CurrentQuery = e.Parameter as Query;
            SearchResultsViewModel viewModel = new SearchResultsViewModel();
            await viewModel.GetAlbumsAndTracks((e.Parameter as Query).QueryWord);
            DataContext = viewModel;
        }

        private void AlbumsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (Album album in e.RemovedItems)
            {
                album.IsSelected = false;
            }
            foreach (Album album in e.AddedItems)
            {
                album.IsSelected = true;
            }
        }

        private void OnShowAllClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            NavigationService.Instance.Frame.Navigate(typeof(ShowAllView), (CurrentQuery, (sender as Button).Tag.ToString()));
        }
    }
}
