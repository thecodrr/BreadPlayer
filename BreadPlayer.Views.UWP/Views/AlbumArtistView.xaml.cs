﻿using BreadPlayer.ViewModels;
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
            this.InitializeComponent();
            grid.DataContext = new AlbumArtistViewModel();
       }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => 
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
    }
}
