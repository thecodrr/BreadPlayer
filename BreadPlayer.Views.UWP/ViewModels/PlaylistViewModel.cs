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
using Windows.UI.Core;
using BreadPlayer.Dialogs;
using Windows.UI.Popups;
using System.Globalization;
using BreadPlayer.Core;
using System.IO;
using Windows.Storage;
using BreadPlayer.Database;
using System.Threading.Tasks;

namespace BreadPlayer.ViewModels
{
	public class PlaylistViewModel : ViewModelBase
    {
        ThreadSafeObservableCollection<Mediafile> songs;
        public ThreadSafeObservableCollection<Mediafile> Songs { get { if (songs == null) { songs = new ThreadSafeObservableCollection<Mediafile>(); } return songs; } set { Set(ref songs, value); } }
        Playlist playlist;
        public Playlist Playlist { get { return playlist; } set { Set(ref playlist, value); } }
        private PlaylistService PlaylistService { get; set; }
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

        async void Delete(object para)
        {
            try
            {
                PlaylistArt = null;
                var mediafile = para as Mediafile;
                if (mediafile == null)
                    mediafile = Player.CurrentlyPlayingFile;
                var pName = Playlist == null ? (para as MenuFlyoutItem).Text : Playlist.Name;
                await PlaylistService.RemoveSongAsync(mediafile);
                Songs.Remove(Songs.First(t => t.Path == mediafile.Path));
                await Refresh();
            }
            catch (Exception ex)
            {
                BLogger.Logger.Error("Error occured while deleting song from playlist.", ex);
            }
        }
        public async Task Refresh()
        {
            await SharedLogic.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                try
                {
                    PlaylistArt = null;
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
                }
                catch (Exception ex)
                {
                    BLogger.Logger.Error("Error occured while refreshing playlist.", ex);
                }
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
            try
            {
                var selectedPlaylist = playlist != null ? playlist as Playlist : Playlist; //get the dictionary containing playlist and songs.
              
                if (selectedPlaylist != null && await SharedLogic.AskForPassword(selectedPlaylist))
                {
                    MessageDialog dia = new MessageDialog("Do you want to delete this playlist?", "Confirmation");
                    dia.Commands.Add(new Windows.UI.Popups.UICommand("Yes") { Id = 0 });
                    dia.Commands.Add(new Windows.UI.Popups.UICommand("No") { Id = 1 });
                    dia.DefaultCommandIndex = 0;
                    dia.CancelCommandIndex = 1;
                    var result = await dia.ShowAsync();
                    if (result.Label == "Yes")
                    {
                        if(Services.NavigationService.Instance.Frame.CurrentSourcePageType != Services.NavigationService.Instance.HomePage.GetType())
                            Services.NavigationService.Instance.NavigateToHome();
                        string path = ApplicationData.Current.LocalFolder.Path + @"\playlists\" + selectedPlaylist.Name + ".db";
                        if (File.Exists(path))
                            File.Delete(path);
                        SharedLogic.PlaylistsItems.Remove(SharedLogic.PlaylistsItems.First(t => t.Label == selectedPlaylist.Name)); //delete from hamburger menu
                        SharedLogic.OptionItems.Remove(SharedLogic.OptionItems.First(t => t.Text == selectedPlaylist.Name)); //delete from context menu
                        await PlaylistService.RemovePlaylistAsync(selectedPlaylist);//delete from database.                        
                    }
                }
            }
            catch (Exception ex)
            {
                BLogger.Logger.Error("Error occured while deleting playlist.", ex);
            }
        }

        async void RenamePlaylist(object playlist)
        {
            try
            {
                var selectedPlaylist = playlist != null ? playlist as Playlist : Playlist; //get the playlist to delete.                    
                if (await SharedLogic.AskForPassword(selectedPlaylist))
                {
                    var dialog = new InputDialog()
                    {
                        Title = "Rename this playlist",
                        Text = selectedPlaylist.Name,
                        Description = selectedPlaylist.Description
                    };
                    var Playlists = new Dictionary<Playlist, IEnumerable<Mediafile>>();
                    if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                    {
                        var pl = new Playlist() { Name = dialog.Text, Description = dialog.Description, Id = selectedPlaylist.Id };
                        string path = ApplicationData.Current.LocalFolder.Path + @"\playlists\";
                        if (File.Exists(path + selectedPlaylist.Name + ".db"))
                            File.Move(path + selectedPlaylist.Name + ".db", path + pl.Name + ".db");
                        SharedLogic.PlaylistsItems.First(t => t.Label == selectedPlaylist.Name).Arguments = pl;
                        SharedLogic.PlaylistsItems.First(t => t.Label == selectedPlaylist.Name).Label = pl.Name; //change playlist name in the hamburgermenu
                        SharedLogic.OptionItems.First(t => t.Text == selectedPlaylist.Name).Text = pl.Name; //change playlist name in context menu of each song.
                        await PlaylistService.UpdatePlaylistAsync(pl);
                        Playlist = pl; //set this.Playlist to pl (local variable);
                    }
                }
            }
            catch (Exception)
            {
                await NotificationManager.ShowMessageAsync("Cannot rename playlist. Please try again.");
            }
        }
        public PlaylistViewModel()
        {
            PlaylistService = new PlaylistService(new KeyValueStoreDatabaseService(Core.SharedLogic.DatabasePath, "", ""));
        }
        public void Init(object data)
        {
            if (data is Playlist playlist)
            {
                Playlist = playlist;
                LoadDB();
            }
            else
            {
                Album album = data as Album;
                IsMenuVisible = false;
                Playlist = new Playlist() { Name = album.AlbumName, Description = album.Artist };
                LoadAlbumSongs(album);               
            }
            Messengers.Messenger.Instance.NotifyColleagues(Messengers.MessageTypes.MSG_PLAYLIST_LOADED, Songs);
        }
        async void LoadAlbumSongs(Album album)
        {
            Songs.AddRange(await new LibraryService(new KeyValueStoreDatabaseService(Core.SharedLogic.DatabasePath, "Tracks","TracksText")).Query(album.AlbumName));
            await Refresh();
        }
        async void LoadDB()
        {
            if (await SharedLogic.AskForPassword(playlist))
            {
                Songs.AddRange(await PlaylistService.GetTracksAsync(playlist.Id));
                await Refresh();
            }
            else
            {
                Services.NavigationService.Instance.NavigateToHome();
            }
        }
        
        public ListView PlaylistSongsListBox;
        bool _isPageLoaded;
        public bool IsPageLoaded { get { return _isPageLoaded; } set { Set(ref _isPageLoaded, value); } }
        
        
        public void Reset()
        {            
            PlaylistArt = null;
            TotalSongs = "0";
            TotalMinutes = "0";
            Songs.Clear();
        }
    }
}
