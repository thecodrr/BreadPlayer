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
using BreadPlayer.Dialogs;
using BreadPlayer.Extensions;
using BreadPlayer.Messengers;
using BreadPlayer.Models;
using BreadPlayer.Service;
using BreadPlayer.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;

namespace BreadPlayer.ViewModels
{
    /// <summary>
    /// ViewModel for Library View (Severe cleanup and documentation needed.)
    /// </summary>
    public class LibraryViewModel : ViewModelBase
    {
        #region Fields        
        ThreadSafeObservableCollection<Playlist> PlaylistCollection = new ThreadSafeObservableCollection<Playlist>();
        ObservableRangeCollection<string> _GenreCollection = new ObservableRangeCollection<string>();
        IEnumerable<Mediafile> files = null;
        public IEnumerable<Mediafile> OldItems;
        bool grouped = false;
        bool libgrouped = false;
        object source;
        bool isPlayingFromPlaylist = false;
        bool libraryLoaded = false;
        #endregion

        #region MessageHandling
        void HandleSearchStartedMessage(Message message)
        {
            if (message.Payload != null)
            {
                message.HandledStatus = MessageHandledStatus.HandledCompleted;
                if (!Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1))
                    Header = message.Payload.ToString();
            }
        }
        void HandleDisposeMessage()
        {
            Reset();
        }
        void HandleUpdateSongCountMessage(Message message)
        {
            if (message.Payload is short || message.Payload is Int32)
            {
                message.HandledStatus = MessageHandledStatus.HandledCompleted;
                SongCount = Convert.ToInt32(message.Payload);
                Status = SongCount.ToString() + " Song(s) Loaded";
                IsLibraryLoading = true;
            }
            else
            {
                IsLibraryLoading = false;
            }
        }
        async void HandleAddPlaylistMessage(Message message)
        {
            var plist = message.Payload as Playlist;
            if (plist != null)
            {
                message.HandledStatus = MessageHandledStatus.HandledCompleted;
                await AddPlaylistAsync(plist, false);
            }
        }
        void HandlePlaySongMessage(Message message)
        {
            if (message.Payload is Mediafile song)
            {
                if (message != null)
                    message.HandledStatus = MessageHandledStatus.HandledCompleted;
                if (song != null)
                {
                    PlayCommand.Execute(song);
                }
            }
        }
        #endregion

        #region Contructor
        /// <summary>
        /// Creates a new instance of LibraryViewModel
        /// </summary>
        public LibraryViewModel()
        {
            Header = "Music Collection";
            MusicLibraryLoaded += LibraryViewModel_MusicLibraryLoaded;
            Dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
            RecentlyPlayedCollection.CollectionChanged += Elements_CollectionChanged;
            LoadLibrary();

            Messenger.Instance.Register(MessageTypes.MSG_PLAY_SONG, new Action<Message>(HandlePlaySongMessage));
            Messenger.Instance.Register(MessageTypes.MSG_DISPOSE, new Action(HandleDisposeMessage));
            Messenger.Instance.Register(MessageTypes.MSG_ADD_PLAYLIST, new Action<Message>(HandleAddPlaylistMessage));
            Messenger.Instance.Register(MessageTypes.MSG_UPDATE_SONG_COUNT, new Action<Message>(HandleUpdateSongCountMessage));
            Messenger.Instance.Register(MessageTypes.MSG_SEARCH_STARTED, new Action<Message>(HandleSearchStartedMessage));
        }
        #endregion

        #region Properties  
        string status = "Loading Library...";
        public string Status
        {
            get { return status; }
            set { Set(ref status, value); }
        }
        private List<string> _alphabetList;
        public List<string> AlphabetList
        {
            get { return _alphabetList; }
            set
            {
                _alphabetList = value;
                OnPropertyChanged();
            }
        }
        
        LiteDB.LiteCollection<Mediafile> recentCol;
        LiteDB.LiteCollection<Mediafile> RecentCollection
        {
            get { if (recentCol == null)
                    recentCol = LibraryService.GetRecentCollection();
                return recentCol; }
            set { Set(ref recentCol, value); }
        }
        
        LibraryService libraryservice;
        public LibraryService LibraryService
        {
            get { if (libraryservice == null)
                    libraryservice = new LibraryService(new DatabaseService());
                return libraryservice; }
            set { Set(ref libraryservice, value); }
        }
        CollectionViewSource viewSource;
        public CollectionViewSource ViewSource
        {
            get { return viewSource; }
            set { Set(ref viewSource, value); }
        }
        bool isMultiSelectModeEnabled = false;
        public bool IsMultiSelectModeEnabled
        {
            get { return isMultiSelectModeEnabled; }
            set { Set(ref isMultiSelectModeEnabled, value); }
        }
        string _header;
        public string Header
        {
            get { return _header; }
            set { Set(ref _header, value); }
        }
        string _genre;
        public string Genre
        {
            get { return _genre; }
            set { Set(ref _genre, value); }
        }
        string _sort = "Unsorted";
        public string Sort
        {
            get { return _sort; }
            set
            {
                Set(ref _sort, value);
                ApplicationData.Current.RoamingSettings.Values["Sort"] = Sort;
            }
        }
        Mediafile selectedItem;
        public Mediafile SelectedItem
        {
            get { return selectedItem; }
            set { Set(ref selectedItem, value); }
        }

        int songCount;
        public int SongCount
        {
            get { return songCount; }
            set { Set(ref songCount, value); }
        }
        /// <summary>
        /// Gets the genre collection in which there are all the genres of all the tracks/Mediafiles in <see cref="FileCollection"/>.
        /// </summary>
        public ObservableRangeCollection<String> GenreCollection
        {
            get { return _GenreCollection; }
        }
        ThreadSafeObservableCollection<Mediafile> _MostEatenCollection;
        /// <summary>
        /// Gets or sets a grouped observable collection of Tracks/Mediafiles. <seealso cref="GroupedObservableCollection{TKey, TElement}"/>
        /// </summary>
        public ThreadSafeObservableCollection<Mediafile> MostEatenSongsCollection
        {
            get { if (_MostEatenCollection == null) _MostEatenCollection = new ThreadSafeObservableCollection<Mediafile>(); return _MostEatenCollection; }
            set { Set(ref _MostEatenCollection, value); }
        }
        ThreadSafeObservableCollection<Mediafile> _RecentlyAddedSongsCollection;
        /// <summary>
        /// Gets or sets a grouped observable collection of Tracks/Mediafiles. <seealso cref="GroupedObservableCollection{TKey, TElement}"/>
        /// </summary>
        public ThreadSafeObservableCollection<Mediafile> RecentlyAddedSongsCollection
        {
            get { if (_RecentlyAddedSongsCollection == null) _RecentlyAddedSongsCollection = new ThreadSafeObservableCollection<Mediafile>(); return _RecentlyAddedSongsCollection; }
            set { Set(ref _RecentlyAddedSongsCollection, value); }
        }
        ThreadSafeObservableCollection<Mediafile> _RecentlyPlayedCollection;
        /// <summary>
        /// Gets or sets a grouped observable collection of Tracks/Mediafiles. <seealso cref="GroupedObservableCollection{TKey, TElement}"/>
        /// </summary>
        public ThreadSafeObservableCollection<Mediafile> RecentlyPlayedCollection
        {
            get { if (_RecentlyPlayedCollection == null) _RecentlyPlayedCollection = new ThreadSafeObservableCollection<Mediafile>(); return _RecentlyPlayedCollection; }
            set { Set(ref _RecentlyPlayedCollection, value); }
        }
        GroupedObservableCollection<string, Mediafile> _TracksCollection;
        /// <summary>
        /// Gets or sets a grouped observable collection of Tracks/Mediafiles. <seealso cref="GroupedObservableCollection{TKey, TElement}"/>
        /// </summary>
        public GroupedObservableCollection<string, Mediafile> TracksCollection
        {
            get
            {
                if (_TracksCollection == null)
                    _TracksCollection = new GroupedObservableCollection<string, Mediafile>(t => t.Title.Remove(1));
                return _TracksCollection;
            }
            set
            {
                Set(ref _TracksCollection, value);
                Messenger.Instance.NotifyColleagues(MessageTypes.MSG_LIBRARY_LOADED, new List<object>() { TracksCollection, grouped });
            }
        }
        public MenuFlyout genreFlyout;
        /// <summary>
        /// Gets or Sets a flyout for genres. This is a dynamic control bound to <see cref="LibraryView"/>.
        /// </summary>
        public MenuFlyout GenreFlyout
        {
            get { return genreFlyout; }
            set { Set(ref genreFlyout, value); }
        }
        bool isLibraryLoading;
        /// <summary>
        /// Gets or Sets <see cref="BreadPlayer.Models.Mediafile"/> for this ViewModel
        /// </summary>
        public bool IsLibraryLoading
        {
            get { return isLibraryLoading; }
            set { Set(ref isLibraryLoading, value); }
        }
        #endregion

        #region Commands 

        #region Definitions
        RelayCommand _deleteCommand;
        RelayCommand _playCommand;
        RelayCommand _stopAfterCommand;
        RelayCommand _addtoplaylistCommand;   
        RelayCommand _refreshViewCommand;
        RelayCommand _initCommand;
        RelayCommand _relocateSongCommand;
        DelegateCommand changeSelectionModeCommand;
        public ICommand RelocateSongCommand
        {
            get
            { if (_relocateSongCommand == null) { _relocateSongCommand = new RelayCommand(RelocateSong); } return _relocateSongCommand; }
        }
        public ICommand ChangeSelectionModeCommand
        {
            get
            { if (changeSelectionModeCommand == null) { changeSelectionModeCommand = new DelegateCommand(ChangeSelectionMode); } return changeSelectionModeCommand; }
        }
        /// <summary>
        /// Gets command for initialization. This calls the <see cref="Init(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand InitCommand
        {
            get
            { if (_initCommand == null) { _initCommand = new RelayCommand(param => this.Init(param)); } return _initCommand; }
        }
      
        /// <summary>
        /// Gets AddToPlaylist command. This calls the <see cref="AddToPlaylist(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand AddToPlaylistCommand
        {
            get
            { if (_addtoplaylistCommand == null) { _addtoplaylistCommand = new RelayCommand(param => this.AddToPlaylist(param)); } return _addtoplaylistCommand; }
        }
        /// <summary>
        /// Gets refresh command. This calls the <see cref="RefreshView(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand RefreshViewCommand
        {
            get
            { if (_refreshViewCommand == null) { _refreshViewCommand = new RelayCommand(param => this.RefreshView(param)); } return _refreshViewCommand; }
        }
        /// <summary>
        /// Gets Play command. This calls the <see cref="Play(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand PlayCommand
        {
            get
            { if (_playCommand == null) { _playCommand = new RelayCommand(param => this.Play(param)); } return _playCommand; }
        }
        
        /// <summary>
        /// Gets Stop command. This calls the <see cref="StopAfter(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand StopAfterCommand
        {
           get
           { if (_stopAfterCommand == null) { _stopAfterCommand = new RelayCommand(param => this.StopAfter(param)); } return _stopAfterCommand; }
        }

        /// <summary>
        /// Gets Play command. This calls the <see cref="Delete(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand DeleteCommand
        {
            get
            { if (_deleteCommand == null) { _deleteCommand = new RelayCommand(param => this.Delete(param)); } return _deleteCommand; }
        }
        #endregion

        #region Implementations  
        /// <summary>
        /// Relocates song to a new location. We only update _id, Path and Length of the song.
        /// </summary>
        /// <param name="para">The Mediafile to relocate</param>
        private async void RelocateSong(object para)
        {
            if (para is Mediafile mediafile)
            {
                FileOpenPicker openPicker = new FileOpenPicker()
                {
                    CommitButtonText = "Relocate Song",
                };
                openPicker.FileTypeFilter.Concat(new List<string>() { ".mp3", ".wav", ".ogg", ".flac", ".m4a" , ".aif", ".wma" });
                var newFile = await openPicker.PickSingleFileAsync();
                if (newFile != null)
                {
                    var newMediafile = await CreateMediafile(newFile);
                    TracksCollection.Elements.Single(t => t.Path == mediafile.Path).Length = newMediafile.Length;
                    TracksCollection.Elements.Single(t => t.Path == mediafile.Path)._id = newMediafile._id;
                    TracksCollection.Elements.Single(t => t.Path == mediafile.Path).Path = newMediafile.Path;
                    LibraryService.UpdateMediafile(TracksCollection.Elements.Single(t => t._id == mediafile._id));
                }
            }
        }
        private void ChangeSelectionMode()
        {
            IsMultiSelectModeEnabled = IsMultiSelectModeEnabled ? false : true;
        }
        /// <summary>
        /// Refreshes the view with new sorting order and/or filtering. <seealso cref="RefreshViewCommand"/>
        /// </summary>
        /// <param name="para"><see cref="MenuFlyoutItem"/> to get sorting/filtering base from.</param>
        async void RefreshView(object para)
        {
            MenuFlyoutItem selectedItem = para as MenuFlyoutItem;
            if (selectedItem.Tag.ToString() == "genre")
            {
                Genre = selectedItem.Text;
                await RefreshView(Genre, null, false).ConfigureAwait(false);
            }
            else
            {
                await RefreshView(null, selectedItem.Tag.ToString()).ConfigureAwait(false);
            }
        }
        /// <summary>
        /// Deletes a song from the FileCollection. <seealso cref="DeleteCommand"/>
        /// </summary>
        /// <param name="path"><see cref="BreadPlayer.Models.Mediafile"/> to delete.</param>
        public async void Delete(object path)
        {
            try
            {
                int index = 0;
                if(SelectedItems.Count > 0)
                {
                    foreach (var item in SelectedItems)
                    {
                        index = TracksCollection.Elements.IndexOf(item);
                        TracksCollection.RemoveItem(item);
                        LibraryService.RemoveMediafile(item);
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
        
        public async void StopAfter(object path)
        {
            Mediafile mediaFile = await GetMediafileFromParameterAsync(path);
            Messenger.Instance.NotifyColleagues(MessageTypes.MSG_STOP_AFTER_SONG, mediaFile);
        }
        
        /// <summary>
        /// Plays the selected file. <seealso cref="PlayCommand"/>
        /// </summary>
        /// <param name="path"><see cref="BreadPlayer.Models.Mediafile"/> to play.</param>
        public async void Play(object path)
        {
            var currentlyPlaying = Player.CurrentlyPlayingFile;
            Mediafile mediaFile = await GetMediafileFromParameterAsync(path, true);
            AddToRecentCollection(mediaFile);
            Messenger.Instance.NotifyColleagues(MessageTypes.MSG_PLAY_SONG, new List<object>() { mediaFile, true, isPlayingFromPlaylist });

            if (currentlyPlaying != null && TracksCollection.Elements.FirstOrDefault(t => t.Path == currentlyPlaying.Path) != null)
            {
                TracksCollection.Elements.FirstOrDefault(t => t.Path == currentlyPlaying.Path).State = PlayerState.Playing;
            }
        }
        
        async void Init(object para)
        {
            NavigationService.Instance.Frame.Navigated += Frame_Navigated;
            if (ViewSource == null)
                ViewSource = (para as Grid).Resources["Source"] as CollectionViewSource;

            await RefreshSourceAsync().ConfigureAwait(false);

            if (source == null && Sort != "Unsorted")
            {
                await LoadCollectionAsync(GetSortFunction(Sort), true).ConfigureAwait(false);
            }
            else if (source == null && Sort == "Unsorted")
            {
                await LoadCollectionAsync(t => t.Title, false).ConfigureAwait(false);
            }
        }
        #endregion

        #endregion

        #region Methods

        private void AddToRecentCollection(Mediafile mediaFile)
        {
            LibraryService = new LibraryService(new DatabaseService());
            RecentCollection = LibraryService.GetRecentCollection();

            if (RecentlyPlayedCollection.Any(t => t.Path == mediaFile.Path))
            {
                RecentlyPlayedCollection.Remove(RecentlyPlayedCollection.First(t => t.Path == mediaFile.Path));
            }
            if (RecentCollection.Exists(t => t.Path == mediaFile.Path))
            {
                RecentCollection.Delete(t => t.Path == mediaFile.Path);
            }
            RecentlyPlayedCollection.Add(mediaFile);
            RecentCollection.Insert(mediaFile);
        }

        private void SendLibraryLoadedMessage(object payload, bool sendMessage)
        {
            if (sendMessage)
            {
                Messenger.Instance.NotifyColleagues(MessageTypes.MSG_LIBRARY_LOADED, payload);
                isPlayingFromPlaylist = true;
            }
        }
        private async Task<Mediafile> GetMediafileFromParameterAsync(object path, bool sendUpdateMessage = false)
        {
            if (path is Mediafile)
            {
                isPlayingFromPlaylist = false;
                return path as Mediafile;
            }
            else if (path is ThreadSafeObservableCollection<Mediafile>)
            {
                SendLibraryLoadedMessage(path as ThreadSafeObservableCollection<Mediafile>, sendUpdateMessage);
                return (path as ThreadSafeObservableCollection<Mediafile>)[0];
            }
            else if (path is Playlist)
            {
                Service.PlaylistService service = new Service.PlaylistService((path as Playlist).Name, (path as Playlist).IsPrivate, (path as Playlist).Password);
                var songList = new ThreadSafeObservableCollection<Mediafile>(await service.GetTracks().ConfigureAwait(false));
                SendLibraryLoadedMessage(songList, sendUpdateMessage);
                return songList[0];
            }
            return null;
        }
        private async Task<ThreadSafeObservableCollection<Mediafile>> GetMostPlayedSongsAsync()
        {
            return await Task.Run(() =>
            {
                foreach (var item in TracksCollection.Elements.Where(t => t.PlayCount > 1))
                {
                    if (MostEatenSongsCollection.Any(t => t.Path != item.Path))
                        MostEatenSongsCollection.Add(item);
                }
                return MostEatenSongsCollection;
            });
        }
        private async Task<ThreadSafeObservableCollection<Mediafile>> GetRecentlyAddedSongsAsync()
        {
            return await Task.Run(() =>
            {
                foreach (var item in TracksCollection.Elements)
                {
                    if (RecentlyAddedSongsCollection.All(t => t.Path != item.Path)
                        && item.AddedDate != null
                        && (DateTime.Now.Subtract(DateTime.Parse(item.AddedDate))).Days < 3)
                    {
                        RecentlyAddedSongsCollection.Add(item);
                    }
                }
                return RecentlyAddedSongsCollection;
            });
        }

        async Task RefreshSourceAsync()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (source != null)
                {
                    if (grouped && source == TracksCollection.Elements)
                        ViewSource.Source = TracksCollection;
                    else
                        ViewSource.Source = source;

                    ViewSource.IsSourceGrouped = grouped;
                }
            });
        }
        void ChangeView(string header, bool group, object src)
        {
            ViewSource.Source = null;
            Header = header;
            grouped = group;
            source = src;
            libgrouped = ViewSource.IsSourceGrouped;
            if((src as ThreadSafeObservableCollection<Mediafile>)?.Any() == true && Player.CurrentlyPlayingFile != null && (src as ThreadSafeObservableCollection<Mediafile>).FirstOrDefault(t => t.Path == Player.CurrentlyPlayingFile?.Path) != null)
                (src as ThreadSafeObservableCollection<Mediafile>).FirstOrDefault(t => t.Path == Player.CurrentlyPlayingFile?.Path).State = PlayerState.Playing;
        }
        async Task LoadCollectionAsync(Func<Mediafile, string> sortFunc, bool group)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                grouped = group;
                TracksCollection = new GroupedObservableCollection<string, Mediafile>(sortFunc);
                TracksCollection.CollectionChanged += TracksCollection_CollectionChanged;

                SongCount = LibraryService.SongCount;
                if (group)
                {
                    ViewSource.Source = TracksCollection;
                }
                else
                    ViewSource.Source = TracksCollection.Elements;

                ViewSource.IsSourceGrouped = group;
                await SplitList(TracksCollection, 300).ConfigureAwait(false);
            });
        }

        /// <summary>
        /// Refresh the view, based on filters and sorting mechanisms.
        /// </summary>
        public async Task RefreshView(string genre = "All genres", string propName = "Title", bool doOrderFiles = true)
        {
            if (doOrderFiles)
            {
                Sort = propName;
                if (propName != "Unsorted")
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
                    {
                        if (files == null)
                            files = TracksCollection.Elements;
                        grouped = true;
                        TracksCollection = new GroupedObservableCollection<string, Mediafile>(GetSortFunction(propName));
                        ViewSource.Source = TracksCollection;
                        ViewSource.IsSourceGrouped = true;
                        TracksCollection.AddRange(files, true, false);
                        UpdateJumplist(propName);

                        await RemoveDuplicateGroups();
                    });
                }
                else
                {
                    ViewSource.Source = TracksCollection.Elements;
                    ViewSource.IsSourceGrouped = false;
                    grouped = false;
                    Messenger.Instance.NotifyColleagues(MessageTypes.MSG_LIBRARY_LOADED, new List<object>() { TracksCollection, grouped });
                }
            }
            else
            {
                Genre = genre;
                TracksCollection = null;
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
                {
                    TracksCollection = new GroupedObservableCollection<string, Mediafile>(t => t.Title);
                    if (genre != "All genres")
                        TracksCollection.AddRange(await LibraryService.Query("Genre", genre).ConfigureAwait(false), true);
                    else
                        TracksCollection.AddRange(OldItems, true);
                });
            }
        }
        
        async Task RemoveDuplicateGroups()
        {
            //the only workaround to remove the first group which is a 'false' duplicate really.
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            { 
                if (ViewSource.IsSourceGrouped)
                {
                    UpdateJumplist(Sort);
                    ViewSource.IsSourceGrouped = false;
                    ViewSource.IsSourceGrouped = true;
                }
            });
        }
        Func<Mediafile, string> GetSortFunction(string propName)
        {
            Func<Mediafile, string> f = null;
            switch (propName)
            {
                case "Title":
                    f = t => GetPropValue(t, propName).ToString().StartsWithLetter() ? (GetPropValue(t, propName) as string).Remove(1).ToUpper() : GetPropValue(t, propName).ToString().StartsWithNumber() ? "#" : GetPropValue(t, propName).ToString().StartsWithSymbol() ? "&" : (GetPropValue(t, propName) as string); //determine whether the Title, by which groups are made, start with number, letter or symbol. On the basis of that we define the Key for each Group.
                    break;
                case "Year":
                    f = t => string.IsNullOrEmpty(GetPropValue(t, propName) as String) ? "Unknown Year" : GetPropValue(t, propName) as string;
                    break;
                case "Length":
                    string[] timeformats = { @"m\:ss", @"mm\:ss", @"h\:mm\:ss", @"hh\:mm\:ss" };
                    f = t => string.IsNullOrEmpty(GetPropValue(t, propName) as String) || GetPropValue(t, propName) as String == "0:00" ? "Unknown Length" : Math.Round((TimeSpan.ParseExact((GetPropValue(t, propName) as String), timeformats, System.Globalization.CultureInfo.InvariantCulture).TotalMinutes)).ToString() + " minutes";
                    break;
                case "TrackNumber":
                    f = t => string.IsNullOrEmpty(GetPropValue(t, propName) as String) ? "Unknown Track No." : (GetPropValue(t, propName) as String);
                    break;
                case "FolderPath":
                    f = t => string.IsNullOrEmpty(GetPropValue(t, propName) as String) ? "Unknown Folder" : new DirectoryInfo((GetPropValue(t, propName) as String)).FullName;
                    break;
                default:
                    f = t => GetPropValue(t, propName) as string; //determine whether the Title, by which groups are made, start with number, letter or symbol. On the basis of that we define the Key for each Group.
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
        async Task CreateGenreMenu()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                GenreFlyout = Application.Current.Resources["GenreFlyout"] as MenuFlyout;
                Genre = "All genres";
                GenreFlyout.Items.Add(CreateMenuItem("All genres"));
                foreach (var genre in TracksCollection.Elements)
                {
                    if (genre.Genre != null && genre.Genre != "NaN" && !GenreFlyout.Items.Any(t => (t as MenuFlyoutItem).Text == genre.Genre))
                    {
                        GenreFlyout.Items.Add(CreateMenuItem(genre.Genre));
                    }
                }
            });
        }

        /// <summary>
        /// Creates menu item. <seealso cref="MenuFlyoutItem"/>
        /// </summary>
        /// <param name="genre">The text of the item.</param>
        /// <returns><see cref="MenuFlyoutItem"/></returns>
        MenuFlyoutItem CreateMenuItem(string genre)
        {
            var item = new MenuFlyoutItem() { Text = genre };
            item.Command = RefreshViewCommand;
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
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Any())
                {                  
                    foreach (var item in items)
                    {
                        if (item.IsOfType(StorageItemTypes.File) && Path.GetExtension(item.Path) == ".mp3")
                        {
                            Mediafile mp3file = null;
                            string path = item.Path;
                            var tempList = new List<Mediafile>();
                            if (TracksCollection.Elements.All(t => t.Path != path))
                            {
                                try
                                {
                                    mp3file = await CreateMediafile(item as StorageFile);
                                    await SettingsViewModel.SaveSingleFileAlbumArtAsync(mp3file).ConfigureAwait(false);
                                    AddMediafile(mp3file);
                                }
                                catch (Exception ex)
                                {
                                    BLogger.Logger.Error("Error occured while drag/drop operation.", ex);
                                }
                            }
                        }
                        else if (item.IsOfType(StorageItemTypes.Folder))
                        {
                           await SettingsVM.AddFolderToLibraryAsync((item as StorageFolder).CreateFileQueryWithOptions(DirectoryWalker.GetQueryOptions()));
                        }

                    }
                    Messenger.Instance.NotifyColleagues(MessageTypes.MSG_ADD_ALBUMS, "");
                }
            }
        }
        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets name of a property in a class. 
        /// </summary>
        /// <param name="src">Class to search for property.</param>
        /// <param name="propName">Property to search for.</param>
        /// <returns></returns>
        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetTypeInfo().GetDeclaredProperty(propName).GetValue(src, null);
        }       

        #endregion

        #region Disposable
        public void Reset()
        {
            LibraryService.Dispose();
            LibraryService = null;
            TracksCollection.Clear();
            RecentlyPlayedCollection.Clear();
            GenreFlyout?.Items.Clear();
            PlaylistsItems?.Clear();
            OldItems = null;
            PlaylistCollection.Clear();
            OptionItems.Clear();
            GenreCollection.Clear();
            SongCount = -1;
        }
        #endregion
      
        #region Library Methods
        /// <summary>
        /// Loads library from the database file.
        /// </summary>
        void LoadLibrary()
        {
            GetSettings();
            OptionItems.Add(new ContextMenuCommand(AddToPlaylistCommand, "New Playlist"));
            if (File.Exists(ApplicationData.Current.LocalFolder.Path + @"\breadplayer.db"))
            {
                RecentlyPlayedCollection.AddRange(LibraryService.GetRecentCollection().FindAll());
                LoadPlaylists();
                UpdateJumplist("Title");
            }
        }
        private async void TracksCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SetNowPlayingSong();
            if (TracksCollection.Elements.Count == SongCount)
            {
                await RemoveDuplicateGroups().ConfigureAwait(false);
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    MusicLibraryLoaded.Invoke(this, new RoutedEventArgs());
                });
                OldItems = TracksCollection.Elements;
                TracksCollection.CollectionChanged -= TracksCollection_CollectionChanged;              
            }           
        }
        private async void LibraryViewModel_MusicLibraryLoaded(object sender, RoutedEventArgs e)
        {
            if (!libraryLoaded)
            {
                libraryLoaded = true;              
                await CreateGenreMenu().ConfigureAwait(false);
                BLogger.Logger.Info("Library successfully loaded!");
                await NotificationManager.ShowMessageAsync("Library successfully loaded!", 4);
                Messenger.Instance.NotifyColleagues(MessageTypes.MSG_LIBRARY_LOADED, new List<object>() { TracksCollection, grouped });
                await Task.Delay(10000);
                Common.DirectoryWalker.SetupDirectoryWatcher(SettingsVM.LibraryFoldersCollection);
            }
        }
        private void SetNowPlayingSong()
        {
            string path = RoamingSettingsHelper.GetSetting<string>("path", "");
            if (!TracksCollection.Elements.Any(t => t.Path == path && t.State == PlayerState.Playing))
            {
                if (TracksCollection.Elements.Any(t => t.State == PlayerState.Playing))
                {
                    var sa = TracksCollection.Elements.Where(l => l.State == PlayerState.Playing);
                    foreach (var mp3 in sa) mp3.State = PlayerState.Stopped;
                }
                if (TracksCollection.Elements.Any(t => t.Path == path))
                {
                    TracksCollection.Elements.FirstOrDefault(t => t.Path == path).State = PlayerState.Playing;
                }
            }           
        }
        #endregion

        #region Playlist Methods

        void LoadPlaylists()
        {
            foreach (var list in LibraryService.GetPlaylists())
            {
                AddPlaylist(list);
            }
        }
        async void AddToPlaylist(object file)
        {
            if (file != null)
            {
                //songList is a variable to initiate both (if available) sources of songs. First is AlbumSongs and the second is the direct library songs.
                List<Mediafile> songList = new List<Mediafile>();
                if ((file as MenuFlyoutItem).Tag == null)
                {
                    songList = (file as MenuFlyoutItem).DataContext is Album ?
                       ((file as MenuFlyoutItem).DataContext as Album).AlbumSongs.ToList() : SelectedItems;
                }
                else
                {
                    songList.Add(Player.CurrentlyPlayingFile);
                }
                var menu = file as MenuFlyoutItem;
                Playlist dictPlaylist = menu.Text == "New Playlist" ? await ShowAddPlaylistDialogAsync() : LibraryService.GetPlaylist(menu?.Text);
                
                if (dictPlaylist != null && await AskForPassword(dictPlaylist))
                    await AddPlaylistAsync(dictPlaylist, true, songList);
            }
            else
            {
                var pList = await ShowAddPlaylistDialogAsync();
                if(pList != null)
                    await AddPlaylistAsync(pList, false);
            }
        }

        async Task<Playlist> ShowAddPlaylistDialogAsync(string title = "Name this playlist", string playlistName = "", string desc = "", string password = "")
        {
            var dialog = new InputDialog()
            {
                Title = title,
                Text = playlistName,
                Description = desc,
                IsPrivate = password.Length > 0,
                Password = password
            };
            if (CoreWindow.GetForCurrentThread().Bounds.Width <= 501)
                dialog.DialogWidth = CoreWindow.GetForCurrentThread().Bounds.Width - 50;
            else
                dialog.DialogWidth = CoreWindow.GetForCurrentThread().Bounds.Width - 100;
            if (await dialog.ShowAsync() == ContentDialogResult.Primary && dialog.Text != "")
            {
                var Playlist = new Playlist();
                Playlist.Name = dialog.Text;
                Playlist.Description = dialog.Description;
                Playlist.IsPrivate = dialog.Password.Length > 0;
                Playlist.Password = dialog.Password;
                if (LibraryService.CheckExists<Playlist>(LiteDB.Query.EQ("Name", Playlist.Name), new PlaylistCollection()))
                {
                    Playlist = await ShowAddPlaylistDialogAsync("Playlist already exists! Please choose another name.", Playlist.Name, Playlist.Description, dialog.Password);
                }
                return Playlist;
            }
            return null;
        }

        public async Task AddSongsToPlaylist(Playlist list, List<Mediafile> songsToadd)
        {
            if (songsToadd.Any())
            {
                PlaylistService service = new PlaylistService(list.Name, list.IsPrivate, list.Password);
                int index = 0;
                foreach (var item in songsToadd)
                {
                    index++;
                    item.State = PlayerState.Stopped;
                    //NotificationManager.ShowMessageAsync(index.ToString() + " of " + songsToadd.Count.ToString() + " added into playlist: " + list.Name);
                    await Task.Run(() =>
                    {
                        service.Insert(item);
                    }).ConfigureAwait(false);
                }
            }
        }
      
        public void AddPlaylist(Playlist Playlist)
        {
            var cmd = new ContextMenuCommand(AddToPlaylistCommand, Playlist.Name);
            OptionItems.Add(cmd);
            PlaylistsItems.Add(new SplitViewMenu.SimpleNavMenuItem
            {
                Arguments = Playlist,
                Label = Playlist.Name,
                DestinationPage = typeof(PlaylistView),
                Symbol = Symbol.List,
                FontGlyph = "\u0045",
                ShortcutTheme = ElementTheme.Dark,
                HeaderVisibility = Visibility.Collapsed
            });
        }
        public async Task AddPlaylistAsync(Playlist plist, bool addsongs, List<Mediafile> songs = null)
        {
            if (!LibraryService.CheckExists<Playlist>(LiteDB.Query.EQ("Name",plist.Name), new PlaylistCollection()))
            {
                AddPlaylist(plist);
                LibraryService.AddPlaylist(plist);
            }
            if (addsongs)
                await AddSongsToPlaylist(plist, songs).ConfigureAwait(false);
        }
        #endregion

        #region Events
        private async void Elements_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await Task.Delay(1000);
            if (RecentlyPlayedCollection.Count <= 100)
            {
                RecentlyPlayedCollection.RemoveAt(RecentlyPlayedCollection.Count + 1);
            }
        }
        private async void Frame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            string param = (e.Parameter ?? string.Empty).ToString();    // e.Parameter can be null and throw exception
            if (e.SourcePageType == typeof(LibraryView))
            {
                switch(param)
                {
                    case "Recent":
                        ChangeView("Recently Played", false, RecentlyPlayedCollection);
                        break;
                    case "MusicCollection":
                        ChangeView("Music Collection", libgrouped, TracksCollection.Elements);
                        break;
                    case "MostEaten":
                        ChangeView("Most Eaten", false, await GetMostPlayedSongsAsync());
                        break;
                    case "RecentlyAdded":
                        ChangeView("Recently Added", false, await GetRecentlyAddedSongsAsync());
                        break;
                    default:
                        break;
                }

                await RefreshSourceAsync().ConfigureAwait(false);
            }
            else if (ViewSource.Source != null)
            {
                source = ViewSource.Source;
                grouped = ViewSource.IsSourceGrouped;
                ViewSource.Source = null;
                GC.Collect();
            }
        }

        public void PlayOnTap(object sender, TappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch && !IsMultiSelectModeEnabled)
            {
                var mediafile = (e.OriginalSource as Border).Tag;
                Play(mediafile);
            }
        }
        public List<Mediafile> SelectedItems { get; set; } = new List<Mediafile>();

        public void SelectionChanged(object para, SelectionChangedEventArgs selectionEvent)
        {
            if (selectionEvent.RemovedItems.Count > 0)
            {
                foreach (var toRemove in selectionEvent.RemovedItems.Cast<Mediafile>())
                {
                    SelectedItems.Remove(toRemove);
                }
            }
            if (selectionEvent.AddedItems.Count > 0)
                SelectedItems.AddRange(selectionEvent.AddedItems.Cast<Mediafile>().ToList());
        }
        #endregion

        public event OnMusicLibraryLoaded MusicLibraryLoaded;
    }

    public delegate void OnMusicLibraryLoaded(object sender, RoutedEventArgs e);    
}
