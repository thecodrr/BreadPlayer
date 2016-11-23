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
using BreadPlayer.Models;
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
using BreadPlayer.ViewModels;
using System.Diagnostics;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;

namespace BreadPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlaylistView
    {        
        PlaylistViewModel PlaylistVM => Core.SharedLogic.PlaylistVM;
        public PlaylistView()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PlaylistVM.Dispose();
            PlaylistVM.Songs = null;
            if (e.Parameter is Playlist)
            {
                var playlist = (e.Parameter as Playlist);
                PlaylistVM.Playlist = playlist;
                LoadDB();
            }
            else
            {
                PlaylistVM.Playlist = new Playlist() { Name = (e.Parameter as Album).AlbumName };
                LoadAlbumSongs(e.Parameter as Album);
                PlaylistVM.Refresh();
            }
                
            this.DataContext = PlaylistVM;
            base.OnNavigatedTo(e);
        }
        async void LoadDB()
        {
            using (Service.PlaylistService service = new Service.PlaylistService(PlaylistVM.Playlist.Name))
            {
                if (service.IsValid)
                {
                    var ss = service.GetTrackCount();
                    PlaylistVM.Songs.AddRange(await service.GetTracks().ConfigureAwait(false), true, false);
                    PlaylistVM.Refresh();
                }
            }
        }
        void LoadAlbumSongs(Album album)
        {
            PlaylistVM.Songs.AddRange(album.AlbumSongs, true, false);
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            GC.Collect();
            base.OnNavigatingFrom(e);
        }

    }
}
