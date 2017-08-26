using BreadPlayer.Core;
using BreadPlayer.Core.Models;
using BreadPlayer.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
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
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var parameter = (ValueTuple<Query, string>)e.Parameter;
            var documentStore = new DocumentStoreDatabaseService(SharedLogic.DatabasePath, "Tracks");
            switch (parameter.Item2)
            {
                case "Toasts":
                    LibraryService libraryService = new LibraryService(documentStore);
                    libraryService.Query(parameter.Item1.QueryWord);
                    break;
                case "Bakers":
                    AlbumArtistService artistService = new AlbumArtistService(documentStore);
                    artistService.QueryArtistsAsync(parameter.Item1.QueryWord);
                    break;
                case "Breads":
                    AlbumArtistService albumService = new AlbumArtistService(documentStore);
                    albumService.QueryAlbumsAsync(parameter.Item1.QueryWord);
                    break;
            }
        }
    }
}
