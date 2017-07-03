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
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using BreadPlayer.Common;
using BreadPlayer.Core;
using BreadPlayer.Core.Common;
using BreadPlayer.Core.Extensions;
using BreadPlayer.Core.Models;
using BreadPlayer.Database;
using BreadPlayer.Dialogs;
using BreadPlayer.Extensions;
using BreadPlayer.Messengers;
using BreadPlayer.Services;
using SplitViewMenu;

namespace BreadPlayer.ViewModels
{
    /// <summary>
    /// ViewModel for Library View (Severe cleanup and documentation needed.)
    /// </summary>
    public class LibraryViewModel : ViewModelBase
    {
        #region Fields
        private IEnumerable<Mediafile> _files;
        private IEnumerable<Mediafile> _oldItems;
        private bool _grouped;
        private bool _libgrouped;
        private object _source;
        private bool _isPlayingFromPlaylist;
        private bool _libraryLoaded;
        #endregion

        #region MessageHandling

        private void HandleSearchStartedMessage(Message message)
        {
            if (message.Payload != null)
            {
                message.HandledStatus = MessageHandledStatus.HandledCompleted;
                //if (!Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1))
                //    Header = message.Payload.ToString();
            }
        }

        private void HandleDisposeMessage()
        {
            Reset();
        }

        private async void HandleUpdateSongCountMessage(Message message)
        {
            if (message.Payload is short || message.Payload is Int32)
            {
                message.HandledStatus = MessageHandledStatus.HandledCompleted;
                SongCount = Convert.ToInt32(message.Payload);
                IsLibraryLoading = true;
            }
            else
            {
                IsLibraryLoading = false;
                await SharedLogic.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    MusicLibraryLoaded?.Invoke(this, new RoutedEventArgs());
                });
            }
        }

        private async void HandleAddPlaylistMessage(Message message)
        {
            Playlist plist = message.Payload as Playlist;

            if (plist == null) return;

            message.HandledStatus = MessageHandledStatus.HandledCompleted;

            await AddPlaylistAsync(plist, false);
        }

        private void HandlePlaySongMessage(Message message)
        {
            if (!(message.Payload is Mediafile song)) return;

            message.HandledStatus = MessageHandledStatus.HandledCompleted;

            PlayCommand.Execute(song);
        }
        #endregion

        #region Contructor
        /// <summary>
        /// Creates a new instance of LibraryViewModel
        /// </summary>
        public LibraryViewModel()
        {
           // Header = "Music Collection";
            MusicLibraryLoaded += LibraryViewModel_MusicLibraryLoaded;
            SharedLogic.Dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
            RecentlyPlayedCollection.CollectionChanged += Elements_CollectionChanged;
            LoadLibrary();

            Messenger.Instance.Register(MessageTypes.MsgPlaySong, new Action<Message>(HandlePlaySongMessage));
            Messenger.Instance.Register(MessageTypes.MsgDispose, HandleDisposeMessage);
            Messenger.Instance.Register(MessageTypes.MsgAddPlaylist, new Action<Message>(HandleAddPlaylistMessage));
            Messenger.Instance.Register(MessageTypes.MsgUpdateSongCount, new Action<Message>(HandleUpdateSongCountMessage));
            Messenger.Instance.Register(MessageTypes.MsgSearchStarted, new Action<Message>(HandleSearchStartedMessage));
        }
        #endregion

        #region Properties 
        
        private List<string> _alphabetList;
        public List<string> AlphabetList
        {
            get => _alphabetList;
            private set => Set(ref _alphabetList, value);
        }

        private LibraryService _libraryservice;

        private LibraryService LibraryService
        {
            get => _libraryservice ?? (_libraryservice =
                       new LibraryService(new KeyValueStoreDatabaseService(SharedLogic.DatabasePath, "Tracks",
                           "TracksText")));
            set => Set(ref _libraryservice, value);
        }

        private PlaylistService _playlistService;

        private PlaylistService PlaylistService => 
            _playlistService ?? (_playlistService = new PlaylistService(
                new KeyValueStoreDatabaseService(
                    SharedLogic.DatabasePath, 
                    "Playlists",
                    "PlaylistsText")));

        private CollectionViewSource _viewSource;

        private CollectionViewSource ViewSource
        {
            get => _viewSource;
            set => Set(ref _viewSource, value);
        }

        private bool _isMultiSelectModeEnabled;
        public bool IsMultiSelectModeEnabled
        {
            get => _isMultiSelectModeEnabled;
            private set => Set(ref _isMultiSelectModeEnabled, value);
        }

        private string _genre;

        private string Genre
        {
            get => _genre;
            set => Set(ref _genre, value);
        }

        private string _sort = "Unsorted";

        private string Sort
        {
            get => _sort;
            set
            {
                Set(ref _sort, value);
                ApplicationData.Current.RoamingSettings.Values["Sort"] = Sort;
            }
        }

        private Mediafile _selectedItem;
        public Mediafile SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }

        private int _songCount;

        public int SongCount
        {
            get => _songCount;
            private set => Set(ref _songCount, value);
        }

        private ThreadSafeObservableCollection<Mediafile> _mostEatenCollection;
        private ThreadSafeObservableCollection<Mediafile> MostEatenSongsCollection => 
            _mostEatenCollection ?? (_mostEatenCollection = new ThreadSafeObservableCollection<Mediafile>());

        private ThreadSafeObservableCollection<Mediafile> _favoriteSongsCollection;
        private ThreadSafeObservableCollection<Mediafile> FavoriteSongsCollection =>
            _favoriteSongsCollection ?? (_favoriteSongsCollection = new ThreadSafeObservableCollection<Mediafile>());

        private ThreadSafeObservableCollection<Mediafile> _recentlyAddedSongsCollection;

        private ThreadSafeObservableCollection<Mediafile> RecentlyAddedSongsCollection => 
            _recentlyAddedSongsCollection ?? (_recentlyAddedSongsCollection = new ThreadSafeObservableCollection<Mediafile>());

        private ThreadSafeObservableCollection<Mediafile> _recentlyPlayedCollection;
        private ThreadSafeObservableCollection<Mediafile> RecentlyPlayedCollection => 
            _recentlyPlayedCollection ?? (_recentlyPlayedCollection = new ThreadSafeObservableCollection<Mediafile>());

        private GroupedObservableCollection<string, Mediafile> _tracksCollection;
        /// <summary>
        /// Gets or sets a grouped observable collection of Tracks/Mediafiles. <seealso cref="GroupedObservableCollection{TKey, TElement}"/>
        /// </summary>
        public GroupedObservableCollection<string, Mediafile> TracksCollection
        {
            get => _tracksCollection ?? (_tracksCollection =
                       new GroupedObservableCollection<string, Mediafile>(GetSortFunction("FolderPath")));
            private set
            {
                Set(ref _tracksCollection, value);
                Messenger.Instance.NotifyColleagues(MessageTypes.MsgLibraryLoaded, new List<object> { TracksCollection, _grouped });
            }
        }

        private MenuFlyout _genreFlyout;
        /// <summary>
        /// Gets or Sets a flyout for genres. This is a dynamic control bound to <see cref="LibraryView"/>.
        /// </summary>
        private MenuFlyout GenreFlyout
        {
            get => _genreFlyout;
            set => Set(ref _genreFlyout, value);
        }

        private bool _isLibraryLoading;
        /// <summary>
        /// Gets or Sets <see cref="Mediafile"/> for this ViewModel
        /// </summary>
        private bool IsLibraryLoading
        {
            get => _isLibraryLoading;
            set => Set(ref _isLibraryLoading, value);
        }
        #endregion

        #region Commands 

        #region Definitions

        private RelayCommand _deleteCommand;
        private RelayCommand _playCommand;
        private RelayCommand _stopAfterCommand;
        private RelayCommand _addtoplaylistCommand;
        private RelayCommand _refreshViewCommand;
        private RelayCommand _initCommand;
        private RelayCommand _relocateSongCommand;
        private DelegateCommand _changeSelectionModeCommand;
        private RelayCommand _addToFavoritesCommand;
        public ICommand RelocateSongCommand
        {
            get
            { if (_relocateSongCommand == null) { _relocateSongCommand = new RelayCommand(RelocateSong); } return _relocateSongCommand; }
        }
        public ICommand ChangeSelectionModeCommand
        {
            get
            { if (_changeSelectionModeCommand == null) { _changeSelectionModeCommand = new DelegateCommand(ChangeSelectionMode); } return _changeSelectionModeCommand; }
        }
        public ICommand AddToFavoritesCommand
        {
            get
            { if (_addToFavoritesCommand == null) { _addToFavoritesCommand = new RelayCommand(AddToFavorites); } return _addToFavoritesCommand; }
        }

        /// <summary>
        /// Gets command for initialization. This calls the <see cref="Init(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand InitCommand
        {
            get
            { if (_initCommand == null) { _initCommand = new RelayCommand(param => Init(param)); } return _initCommand; }
        }
      
        /// <summary>
        /// Gets AddToPlaylist command. This calls the <see cref="AddToPlaylist(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand AddToPlaylistCommand
        {
            get
            { if (_addtoplaylistCommand == null) { _addtoplaylistCommand = new RelayCommand(param => AddToPlaylist(param)); } return _addtoplaylistCommand; }
        }
        /// <summary>
        /// Gets refresh command. This calls the <see cref="RefreshView(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand RefreshViewCommand
        {
            get
            { if (_refreshViewCommand == null) { _refreshViewCommand = new RelayCommand(param => RefreshView(param)); } return _refreshViewCommand; }
        }
        /// <summary>
        /// Gets Play command. This calls the <see cref="Play(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand PlayCommand
        {
            get
            { if (_playCommand == null) { _playCommand = new RelayCommand(param => Play(param)); } return _playCommand; }
        }
        
        /// <summary>
        /// Gets Stop command. This calls the <see cref="StopAfter(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand StopAfterCommand
        {
           get
           { if (_stopAfterCommand == null) { _stopAfterCommand = new RelayCommand(param => StopAfter(param)); } return _stopAfterCommand; }
        }

        /// <summary>
        /// Gets Play command. This calls the <see cref="Delete(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand DeleteCommand
        {
            get
            { if (_deleteCommand == null) { _deleteCommand = new RelayCommand(param => Delete(param)); } return _deleteCommand; }
        }
        #endregion

        #region Implementations 
        private async void AddToFavorites(object para)
        {
            var mediaFile = para as Mediafile;
            if (mediaFile != null)
            {
                mediaFile.IsFavorite = true;
                await LibraryService.UpdateMediafile(mediaFile);
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
                foreach (var extenstion in new List<string> { ".mp3", ".wav", ".ogg", ".flac", ".m4a", ".aif", ".wma" })
                {
                    openPicker.FileTypeFilter.Add(extenstion);
                }
                var newFile = await openPicker.PickSingleFileAsync();
                if (newFile != null)
                {
                    var newMediafile = await SharedLogic.CreateMediafile(newFile);
                    TracksCollection.Elements.Single(t => t.Path == mediafile.Path).Length = newMediafile.Length;
                    TracksCollection.Elements.Single(t => t.Path == mediafile.Path).Id = newMediafile.Id;
                    TracksCollection.Elements.Single(t => t.Path == mediafile.Path).Path = newMediafile.Path;
                    await LibraryService.UpdateMediafile(TracksCollection.Elements.Single(t => t.Id == mediafile.Id));
                }
            }
        }
        private void ChangeSelectionMode()
        {
            IsMultiSelectModeEnabled = !IsMultiSelectModeEnabled;
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
        /// <summary>
        /// Deletes a song from the FileCollection. <seealso cref="DeleteCommand"/>
        /// </summary>
        /// <param name="path"><see cref="Mediafile"/> to delete.</param>
        private async void Delete(object path)
        {
            try
            {
                var index = 0;
                if(SelectedItems.Count > 0)
                {
                    foreach (var item in SelectedItems)
                    {
                        index = TracksCollection.Elements.IndexOf(item);
                        TracksCollection.RemoveItem(item);
                        await LibraryService.RemoveMediafile(item);
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
                BLogger.Logger.Error("Error occured while deleting a song from collection and list.", ex);
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
        
        /// <summary>
        /// Plays the selected file. <seealso cref="PlayCommand"/>
        /// </summary>
        /// <param name="path"><see cref="Mediafile"/> to play.</param>
        private async void Play(object path)
        {
            var mediaFile =  await GetMediafileFromParameterAsync(path, true);
            if (mediaFile != null)
            {
                Messenger.Instance.NotifyColleagues(MessageTypes.MsgPlaySong, new List<object> { mediaFile, true, _isPlayingFromPlaylist });
                mediaFile.LastPlayed = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            }
        }

        private async void Init(object para)
        {
            NavigationService.Instance.Frame.Navigated += Frame_Navigated;
            if (ViewSource == null)
            {
                ViewSource = ((Grid) para).Resources["Source"] as CollectionViewSource;
            }

            await RefreshSourceAsync().ConfigureAwait(false);

            if (_source == null && Sort != "Unsorted")
            {
                await LoadCollectionAsync(GetSortFunction(Sort), true).ConfigureAwait(false);
            }
            else if (_source == null && Sort == "Unsorted")
            {
                await LoadCollectionAsync(GetSortFunction("FolderPath"), false).ConfigureAwait(false);
            }
        }
        #endregion

        #endregion
        
        #region Methods 
        private void SendLibraryLoadedMessage(object payload, bool sendMessage)
        {
            if (!sendMessage)
            {
                return;
            }

            Messenger.Instance.NotifyColleagues(MessageTypes.MsgLibraryLoaded, payload);
            _isPlayingFromPlaylist = true;
        }
        private async Task<Mediafile> GetMediafileFromParameterAsync(object path, bool sendUpdateMessage = false)
        {
            if (path is Mediafile mediaFile)
            {
                _isPlayingFromPlaylist = false;
               // SendLibraryLoadedMessage(TracksCollection.Elements, true);
                return mediaFile;
            }
            if (path is IEnumerable<Mediafile> tmediaFile)
            {
                var col = new ThreadSafeObservableCollection<Mediafile>(tmediaFile);
                SendLibraryLoadedMessage(col, sendUpdateMessage);
                return col[0];
            }
            if (path is Playlist playlist)
            { 
                var songList = new ThreadSafeObservableCollection<Mediafile>(await PlaylistService.GetTracksAsync(playlist.Id));
                SendLibraryLoadedMessage(songList, sendUpdateMessage);
                return songList[0];
            }
            if(path is Album album)
            {
                var songList = new ThreadSafeObservableCollection<Mediafile>(await LibraryService.Query(album.AlbumName + " " + album.Artist));
                SendLibraryLoadedMessage(songList, sendUpdateMessage);
                return songList[0];
            }
            return null;
        }

        private async Task<ThreadSafeObservableCollection<Mediafile>> GetMostPlayedSongsAsync()
        {
            return await Task.Run(() =>
            {
                MostEatenSongsCollection.AddRange(
                    TracksCollection.Elements.Where(t => t.PlayCount > 1 &&
                                                         MostEatenSongsCollection.All(a => a.Path != t.Path)));
                return MostEatenSongsCollection;
            });
        }

        private async Task<ThreadSafeObservableCollection<Mediafile>> GetRecentlyPlayedSongsAsync()
        {
            return await Task.Run(() =>
            {
                RecentlyPlayedCollection.AddRange(
                    TracksCollection.Elements.Where(t => t.LastPlayed != null
                                                         && (DateTime.Now.Subtract(DateTime.Parse(t.LastPlayed)))
                                                         .Days <= 2
                                                         && RecentlyPlayedCollection.All(a => a.Path != t.Path)));

                return RecentlyPlayedCollection;
            });
        }

        private async Task<ThreadSafeObservableCollection<Mediafile>> GetFavoriteSongs()
        {
            return await Task.Run(() =>
            {
                FavoriteSongsCollection.AddRange(TracksCollection.Elements.Where(t => t.IsFavorite));
                return FavoriteSongsCollection;
            });
        }
        private async Task<ThreadSafeObservableCollection<Mediafile>> GetRecentlyAddedSongsAsync()
        {
            return await Task.Run(() =>
            {
                RecentlyAddedSongsCollection.AddRange(TracksCollection.Elements.Where(item => item.AddedDate != null && (DateTime.Now.Subtract(DateTime.Parse(item.AddedDate))).Days < 3 && !RecentlyAddedSongsCollection.All(t => t.Path == item.Path)));
                return RecentlyAddedSongsCollection;
            });
        }

        private async Task RefreshSourceAsync()
        {
            await SharedLogic.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
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

        private async Task ChangeView(string header, bool group, object src)
        {
            await SharedLogic.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                ViewSource.Source = null;
                //Header = header;
                _grouped = group;
                _source = src;
                _libgrouped = ViewSource.IsSourceGrouped;
                var tMediaFile = src as ThreadSafeObservableCollection<Mediafile>;
                if (tMediaFile?.Any() == true && Player.CurrentlyPlayingFile != null && tMediaFile.FirstOrDefault(t => t.Path == Player.CurrentlyPlayingFile?.Path) != null)
                {
                    tMediaFile.FirstOrDefault(t => t.Path == Player.CurrentlyPlayingFile?.Path).State = PlayerState.Playing;
                }
            });
        }

        private async Task LoadCollectionAsync(Func<Mediafile, string> sortFunc, bool group)
        {
            await SharedLogic.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                _grouped = group;
                TracksCollection = new GroupedObservableCollection<string, Mediafile>(sortFunc);
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
                //await SplitList(TracksCollection, 300).ConfigureAwait(false);
                await TracksCollection.AddRange(await LibraryService.GetAllMediafiles());
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
                    await SharedLogic.Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
                    {
                        if (_files == null)
                        {
                            _files = TracksCollection.Elements;
                        }

                        _grouped = true;
                        TracksCollection = new GroupedObservableCollection<string, Mediafile>(GetSortFunction(propName));
                        ViewSource.Source = TracksCollection;
                        ViewSource.IsSourceGrouped = true;
                        await TracksCollection.AddRange(_files, true, false);
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
            await SharedLogic.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            { 
                if (ViewSource.IsSourceGrouped)
                {
                    UpdateJumplist(Sort);
                    ViewSource.IsSourceGrouped = false;
                    ViewSource.IsSourceGrouped = true;
                }
            });
        }

        private static Func<Mediafile, string> GetSortFunction(string propName)
        {
            Func<Mediafile, string> f;
            switch (propName)
            {
                case "Title":
                    //determine whether the Title, by which groups are made, start with number, letter or symbol. On the basis of that we define the Key for each Group.
                    f = t =>
                    {
                        if (t.Title.StartsWithLetter())
                        {
                            return t.Title[0].ToString().ToUpper();
                        }

                        if (t.Title.StartsWithNumber())
                        {
                            return "#";
                        }

                        if (t.Title.StartsWithSymbol())
                        {
                            return "&";
                        }

                        return t.Title;
                    };
                    break;
                case "Year":
                    f = t => string.IsNullOrEmpty(t.Year) ? "Unknown Year" : t.Year;
                    break;
                case "Length":
                    string[] timeformats = { @"m\:ss", @"mm\:ss", @"h\:mm\:ss", @"hh\:mm\:ss" };
                    f = t => string.IsNullOrEmpty(t.Length) || t.Length == "0:00" 
                    ? "Unknown Length"
                    : Math.Round(TimeSpan.ParseExact(t.Length, timeformats, CultureInfo.InvariantCulture).TotalMinutes) + " minutes";
                    break;
                case "TrackNumber":
                    f = t => string.IsNullOrEmpty(t.TrackNumber) ? "Unknown Track No." : t.TrackNumber;
                    break;
                case "FolderPath":
                    f = t => string.IsNullOrEmpty(t.FolderPath) ? "Unknown Folder" : new DirectoryInfo(t.FolderPath).FullName;
                    break;
                default:
                    f = t => GetPropValue(t, propName) as string; 
                    break;
            }          
            return f;
        }
        private async void UpdateJumplist(string propName)
        {
            try
            {
                switch (propName)
                {
                    case "Year":
                    case "TrackNumber":
                        AlphabetList = TracksCollection.Keys.DistinctBy(t => t).ToList();
                        break;
                    case "Length":
                        AlphabetList = TracksCollection.Keys.Select(t => t.Replace(" minutes", "")).DistinctBy(a => a).ToList();
                        break;
                    case "FolderPath":
                        AlphabetList = TracksCollection.Keys.Select(t => new DirectoryInfo(t).Name.Remove(1)).DistinctBy(t => t).ToList();
                        break;
                    default:
                        AlphabetList = "&#ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray().Select(x => x.ToString()).ToList();
                        break;
                }
                AlphabetList.Sort();
            }
            catch(ArgumentOutOfRangeException ex)
            {
                await NotificationManager.ShowMessageAsync("Unable to update jumplist due to some problem with TracksCollection. ERROR INFO: " + ex.Message);
            }
        }
        
        /// <summary>
        /// Creates genre menu.
        /// </summary>
        private async Task CreateGenreMenu()
        {
            await SharedLogic.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                GenreFlyout = Application.Current.Resources["GenreFlyout"] as MenuFlyout;
                Genre = "All genres";
                GenreFlyout?.Items?.Add(CreateMenuItem("All genres"));
                var genres = TracksCollection.Elements.GroupBy(t => t.Genre);
                foreach (var genre in genres)
                {
                    if (GenreFlyout != null && (genre.Key != null && genre.Key != "NaN" && GenreFlyout.Items.All(t => ((MenuFlyoutItem) t).Text != genre.Key)))
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

        private void GetSettings()
        {
            Sort = RoamingSettingsHelper.GetSetting<string>("Sort", "Unsorted");
        }

        public async void DropFiles(object sender, DragEventArgs e)
        {
            if (!e.DataView.Contains(StandardDataFormats.StorageItems)) return;
            var items = await e.DataView.GetStorageItemsAsync();
            if (!items.Any()) return;
            foreach (var item in items)
            {
                if (!item.IsOfType(StorageItemTypes.File) || Path.GetExtension(item.Path) != ".mp3")
                {
                    if (!item.IsOfType(StorageItemTypes.Folder)) continue;
                    await SharedLogic.SettingsVm.AddFolderToLibraryAsync(
                        await ((StorageFolder)item).CreateFileQueryWithOptions(
                            DirectoryWalker.GetQueryOptions()).GetFilesAsync());
                }
                else
                {
                    var path = item.Path;
                    var tempList = new List<Mediafile>();
                    if (!TracksCollection.Elements.All(t => t.Path != path)) continue;
                    try
                    {
                        var mp3File = await SharedLogic.CreateMediafile(item as StorageFile);
                        await SettingsViewModel.SaveSingleFileAlbumArtAsync(mp3File).ConfigureAwait(false);
                        SharedLogic.AddMediafile(mp3File);
                    }
                    catch (Exception ex)
                    {
                        BLogger.Logger.Error("Error occured while drag/drop operation.", ex);
                    }
                }
            }
            Messenger.Instance.NotifyColleagues(MessageTypes.MsgAddAlbums, "");
        }
        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets name of a property in a class. 
        /// </summary>
        /// <param name="src">Class to search for property.</param>
        /// <param name="propName">Property to search for.</param>
        /// <returns></returns>
        private static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetTypeInfo().GetDeclaredProperty(propName).GetValue(src, null);
        }       

        #endregion

        #region Disposable

        private void Reset()
        {
            LibraryService.Dispose();
            LibraryService = null;
            TracksCollection.Clear();
            RecentlyPlayedCollection.Clear();
            GenreFlyout?.Items?.Clear();
            SharedLogic.PlaylistsItems?.Clear();
            _oldItems = null;
            SharedLogic.OptionItems.Clear();
            SongCount = -1;
        }
        #endregion

        #region Library Methods
        /// <summary>
        /// Loads library from the database file.
        /// </summary>
        private async void LoadLibrary()
        {
            GetSettings();
            SharedLogic.OptionItems.Add(new ContextMenuCommand(AddToPlaylistCommand, "New Playlist"));
            await LoadPlaylists();
            UpdateJumplist("Title");
        }     
      
        #endregion

        #region Playlist Methods

        private async Task LoadPlaylists()
        {
            foreach (var list in await PlaylistService.GetPlaylistsAsync())
            {
                AddPlaylist(list);
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
                    songList = SelectedItems;
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
                if(pList != null)
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
                await Task.Run(async () =>
                {
                    await PlaylistService.InsertTracksAsync(songsToadd.Where(t => !PlaylistService.Exists(t.Id)), list);
                });
            }
        }

        private void AddPlaylist(Playlist playlist)
        {
            var cmd = new ContextMenuCommand(AddToPlaylistCommand, playlist.Name);
            SharedLogic.OptionItems.Add(cmd);
            SharedLogic.PlaylistsItems.Add(new SimpleNavMenuItem
            {
                Arguments = playlist,
                Label = playlist.Name,
                DestinationPage = typeof(PlaylistView),
                Symbol = Symbol.List,
                FontGlyph = "\u0045",
                ShortcutTheme = ElementTheme.Dark,
                HeaderVisibility = Visibility.Collapsed
            });
        }

        private async Task AddPlaylistAsync(Playlist plist, bool addsongs, List<Mediafile> songs = null)
        {
            await SharedLogic.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
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
        #endregion

        #region Events
        private async void TracksCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (TracksCollection.Elements.Count == SongCount)
            {
                await RemoveDuplicateGroups().ConfigureAwait(false);
                await SharedLogic.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    MusicLibraryLoaded?.Invoke(this, new RoutedEventArgs());
                });
                _oldItems = TracksCollection.Elements;
                TracksCollection.CollectionChanged -= TracksCollection_CollectionChanged;
            }
        }
        private async void LibraryViewModel_MusicLibraryLoaded(object sender, RoutedEventArgs e)
        {
            if (!_libraryLoaded)
            {
                _libraryLoaded = true;
                await CreateGenreMenu().ConfigureAwait(false);
                BLogger.Logger.Info("Library successfully loaded!");
                await NotificationManager.ShowMessageAsync("Library successfully loaded!", 4);
                Messenger.Instance.NotifyColleagues(MessageTypes.MsgLibraryLoaded, new List<object> { TracksCollection, _grouped });
            }
        }
        private async void Elements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            await Task.Delay(1000);
            if (RecentlyPlayedCollection.Count <= 100)
            {
                RecentlyPlayedCollection.RemoveAt(RecentlyPlayedCollection.Count + 1);
            }
        }
        private async void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            string param = (e.Parameter ?? string.Empty).ToString();    // e.Parameter can be null and throw exception
            if (e.SourcePageType == typeof(LibraryView))
            {
                switch(param)
                {
                    case "Recent":
                        await ChangeView("Recently Played", false, await GetRecentlyPlayedSongsAsync().ConfigureAwait(false));
                        break;
                    case "MostEaten":
                        await ChangeView("Most Eaten", false, await GetMostPlayedSongsAsync().ConfigureAwait(false));
                        break;
                    case "RecentlyAdded":
                        await ChangeView("Recently Added", false, await GetRecentlyAddedSongsAsync().ConfigureAwait(false));
                        break;
                    case "Favorites":
                        await ChangeView("Favorites", false, await GetFavoriteSongs().ConfigureAwait(false));
                        break;
                    default:
                        await ChangeView("Music Collection", _libgrouped, TracksCollection.Elements);
                        break;
                }
                await RefreshSourceAsync().ConfigureAwait(false); 
            }
            else if (ViewSource.Source != null)
            {
                _source = ViewSource.Source;
                _grouped = ViewSource.IsSourceGrouped;
                ViewSource.Source = null;
            }
        }

        public void PlayOnTap(object sender, TappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType == PointerDeviceType.Touch && !IsMultiSelectModeEnabled)
            {
                Play(((Border) e.OriginalSource).DataContext);
            }
        }

        private List<Mediafile> SelectedItems { get; } = new List<Mediafile>();

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
        #endregion

        public event OnMusicLibraryLoaded MusicLibraryLoaded;
    }

    public delegate void OnMusicLibraryLoaded(object sender, RoutedEventArgs e);    
}
