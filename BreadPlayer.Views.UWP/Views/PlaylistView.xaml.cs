/* 
	BreadPlayer. A music player made for Windows 10 store.
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

using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using BreadPlayer.Extensions;
using BreadPlayer.ViewModels;
using System;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BreadPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlaylistView
    {
        private double _maxFontSize;
        private double _minFontSize;
        public PlaylistView()
        {
            InitializeComponent();
            Window.Current.SizeChanged += Current_SizeChanged;
            _maxFontSize = Window.Current.Bounds.Width < 600 ? 34 : 60;
            _minFontSize = Window.Current.Bounds.Width < 600 ? 24 : 50;
        }

        private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            _maxFontSize = Window.Current.Bounds.Width < 600 ? 34 : 60;
            _minFontSize = Window.Current.Bounds.Width < 600 ? 24 : 50;
        }

        private PlaylistViewModel _playlistVm;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _playlistVm = Application.Current.Resources["PlaylistVM"] as PlaylistViewModel;
            _playlistVm.Init(e.Parameter);
            DataContext = _playlistVm;
            base.OnNavigatedTo(e);
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Window.Current.SizeChanged -= Current_SizeChanged;
            _playlistVm.Reset();
            _playlistVm = null;
            fileBox.ItemsSource = null;
            base.OnNavigatedFrom(e);
        }     
    }
}
