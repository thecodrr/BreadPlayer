/* 
	Macalifa. A music player made for Windows 10 store.
    Copyright (C) 2016  theweavrs (Abdullah Atta)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
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
