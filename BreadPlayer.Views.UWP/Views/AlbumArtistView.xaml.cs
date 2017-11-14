using BreadPlayer.Core.Common;
using BreadPlayer.Interfaces;
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
            this.Loaded += OnPageLoaded;
            CoreWindow.GetForCurrentThread().SizeChanged += OnWindowSizeChanged;
        }

        private async void OnPageLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                if (_currentState == "AlbumView")
                {
                    albumListView.ItemsSource = null;
                    albumListView.ItemsSource = (grid.DataContext as AlbumArtistViewModel).AlbumCollection;
                    await (grid.DataContext as AlbumArtistViewModel).AlbumCollection.RefreshAsync();
                    (grid.DataContext as AlbumArtistViewModel).LoadAlbums();
                }
                else if (_currentState == "ArtistView")
                {
                    albumListView.ItemsSource = null;
                    albumListView.ItemsSource = (grid.DataContext as AlbumArtistViewModel).ArtistsCollection;
                    await (grid.DataContext as AlbumArtistViewModel).ArtistsCollection.RefreshAsync();
                    (grid.DataContext as AlbumArtistViewModel).LoadArtists();
                }
                SetTemplate();
            });
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

        protected  override void OnNavigatedTo(NavigationEventArgs e)
        {
            _currentState = e.Parameter.ToString();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            this.Loaded -= OnPageLoaded;
            CoreWindow.GetForCurrentThread().SizeChanged -= OnWindowSizeChanged;
            (grid.DataContext as AlbumArtistViewModel).ArtistsCollection = null;
            (grid.DataContext as AlbumArtistViewModel).AlbumCollection = null;
            albumListView.ItemsSource = null;
            GC.Collect();
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