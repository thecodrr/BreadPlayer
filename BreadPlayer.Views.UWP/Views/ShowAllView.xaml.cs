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
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            Window.Current.SizeChanged += Current_SizeChanged;
        }
        string _recordType = "";
        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            UpdateTemplate(e.Size.Width);
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
                    if (searchResultsList.ItemTemplate != toastTemplate)
                    {
                        searchResultsList.ItemTemplate = toastTemplate as DataTemplate;
                        searchResultsList.ItemContainerStyle = Resources["ToastItemStyle"] as Style;
                    }
                    break;
                case "Breads":
                    if (searchResultsList.ItemTemplate != albumTemplate)
                    {
                        searchResultsList.ItemTemplate = albumTemplate as DataTemplate;
                    }
                    break;
                case "Bakers":
                    if (searchResultsList.ItemTemplate != artistTemplate)
                    {
                        searchResultsList.ItemTemplate = artistTemplate as DataTemplate;
                    }
                    break;
            }
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var parameter = (ValueTuple<Query, string>)e.Parameter;
            _recordType = parameter.Item2;
            switch (parameter.Item2)
            {
                case "Toasts":
                    var documentStore = new KeyValueStoreDatabaseService(SharedLogic.Instance.DatabasePath, "Tracks");
                    LibraryService libraryService = new LibraryService(documentStore);
                    var mediaFiles = await libraryService.Query(parameter.Item1.QueryWord);
                    if(mediaFiles != null)
                        searchResultsList.ItemsSource = mediaFiles;
                    break;
                case "Bakers":
                    searchResultsList.ItemsPanel = this.Resources["BreadsBakersPanel"] as ItemsPanelTemplate;
                    var artists = await SharedLogic.Instance.AlbumArtistService.QueryArtistsAsync(parameter.Item1.QueryWord);
                    if(artists != null)
                        searchResultsList.ItemsSource = artists;
                    break;

                case "Breads":
                    searchResultsList.ItemsPanel = this.Resources["BreadsBakersPanel"] as ItemsPanelTemplate;
                    var albums = await SharedLogic.Instance.AlbumArtistService.QueryAlbumsAsync(parameter.Item1.QueryWord);
                    if(albums != null)
                        searchResultsList.ItemsSource = albums; 
                    break;
            }
            UpdateTemplate(Window.Current.Bounds.Width);
        }
    }
}