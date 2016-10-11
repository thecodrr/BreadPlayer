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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macalifa.Models;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Controls;
using System.Windows.Input;
using Macalifa.Services;
using Windows.UI.Xaml;
using Windows.UI.Core;
using Macalifa.Dialogs;

namespace Macalifa.ViewModels
{
    public class PlaylistViewModel : ViewModelBase
    {
        ThreadSafeObservableCollection<Mediafile> songs;
        public ThreadSafeObservableCollection<Mediafile> Songs { get { if (songs == null) { songs = new ThreadSafeObservableCollection<Mediafile>(); } return songs; } set { Set(ref songs, value); } }
        Playlist playlist;
        public Playlist Playlist { get { return playlist; } set { Set(ref playlist, value); } }
        string totalSongs;
        public string TotalSongs
        {
            get
            {
                totalSongs = Songs.Count.ToString() + " Songs";
                return totalSongs;
            }
            set
            {
                totalSongs = "0";
                Set(ref totalSongs, value);
            }
        }
        string totalMinutes;
        public string TotalMinutes
        {
            get
            {
                totalMinutes = Songs.Sum(t => TimeSpan.FromSeconds(Convert.ToDouble(t.Length)).Minutes) + " Minutes";
                return totalMinutes;
            }
            set
            {
                totalMinutes = "0";
                Set(ref totalMinutes, value);
            }
        }

        ImageSource playlistArt;
        public ImageSource PlaylistArt
        {
            get
            {
                if (Songs.Any() && Songs.Any(s => !string.IsNullOrEmpty(s.AttachedPicture)))
                {
                    BitmapImage image = new BitmapImage(new Uri(Songs.FirstOrDefault(s => !string.IsNullOrEmpty(s.AttachedPicture)).AttachedPicture, UriKind.RelativeOrAbsolute));
                    playlistArt = image;
                    return playlistArt;
                }
                return null;                            
            }
            set {
                if (Songs.Any())
                    playlistArt = null;
                Set(ref playlistArt, value);
            }
        }
        RelayCommand _deleteCommand;
        /// <summary>
        /// Gets Play command. This calls the <see cref="Delete(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand DeleteCommand
        {
            get
            { if (_deleteCommand == null) { _deleteCommand = new RelayCommand(param => this.Delete(param)); } return _deleteCommand; }
        }
        void Delete(object para)
        {
            var ps = Playlist.Name;
            var mediafile = para as Mediafile;
            mediafile.Playlists.Remove(mediafile.Playlists.Single(t => t.Name == Playlist.Name));
            Songs.Remove(mediafile);
            LibVM.db.Update(mediafile);
            Refresh();
        }
        void Refresh()
        {
            //refreshes the values by getting and setting the same properties.
            PlaylistArt = Songs.Any() ? PlaylistArt : null;
            TotalSongs = TotalSongs;
            TotalMinutes = TotalMinutes;
        }
        RelayCommand _renamePlaylistCommand;
        /// <summary>
        /// Gets command for initialization. This calls the <see cref="Init(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand RenamePlaylistCommand
        {
            get
            { if (_renamePlaylistCommand == null) { _renamePlaylistCommand = new RelayCommand(param => this.RenamePlaylist(param)); } return _renamePlaylistCommand; }
        }
        async void RenamePlaylist(object playlist)
        {
            var selectedPlaylist = (playlist as Dictionary<Playlist, IEnumerable<Mediafile>>).First().Key;
            var songs = (playlist as Dictionary<Playlist, IEnumerable<Mediafile>>).First().Value;
            var dialog = new InputDialog()
            {
                Title = "Rename this playlist",
                Text = selectedPlaylist.Name,
                Description = selectedPlaylist.Description
            };
            var Playlists = new Dictionary<Playlist, IEnumerable<Mediafile>>();
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                var pl = new Playlist() { Name = dialog.Text, Description = dialog.Description };

                if (songs.Count() > 0)
                {
                    foreach (var file in songs)
                    {
                        file.Playlists.First(t => t.Name == selectedPlaylist.Name).Name = pl.Name;
                        file.Playlists.First(t => t.Name == pl.Name).Description = pl.Description;
                        LibVM.db.Update(file);
                    }
                }
                ShellVM.PlaylistsItems.First(t => t.Label == selectedPlaylist.Name).Label = pl.Name;
                LibVM.Options.First(t => t.Text == selectedPlaylist.Name).Text = pl.Name;
                LibVM.db.playlists.FindOne(t => t.Name == selectedPlaylist.Name);
                Playlists.Add(pl, Core.CoreMethods.LibVM.TracksCollection.Elements.Where(a => a.Playlists.All(t => t.Name == pl.Name) && a.Playlists.Count == 1));
                ShellVM.PlaylistsItems.First(t => t.Label == pl.Name).Arguments = Playlists;
                Playlist = pl;
            }
        }
        public PlaylistViewModel()
        {
        }

    }
}
