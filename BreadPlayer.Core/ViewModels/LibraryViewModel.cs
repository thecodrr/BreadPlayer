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
using Windows.UI.Core;
using System.IO;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media;
using BreadPlayer.Models;
using System.Collections.ObjectModel;
using BreadPlayer.Core;
using BreadPlayer.Services;
using System.Windows.Input;
using System.Reflection;
using Windows.Data.Json;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using BreadPlayer.Database;
using Windows.UI.Xaml.Controls;
using BreadPlayer.Extensions;
using Windows.UI.Xaml.Data;
using System.Diagnostics;
using Windows.System;
using BreadPlayer.Events;
using BreadPlayer.Dialogs;
using System.Security.Cryptography;
using SplitViewMenu;
using Windows.Storage.AccessCache;
using Windows.Foundation.Metadata;
using Windows.Foundation;
using System.Text.RegularExpressions;
using Extensions;

namespace BreadPlayer.ViewModels
{
    public class LibraryViewModel : ViewModelBase, IDisposable
    {
        #region Fields        
        ThreadSafeObservableCollection<Playlist> PlaylistCollection = new ThreadSafeObservableCollection<Playlist>();
        ObservableRangeCollection<String> _GenreCollection = new ObservableRangeCollection<string>();
        IEnumerable<Mediafile> files = null;
        public IEnumerable<Mediafile> OldItems;
        public ListView FileListBox;
        public static string Path = "";
        #endregion

        #region Contructor
        /// <summary>
        /// Creates a new instance of LibraryViewModel
        /// </summary>
        public LibraryViewModel()
        {
            Header = "Music Library";
            MusicLibraryLoaded += LibraryViewModel_MusicLibraryLoaded;
            GenericService<LibraryViewModel>.vm = this;
            GenericService<LibraryViewModel> service = GenericService<LibraryViewModel>.Instance;
            Dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
            RecentlyPlayedCollection.CollectionChanged += Elements_CollectionChanged;
            LoadLibrary();
        }
        private async void Elements_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await Task.Delay(1000);
           if(RecentlyPlayedCollection.Count <= 100)
            {              
                RecentlyPlayedCollection.RemoveAt(RecentlyPlayedCollection.Count + 1);
            }
        }

        bool grouped = false;
        bool libgrouped = false;
        object source;
      
        private async void Frame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            if (e.SourcePageType == typeof(LibraryView))
            {                
                if (e.Parameter.ToString() == "Recent")              
                   ChangeView("Recently Played", false, RecentlyPlayedCollection);                
                else
                    ChangeView("Music Library", libgrouped, TracksCollection.Elements);
                await RefreshSourceAsync().ConfigureAwait(false);
            }
            else
            {
                AlbumArtistVM.AlbumCollection = null;
                if (ViewSource.Source != null)
                {
                    source = ViewSource.Source;
                    grouped = ViewSource.IsSourceGrouped;
                    ViewSource.Source = null;
                    GC.Collect();
                }
            }

        }
        #endregion

        #region Properties 
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

        DatabaseQueryMethods database;
        public DatabaseQueryMethods Database
        {
            get { if (database == null) database = new DatabaseQueryMethods(); return database; }
            set { Set(ref database, value); }
        }
        ThreadSafeObservableCollection<ContextMenuCommand> items = new ThreadSafeObservableCollection<ContextMenuCommand>();
        public ThreadSafeObservableCollection<ContextMenuCommand> OptionItems { get { return items; } set { Set(ref items, value); } }
        CollectionViewSource viewSource;
        public CollectionViewSource ViewSource
        {
            get { return viewSource; }
            set { Set(ref viewSource, value); }
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
            set { Set(ref _sort, value); }
        }
        Mediafile selectedItem;
        public Mediafile SelectedItem
        {
            get { return selectedItem; }
            set { Set(ref selectedItem, value); }
        }
        int selectedIndex;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { Set(ref selectedIndex, value); }
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
            get { if (_TracksCollection == null) _TracksCollection = new GroupedObservableCollection<string, Mediafile>(t => t.Title.Remove(1)); return _TracksCollection; }
            set { Set(ref _TracksCollection, value); }
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
        Mediafile _mediaFile;
        /// <summary>
        /// Gets or Sets <see cref="BreadPlayer.Models.Mediafile"/> for this ViewModel
        /// </summary>
        public Mediafile Mediafile
        {
            get { return _mediaFile; }
            set { Set(ref _mediaFile, value); }
        }
        #endregion

        #region Commands 

        #region Definitions
        RelayCommand _showPropertiesCommand;
        RelayCommand _deleteCommand;
        RelayCommand _playCommand;
        RelayCommand _addtoplaylistCommand;
        RelayCommand _opensonglocationCommand;
        RelayCommand _refreshViewCommand;
        RelayCommand _initCommand;
        RelayCommand _selectionChangedCommand;
        /// <summary>
        /// Gets command for initialization. This calls the <see cref="Init(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand SelectionChangedCommand
        {
            get
            { if (_selectionChangedCommand == null) { _selectionChangedCommand = new RelayCommand(param => this.SelectionChanged(param)); } return _selectionChangedCommand; }
        }
        /// <summary>
        /// Gets command for initialization. This calls the <see cref="Init(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand ShowPropertiesCommand
        {
            get
            { if (_showPropertiesCommand == null) { _showPropertiesCommand = new RelayCommand(param => this.ShowProperties(param)); } return _showPropertiesCommand; }
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
        /// Gets command for open song location. This calls the <see cref="OpenSongLocation(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand OpenSongLocationCommand
        {
            get
            { if (_opensonglocationCommand == null) { _opensonglocationCommand = new RelayCommand(param => this.OpenSongLocation(param)); } return _opensonglocationCommand; }
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
        /// Gets Play command. This calls the <see cref="Delete(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand DeleteCommand
        {
            get
            { if (_deleteCommand == null) { _deleteCommand = new RelayCommand(param => this.Delete(param)); } return _deleteCommand; }
        }
        #endregion

        #region Implementations
        void SelectionChanged(object para)
        {
            var selectionEvent = (para as SelectionChangedEventArgs);
            if(selectionEvent.RemovedItems.Count > 0)
                foreach(var toRemove in selectionEvent.RemovedItems.Cast<Mediafile>())
                {
                    SelectedItems.Remove(toRemove);
                }
            if(selectionEvent.AddedItems.Count > 0)
                SelectedItems.AddRange(selectionEvent.AddedItems.Cast<Mediafile>().ToList());
        }
        async void ShowProperties(object para)
        {
            Mediafile file = null;
            if (para is Mediafile)
                file = para as Mediafile;
            else
                file = Player.CurrentlyPlayingFile;
            BreadPlayer.Dialogs.TagDialog tag = new BreadPlayer.Dialogs.TagDialog(file);
            if (CoreWindow.GetForCurrentThread().Bounds.Width >= 501)
                tag.MaxWidth = CoreWindow.GetForCurrentThread().Bounds.Width - 10;
            await tag.ShowAsync();
        }

        /// <summary>
        /// Refreshes the view with new sorting order and/or filtering. <seealso cref="RefreshViewCommand"/>
        /// </summary>
        /// <param name="para"><see cref="MenuFlyoutItem"/> to get sorting/filtering base from.</param>
        void RefreshView(object para)
        {
            MenuFlyoutItem selectedItem = para as MenuFlyoutItem;
            if (selectedItem.Tag.ToString() == "genre")
            {
                Genre = selectedItem.Text;
                RefreshView(Genre, null, false);
            }
            else
            {
                RefreshView(null, selectedItem.Tag.ToString());
            }
        }
        /// <summary>
        /// Deletes a song from the FileCollection. <seealso cref="DeleteCommand"/>
        /// </summary>
        /// <param name="path"><see cref="BreadPlayer.Models.Mediafile"/> to delete.</param>
        public async void Delete(object path)
        {
            int index = 0;
            foreach (var item in SelectedItems)
            {
                index = TracksCollection.Elements.IndexOf(item);
                TracksCollection.RemoveItem(item);
                Database.Remove(item);
                SongCount--;
            }
            await Task.Delay(100);
            SelectedItem = index < TracksCollection.Elements.Count ? TracksCollection.Elements.ElementAt(index) : TracksCollection.Elements.ElementAt(index - 1);    
        }
        /// <summary>
        /// Plays the selected file. <seealso cref="PlayCommand"/>
        /// </summary>
        /// <param name="path"><see cref="BreadPlayer.Models.Mediafile"/> to play.</param>
        public async void Play(object path)
        {
            if(path is Mediafile)
            {
                Mediafile mp3File = path as Mediafile;
                if (RecentlyPlayedCollection.Any(t => t.Path == mp3File.Path))
                {
                    RecentlyPlayedCollection.Remove(RecentlyPlayedCollection.First(t => t.Path == mp3File.Path));
                }
                if (Database.recent.Exists(t => t._id == mp3File._id))
                {
                    Database.recent.Delete(t => t._id == mp3File._id);
                }
                RecentlyPlayedCollection.Add(mp3File);
                Database.recent.Insert(mp3File);
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async() =>
                {
                    ShellVM.Load(mp3File, true);
                    (PlayCommand as RelayCommand).IsEnabled = false;
                    await Task.Delay(100);
                    (PlayCommand as RelayCommand).IsEnabled = true;
                });
            }
            //mp3File.State = PlayerState.Playing;
        }
        async void OpenSongLocation(object file)
        {
            var mp3File = file as Mediafile;
            if (mp3File == null)
                mp3File = Player.CurrentlyPlayingFile;
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(System.IO.Path.GetDirectoryName(mp3File.Path));
            if(folder != null)
            {
                StorageFile storageFile = await StorageFile.GetFileFromPathAsync(mp3File.Path);
                var folderOptions = new FolderLauncherOptions();
                folderOptions.ItemsToSelect.Add(storageFile);
                await Launcher.LaunchFolderAsync(folder, folderOptions);
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
                await LoadCollectionAsync(GetSortFunction(LibVM.Sort), true).ConfigureAwait(false);    
            }
            else if (source == null && Sort == "Unsorted")
            {
                await LoadCollectionAsync(t => t.Title, false).ConfigureAwait(false);
            }
        }
        #endregion

        #endregion

        #region Methods
       
        async Task RefreshSourceAsync()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (SongCount <= 0)
                    SongCount = Database.tracks.Count();
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
        }
        async Task LoadCollectionAsync(Func<Mediafile, string> sortFunc, bool group)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                TracksCollection = new GroupedObservableCollection<string, Mediafile>(sortFunc);
                TracksCollection.CollectionChanged += TracksCollection_CollectionChanged;

                if (group)
                    ViewSource.Source = TracksCollection;
                else
                    ViewSource.Source = TracksCollection.Elements;

                ViewSource.IsSourceGrouped = group;

                TracksCollection.AddRange(await Database.GetTracks().ConfigureAwait(false), true);
                grouped = group;
            });
        }

        /// <summary>
        /// Refresh the view, based on filters and sorting mechanisms.
        /// </summary>
        public async void RefreshView(string genre = "All genres", string propName = "Title", bool doOrderFiles = true)
        {

            if (doOrderFiles)
            {
                Sort = propName;
                if (propName != "Unsorted")
                {
                    if (files == null)
                        files = TracksCollection.Elements;
                    TracksCollection = new GroupedObservableCollection<string, Mediafile>(GetSortFunction(propName));
                    ViewSource.Source = TracksCollection;
                    ViewSource.IsSourceGrouped = true;
                    TracksCollection.AddRange(files, true, false);
                    TracksCollection.CollectionChanged += TracksCollection_CollectionChanged1;
                    grouped = true;
                }
                else
                {
                    ViewSource.Source = TracksCollection.Elements;
                    ViewSource.IsSourceGrouped = false;
                    grouped = false;
                }
            }
            else
            {
                Genre = genre;
                TracksCollection = null;
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    TracksCollection = new GroupedObservableCollection<string, Mediafile>(t => t.Title);
                    if (genre != "All genres")
                        TracksCollection.AddRange(Database.tracks.Find(t => t.Genre == genre), true);
                    else
                        TracksCollection.AddRange(OldItems, true);
                });
            }
        }

        private async void TracksCollection_CollectionChanged1(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
           await RemoveDuplicateGroups().ConfigureAwait(false);
        }
        async Task RemoveDuplicateGroups()
        {
            //the only workaround to remove the first group which is an 'false' duplicate really.
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (ViewSource.IsSourceGrouped)
                {
                    ViewSource.IsSourceGrouped = false;
                    ViewSource.IsSourceGrouped = true;
                }
            });
        }
        Func<Mediafile, string> GetSortFunction(string propName)
        {
            Func<Mediafile, string> f = null;
            if (propName == "Title")
            {
                f = t => StartsWithLetter(GetPropValue(t, propName) as string) ? (GetPropValue(t, propName) as string).Remove(1).ToUpper() : StartsWithNumber(GetPropValue(t, propName) as string) ? "#" : StartsWithSymbol(GetPropValue(t, propName) as string) ? "&" : (GetPropValue(t, propName) as string); //determine whether the Title, by which groups are made, start with number, letter or symbol. On the basis of that we define the Key for each Group.
            }
            else
            {
                f = t => GetPropValue(t, propName) as string; //determine whether the Title, by which groups are made, start with number, letter or symbol. On the basis of that we define the Key for each Group.
            }
            return f;
        }

        bool StartsWithLetter(string input)
        {
            return Regex.Match(input.Remove(1), "[a-zA-Z]").Success;
        }
        bool StartsWithNumber(string input)
        {
            return Regex.Match(input.Remove(1), "\\d").Success;
        }
        bool StartsWithSymbol(string input)
        {
            return Regex.Match(input.Remove(1), "[^a-zA-Z0-9]").Success;
        }
        /// <summary>
        /// Creates genre menu.
        /// </summary>
        async Task CreateGenreMenu()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                GenreFlyout = new MenuFlyout();
                GenreFlyout.MenuFlyoutPresenterStyle = App.Current.Resources["CustomFlyoutPresenter"] as Style;
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

        /// <summary>
        /// Gets a <see cref="StorageFile"/> as a <see cref="Stream"/>.
        /// </summary>
        /// <returns><see cref="Stream"/></returns>
        public static async Task<Stream> GetFileAsStream()
        {
            try
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(Path);
                if (System.IO.Path.GetExtension(file.Path) == ".mp3")
                {
                    var stream = await file.OpenAsync(FileAccessMode.ReadWrite);
                    return stream.AsStream();
                }
            }
            catch { }

            return null;
        }

        #endregion

        #region IDisposable
        public void Dispose()
        {
            Database.Dispose();
            TracksCollection.Clear();
            RecentlyPlayedCollection.Clear();
            ShellVM.PlaylistsItems.Clear();
            OldItems = null;
            PlaylistCollection.Clear();
            OptionItems.Clear();
            GenreCollection.Clear();
        }
        #endregion
      
        #region Library Methods
        /// <summary>
        /// Loads library from the database file.
        /// </summary>
        void LoadLibrary()
        {
            OptionItems.Add(new ContextMenuCommand(AddToPlaylistCommand, "New Playlist"));
            if (File.Exists(ApplicationData.Current.LocalFolder.Path + @"\breadplayer.db") && Database.IsValid)
            {
                RecentlyPlayedCollection.AddRange(Database.recent.FindAll());
                LoadPlaylists();               
                AlphabetList = "&#ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray().Select(x => x.ToString()).ToList();
            }
        }
        private async void TracksCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {  
            if (TracksCollection.Elements.Count > 0 && e.NewItems?.Count == SongCount)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { MusicLibraryLoaded.Invoke(this, new RoutedEventArgs()); }); //no use raising an event when library isn't ready.             
                OldItems = TracksCollection.Elements;
                ShellVM.PlayPauseCommand.IsEnabled = true;
                TracksCollection.CollectionChanged -= TracksCollection_CollectionChanged;
            }
        }

        private async void LibraryViewModel_MusicLibraryLoaded(object sender, RoutedEventArgs e)
        {
            await RemoveDuplicateGroups().ConfigureAwait(false);
            await CreateGenreMenu().ConfigureAwait(false);
            await NotificationManager.ShowAsync("Library successfully loaded!", "Loaded");
            await Task.Delay(10000);
            Common.DirectoryWalker.SetupDirectoryWatcher(SettingsVM.LibraryFoldersCollection);

        }
        /// <summary>
        /// Asynchronously saves all the album arts in the library. 
        /// </summary>
        /// <param name="Data">ID3 tag of the song to get album art data from.</param>
        public async Task SaveImages(Windows.Storage.FileProperties.StorageItemThumbnail thumb, Mediafile file)
        {
            var albumartFolder = ApplicationData.Current.LocalFolder;
            var md5Path = (file.Album + file.LeadArtist).ToLower().ToSha1();
           
            if (!File.Exists(albumartFolder.Path + @"\AlbumArts\" + md5Path + ".jpg"))
            {
                IBuffer buf;
                Windows.Storage.Streams.Buffer inputBuffer = new Windows.Storage.Streams.Buffer(1024);
                var albumart = await albumartFolder.CreateFileAsync(@"AlbumArts\" + md5Path + ".jpg", CreationCollisionOption.FailIfExists).AsTask().ConfigureAwait(false);
                using (IRandomAccessStream albumstream = await albumart.OpenAsync(FileAccessMode.ReadWrite).AsTask().ConfigureAwait(false))
                {
                    try
                    {
                        while ((buf = (await thumb.ReadAsync(inputBuffer, inputBuffer.Capacity, Windows.Storage.Streams.InputStreamOptions.None).AsTask().ConfigureAwait(false))).Length > 0)
                            await albumstream.WriteAsync(buf).AsTask().ConfigureAwait(false);

                    }
                    catch (Exception ex) { NotificationManager.ShowAsync(ex.Message + "||" + file.Path); }
                   
                }
                thumb.Dispose();
            }
        }
        #endregion

        #region Playlist Methods

        void LoadPlaylists()
        {
            foreach (var list in Database.playlists.FindAll())
            {
                AddPlaylist(list);
            }
        }
        public List<Mediafile> SelectedItems { get; set; } = new List<Mediafile>();
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
                Playlist dictPlaylist = menu.Text == "New Playlist" ? await ShowAddPlaylistDialog() : new Playlist() { Name = menu?.Text };
                if (dictPlaylist != null)
                    await AddPlaylist(dictPlaylist, true, songList);
            }
            else
            {
                var pList = await ShowAddPlaylistDialog();
                if(pList != null)
                    await AddPlaylist(pList, false);
            }
        }

        async Task<Playlist> ShowAddPlaylistDialog(string title = "Name this playlist", string playlistName = "", string desc = "")
        {
            var dialog = new InputDialog()
            {
                Title = title,
                Text = playlistName,
                Description = desc
            };          
            if (await dialog.ShowAsync() == ContentDialogResult.Primary && dialog.Text != "")
            {
                var Playlist = new Playlist();
                Playlist.Name = dialog.Text;
                Playlist.Description = dialog.Description;
                if (Database.playlists.Exists(t => t.Name == Playlist.Name))
                {
                    Playlist = await ShowAddPlaylistDialog("Playlist already exists! Please choose another name.", Playlist.Name, Playlist.Description);
                }
               return Playlist;
            }
            else
               return null;
        }
        public async void AddSongsToPlaylist(Playlist list, List<Mediafile> songsToadd)
        {
            if (songsToadd.Any())
            {
                Database.CreatePlaylistDB(list.Name);
                int index = 0;
                foreach (var item in songsToadd)
                {
                    if (!Database.tracks.Exists(t => t._id == item._id))
                    {
                        index++;
                        item.State = PlayerState.Stopped;
                        //NotificationManager.ShowAsync(index.ToString() + " of " + songsToadd.Count.ToString() + " added into playlist: " + list.Name);
                        await Task.Run(() =>
                        {
                            Database.Insert(item);
                        }).ConfigureAwait(false);
                   }
                }
                Database.CreateDB();
            }
        }
      
        public void AddPlaylist(Playlist Playlist)
        {
            var cmd = new ContextMenuCommand(AddToPlaylistCommand, Playlist.Name);
            OptionItems.Add(cmd);          
            ShellVM.PlaylistsItems.Add(new SplitViewMenu.SimpleNavMenuItem
            {
                Arguments = Playlist,
                Label = Playlist.Name,
                DestinationPage = typeof(PlaylistView),
                Symbol = Symbol.List,
                FontGlyph = "\ue823"
            });
        }
        public async Task AddPlaylist(Playlist plist, bool addsongs, List<Mediafile> songs = null)
        {
            if (!Database.playlists.Exists(t => t.Name == plist.Name))
            {
                AddPlaylist(plist);
                Database.playlists.Insert(plist);
            }
            if (addsongs)
                AddSongsToPlaylist(plist, songs);
        }
        #endregion
        
        public event OnMusicLibraryLoaded MusicLibraryLoaded;
    }

    public delegate void OnMusicLibraryLoaded(object sender, RoutedEventArgs e);    
}
