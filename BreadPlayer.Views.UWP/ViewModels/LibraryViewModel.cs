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

using BreadPlayer.Common;
using BreadPlayer.Core;
using BreadPlayer.Core.Common;
using BreadPlayer.Core.Extensions;
using BreadPlayer.Core.Models;
using BreadPlayer.Database;
using BreadPlayer.Dispatcher;
using BreadPlayer.Extensions;
using BreadPlayer.Helpers;
using BreadPlayer.Interfaces;
using BreadPlayer.Messengers;
using BreadPlayer.Services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Input;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace BreadPlayer.ViewModels
{
    public delegate void OnMusicLibraryLoaded(object sender, RoutedEventArgs e);

    /// <summary>
    /// ViewModel for Library View (Severe cleanup and documentation needed.)
    /// </summary>
    public class LibraryViewModel : ObservableObject
    {
        #region Fields
        private IEnumerable<Mediafile> _files;
        private bool _grouped;
        private bool _isPlayingFromPlaylist;
        private bool _libgrouped;
        private bool _libraryLoaded;
        private IEnumerable<Mediafile> _oldItems;
        private object _source;
        private List<string> _alphabetList;
        private string _genre;
        private MenuFlyout _genreFlyout;
        private bool _isLibraryLoading;
        private bool _isMultiSelectModeEnabled;
        private LibraryService _libraryservice;
        private PlaylistService _playlistService;
        private Mediafile _selectedItem;
        private int _songCount;
        private string _sort = "Unsorted";
        private GroupedObservableCollection<IGroupKey, Mediafile> _tracksCollection;
        private CollectionViewSource _viewSource;
        #endregion Fields

        #region MessageHandling
        private async void HandleImportFolder(Message message)
        {
            if (message.Payload is IEnumerable<Mediafile> songs)
            {
                message.HandledStatus = MessageHandledStatus.HandledCompleted;
               
                await SharedLogic.Instance.NotificationManager.ShowMessageAsync("Saving songs into database. Please wait...", 3);
                await LibraryService.AddMediafiles(songs).ConfigureAwait(false);

                await SharedLogic.Instance.NotificationManager.ShowMessageAsync("Adding songs into library. Please wait...", 3);
                TracksCollection.AddRange(songs);
               

                IsLibraryLoading = false;
                await BreadDispatcher.InvokeAsync(() =>
                {
                    MusicLibraryLoaded?.Invoke(this, new RoutedEventArgs());
                });
            }
        }
        private void HandleDisposeMessage()
        {
            Reset();
        }

        private void HandlePlaySongMessage(Message message)
        {
            if (message.Payload is Mediafile song)
            {
                message.HandledStatus = MessageHandledStatus.HandledCompleted;
                PlayCommand.Execute(song);
            }
        }

        private void HandleUpdateSongCountMessage(Message message)
        {
            if (message.Payload is short || message.Payload is Int32)
            {
                message.HandledStatus = MessageHandledStatus.HandledCompleted;
                SongCount = Convert.ToInt32(message.Payload);
                IsLibraryLoading = true;
            }            
        }
        #endregion MessageHandling

        #region Contructor

        /// <summary>
        /// Creates a new instance of LibraryViewModel
        /// </summary>
        public LibraryViewModel()
        {
            MusicLibraryLoaded += LibraryViewModel_MusicLibraryLoaded;
            LoadLibrary();

            Messenger.Instance.Register(MessageTypes.MsgPlaySong, new Action<Message>(HandlePlaySongMessage));
            Messenger.Instance.Register(MessageTypes.MsgDispose, HandleDisposeMessage);
            Messenger.Instance.Register(MessageTypes.MsgUpdateSongCount, new Action<Message>(HandleUpdateSongCountMessage));
            Messenger.Instance.Register(MessageTypes.MsgImportFolder, new Action<Message>(HandleImportFolder));
        }

        #endregion Contructor

        #region Properties
        private List<Mediafile> SelectedItems { get; } = new List<Mediafile>();
        public List<string> AlphabetList
        {
            get => _alphabetList;
            private set => Set(ref _alphabetList, value);
        }
        public bool IsMultiSelectModeEnabled
        {
            get => _isMultiSelectModeEnabled;
            private set => Set(ref _isMultiSelectModeEnabled, value);
        }
        public Mediafile SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }

        public int SongCount
        {
            get => _songCount;
            private set => Set(ref _songCount, value);
        }

        /// <summary>
        /// Gets or sets a grouped observable collection of Tracks/Mediafiles. <seealso cref="GroupedObservableCollection{TKey, TElement}"/>
        /// </summary>
        public GroupedObservableCollection<IGroupKey, Mediafile> TracksCollection
        {
            get => _tracksCollection ?? (_tracksCollection =
                       new GroupedObservableCollection<IGroupKey, Mediafile>(GetSortFunction("FolderPath")));
            private set
            {
                Set(ref _tracksCollection, value);
                Messenger.Instance.NotifyColleagues(MessageTypes.MsgLibraryLoaded, new List<object> { TracksCollection, _grouped });
            }
        }

        public CollectionViewSource ViewSource
        {
            get => _viewSource;
            set => Set(ref _viewSource, value);
        }

        private string Genre
        {
            get => _genre;
            set => Set(ref _genre, value);
        }

        /// <summary>
        /// Gets or Sets a flyout for genres. This is a dynamic control bound to <see cref="LibraryView"/>.
        /// </summary>
        private MenuFlyout GenreFlyout
        {
            get => _genreFlyout;
            set => Set(ref _genreFlyout, value);
        }

        /// <summary>
        /// Gets or Sets <see cref="Mediafile"/> for this ViewModel
        /// </summary>
        public bool IsLibraryLoading
        {
            get => _isLibraryLoading;
            set => Set(ref _isLibraryLoading, value);
        }

        private LibraryService LibraryService
        {
            get => _libraryservice ?? (_libraryservice =
                       new LibraryService(new KeyValueStoreDatabaseService(SharedLogic.Instance.DatabasePath, "Tracks")));
            set => Set(ref _libraryservice, value);
        }
        private PlaylistService PlaylistService =>
            _playlistService ?? (_playlistService = new PlaylistService(
                new KeyValueStoreDatabaseService(
                    SharedLogic.Instance.DatabasePath,
                    "Playlists")));
        private string Sort
        {
            get => _sort;
            set
            {
                Set(ref _sort, value);
                SettingsHelper.SaveLocalSetting("Sort", _sort);
            }
        }
        #endregion Properties

        #region Commands

        #region Definitions
        private RelayCommand _addToFavoritesCommand;
        private DelegateCommand _changeSelectionModeCommand;
        private RelayCommand _deleteCommand;
        private RelayCommand _initCommand;
        private RelayCommand _playCommand;
        private RelayCommand _refreshViewCommand;
        private RelayCommand _relocateSongCommand;
        private RelayCommand _stopAfterCommand;
        private DelegateCommand _importFolderCommand;
        public DelegateCommand ImportFolderCommand { get { if (_importFolderCommand == null) { _importFolderCommand = new DelegateCommand(ImportFolder); } return _importFolderCommand; } }

        public ICommand AddToFavoritesCommand
        {
            get
            { if (_addToFavoritesCommand == null) { _addToFavoritesCommand = new RelayCommand(AddToFavorites); } return _addToFavoritesCommand; }
        }

        public ICommand ChangeSelectionModeCommand
        {
            get
            { if (_changeSelectionModeCommand == null) { _changeSelectionModeCommand = new DelegateCommand(ChangeSelectionMode); } return _changeSelectionModeCommand; }
        }
        
        public ICommand DeleteCommand
        {
            get
            { if (_deleteCommand == null) { _deleteCommand = new RelayCommand(param => Delete(param)); } return _deleteCommand; }
        }
        
        public ICommand InitCommand
        {
            get
            { if (_initCommand == null) { _initCommand = new RelayCommand(param => Init(param)); } return _initCommand; }
        }

        public ICommand PlayCommand
        {
            get
            { if (_playCommand == null) { _playCommand = new RelayCommand(param => Play(param)); } return _playCommand; }
        }
        
        public ICommand RefreshViewCommand
        {
            get
            { if (_refreshViewCommand == null) { _refreshViewCommand = new RelayCommand(param => RefreshView(param)); } return _refreshViewCommand; }
        }

        public ICommand RelocateSongCommand
        {
            get
            { if (_relocateSongCommand == null) { _relocateSongCommand = new RelayCommand(RelocateSong); } return _relocateSongCommand; }
        }

        public ICommand StopAfterCommand
        {
            get
            { if (_stopAfterCommand == null) { _stopAfterCommand = new RelayCommand(param => StopAfter(param)); } return _stopAfterCommand; }
        }
        #endregion Definitions

        #region Implementations
        /// <summary>
        /// Loads songs from a specified folder into the library. <seealso cref="LoadCommand"/>
        /// </summary>
        public async void ImportFolder()
        {
            try
            {
                var musicLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);
                StorageFolder folder = null;
                if (musicLibrary != null)
                {
                    folder = await musicLibrary.RequestAddFolderAsync();
                }
                else
                {
                    BLogger.I("Music Library is not declared on this device. Using Folder Picker to import folder.");
                    FolderPicker folderPicker = new FolderPicker()
                    {
                        CommitButtonText = "Import folder",
                        ViewMode = PickerViewMode.List,
                        SuggestedStartLocation = PickerLocationId.MusicLibrary,
                    };
                    folderPicker.FileTypeFilter.Add("*");
                    folder = await folderPicker.PickSingleFolderAsync();
                }
                if (folder != null)
                {
                    StorageApplicationPermissions.FutureAccessList.Add(folder);
                    
                    await LibraryHelper.ImportFolderIntoLibraryAsync(folder).ConfigureAwait(false);
                }
            }
            catch (UnauthorizedAccessException)
            {
                await SharedLogic.Instance.NotificationManager.ShowMessageAsync("You are not authorized to access this folder. Please choose another folder or try again.");
            }
        }
        private async void AddToFavorites(object para)
        {
            if (para is Mediafile mediafile)
            {
                mediafile.IsFavorite = true;
                await LibraryService.UpdateMediafile(mediafile);
            }
        }
        /// <summary>
        /// Relocates song to a new location. We only update _id, Path and Length of the song.
        /// </summary>
        /// <param name="para">The Mediafile to relocate</param>
        private async void RelocateSong(object para)
        {
            if (para is Mediafile mediafile)
            {
                FileOpenPicker openPicker = new FileOpenPicker
                {
                    CommitButtonText = "Relocate Song"
                };
                foreach (var extenstion in new string[] { ".mp3", ".wav", ".ogg", ".flac", ".m4a", ".aif", ".wma", ".aac" })
                {
                    openPicker.FileTypeFilter.Add(extenstion);
                }
                var newFile = await openPicker.PickSingleFileAsync();
                if (newFile != null)
                {
                    var newMediafile = await TagReaderHelper.CreateMediafile(newFile);
                    TracksCollection.Elements.GetSongByPath(mediafile.Path).Length = newMediafile.Length;
                    TracksCollection.Elements.GetSongByPath(mediafile.Path).Id = newMediafile.Id;
                    TracksCollection.Elements.GetSongByPath(mediafile.Path).Path = newMediafile.Path;
                    await LibraryService.UpdateMediafile(TracksCollection.Elements.First(t => t.Id == mediafile.Id));
                }
            }
        }
        private void ChangeSelectionMode()
        {
            IsMultiSelectModeEnabled = !IsMultiSelectModeEnabled;
        }

        /// <summary>
        /// Deletes a song from the FileCollection. <seealso cref="DeleteCommand"/>
        /// </summary>
        /// <param name="path"><see cref="Mediafile"/> to delete.</param>
        private async void Delete(object path)
        {
            try
            {
                var index = 0;
                if (SelectedItems.Count > 0)
                {
                    foreach (var item in SelectedItems)
                    {
                        index = TracksCollection.Elements.IndexOf(item);
                        TracksCollection.RemoveItem(item);
                        await LibraryService.RemoveMediafile(item);
                        SongCount--;
                        break;
                    }
                }

                if (TracksCollection.Elements.Count > 0)
                {
                    await Task.Delay(100);
                    SelectedItem = index < TracksCollection.Elements.Count ? TracksCollection.Elements.ElementAt(index) : TracksCollection.Elements.ElementAt(index - 1);
                }
            }
            catch (Exception ex)
            {
                BLogger.E("Error occured while deleting a song from collection and list.", ex);
            }
        }

        private async void Init(object para)
        {
            IsLibraryLoading = true;
            if (ViewSource == null)
            {
                ViewSource = ((Grid)para).Resources["Source"] as CollectionViewSource;
            }

            await RefreshSourceAsync().ConfigureAwait(false);

            if (_source == null)
            {
                if (Sort != "Unsorted")
                {
                    await LoadCollectionAsync(GetSortFunction(Sort), true).ConfigureAwait(false);
                }
                else
                {
                    await LoadCollectionAsync(GetSortFunction("FolderPath"), false).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Plays the selected file. <seealso cref="PlayCommand"/>
        /// </summary>
        /// <param name="path"><see cref="Mediafile"/> to play.</param>
        private async void Play(object path)
        {
            var mediaFile = await GetMediafileFromParameterAsync(path, true);
            if (mediaFile != null)
            {
                Messenger.Instance.NotifyColleagues(MessageTypes.MsgPlaySong, new List<object> { mediaFile, true, _isPlayingFromPlaylist });
            }
        }

        /// <summary>
        /// Refreshes the view with new sorting order and/or filtering. <seealso cref="RefreshViewCommand"/>
        /// </summary>
        /// <param name="para"><see cref="MenuFlyoutItem"/> to get sorting/filtering base from.</param>
        private async void RefreshView(object para)
        {
            var selectedItem = para as MenuFlyoutItem;
            if (selectedItem?.Tag.ToString() == "genre")
            {
                Genre = selectedItem.Text;
                await RefreshView(Genre, null, false).ConfigureAwait(false);
            }
            else
            {
                await RefreshView(null, selectedItem?.Tag.ToString()).ConfigureAwait(false);
            }
        }

        private async void StopAfter(object path)
        {
            var mediaFile = await GetMediafileFromParameterAsync(path);
            if (mediaFile != null)
            {
                Messenger.Instance.NotifyColleagues(MessageTypes.MsgStopAfterSong, mediaFile);
            }
        }
        #endregion Implementations

        #endregion Commands

        #region Methods

        public async void DropFiles(object sender, DragEventArgs e)
        {
            if (!e.DataView.Contains(StandardDataFormats.StorageItems)) return;
            var items = await e.DataView.GetStorageItemsAsync();
            if (!items.Any()) return;
            foreach (var item in items)
            {
                if (!item.IsOfType(StorageItemTypes.File) || StorageExtensions.IsItemPotentialMediafile(item))
                {
                    if (!item.IsOfType(StorageItemTypes.Folder)) continue;
                    await LibraryHelper.ImportFolderIntoLibraryAsync(((StorageFolder)item));
                }
                else
                {
                    var path = item.Path;
                    //var tempList = new List<Mediafile>();
                    if (!TracksCollection.Elements.All(t => t.Path != path)) continue;
                    try
                    {
                        var mp3File = await TagReaderHelper.CreateMediafile(item as StorageFile);
                        await LibraryHelper.SaveSingleFileAlbumArtAsync(mp3File).ConfigureAwait(false);
                        SharedLogic.Instance.AddMediafile(mp3File);
                    }
                    catch (Exception ex)
                    {
                        BLogger.E("Error occured while drag/drop operation.", ex);
                    }
                }
            }
            Messenger.Instance.NotifyColleagues(MessageTypes.MsgAddAlbums, "");
        }

        public async Task SplitList(int nSize = 30)
        {
            for (int i = 0; i < SongCount; i += nSize)
            {
                TracksCollection.AddRange(await LibraryService.GetRangeOfMediafiles(i, Math.Min(nSize, SongCount - i)).ConfigureAwait(false));
            }
        }

        private static Func<Mediafile, IGroupKey> GetSortFunction(string propName)
        {
            Func<Mediafile, IGroupKey> f = null;
            switch (propName)
            {
                case "Title":
                    //determine whether the Title, by which groups are made, start with number, letter or symbol. On the basis of that we define the Key for each Group.
                    f = t =>
                    {
                        if (t.Title.StartsWithLetter())
                        {
                            return new TitleGroupKey() { Key = t.Title[0].ToString().ToUpper(), FirstElement = t };
                        }

                        if (t.Title.StartsWithNumber())
                        {
                            return new TitleGroupKey() { Key = "#", FirstElement = t };
                        }

                        if (t.Title.StartsWithSymbol())
                        {
                            return new TitleGroupKey() { Key = "&", FirstElement = t };
                        }

                        return new TitleGroupKey() { Key = t.Title, FirstElement = t };
                    };
                    break;

                case "Year":
                    f = t => new TitleGroupKey() { Key = string.IsNullOrEmpty(t.Year) ? "Unknown Year" : t.Year, FirstElement = t };
                    break;

                case "Length":
                    string[] timeformats = { @"m\:ss", @"mm\:ss", @"h\:mm\:ss", @"hh\:mm\:ss" };
                    f = t => new TitleGroupKey()
                    {
                        Key = string.IsNullOrEmpty(t.Length) || t.Length == "0:00"
                    ? "Unknown Length"
                    : Math.Round(TimeSpan.ParseExact(t.Length, timeformats, CultureInfo.InvariantCulture).TotalMinutes) + " minutes"
                    , FirstElement = t
                    };
                    break;

                case "TrackNumber":
                    f = t => new TitleGroupKey() { Key = t.TrackNumber == 0 ? "Unknown Track No." : t.TrackNumber.ToString(), FirstElement = t };
                    break;

                case "FolderPath":
                    f = t => new TitleGroupKey() { Key = string.IsNullOrEmpty(t.FolderPath) ? "Unknown Folder" : new DirectoryInfo(t.FolderPath).FullName, FirstElement = t };
                    break;

                case "Album":
                    f = t => new AlbumGroupKey() { Key = t.Album, FirstElement = t };
                    break;

                case "LeadArtist":
                    f = t => new ArtistGroupKey() { Key = t.LeadArtist };
                    break;
                case "Genre":
                    f = t => new TitleGroupKey() { Key = string.IsNullOrEmpty(t.Genre) ? "Unknown Genre" : t.Genre, FirstElement = t };
                    break;
            }
            return f;
        }

        private async Task ChangeView(string header, bool group, object src)
        {
            await BreadDispatcher.InvokeAsync(() =>
            {
                ViewSource.Source = null;
                //Header = header;
                _grouped = group;
                _source = src;
                _libgrouped = ViewSource.IsSourceGrouped;
                var tMediaFile = src as ThreadSafeObservableCollection<Mediafile>;
                if (tMediaFile?.Any() == true && SharedLogic.Instance.Player.CurrentlyPlayingFile != null && tMediaFile.FirstOrDefault(t => t.Path == SharedLogic.Instance.Player.CurrentlyPlayingFile?.Path) != null)
                {
                    tMediaFile.FirstOrDefault(t => t.Path == SharedLogic.Instance.Player.CurrentlyPlayingFile?.Path).State = PlayerState.Playing;
                }
            });
        }

        /// <summary>
        /// Creates genre menu.
        /// </summary>
        private async Task CreateGenreMenu()
        {
            await BreadDispatcher.InvokeAsync(() =>
            {
                GenreFlyout = Application.Current.Resources["GenreFlyout"] as MenuFlyout;
                Genre = "All genres";
                GenreFlyout?.Items?.Add(CreateMenuItem("All genres"));
                var genres = TracksCollection.Elements.GroupBy(t => t.Genre);
                foreach (var genre in genres)
                {
                    if (GenreFlyout != null && (genre.Key != null && genre.Key != "NaN" && GenreFlyout.Items.All(t => ((MenuFlyoutItem)t).Text != genre.Key)))
                    {
                        GenreFlyout.Items?.Add(CreateMenuItem(genre.Key));
                    }
                }
            });
        }

        /// <summary>
        /// Creates menu item. <seealso cref="MenuFlyoutItem"/>
        /// </summary>
        /// <param name="genre">The text of the item.</param>
        /// <returns><see cref="MenuFlyoutItem"/></returns>
        private MenuFlyoutItem CreateMenuItem(string genre)
        {
            var item = new MenuFlyoutItem
            {
                Text = genre,
                Command = RefreshViewCommand
            };
            item.CommandParameter = item;
            item.Tag = "genre";
            return item;
        }

        private async void FillKeys()
        {
            foreach (var group in TracksCollection)
            {
                if (group.Key is TitleGroupKey titleGroupKey)
                {
                    titleGroupKey.TotalAlbums = group.GroupBy(t => t.Album).Count();
                    titleGroupKey.TotalArtists = group.GroupBy(t => t.LeadArtist).Count();
                    titleGroupKey.TotalPlays = group.Sum(t => t.PlayCount);
                }
                else if(group.Key is ArtistGroupKey artistGroupKey)
                {
                    artistGroupKey.FirstElement = await SharedLogic.Instance.AlbumArtistService.GetArtistAsync(artistGroupKey.Key.ToLower());
                }
                else
                    return;
            }
        }

        private async Task<Mediafile> GetMediafileFromParameterAsync(object path, bool sendUpdateMessage = false)
        {
            if (path is Mediafile mediaFile)
            {
                _isPlayingFromPlaylist = false;
                return mediaFile;
            }
            else if (path is IEnumerable<Mediafile> tmediaFile)
            {
                var col = new ThreadSafeObservableCollection<Mediafile>(tmediaFile);
                SendLibraryLoadedMessage(col, sendUpdateMessage);
                return col[0];
            }
            else if (path is Playlist playlist)
            {
                var songList = new ThreadSafeObservableCollection<Mediafile>(await PlaylistService.GetTracksAsync(playlist.Id));
                SendLibraryLoadedMessage(songList, sendUpdateMessage);
                return songList[0];
            }
            else if (path is Album album)
            {
                var songList = new ThreadSafeObservableCollection<Mediafile>(await LibraryService.Query(album.TextSearchKey));
                SendLibraryLoadedMessage(songList, sendUpdateMessage);
                return songList[0];
            }
            else if (path is Artist artist)
            {
                var songList = new ThreadSafeObservableCollection<Mediafile>(await LibraryService.Query(artist.TextSearchKey));
                SendLibraryLoadedMessage(songList, sendUpdateMessage);
                return songList[0];
            }
            return null;
        }

        private void GetSettings()
        {
            Sort = SettingsHelper.GetLocalSetting<string>("Sort", "Unsorted");
        }

        private async Task LoadCollectionAsync(Func<Mediafile, IGroupKey> sortFunc, bool group)
        {
            await BreadDispatcher.InvokeAsync(async () =>
            {
                _grouped = group;
                TracksCollection = new GroupedObservableCollection<IGroupKey, Mediafile>(sortFunc);
                TracksCollection.CollectionChanged += TracksCollection_CollectionChanged;

                SongCount = LibraryService.SongCount;
                if (group)
                {
                    ViewSource.Source = TracksCollection;
                }
                else
                {
                    ViewSource.Source = TracksCollection.Elements;
                }

                ViewSource.IsSourceGrouped = group;
                // await SplitList(300).ConfigureAwait(false);
                TracksCollection.AddRange(await LibraryService.GetAllMediafiles().ConfigureAwait(false));

                IsLibraryLoading = false;
            });
        }

        private async Task RefreshSourceAsync()
        {
            await BreadDispatcher.InvokeAsync(() =>
            {
                if (_source == null)
                {
                    return;
                }

                if (_grouped && _source == TracksCollection.Elements)
                {
                    ViewSource.Source = TracksCollection;
                }
                else
                {
                    ViewSource.Source = _source;
                }
                
                ViewSource.IsSourceGrouped = _grouped;
            });
        }

        /// <summary>
        /// Refresh the view, based on filters and sorting mechanisms.
        /// </summary>
        private async Task RefreshView(string genre = "All genres", string propName = "Title", bool doOrderFiles = true)
        {
            if (doOrderFiles)
            {
                Sort = propName;
                if (propName != "Unsorted")
                {
                    await BreadDispatcher.InvokeAsync(async () =>
                    {
                        if (_files == null)
                        {
                            _files = TracksCollection.Elements;
                        }

                        _grouped = true;
                        TracksCollection = new GroupedObservableCollection<IGroupKey, Mediafile>(GetSortFunction(propName));
                        ViewSource.Source = TracksCollection;
                        ViewSource.IsSourceGrouped = true;
                        TracksCollection.AddRange(_files);
                        UpdateJumplist(propName);
                        await RemoveDuplicateGroups();
                    });
                }
                else
                {
                    ViewSource.Source = TracksCollection.Elements;
                    ViewSource.IsSourceGrouped = false;
                    _grouped = false;
                    Messenger.Instance.NotifyColleagues(MessageTypes.MsgLibraryLoaded, new List<object> { TracksCollection, _grouped });
                }
            }
            else
            {
                Genre = genre;
                ThreadSafeObservableCollection<Mediafile> filteredSongsCollection = new ThreadSafeObservableCollection<Mediafile>();
                if (genre != "All genres")
                {
                    var results = await LibraryService.Query(genre);
                    filteredSongsCollection.AddRange(results.ToList());
                }
                else
                {
                    filteredSongsCollection.AddRange(_oldItems);
                }

                await ChangeView("Music Collection", false, filteredSongsCollection);
                await RefreshSourceAsync();
            }
        }

        private async Task RemoveDuplicateGroups()
        {
            //the only workaround to remove the first group which is a 'false' duplicate really.
            await BreadDispatcher.InvokeAsync(() =>
            {
                if (ViewSource.IsSourceGrouped)
                {
                    UpdateJumplist(Sort);
                    ViewSource.IsSourceGrouped = false;
                    ViewSource.IsSourceGrouped = true;
                    FillKeys();
                }
            });
        }

        private void SendLibraryLoadedMessage(object payload, bool sendMessage)
        {
            if (!sendMessage)
            {
                return;
            }

            Messenger.Instance.NotifyColleagues(MessageTypes.MsgLibraryLoaded, payload);
            _isPlayingFromPlaylist = true;
        }
        private async void UpdateJumplist(string propName)
        {
            try
            {
                switch (propName)
                {
                    case "Year":
                    case "TrackNumber":
                        AlphabetList = TracksCollection.Keys.DistinctBy(t => t.Key).Select(t => t.Key).ToList();
                        break;

                    case "Length":
                        AlphabetList = TracksCollection.Keys.Select(t => t.Key.Replace(" minutes", "")).DistinctBy(a => a).ToList();
                        break;

                    case "FolderPath":
                        AlphabetList = TracksCollection.Keys.Select(t => new DirectoryInfo(t.Key).Name.Remove(1)).DistinctBy(t => t).ToList();
                        break;

                    default:
                        AlphabetList = "&#ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray().Select(x => x.ToString()).ToList();
                        break;
                }
                AlphabetList.Sort();
            }
            catch (ArgumentOutOfRangeException ex)
            {
                await SharedLogic.Instance.NotificationManager.ShowMessageAsync("Unable to update jumplist due to some problem with TracksCollection. ERROR INFO: " + ex.Message);
            }
        }
        #endregion Methods        

        #region Disposable

        private void Reset()
        {
            LibraryService.Dispose();
            LibraryService = null;
            TracksCollection.Clear();
            GenreFlyout?.Items?.Clear();
            _oldItems = null;
            SharedLogic.Instance.OptionItems.Clear();
            SongCount = -1;
        }

        #endregion Disposable

        #region Library Methods

        /// <summary>
        /// Loads library from the database file.
        /// </summary>
        private void LoadLibrary()
        {
            GetSettings();
            UpdateJumplist("Title");
        }

        #endregion Library Methods

        #region Events
        public void PlayOnTap(object sender, TappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType == PointerDeviceType.Touch && !IsMultiSelectModeEnabled)
            {
                Play(((FrameworkElement)e.OriginalSource).DataContext);
            }
        }

        public void SelectionChanged(object para, SelectionChangedEventArgs selectionEvent)
        {
            if (selectionEvent.RemovedItems.Count > 0)
            {
                foreach (Mediafile toRemove in selectionEvent.RemovedItems)
                {
                    toRemove.IsSelected = false;
                    SelectedItems.Remove(toRemove);
                }
            }
            if (selectionEvent.AddedItems.Count > 0)
            {
                foreach (Mediafile item in selectionEvent.AddedItems)
                {
                    item.IsSelected = true;
                    SelectedItems.Add(item);
                }
            }
        }        

        private async void LibraryViewModel_MusicLibraryLoaded(object sender, RoutedEventArgs e)
        {
            if (!_libraryLoaded)
            {
                _libraryLoaded = true;
                await CreateGenreMenu().ConfigureAwait(false);
                BLogger.I("Library successfully loaded!");
                Messenger.Instance.Register(MessageTypes.MsgImportFolder, new Action<Message>(HandleImportFolder));
                await SharedLogic.Instance.NotificationManager.ShowMessageAsync("Library successfully loaded!", 4);
                Messenger.Instance.NotifyColleagues(MessageTypes.MsgLibraryLoaded, new List<object> { TracksCollection, _grouped });
            }
        }

        private async void TracksCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (TracksCollection.Elements.Count == SongCount)
            {
                await RemoveDuplicateGroups().ConfigureAwait(false);
                await BreadDispatcher.InvokeAsync(() =>
                {
                    MusicLibraryLoaded?.Invoke(this, new RoutedEventArgs());
                });
                _oldItems = TracksCollection.Elements;
                TracksCollection.CollectionChanged -= TracksCollection_CollectionChanged;
            }
        }
        #endregion Events

        public event OnMusicLibraryLoaded MusicLibraryLoaded;
    }
}
