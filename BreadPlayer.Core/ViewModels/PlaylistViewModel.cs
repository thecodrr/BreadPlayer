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
using BreadPlayer.Models;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Controls;
using System.Windows.Input;
using Windows.UI.Core;
using BreadPlayer.Dialogs;
using BreadPlayer.Extensions;
using Windows.UI.Popups;
using System.Globalization;
using BreadPlayer.Core;
using System.IO;
using Windows.Storage;
using BreadPlayer.Service;

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
                return playlistArt;
            }
            set
            {
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
            PlaylistArt = null;
            var mediafile = para as Mediafile;
            if (mediafile == null)
                mediafile = Player.CurrentlyPlayingFile;
            var pName = Playlist == null ? (para as MenuFlyoutItem).Text : Playlist.Name;
            using (PlaylistService service = new PlaylistService(Playlist.Name))
            {
                service.Remove(mediafile);
                Songs.Remove(mediafile);
                // mediafile.Playlists.Remove(mediafile.Playlists.Single(t => t.Name == pName));
                // Songs.Remove(mediafile);
                // LibVM.Database.Update(mediafile);
                Refresh();
            }
            
        }
       public async void Refresh()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => 
            {
                TotalMinutes = string.Format("{0:0.0}", Math.Truncate(Songs.Sum(t => TimeSpan.ParseExact(t.Length, "mm\\:ss", CultureInfo.InvariantCulture).TotalMinutes) * 10) / 10) + " Minutes";
                TotalSongs = Songs.Count.ToString() + " Songs";
                if (Songs.Any(s => !string.IsNullOrEmpty(s.AttachedPicture)) && PlaylistArt == null)
                {
                    BitmapImage image = new BitmapImage(new Uri(Songs.FirstOrDefault(s => !string.IsNullOrEmpty(s.AttachedPicture)).AttachedPicture, UriKind.RelativeOrAbsolute));
                   
                    PlaylistArt = image;
                    Themes.ThemeManager.SetThemeColor(Songs.FirstOrDefault(s => !string.IsNullOrEmpty(s.AttachedPicture)).AttachedPicture);
                }
                var mp3 = Songs?.FirstOrDefault(t => t.Path == Player.CurrentlyPlayingFile?.Path);
                if (mp3 != null) mp3.State = PlayerState.Playing;
            });
            
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
            var selectedPlaylist = playlist != null ? playlist as Playlist : Playlist; //get the dictionary containing playlist and songs.
            MessageDialog dia = new MessageDialog("Do you want to delete this playlist?", "Confirmation");
            dia.Commands.Add(new Windows.UI.Popups.UICommand("Yes") { Id = 0 });
            dia.Commands.Add(new Windows.UI.Popups.UICommand("No") { Id = 1 });
            dia.DefaultCommandIndex = 0;
            dia.CancelCommandIndex = 1;
            var result = await dia.ShowAsync();
            if (result.Label == "Yes")
            {
                string path = ApplicationData.Current.LocalFolder.Path + @"\playlists\" + selectedPlaylist.Name + ".db";
                if (File.Exists(path)) File.Delete(path);
                PlaylistsItems.Remove(PlaylistsItems.First(t => t.Label == selectedPlaylist.Name)); //delete from hamburger menu
                OptionItems.Remove(OptionItems.First(t => t.Text == selectedPlaylist.Name)); //delete from context menu
                using (LibraryService service = new LibraryService(new DatabaseService()))
                {
                    service.RemovePlaylist(selectedPlaylist);//delete from database.
                }
             }
        }

        async void RenamePlaylist(object playlist)
        {
            var selectedPlaylist = playlist != null ? playlist as Playlist : Playlist; //get the playlist to delete.
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
                string path = ApplicationData.Current.LocalFolder.Path + @"\playlists\";
                if (File.Exists(path + selectedPlaylist.Name + ".db"))
                    File.Move(path + selectedPlaylist.Name + ".db", path + pl.Name + ".db");
                PlaylistsItems.First(t => t.Label == selectedPlaylist.Name).Arguments = pl;
                PlaylistsItems.First(t => t.Label == selectedPlaylist.Name).Label = pl.Name; //change playlist name in the hamburgermenu
                OptionItems.First(t => t.Text == selectedPlaylist.Name).Text = pl.Name; //change playlist name in context menu of each song.
                using (LibraryService service = new LibraryService(new DatabaseService()))
                {
                    service.RemovePlaylist(selectedPlaylist);//delete from database.
                    service.AddPlaylist(pl); //add new playlist in the database.
                    Playlist = pl; //set this.Playlist to pl (local variable);
                }
            }
        }
        public PlaylistViewModel()
        {
        }
        public PlaylistViewModel(object data)
        {
            if (data is Playlist)
            {
                Playlist = data as Playlist;
                LoadDB();
            }
            else
            {
                Playlist = new Playlist() { Name = (data as Album).AlbumName, Description = (data as Album).Artist};
                LoadAlbumSongs(data as Album);
                Refresh();
            }
            Messengers.Messenger.Instance.NotifyColleagues(Messengers.MessageTypes.MSG_PLAYLIST_LOADED, Songs);
        }
        void LoadAlbumSongs(Album album)
        {
            Songs.AddRange(album.AlbumSongs);          
        }
        async void LoadDB()
        {
            using (Service.PlaylistService service = new Service.PlaylistService(Playlist.Name))
            {
                if (service.IsValid)
                {
                    var ss = service.GetTrackCount();
                    Songs.AddRange(await service.GetTracks().ConfigureAwait(false));
                    Refresh();
                }
            }
        }
        private void Elements_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Refresh();
        }
        
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
            Songs.CollectionChanged += Elements_CollectionChanged;           
        }
        
        public void Reset()
        {
            
            PlaylistArt = null;
            TotalSongs = "0";
            TotalMinutes = "0";
            Songs.Clear();
        }
    }
}
