using BreadPlayer.Core;
using BreadPlayer.Core.Common;
using BreadPlayer.Core.Models;
using BreadPlayer.Database;
using BreadPlayer.Dialogs;
using BreadPlayer.Dispatcher;
using BreadPlayer.Extensions;
using BreadPlayer.Messengers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace BreadPlayer.ViewModels
{
    public class PlaylistsCollectionViewModel : ViewModelBase
    {
        private PlaylistService _playlistService;

        private PlaylistService PlaylistService =>
            _playlistService ?? (_playlistService = new PlaylistService(
                new DocumentStoreDatabaseService(
                    SharedLogic.DatabasePath,
                    "Playlists")));
        public ThreadSafeObservableCollection<Playlist> Playlists { get; set; }

        private RelayCommand _addtoplaylistCommand;
        /// <summary>
        /// Gets AddToPlaylist command. This calls the <see cref="AddToPlaylist(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand AddToPlaylistCommand
        {
            get
            { if (_addtoplaylistCommand == null) { _addtoplaylistCommand = new RelayCommand(param => AddToPlaylist(param)); } return _addtoplaylistCommand; }
        }
        public PlaylistsCollectionViewModel()
        {
            Playlists = new ThreadSafeObservableCollection<Playlist>();
            SharedLogic.OptionItems.Add(new ContextMenuCommand(AddToPlaylistCommand, "New Playlist"));
            Messengers.Messenger.Instance.Register(Messengers.MessageTypes.MsgAddPlaylist, new Action<Messengers.Message>(HandleAddPlaylistMessage));
        }
        private async void HandleAddPlaylistMessage(Message message)
        {
            if (message.Payload is Playlist plist)
            {
                message.HandledStatus = MessageHandledStatus.HandledCompleted;
                await AddPlaylistAsync(plist, false);
            }
        }

        private async Task AddPlaylistAsync(Playlist plist, bool addsongs, IEnumerable<Mediafile> songs = null)
        {
            await BreadDispatcher.InvokeAsync(async () =>
            {
                if (!PlaylistService.PlaylistExists(plist.Name))
                {
                    AddPlaylist(plist);
                    await PlaylistService.AddPlaylistAsync(plist);
                }
                if (addsongs)
                {
                    await AddSongsToPlaylist(plist, songs.ToList());
                }
            });
        }

        private async Task LoadPlaylists()
        {
            var playlists = await PlaylistService.GetPlaylistsAsync();
            if (playlists != null)
            {
                foreach (var list in playlists)
                {
                    AddPlaylist(list);
                }
            }
        }

        private async void AddToPlaylist(object file)
        {
            if (file != null)
            {
                var menu = file as MenuFlyoutItem;
                //songList is a variable to initiate both (if available) sources of songs. First is AlbumSongs and the second is the direct library songs.
                var songList = new List<Mediafile>();
                if (menu?.Tag == null)
                {
                    if (menu?.DataContext is Album album)
                    {
                        var albumSongs = await new LibraryService(new DocumentStoreDatabaseService(SharedLogic.DatabasePath, "Tracks")).Query((album.AlbumName));
                        if (albumSongs?.Any() == true)
                            songList.AddRange(albumSongs);
                    }
                    else if (SettingsViewModel.TracksCollection.Elements.Any(t => t.IsSelected) == true)
                        songList.AddRange(SettingsViewModel.TracksCollection.Elements.Where(t => t.IsSelected));
                }
                else
                {
                    songList.Add(Player.CurrentlyPlayingFile);
                }
                var dictPlaylist = menu?.Text == "New Playlist" ? await ShowAddPlaylistDialogAsync() : await PlaylistService.GetPlaylistAsync(menu?.Text);
                bool proceed;
                if (menu?.Text != "New Playlist")
                {
                    proceed = await SharedLogic.AskForPassword(dictPlaylist);
                }
                else
                {
                    proceed = true;
                }

                if (dictPlaylist != null && proceed)
                {
                    await AddPlaylistAsync(dictPlaylist, true, songList);
                }
            }
            else
            {
                var pList = await ShowAddPlaylistDialogAsync();
                if (pList != null)
                {
                    await AddPlaylistAsync(pList, false);
                }
            }
        }

        private async Task<Playlist> ShowAddPlaylistDialogAsync(string title = "Name this playlist", string playlistName = "", string desc = "", string password = "")
        {
            var dialog = new InputDialog
            {
                Title = title,
                Text = playlistName,
                Description = desc,
                IsPrivate = password.Length > 0,
                Password = password
            };
            if (CoreWindow.GetForCurrentThread().Bounds.Width <= 501)
            {
                dialog.DialogWidth = CoreWindow.GetForCurrentThread().Bounds.Width - 50;
            }
            else
            {
                dialog.DialogWidth = CoreWindow.GetForCurrentThread().Bounds.Width - 300;
            }

            if (await dialog.ShowAsync() == ContentDialogResult.Primary && dialog.Text != "")
            {
                var salthash = PasswordStorage.CreateHash(dialog.Password);
                var playlist = new Playlist()
                {
                    Name = dialog.Text,
                    Description = dialog.Description,
                    IsPrivate = dialog.Password.Length > 0,
                    Hash = salthash.Hash,
                    Salt = salthash.Salt
                };
                if (PlaylistService.PlaylistExists(playlist.Name))
                {
                    playlist = await ShowAddPlaylistDialogAsync("Playlist already exists! Please choose another name.", playlist.Name, playlist.Description);
                }
                return playlist;
            }
            return null;
        }

        private async Task AddSongsToPlaylist(Playlist list, IReadOnlyCollection<Mediafile> songsToadd)
        {
            if (songsToadd.Any())
            {
                await PlaylistService.InsertTracksAsync(songsToadd.Where(t => !PlaylistService.Exists(t.Id)), list);
            }
        }

        private void AddPlaylist(Playlist playlist)
        {
            var cmd = new ContextMenuCommand(AddToPlaylistCommand, playlist.Name);
            SharedLogic.OptionItems.Add(cmd);
            Playlists.Add(playlist);
            //SharedLogic.PlaylistsItems.Add(new SimpleNavMenuItem
            //{
            //    Arguments = playlist,
            //    Label = playlist.Name,
            //    DestinationPage = typeof(PlaylistView),
            //    Symbol = Symbol.List,
            //    FontGlyph = "\u0045",
            //    ShortcutTheme = ElementTheme.Dark,
            //    HeaderVisibility = Visibility.Collapsed
            //});
        }
    }
}
