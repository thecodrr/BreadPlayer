using Macalifa.Models;
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
using Macalifa.ViewModels;
namespace Macalifa
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Albums
    {
        public ObservableRangeCollection<Mediafile> Playlist = new ObservableRangeCollection<Mediafile>();
        PlaylistViewModel vm;
        public Albums()
        {
            this.InitializeComponent();
                     
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            a.Text = "asjkdasjk ";
            base.OnNavigatingFrom(e);
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
          
            vm = new PlaylistViewModel(Playlist);          
            var list =  (e.Parameter as IEnumerable<Mediafile>).ToList();
            list.ForEach(Playlist.Add);
            this.DataContext = vm;
            playListBox.ItemsSource = vm.Playlist;
            playListBox.DataContext = vm.Playlist;
            base.OnNavigatedTo(e);
        }

        private void playListBox_Loaded(object sender, RoutedEventArgs e)
        {
          
        }
    }
}
