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
    public class LibraryViewModel : ViewModelBase
    {
        #region Fields
        public QueryMethods db = new QueryMethods();
        public CoreDispatcher Dispatcher;
        ThreadSafeObservableCollection<Playlist> PlaylistCollection = new ThreadSafeObservableCollection<Playlist>();
        ObservableRangeCollection<String> _GenreCollection = new ObservableRangeCollection<string>();
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
            RecentlyPlayedCollection.Elements.CollectionChanged += Elements_CollectionChanged;
            LoadLibrary();
        }

        private async void Elements_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await Task.Delay(1000);
           if(RecentlyPlayedCollection.Elements.Count <= 100)
            {              
                RecentlyPlayedCollection.Elements.RemoveAt(RecentlyPlayedCollection.Count + 1);
            }
        }

        double recentscrolloffset = 0;
        double libraryscrolloffset = 0;
        private void Frame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            var s = FileListBox.GetFirstDescendantOfType<Border>().GetFirstDescendantOfType<ScrollViewer>();
            if (e.SourcePageType == typeof(LibraryView))
            {
                if (e.Parameter is string && e.Parameter.ToString() == "Recent")
                {
                    libraryscrolloffset = s.VerticalOffset;
                    ViewSource.Source = RecentlyPlayedCollection.Elements;
                    ViewSource.IsSourceGrouped = false;
                    Header = "Recently Played";
                    s.ChangeView(0, recentscrolloffset, 1.0f, false);
                }
                else
                {
                    var mp3 = TracksCollection?.Elements?.SingleOrDefault(t => t.Path == Player.CurrentlyPlayingFile.Path);
                    mp3.State = PlayerState.Playing;
                    recentscrolloffset = s.VerticalOffset;
                    Header = "Music Library";
                    if (ViewSource.Source != TracksCollection.Elements)
                        ViewSource.Source = TracksCollection.Elements;
                    s.ChangeView(0, libraryscrolloffset, 1.0f, false);
                }
            }

        }
        #endregion

        #region Properties  
        ThreadSafeObservableCollection<ContextMenuCommand> items = new ThreadSafeObservableCollection<ContextMenuCommand>();
        public ThreadSafeObservableCollection<ContextMenuCommand> OptionItems { get { return items; } set { Set(ref items, value); } }
        CollectionViewSource viewSource;
        public CollectionViewSource ViewSource
        {
            get { if (viewSource == null) viewSource = new CollectionViewSource(); return viewSource; }
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
        GroupedObservableCollection<string, Mediafile> _RecentlyPlayedCollection;
        /// <summary>
        /// Gets or sets a grouped observable collection of Tracks/Mediafiles. <seealso cref="GroupedObservableCollection{TKey, TElement}"/>
        /// </summary>
        public GroupedObservableCollection<string, Mediafile> RecentlyPlayedCollection
        {
            get { if (_RecentlyPlayedCollection == null) _RecentlyPlayedCollection = new GroupedObservableCollection<string, Mediafile>(t => t.Title); return _RecentlyPlayedCollection; }
            set { Set(ref _RecentlyPlayedCollection, value); }
        }
        GroupedObservableCollection<string, Mediafile> _TracksCollection;
        /// <summary>
        /// Gets or sets a grouped observable collection of Tracks/Mediafiles. <seealso cref="GroupedObservableCollection{TKey, TElement}"/>
        /// </summary>
        public GroupedObservableCollection<string, Mediafile> TracksCollection
        {
            get { if (_TracksCollection == null) _TracksCollection = new GroupedObservableCollection<string, Mediafile>(t => t.Title); return _TracksCollection; }
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
        async void ShowProperties(object para)
        {
            Mediafile file = null;
            if (para is Mediafile)
                file = para as Mediafile;
            else
                file = Player.CurrentlyPlayingFile;
            BreadPlayer.Dialogs.TagDialog tag = new BreadPlayer.Dialogs.TagDialog(file);
         
            if (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1))
            {
                tag.MinWidth = 0;
              
            }
            else
            {
                tag.MaxHeight = 550;
            }
              
                await tag.ShowAsync();
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
        public void Delete(object path)
        {
            if (path is ListView)
            {
                if (FileListBox == null) FileListBox = path as ListView;
            }
            else
            {
                var menu = path as MenuFlyoutItem;
                if (FileListBox == null) FileListBox = (menu.Tag as ListViewItem).GetAncestorsOfType<ListView>().ToList()[0];
            }
            var index = FileListBox.SelectedIndex;
            Mediafile mp3File = FileListBox.SelectedItem as Mediafile;
            TracksCollection.RemoveItem(mp3File);
            FileListBox.SelectedIndex = index - 1;
            FileListBox.Focus(FocusState.Programmatic);
         
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
                if (RecentlyPlayedCollection.Elements.Any(t => t.Path == mp3File.Path))
                {
                    RecentlyPlayedCollection.Elements.Remove(RecentlyPlayedCollection.Elements.First(t => t.Path == mp3File.Path));
                }
                if (db.recent.Exists(t => t._id == mp3File._id))
                {
                    db.recent.Delete(t => t._id == mp3File._id);
                }
                RecentlyPlayedCollection.AddItem(mp3File, true);
                db.recent.Insert(mp3File);
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    ShellVM.Play(null, mp3File);
                });
            }
            //mp3File.State = PlayerState.Playing;
        }
        async void OpenSongLocation(object file)
        {
            var mp3File = file as Mediafile;
            if (mp3File == null)
                mp3File = Player.CurrentlyPlayingFile;
            StorageFolder folder = await (await StorageFile.GetFileFromPathAsync(mp3File.Path)).GetParentAsync();
            StorageFile storageFile = await StorageFile.GetFileFromPathAsync(mp3File.Path);
            var folderOptions = new FolderLauncherOptions();
            folderOptions.ItemsToSelect.Add(storageFile);
            await Launcher.LaunchFolderAsync(folder, folderOptions);
        }
       async void Init(object para)
        {
            NavigationService.Instance.Frame.Navigated += Frame_Navigated;
            ViewSource = null;
            ViewSource = (para as Grid).Resources["Source"] as CollectionViewSource;
            var childern = (para as Grid).Children;
            var listview = childern.OfType<SemanticZoom>().ToList()[0].ZoomedInView as ListView;
            FileListBox = listview;

        }
        #endregion

        #endregion

        #region Methods
        IEnumerable<Mediafile> files = null;
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
                    await Task.Run(async () =>
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                        {
                            if (files == null)
                                files = TracksCollection.Elements;
                            Func<Mediafile, string> f = null;
                            if (propName == "Title")
                            {
                                f = t => StartsWithLetter(GetPropValue(t, propName) as string) ? (GetPropValue(t, propName) as string).Remove(1).ToUpper() : StartsWithNumber(GetPropValue(t, propName) as string) ? "#" : StartsWithSymbol(GetPropValue(t, propName) as string) ? "&" : (GetPropValue(t, propName) as string); //determine whether the Title, by which groups are made, start with number, letter or symbol. On the basis of that we define the Key for each Group.
                            }
                            else
                            {
                                f = t => GetPropValue(t, propName) as string; //determine whether the Title, by which groups are made, start with number, letter or symbol. On the basis of that we define the Key for each Group.
                            }
                            TracksCollection = new GroupedObservableCollection<string, Mediafile>(f);
                            TracksCollection.AddRange(files, true);
                            ViewSource.Source = TracksCollection;
                            ViewSource.IsSourceGrouped = true;
                        }).AsTask().ConfigureAwait(false);
                    }).ConfigureAwait(false);
                    TracksCollection.RemoveAt(0); //there seems to be an addition of extra group at the top always. So we have to remove that until a workaround is found.
                }
                else
                {
                    ViewSource.Source = TracksCollection.Elements;
                    ViewSource.IsSourceGrouped = false;
                }
            }
            else
            {
                Genre = genre;
                TracksCollection = null;
               await Dispatcher.RunAsync(CoreDispatcherPriority.High,  () =>
                {
                    TracksCollection = new GroupedObservableCollection<string, Mediafile>(t => t.Title);
                    if (genre != "All genres")
                        TracksCollection.AddRange(db.tracks.Find(t => t.Genre == genre), true);
                    else
                        TracksCollection.AddRange(OldItems, true);
                });
            }
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
        #region Library Methods
        /// <summary>
        /// Loads library from the database file.
        /// </summary>
        async  void LoadLibrary()
        {
            if (File.Exists(ApplicationData.Current.LocalFolder.Path + @"\breadplayer.db"))
            {
                
                    TracksCollection = new GroupedObservableCollection<string, Mediafile>(t => t.Title);
             
                TracksCollection.AddRange(await db.GetTracks().ConfigureAwait(false), true);
                SongCount = 1;
                RecentlyPlayedCollection.AddRange(db.recent.FindAll(), true);
                    if (TracksCollection.Elements.Count > 0)
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {  MusicLibraryLoaded.Invoke(this, new RoutedEventArgs()); }); //no use raising an event when library isn't ready.             
                        ShellVM.PlayPauseCommand.IsEnabled = true;
                    }
                    else
                    {
                        await LoadPlaylists().ConfigureAwait(false);
                    }
                SongCount = TracksCollection.Elements.Count;
                AlphabetList = "&#ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray().Select(x => x.ToString()).ToList();


                //OldItems = db.GetTracks();
            }
        }
        private async void LibraryViewModel_MusicLibraryLoaded(object sender, RoutedEventArgs e)
        {
            await LoadPlaylists().ConfigureAwait(false);
             await AlbumArtistVM.AddAlbums().ConfigureAwait(false);
            await CreateGenreMenu().ConfigureAwait(false);
            OldItems = TracksCollection.Elements;
            await NotificationManager.ShowAsync("Library successfully loaded!", "Loaded");
            ShellVM.UpcomingSong = await ShellVM.GetUpcomingSong().ConfigureAwait(false);
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
       
        async Task LoadPlaylists()
        {
            OptionItems.Add(new ContextMenuCommand(AddToPlaylistCommand, "New Playlist"));
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                PlaylistCollection.AddRange(db.tracks.Find(t => t.Playlists.Count > 0).SelectMany(t => t.Playlists));
                PlaylistCollection.AddRange(db.playlists.FindAll());
                foreach (var list in PlaylistCollection.DistinctBy(t => t.Name))
                {
                    if (list.Songs.Count <= 0)
                        AddPlaylist(new Dictionary<Playlist, IEnumerable<Mediafile>>(), list.Name, list.Description);
                    else
                    {
                        var dict = new Dictionary<Playlist, IEnumerable<Mediafile>>();
                        dict.Add(list, list.Songs);
                        AddPlaylist(dict, list.Name, list.Description);
                    }
                }
            }).AsTask().ConfigureAwait(false);
        }
        async void AddToPlaylist(object file)
        {
            if (file != null)
            {
                //songList is a variable to initiate both (if available) sources of songs. First is AlbumSongs and the second is the direct library songs.
                ThreadSafeObservableCollection<Mediafile> songList = null;
                if((file as MenuFlyoutItem).Tag == null)
                    songList = (file as MenuFlyoutItem).DataContext is Album ? ((file as MenuFlyoutItem).DataContext as Album).AlbumSongs : new ThreadSafeObservableCollection<Mediafile>(FileListBox.SelectedItems.Cast<Mediafile>());
                else
                {
                    songList = new ThreadSafeObservableCollection<Mediafile>();
                    songList.Add(Player.CurrentlyPlayingFile);
                }
                var menu = file as MenuFlyoutItem;
                var dictPlaylist = new Dictionary<Playlist, IEnumerable<Mediafile>>();
                if (menu.Text == "New Playlist")
                {
                    dictPlaylist = await ShowAddPlaylistDialog();
                }
                if(dictPlaylist != null)
                {
                    if (songList.Any())
                    {
                        foreach (Mediafile s in songList)
                        {
                            var playlistName = dictPlaylist.Count > 0 ? dictPlaylist.First().Key.Name : menu.Text;
                            if (!s.Playlists.Any(t => t.Name == playlistName)) //dupe check
                            {
                                s.Playlists.Add(new Playlist() { Name = playlistName, Description = ""});
                                if ((file as MenuFlyoutItem).Tag == null) db.Update(s);
                                else db.Update(s, true);                              
                            }
                        }
                        
                    }                  
                }
              
            }
            else
            {
                await ShowAddPlaylistDialog();
            }
        }

        async Task<Dictionary<Playlist, IEnumerable<Mediafile>>> ShowAddPlaylistDialog()
        {
            var dialog = new InputDialog()
            {
                Title = "Name this playlist",
            };
            var Playlists = new Dictionary<Playlist, IEnumerable<Mediafile>>();
            if (await dialog.ShowAsync() == ContentDialogResult.Primary && dialog.Text != "")
            {
                if (!PlaylistCollection.Any(t => t.Name == dialog.Text))
                {
                    AddPlaylist(Playlists, dialog.Text, dialog.Description);
                }
                return Playlists;
            }
            else
               return null;
        }
        public void AddPlaylist(Dictionary<Playlist, IEnumerable<Mediafile>> Playlists, string label, string description)
        {
            var pl = new Playlist() { Name = label, Description = description };
            Playlists.Add(pl, TracksCollection.Elements.Where(a => a.Playlists.Any(t => t.Name == pl.Name) && a.Playlists.Count > 0));
            var cmd = new ContextMenuCommand(AddToPlaylistCommand, pl.Name);
            OptionItems.Add(cmd);
            pl.Songs.AddRange(Playlists.Values.First());
            db.playlists.Insert(pl);
            ShellVM.PlaylistsItems.Add(new SplitViewMenu.SimpleNavMenuItem
            {
                Arguments = Playlists,
                Label = label,
                DestinationPage = typeof(PlaylistView),
                Symbol = Symbol.List,
                FontGlyph = "\ue823"
            });
        }
        #endregion
        
        public event OnMusicLibraryLoaded MusicLibraryLoaded;
    }

    public delegate void OnMusicLibraryLoaded(object sender, RoutedEventArgs e);    
}
