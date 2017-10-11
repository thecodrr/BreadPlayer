using BreadPlayer.Core;
using BreadPlayer.Core.Models;
using BreadPlayer.Database;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
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
            Window.Current.SizeChanged += Current_SizeChanged;
        }
        string _recordType = "";
        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            //UpdateTemplate(e.Size.Width);
        }
        private void UpdateTemplate(double width)
        {
            if (width > 600)
            {
                SetTemplate(App.Current.Resources["ArtistTemplate"], App.Current.Resources["AlbumTemplate"], App.Current.Resources["MediafileTemplate"]);
            }
            else
            {
                SetTemplate(App.Current.Resources["ArtistMobileTemplate"], App.Current.Resources["AlbumMobileTemplate"], App.Current.Resources["MediafileMobileTemplate"]);
            }
        }
        private void SetTemplate(object artistTemplate, object albumTemplate, object toastTemplate)
        {
            switch (_recordType)
            {
                case "Toasts":
                    searchResultsList.ItemTemplate = toastTemplate as DataTemplate;
                    searchResultsList.ItemContainerStyle = Resources["ToastItemStyle"] as Style;
                    break;
                case "Breads":
                    searchResultsList.ItemTemplate = albumTemplate as DataTemplate;
                    break;
                case "Bakers":
                    searchResultsList.ItemTemplate = artistTemplate as DataTemplate;
                    break;
            }
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var parameter = (ValueTuple<Query, string>)e.Parameter;
            var documentStore = new DocumentStoreDatabaseService(SharedLogic.Instance.DatabasePath, "Tracks");
            _recordType = parameter.Item2;
            switch (parameter.Item2)
            {
                case "Toasts":
                    LibraryService libraryService = new LibraryService(documentStore);
                    searchResultsList.ItemsSource = await libraryService.Query(parameter.Item1.QueryWord);
                    break;

                case "Bakers":
                    searchResultsList.ItemsPanel = this.Resources["BreadsBakersPanel"] as ItemsPanelTemplate;
                    AlbumArtistService artistService = new AlbumArtistService(documentStore);
                    searchResultsList.ItemsSource = await artistService.QueryArtistsAsync(parameter.Item1.QueryWord);
                    break;

                case "Breads":
                    searchResultsList.ItemsPanel = this.Resources["BreadsBakersPanel"] as ItemsPanelTemplate;
                    AlbumArtistService albumService = new AlbumArtistService(documentStore);
                    searchResultsList.ItemsSource = await albumService.QueryAlbumsAsync(parameter.Item1.QueryWord);
                    break;
            }
            UpdateTemplate(Window.Current.Bounds.Width);
        }
    }
}