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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadPlayer.Models;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Controls;
using System.Windows.Input;
using BreadPlayer.Services;
using Windows.UI.Xaml;
using Windows.UI.Core;
using BreadPlayer.Dialogs;
using BreadPlayer.Extensions;
using Windows.UI.Popups;
using System.Globalization;
using BreadPlayer.Core;

namespace BreadPlayer.ViewModels
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
                totalMinutes = string.Format("{0:0.0}", Math.Truncate(Songs.Sum(t => TimeSpan.ParseExact(t.Length, "mm\\:ss", CultureInfo.InvariantCulture).TotalMinutes) * 10) / 10)  + " Minutes";
                return totalMinutes;
            }
            set
            {
                totalMinutes = "0";
                Set(ref totalMinutes, value);
            }
        }
        bool isMenuVisible = true;
       public bool IsMenuVisible
        {
            get { return isMenuVisible; }
            set { Set(ref isMenuVisible, value); }
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
            var mediafile = para as Mediafile;
            if (mediafile == null)
                mediafile = LibVM.TracksCollection.Elements.First(t => t.Path == Player.CurrentlyPlayingFile.Path);
            var pName = Playlist == null ? (para as MenuFlyoutItem).Text : Playlist.Name;
            mediafile.Playlists.Remove(mediafile.Playlists.Single(t => t.Name == pName));
            Songs.Remove(mediafile);
            LibVM.Database.Update(mediafile);
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
        /// Gets command for playlist rename. This calls the <see cref="RenamePlaylist(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand RenamePlaylistCommand
        {
            get
            { if (_renamePlaylistCommand == null) { _renamePlaylistCommand = new RelayCommand(param => this.RenamePlaylist(param)); } return _renamePlaylistCommand; }
        }

        RelayCommand _deletePlaylistCommand;
        /// <summary>
        /// Gets command for playlist delete. This calls the <see cref="DeletePlaylist(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand DeletePlaylistCommand
        {
            get
            { if (_deletePlaylistCommand == null) { _deletePlaylistCommand = new RelayCommand(param => this.DeletePlaylist(param)); } return _deletePlaylistCommand; }
        }
        async void DeletePlaylist(object playlist)
        {
            var stop = System.Diagnostics.Stopwatch.StartNew();
            var dictPl = playlist != null ? (playlist as Dictionary<Playlist, IEnumerable<Mediafile>>).First() : CurrentDictionary.First(); //get the dictionary containing playlist and songs.
            var selectedPlaylist = dictPl.Key; //get selected playlist
            var songs = dictPl.Value; //get songs of the playlist
            Windows.UI.Popups.MessageDialog dia = new Windows.UI.Popups.MessageDialog("Do you want to delete this playlist?", "Confirmation");
            dia.Commands.Add(new Windows.UI.Popups.UICommand("Yes") { Id = 0 });
            dia.Commands.Add(new Windows.UI.Popups.UICommand("No") { Id = 1 });
            dia.DefaultCommandIndex = 0;
            dia.CancelCommandIndex = 1;
            var result = await dia.ShowAsync();
            if (result.Label == "Yes")
            {
                if (songs.Count() > 0) //check to see if there are any songs. If not then the playlist is empty.
                {
                    foreach (var file in songs)
                    {
                        file.Playlists.Remove(file.Playlists.First(t => t.Name == selectedPlaylist.Name)); //remove playlist from the song.
                        LibVM.Database.Update(file);//update database and save all changes.
                    }
                }
                ShellVM.PlaylistsItems.Remove(ShellVM.PlaylistsItems.First(t => t.Label == selectedPlaylist.Name)); //delete from hamburger menu
                LibVM.OptionItems.Remove(LibVM.OptionItems.First(t => t.Text == selectedPlaylist.Name)); //delete from context menu
                LibVM.Database.playlists.Delete(t => t.Name == selectedPlaylist.Name); //delete from database.
                
            }
            stop.Stop();
            System.Diagnostics.Debug.WriteLine("It took: " + stop.ElapsedMilliseconds.ToString());
        }

        async void RenamePlaylist(object playlist)
        {
            var stop = System.Diagnostics.Stopwatch.StartNew();
            var dictPl = playlist != null ? (playlist as Dictionary<Playlist, IEnumerable<Mediafile>>).First() : CurrentDictionary.First(); //get the dictionary containing playlist and songs.
            var selectedPlaylist = dictPl.Key; //get selected playlist
            var songs = dictPl.Value; //get songs of the playlist
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
                        LibVM.Database.Update(file); //update database saving all songs and changes.
                    }
                   
                }
                ShellVM.PlaylistsItems.First(t => t.Label == selectedPlaylist.Name).Label = pl.Name; //change playlist name in the hamburgermenu
                LibVM.OptionItems.First(t => t.Text == selectedPlaylist.Name).Text = pl.Name; //change playlist name in context menu of each song.
                LibVM.Database.playlists.FindOne(t => t.Name == selectedPlaylist.Name); //change playlist name in the 'playlist' collection in the database.
                dictPl.Key.Name = pl.Name;
                Playlist = pl; //set this.Playlist to pl (local variable);
                
            }
            stop.Stop();
            System.Diagnostics.Debug.WriteLine("It took: " + stop.ElapsedMilliseconds.ToString());
        }
        public PlaylistViewModel()
        {
        }
        public Dictionary<Playlist, IEnumerable<Mediafile>> CurrentDictionary { get; set; }
        RelayCommand _initCommand;
        /// <summary>
        /// Gets command for initialization. This calls the <see cref="Init(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand InitCommand
        {
            get
            { if (_initCommand == null) { _initCommand = new RelayCommand(param => this.Init(param)); } return _initCommand; }
        }
        public ListView PlaylistSongsListBox;
        bool _isPageLoaded;
        public bool IsPageLoaded { get { return _isPageLoaded; } set { Set(ref _isPageLoaded, value); } }
        void Init(object para)
        {
            IsPageLoaded = true;
            var childern = para as UIElementCollection;
            var fileBox = childern.OfType<ListView>().ToList()[0];
            PlaylistSongsListBox = fileBox;
            var mp3 = PlaylistVM?.Songs?.SingleOrDefault(t => t.Path == Player.CurrentlyPlayingFile.Path);
            if(mp3 != null)mp3.State = PlayerState.Playing;
        }
    }
}
