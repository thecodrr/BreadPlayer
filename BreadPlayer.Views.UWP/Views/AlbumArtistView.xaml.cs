using System;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;
using BreadPlayer.ViewModels;
using BreadPlayer.Core.Models;
using BreadPlayer.Core.Common;

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
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => 
            {
                if (e.Parameter.ToString() == "AlbumView")
                {
                    albumListView.ItemTemplate = App.Current.Resources["AlbumTemplate"] as Windows.UI.Xaml.DataTemplate;
                    albumListView.ItemsSource = (grid.DataContext as AlbumArtistViewModel).AlbumCollection;
                    (grid.DataContext as AlbumArtistViewModel).LoadAlbums().ConfigureAwait(false);
                }
                else if (e.Parameter.ToString() == "ArtistView")
                {
                    albumListView.ItemTemplate = App.Current.Resources["ArtistTemplate"] as Windows.UI.Xaml.DataTemplate;
                    albumListView.ItemsSource = (grid.DataContext as AlbumArtistViewModel).ArtistsCollection;
                    (grid.DataContext as AlbumArtistViewModel).LoadArtists().ConfigureAwait(false);
                }
            });
            base.OnNavigatedTo(e);
        }
        
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //(grid.Resources["Source"] as CollectionViewSource).Source = null;
            //(grid.DataContext as AlbumArtistViewModel).AlbumCollection = null;
            base.OnNavigatedFrom(e);
        }

        private void albumListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (ISelectable record in e.RemovedItems)
            {
                record.IsSelected = false;
            }
            foreach (ISelectable record in e.AddedItems)
            {
                record.IsSelected = true;
            }
        }
    }
}
