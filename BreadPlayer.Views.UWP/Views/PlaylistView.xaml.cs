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
            _playlistVm.Songs.Clear();
            base.OnNavigatedFrom(e);
        }
        private void fileBox_Loaded(object sender, RoutedEventArgs e)
        {
            fileBox.FindChildOfType<ScrollViewer>().ViewChanging += PlaylistView_ViewChanging;
        }
     
        private void PlaylistView_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            if (e.NextView.VerticalOffset < 15)// > (sender as ScrollViewer).VerticalOffset)
            {
                if(art.Height == 264)
                {
                   art.ZoomAnimate(264, 354, "Height");
                }
                if (headerText.FontSize < _maxFontSize)
                {
                    headerText.FontSize = headerText.FontSize + 2;
                    headerDesc.FontSize++;
                }
            }
            else
            {
                if (art.Height == 354)
                {
                    art.ZoomAnimate(354, 264, "Height");
                }
                if (headerText.FontSize > _minFontSize)
                {
                    headerText.FontSize = headerText.FontSize - 2;
                    headerDesc.FontSize--;
                }
            }
        }
        
    }
}
