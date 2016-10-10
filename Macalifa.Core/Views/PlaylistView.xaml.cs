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
using System.Diagnostics;
using Windows.UI.Xaml.Media.Imaging;

namespace Macalifa
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlaylistView
    {
        public ThreadSafeObservableCollection<Mediafile> Playlist = new ThreadSafeObservableCollection<Mediafile>();
        
        PlaylistViewModel PlaylistVM => Core.CoreMethods.PlaylistVM;
        public PlaylistView()
        {
            this.InitializeComponent();
                     
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {      
            var list = (e.Parameter as Dictionary<Playlist, IEnumerable<Mediafile>>);
            Debug.Write(list.Count);
            list.First().Value.ToList().ForEach(Playlist.Add);
            PlaylistVM.Songs = Playlist;
            PlaylistVM.Playlist = list.First().Key;
            this.DataContext = PlaylistVM;
            playListBox.ItemsSource = PlaylistVM.Songs;
            playListBox.DataContext = PlaylistVM.Songs;

            base.OnNavigatedTo(e);
        }

    }
}
