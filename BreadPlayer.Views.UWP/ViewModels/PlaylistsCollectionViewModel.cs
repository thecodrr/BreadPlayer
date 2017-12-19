using BreadPlayer.Core;
using BreadPlayer.Core.Common;
using BreadPlayer.Core.Models;
using BreadPlayer.Database;
using BreadPlayer.Dialogs;
using BreadPlayer.Dispatcher;
using BreadPlayer.Extensions;
using BreadPlayer.Helpers;
using BreadPlayer.Interfaces;
using BreadPlayer.Messengers;
using BreadPlayer.PlaylistBus;
using BreadPlayer.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace BreadPlayer.ViewModels
{
    public class PlaylistsCollectionViewModel : ObservableObject
    {
        private RelayCommand _addtoplaylistCommand;
        private RelayCommand _deletePlaylistCommand;
        private RelayCommand _exportPlaylistCommand;
        private RelayCommand _renamePlaylistCommand;

        private DelegateCommand _importPlaylistCommand;

        ThreadSafeObservableCollection<Playlist> playlists;

        private PlaylistService _playlistService;
        public PlaylistsCollectionViewModel()
        {
            Playlists = new ThreadSafeObservableCollection<Playlist>();
            Init();
            Messenger.Instance.Register(Messengers.MessageTypes.MsgAddPlaylist, new Action<Message>(HandleAddPlaylistMessage));
            Messenger.Instance.Register(Messengers.MessageTypes.MsgRemovePlaylist, new Action<Message>(HandleRemovePlaylistMessage));
        }
        
        public ICommand AddToPlaylistCommand
        {
            get
            { if (_addtoplaylistCommand == null) { _addtoplaylistCommand = new RelayCommand(param => AddToPlaylist(param)); } return _addtoplaylistCommand; }
        }
        public ICommand DeletePlaylistCommand
        {
            get
            { if (_deletePlaylistCommand == null) { _deletePlaylistCommand = new RelayCommand(param => DeletePlaylist(param)); } return _deletePlaylistCommand; }
        }

        public ICommand ExportPlaylistCommand
        {
            get
            { if (_exportPlaylistCommand == null) { _exportPlaylistCommand = new RelayCommand(param => ExportPlaylist(param)); } return _exportPlaylistCommand; }
        }
        public ICommand RenamePlaylistCommand
        {
            get
            { if (_renamePlaylistCommand == null) { _renamePlaylistCommand = new RelayCommand(param => RenamePlaylist(param)); } return _renamePlaylistCommand; }
        }
        public DelegateCommand ImportPlaylistCommand { get { if (_importPlaylistCommand == null) { _importPlaylistCommand = new DelegateCommand(ImportPlaylists); } return _importPlaylistCommand; } }
        public ThreadSafeObservableCollection<Playlist> Playlists
        {
            get => playlists;
            set => Set(ref playlists, value);
        }

        private PlaylistService PlaylistService =>
                                            _playlistService ?? (_playlistService = new PlaylistService(
                new KeyValueStoreDatabaseService(
                    SharedLogic.Instance.DatabasePath,
                    "Playlists")));
        private void AddPlaylist(Playlist playlist)
        {
            Playlists.Add(playlist);
            SharedLogic.Instance.OptionItems.Add(new ContextMenuCommand(AddToPlaylistCommand, playlist.Name, "\uE710"));
        }
        private async void ImportPlaylists()
        {
            FileOpenPicker openPicker = new FileOpenPicker()
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.MusicLibrary
            };
            openPicker.FileTypeFilter.Add(".m3u");
            openPicker.FileTypeFilter.Add(".pls");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                StorageApplicationPermissions.FutureAccessList.Add(file);

                IPlaylist playlist = null;
                if (Path.GetExtension(file.Path) == ".m3u")
                {
                    playlist = new M3U();
                }
                else
                {
                    playlist = new Pls();
                }

                var plist = new Playlist { Name = file.DisplayName, IsExternal = true, Path = file.Path };
                Messenger.Instance.NotifyColleagues(MessageTypes.MsgAddPlaylist, plist);
            }
        }

        private async void RenamePlaylist(object playlist)
        {
            try
            {
                var selectedPlaylist = playlist as Playlist;
                if (await SharedLogic.Instance.AskForPassword(selectedPlaylist))
                {
                    var dialog = new InputDialog
                    {
                        Title = "Rename this playlist",
                        Text = selectedPlaylist.Name,
                        Description = selectedPlaylist.Description
                    };
                    if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                    {
                        string oldName = selectedPlaylist.Name; //save old name
                        selectedPlaylist.Name = dialog.Text;
                        selectedPlaylist.Description = dialog.Description;
                        SharedLogic.Instance.OptionItems.First(t => t.Text == oldName).Text = selectedPlaylist.Name; //change playlist name in context menu of each song.
                        await PlaylistService.UpdatePlaylistAsync(selectedPlaylist);
                        //Playlist = pl; //set this.Playlist to pl (local variable);
                    }
                }
            }
            catch (Exception)
            {
                await SharedLogic.Instance.NotificationManager.ShowMessageAsync("Cannot rename playlist. Please try again.");
            }
        }
        private async void ExportPlaylist(object para)
        {
            var playlist = (Playlist)para;
            if (playlist == null)
                return;
            var songs = new List<Mediafile>();
            if (playlist.IsExternal)
            {
                IPlaylist extPlaylist = Path.GetExtension(playlist.Path) == ".m3u" ? new M3U() : (IPlaylist)new Pls();
                songs.AddRange(await extPlaylist.LoadPlaylist(await StorageFile.GetFileFromPathAsync(playlist.Path)));
            }
            else
            {
                songs.AddRange(await PlaylistService.GetTracksAsync(playlist.Id).ConfigureAwait(false));
            }
            await PlaylistHelper.SavePlaylist(playlist, songs);
        }
        private async void DeletePlaylist(object playlist)
        {
            try
            {
                var selectedPlaylist = playlist as Playlist;

                if (selectedPlaylist != null && await SharedLogic.Instance.AskForPassword(selectedPlaylist))
                {
                    MessageDialog dia = new MessageDialog("Do you want to delete this playlist?", "Confirmation");
                    dia.Commands.Add(new UICommand("Yes") { Id = 0 });
                    dia.Commands.Add(new UICommand("No") { Id = 1 });
                    dia.DefaultCommandIndex = 0;
                    dia.CancelCommandIndex = 1;
                    var result = await dia.ShowAsync();
                    if (result.Label == "Yes")
                    {
                        if (NavigationService.Instance.Frame.CurrentSourcePageType != NavigationService.Instance.HomePage.GetType())
                        {
                            NavigationService.Instance.NavigateToHome();
                        }

                        Playlists.Remove(selectedPlaylist);
                        SharedLogic.Instance.OptionItems.Remove(SharedLogic.Instance.OptionItems.First(t => t.Text == selectedPlaylist.Name)); //delete from context menu
                        await PlaylistService.RemovePlaylistAsync(selectedPlaylist);//delete from database.
                    }
                }
            }
            catch (Exception ex)
            {
                BLogger.E("Error occured while deleting playlist.", ex);
            }
        }
        private async Task AddPlaylistAsync(Playlist plist, bool addsongs, IEnumerable<Mediafile> songs = null)
        {
            await BreadDispatcher.InvokeAsync(async () =>
            {
                if (!PlaylistService.PlaylistExists(plist.Name))
                {
                    await PlaylistService.AddPlaylistAsync(plist);
                    AddPlaylist(plist);
                }
                if (addsongs)
                {
                    IEnumerable<ChildSong> playlistSongs
                   = songs.Select(x => new ChildSong()
                   {
                       SongId = x.Id,
                       PlaylistId = plist.Id
                   });
                    var db = new KeyValueStoreDatabaseService(SharedLogic.Instance.DatabasePath, "PlaylistSongs");
                    await db.InsertRecords(playlistSongs);
                    await AddSongsToPlaylist(plist);
                }
            });
        }

        private async Task AddSongsToPlaylist(Playlist list)
        {
            //await PlaylistService.InsertTracksAsync(songsToadd.Where(t => !PlaylistService.Exists(t.Id)), list);
            var pSongs = (await PlaylistService.GetTracksAsync(list.Id)).ToList();

            //update duration and songs count
            var collectionPlaylist = Playlists.FirstOrDefault(t => t.Name == list.Name);
            collectionPlaylist.SongsCount = pSongs.Count + " songs";
            collectionPlaylist.ImagePath = pSongs.First(t => t.AttachedPicture != null)?.AttachedPicture ?? "";
            if(!string.IsNullOrEmpty(collectionPlaylist.ImagePath))
                collectionPlaylist.ImageColor = (await SharedLogic.Instance.GetDominantColor(await StorageFile.GetFileFromPathAsync(collectionPlaylist.ImagePath))).ToHexString();
            collectionPlaylist.Duration = string.Format("{0:0.0}", Math.Truncate(pSongs.Sum(t => TimeSpan.ParseExact(IsHour(t.Length) ? t.Length : "00:" + t.Length, @"hh\:mm\:ss", CultureInfo.InvariantCulture).TotalMinutes) * 10) / 10) + " Minutes";
            await PlaylistService.UpdatePlaylistAsync(collectionPlaylist).ConfigureAwait(false);
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
                        var albumSongs = await new LibraryService(new KeyValueStoreDatabaseService(SharedLogic.Instance.DatabasePath, "Tracks")).Query((album.AlbumName));
                        if (albumSongs?.Any() == true)
                            songList.AddRange(albumSongs);
                    }
                    else if (SettingsViewModel.TracksCollection.Elements.Any(t => t.IsSelected) == true)
                        songList.AddRange(SettingsViewModel.TracksCollection.Elements.Where(t => t.IsSelected));
                }
                else
                {
                    songList.Add(SharedLogic.Instance.Player.CurrentlyPlayingFile);
                }
                var dictPlaylist = menu?.Text == "New Playlist" ? await ShowAddPlaylistDialogAsync() : await PlaylistService.GetPlaylistAsync(menu?.Text);
                bool proceed;
                if (menu?.Text != "New Playlist")
                {
                    proceed = await SharedLogic.Instance.AskForPassword(dictPlaylist);
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

        private async void HandleAddPlaylistMessage(Message message)
        {
            if (message.Payload is Playlist plist)
            {
                message.HandledStatus = MessageHandledStatus.HandledCompleted;
                await AddPlaylistAsync(plist, false);
            }
        }

        private void HandleRemovePlaylistMessage(Message message)
        {
            if (message.Payload is Playlist plist)
            {
                message.HandledStatus = MessageHandledStatus.HandledCompleted;
                DeletePlaylist(plist);
            }
        }

        private async void Init()
        {
            await LoadPlaylists().ConfigureAwait(false);
        }
        private bool IsHour(string length)
        {
            return length.Count(t => t == ':') == 2;
        }

        private async Task LoadPlaylists()
        {
            SharedLogic.Instance.OptionItems.Add(new ContextMenuCommand(AddToPlaylistCommand, "New Playlist", "\uE710"));
            var playlists = await PlaylistService.GetPlaylistsAsync().ConfigureAwait(false);
            if (playlists != null)
            {
                Playlists.AddRange(playlists);
                SharedLogic.Instance.OptionItems.AddRange(playlists.Select(t => new ContextMenuCommand(AddToPlaylistCommand, t.Name)));
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
    }
}