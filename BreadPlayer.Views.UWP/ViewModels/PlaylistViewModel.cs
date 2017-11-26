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

using BreadPlayer.Core;
using BreadPlayer.Core.Common;
using BreadPlayer.Core.Models;
using BreadPlayer.Database;
using BreadPlayer.Dialogs;
using BreadPlayer.Dispatcher;
using BreadPlayer.Extensions;
using BreadPlayer.Interfaces;
using BreadPlayer.Messengers;
using BreadPlayer.PlaylistBus;
using BreadPlayer.Services;
using BreadPlayer.Themes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace BreadPlayer.ViewModels
{
    public class PlaylistViewModel : ObservableObject
    {
        private ThreadSafeObservableCollection<Mediafile> _songs;

        public ThreadSafeObservableCollection<Mediafile> Songs
        {
            get { if (_songs == null) { _songs = new ThreadSafeObservableCollection<Mediafile>(); } return _songs; }
            set => Set(ref _songs, value);
        }

        private Playlist _playlist;

        public Playlist Playlist
        {
            get => _playlist;
            set => Set(ref _playlist, value);
        }

        private PlaylistService PlaylistService { get; set; }
        private string _totalSongs;

        public string TotalSongs
        {
            get => _totalSongs;
            set
            {
                _totalSongs = "0";
                Set(ref _totalSongs, value);
            }
        }

        private bool _isPlaylistLoading;

        public bool IsPlaylistLoading
        {
            get => _isPlaylistLoading;
            set
            {
                Set(ref _isPlaylistLoading, value);
            }
        }

        private string _totalMinutes;

        public string TotalMinutes
        {
            get => _totalMinutes;
            set
            {
                _totalMinutes = "0";
                Set(ref _totalMinutes, value);
            }
        }

        private bool _isMenuVisible = true;

        public bool IsMenuVisible
        {
            get => _isMenuVisible;
            set => Set(ref _isMenuVisible, value);
        }

        private ImageSource _playlistArt;

        public ImageSource PlaylistArt
        {
            get => _playlistArt;
            set
            {
                if (Songs.Any())
                {
                    _playlistArt = null;
                }

                Set(ref _playlistArt, value);
            }
        }

        private RelayCommand _deleteCommand;

        /// <summary>
        /// Gets Play command. This calls the <see cref="Delete(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand DeleteCommand
        {
            get
            { if (_deleteCommand == null) { _deleteCommand = new RelayCommand(param => Delete(param)); } return _deleteCommand; }
        }

        private async void Delete(object para)
        {
            try
            {
                PlaylistArt = null;
                var mediafile = para as Mediafile;
                if (mediafile == null)
                {
                    mediafile = SharedLogic.Instance.Player.CurrentlyPlayingFile;
                }
                await PlaylistService.RemoveSongAsync(mediafile);
                Songs.Remove(Songs.First(t => t.Path == mediafile.Path));
                Refresh();
            }
            catch (Exception ex)
            {
                BLogger.E("Error occured while deleting song from playlist.", ex);
            }
        }

        private bool IsHour(string length)
        {
            return length.Count(t => t == ':') == 2;
        }

        public void Refresh()
        {
            try
            {
                PlaylistArt = null;
                TotalMinutes = string.Format("{0:0.0}", Math.Truncate(Songs.Sum(t => TimeSpan.ParseExact(IsHour(t.Length) ? t.Length : "00:" + t.Length, @"hh\:mm\:ss", CultureInfo.InvariantCulture).TotalMinutes) * 10) / 10) + " Minutes";
                TotalSongs = Songs.FastCount + " Songs";
                var mp3 = Songs?.FirstOrDefault(t => t.Path == SharedLogic.Instance.Player.CurrentlyPlayingFile?.Path);
                if (mp3 != null)
                {
                    mp3.State = PlayerState.Playing;
                }
            }
            catch (Exception ex)
            {
                BLogger.E("Error occured while refreshing playlist.", ex);
            }
        }
        public PlaylistViewModel()
        {
            PlaylistService = new PlaylistService(new KeyValueStoreDatabaseService(SharedLogic.Instance.DatabasePath, "Playlists"));
        }

        public async Task Init(object data)
        {
            IsPlaylistLoading = true;
            if (data is Playlist playlist)
            {
                Playlist = playlist;
                LoadDb();
            }
            else if (data is Album album)
            {
                IsMenuVisible = false;
                Playlist = new Playlist { Name = album.AlbumName, Description = album.Artist };
                LoadAlbumSongs(album);
            }
            else if (data is Artist artist)
            {
                IsMenuVisible = false;
                Playlist = new Playlist { Name = artist.Name};
                if (artist.Bio != null)
                    Playlist.Description = await artist.Bio.UnzipAsync();
                LoadArtistSongs(artist);
            }
        }

        private void LoadArtistSongs(Artist artist)
        {
            if (artist == null || string.IsNullOrEmpty(artist.Name))
                return;
            Songs.AddRange(SettingsViewModel.TracksCollection.Elements.Where(t => t.LeadArtist == artist.Name));
            Refresh();
            Messenger.Instance.NotifyColleagues(MessageTypes.MsgPlaylistLoaded, Songs);
            IsPlaylistLoading = false;
        }

        private void LoadAlbumSongs(Album album)
        {
            if (album == null || string.IsNullOrEmpty(album.AlbumName))
                return;
            var s = SettingsViewModel.TracksCollection.Elements.Where(t => t.Album == album.AlbumName).OrderBy(t => t.TrackNumber);
            Songs.AddRange(s);
            Refresh();
            Messenger.Instance.NotifyColleagues(MessageTypes.MsgPlaylistLoaded, Songs);
            IsPlaylistLoading = false;
        }

        private async void LoadDb()
        {
            if (await SharedLogic.Instance.AskForPassword(_playlist))
            {
                if (_playlist.IsExternal)
                {
                    IPlaylist extPlaylist = Path.GetExtension(_playlist.Path) == ".m3u" ? (IPlaylist)new M3U() : (IPlaylist)new Pls();
                    Songs.AddRange(await extPlaylist.LoadPlaylist(await StorageFile.GetFileFromPathAsync(_playlist.Path)));
                }
                else
                {
                    var playlistSongs = await PlaylistService.GetTracksAsync(_playlist.Id);
                    if (playlistSongs != null)
                        Songs.AddRange(playlistSongs);
                    foreach (var playlistSong in Songs)
                    {
                        playlistSong.IsPlaylistSong = true;
                    }
                }
                Refresh();
                Messenger.Instance.NotifyColleagues(MessageTypes.MsgPlaylistLoaded, Songs);
                IsPlaylistLoading = false;
            }
            else
            {
                NavigationService.Instance.NavigateToHome();
            }
        }

        private bool _isPageLoaded;

        public bool IsPageLoaded
        {
            get => _isPageLoaded;
            set => Set(ref _isPageLoaded, value);
        }

        public void Reset()
        {
            PlaylistArt = null;
            TotalSongs = "0";
            TotalMinutes = "0";
            Songs = null;
        }
    }
}