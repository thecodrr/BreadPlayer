using BreadPlayer.Core.Common;
using BreadPlayer.ViewModels;
using System;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BreadPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AlbumArtistView : Page
    {
        private string _currentState = "";

        public AlbumArtistView()
        {
            InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            CoreWindow.GetForCurrentThread().SizeChanged += OnWindowSizeChanged;
        }

        private void OnWindowSizeChanged(CoreWindow sender, WindowSizeChangedEventArgs args)
        {
            SetTemplate();
        }

        private void SetTemplate()
        {
            var albumWideTemplate = App.Current.Resources["AlbumTemplate"] as Windows.UI.Xaml.DataTemplate;
            var artistWideTemplate = App.Current.Resources["ArtistTemplate"] as Windows.UI.Xaml.DataTemplate;
            var albumMobileTemplate = App.Current.Resources["AlbumMobileTemplate"] as Windows.UI.Xaml.DataTemplate;
            var artistMobileTemplate = App.Current.Resources["ArtistMobileTemplate"] as Windows.UI.Xaml.DataTemplate;

            if (CoreWindow.GetForCurrentThread().Bounds.Width >= 800)
            {
                if (_currentState == "AlbumView" && albumListView.ItemTemplate != albumWideTemplate)
                {
                    albumListView.ItemTemplate = albumWideTemplate;
                }
                else if (_currentState == "ArtistView" && albumListView.ItemTemplate != artistWideTemplate)
                {
                    albumListView.ItemTemplate = artistWideTemplate;
                }
            }
            else
            {
                if (_currentState == "AlbumView" && albumListView.ItemTemplate != albumMobileTemplate)
                {
                    albumListView.ItemTemplate = albumMobileTemplate;
                }
                else if (_currentState == "ArtistView" && albumListView.ItemTemplate != artistMobileTemplate)
                {
                    albumListView.ItemTemplate = artistMobileTemplate;
                }
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            _currentState = e.Parameter.ToString();
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                SetTemplate();
                if (e.Parameter.ToString() == "AlbumView")
                {
                    albumListView.ItemsSource = (grid.DataContext as AlbumArtistViewModel).AlbumCollection;
                    (grid.DataContext as AlbumArtistViewModel).LoadAlbums().ConfigureAwait(false);
                }
                else if (e.Parameter.ToString() == "ArtistView")
                {
                    albumListView.ItemsSource = (grid.DataContext as AlbumArtistViewModel).ArtistsCollection;
                    (grid.DataContext as AlbumArtistViewModel).LoadArtists().ConfigureAwait(false);
                }
                else if (e.Parameter.ToString() == "Clear")
                {
                    albumListView.ItemsSource = null;
                    albumListView.ItemsSource = (grid.DataContext as AlbumArtistViewModel).ArtistsCollection;
                    GC.Collect();
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