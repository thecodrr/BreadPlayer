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

using BreadPlayer.Dispatcher;
using BreadPlayer.ViewModels;
using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BreadPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlaylistView
    {
        public PlaylistView()
        {
            InitializeComponent();
            this.Loaded += OnPageLoaded;
        }

        private async void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            if (parameter != null)
            {
                await BreadDispatcher.InvokeAsync(async() =>
                {
                    await _playlistVm.Init(parameter).ConfigureAwait(false);
                });
            }
        }

        private PlaylistViewModel _playlistVm;
        private object parameter;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            parameter = e.Parameter;
            _playlistVm = Application.Current.Resources["PlaylistVM"] as PlaylistViewModel;
            DataContext = _playlistVm;
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            _playlistVm.Reset();
            _playlistVm = null;
            this.Loaded -= OnPageLoaded;
            //fileBox.ItemsSource = null;
        }        
    }
}