using System;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;
using BreadPlayer.ViewModels;
using BreadPlayer.Core.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BreadPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AlbumArtistView : Page
    {
        public AlbumArtistView()
        {
            InitializeComponent();
            grid.DataContext = new AlbumArtistViewModel();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => 
            {
                (grid.Resources["Source"] as CollectionViewSource).Source = (grid.DataContext as AlbumArtistViewModel).AlbumCollection;
                (grid.DataContext as AlbumArtistViewModel).LoadAlbums().ConfigureAwait(false);
            });
            base.OnNavigatedTo(e);
        }
        
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            (grid.Resources["Source"] as CollectionViewSource).Source = null;
            (grid.DataContext as AlbumArtistViewModel).AlbumCollection = null;
            base.OnNavigatedFrom(e);
        }

        private void albumListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach(Album album in e.RemovedItems)
            {
                album.IsSelected = false;
            }
            foreach(Album album in e.AddedItems)
            {
                album.IsSelected = true;
            }
        }
    }
}
