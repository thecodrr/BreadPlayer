using BreadPlayer.Core;
using BreadPlayer.Core.Models;
using BreadPlayer.Database;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BreadPlayer.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShowAllView : Page
    {
        public ShowAllView()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var parameter = (ValueTuple<Query, string>)e.Parameter;
            var documentStore = new DocumentStoreDatabaseService(SharedLogic.DatabasePath, "Tracks");
            switch (parameter.Item2)
            {
                case "Toasts":
                    LibraryService libraryService = new LibraryService(documentStore);
                    searchResultsList.ItemsSource = await libraryService.Query(parameter.Item1.QueryWord);
                    break;

                case "Bakers":
                    searchResultsList.ItemsPanel = this.Resources["BreadsBakersPanel"] as ItemsPanelTemplate;
                    searchResultsList.ItemTemplate = App.Current.Resources["ArtistTemplate"] as DataTemplate;
                    AlbumArtistService artistService = new AlbumArtistService(documentStore);
                    searchResultsList.ItemsSource = await artistService.QueryArtistsAsync(parameter.Item1.QueryWord);
                    break;

                case "Breads":
                    searchResultsList.ItemsPanel = this.Resources["BreadsBakersPanel"] as ItemsPanelTemplate;
                    searchResultsList.ItemTemplate = App.Current.Resources["AlbumTemplate"] as DataTemplate;
                    AlbumArtistService albumService = new AlbumArtistService(documentStore);
                    searchResultsList.ItemsSource = await albumService.QueryAlbumsAsync(parameter.Item1.QueryWord);
                    break;
            }
        }
    }
}