using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using System.IO;
using Windows.Storage;
using Windows.Storage.Pickers;
using Macalifa.Tags;
using Macalifa.Tags.ID3;
using Macalifa.Tags.ID3.ID3v2Frames;
using Macalifa.Tags.ID3.ID3v2Frames.TextFrames;
using Windows.UI.Xaml.Media;
using Macalifa.Models;
using System.Collections.ObjectModel;
using Macalifa.Core;
using Macalifa.Services;
using System.Windows.Input;
using System.Reflection;
using Windows.Data.Json;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using Macalifa.Database;
using Windows.UI.Xaml.Controls;
using Macalifa.Extensions;
using Windows.UI.Xaml.Data;
using System.Diagnostics;
using Windows.System;
using Macalifa.Events;
namespace Macalifa.ViewModels
{
    public class LibraryViewModel : ViewModelBase
    {
        #region Fields
        public QueryMethods db = new QueryMethods();
        public CoreDispatcher Dispatcher;
        ThreadSafeObservableCollection<Playlist> PlaylistCollection = new ThreadSafeObservableCollection<Playlist>();
        ObservableRangeCollection<String> _GenreCollection = new ObservableRangeCollection<string>();
        public List<Mediafile> OldItems;
        public ListBox FileListBox;
        public static string Path = "";
        #endregion

        #region Contructor
        /// <summary>
        /// Creates a new instance of LibraryViewModel
        /// </summary>
        public LibraryViewModel()
        {
            LibraryViewService.vm = this;
            LibraryViewService service = LibraryViewService.Instance;
            Album = "asdasdas";
            Dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
            LoadCommand = new DelegateCommand(Load);
            db.CreateDB();
            LoadLibrary();
        }
        #endregion

        #region Properties
        public string _album;
        public string Album
        {
            get { return _album; }
            set { Set(ref _album, value); }
        }
        public string _genre;
        public string Genre
        {
            get { return _genre; }
            set { Set(ref _genre, value); }
        }
        /// <summary>
        /// Gets the genre collection in which there are all the genres of all the tracks/Mediafiles in <see cref="FileCollection"/>.
        /// </summary>
        public ObservableRangeCollection<String> GenreCollection
        {
            get { return _GenreCollection; }
        }
        /// <summary>
        /// Gets the instance of <see cref="ShellViewModel"/>
        /// </summary>
        ShellViewModel ShellVM => ShellViewService.Instance.ShellVM;
        /// <summary>
        /// Gets read-only instance of <see cref="MacalifaPlayer"/>
        /// </summary>
        public MacalifaPlayer Player => MacalifaPlayerService.Instance.Player;
      
        GroupedObservableCollection<string, Mediafile> _TracksCollection;
        /// <summary>
        /// Gets or sets a grouped observable collection of Tracks/Mediafiles. <seealso cref="GroupedObservableCollection{TKey, TElement}"/>
        /// </summary>
        public GroupedObservableCollection<string, Mediafile> TracksCollection
        {
            get { return _TracksCollection; }
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
        /// Gets or Sets <see cref="Macalifa.Models.Mediafile"/> for this ViewModel
        /// </summary>
        public Mediafile Mediafile
        {
            get { return _mediaFile; }
            set { Set(ref _mediaFile, value); }
        }
        #endregion

        #region Commands 

        #region Definitions
        RelayCommand _deleteCommand;
        RelayCommand _playCommand;
        RelayCommand _addtoplaylistCommand;
        RelayCommand _opensonglocationCommand;
        RelayCommand _refreshViewCommand;
        RelayCommand _initCommand;
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
        /// <summary>
        /// Gets load library command. This calls the <see cref="Load"/> method.
        /// </summary>
        public DelegateCommand LoadCommand { get; private set; }
        #endregion

        #region Implementations
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
        /// <param name="path"><see cref="Macalifa.Models.Mediafile"/> to delete.</param>
        public void Delete(object path)
        {
            if(path is ListBox)
            {
                if (FileListBox == null) FileListBox = path as ListBox;
            }
            else
            {
                var menu = path as MenuFlyoutItem;
                if (FileListBox == null) FileListBox = (menu.Tag as ListBoxItem).GetAncestorsOfType<ListBox>().ToList()[0];
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
        /// <param name="path"><see cref="Macalifa.Models.Mediafile"/> to play.</param>
        public async void Play(object path)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
            {
                var mediaFile = TracksCollection.Elements.SingleOrDefault(t => t.State == PlayerState.Playing);
                if (mediaFile != null) mediaFile.State = PlayerState.Stopped;
                Mediafile mp3File = path as Mediafile;
                StorageFile file = await StorageFile.GetFileFromPathAsync(mp3File.path);
                if (file != null)
                {
                    await Player.Load(file.Path);
                    ShellVM.Length = Player.Length;
                    ShellVM.PlayPauseCommand.IsEnabled = true;
                    ShellVM.PlayPauseCommand.Execute(null);
                    ShellVM.Tags = Player.Tags;
                }
                Album = Player.PlayerState.ToString();
                mp3File.State = PlayerState.Playing;

                GC.Collect();
            });
        }
        #endregion

        #endregion

        #region Methods
        /// <summary>
        /// Refresh the view, based on filters and sorting mechanisms.
        /// </summary>
        public async void RefreshView(string genre = "All genres", string propName = "Title", bool doOrderFiles = true)
        {
            IEnumerable<Mediafile> files = null;
            if (doOrderFiles)
            {
                if (propName != "Unsorted")
                {
                    files = TracksCollection.Elements.Where(t => t.Path != "").OrderBy(t => GetPropValue(t, propName)).ToList();
                    TracksCollection = null;
                    TracksCollection = new GroupedObservableCollection<string, Mediafile>(t => propName == "Title" ? (GetPropValue(t, propName) as string).Remove(1).ToUpper() : (GetPropValue(t, propName) as string), files, a => a.Title.Remove(1).ToUpper());
                }
            }
            else
            {
                //FileCollection.Clear();
                //files = genre != "All genres" ? OldItems.Where(t => t.Genre == genre) : OldItems;
                //FileCollection.AddRange(files);
            }
           
        }

        /// <summary>
        /// Creates genre menu.
        /// </summary>
        void CreateGenreMenu()
        {
            GenreFlyout = new MenuFlyout();
            Genre = "All genres";
            GenreFlyout.Items.Add(CreateMenuItem("All genres"));
            foreach (var genre in TracksCollection.Elements)
            {
                if (genre.Genre != null && genre.Genre != "NaN" && !GenreFlyout.Items.Any(t => (t as MenuFlyoutItem).Text == genre.Genre))
                {
                    GenreFlyout.Items.Add(CreateMenuItem(genre.Genre));
                }
            }
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
        String GetStringForNullOrEmptyProperty(string data, string setInstead)
        {
            return string.IsNullOrEmpty(data) ? setInstead : data;
        }
        public async Task<string> GetTextFrame(ID3v2 info, string FrameID)
        {
            // string Text = "";
            return await Task.Run(() =>
            {
                foreach (TextFrame TF in info.TextFrames)
                    if (TF.FrameID == FrameID)
                    {
                        return TF.Text;
                    }
                return "";
            });

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

        #region Library Methods
        /// <summary>
        /// Loads library from the database file.
        /// </summary>
        void LoadLibrary()
        {
            if (File.Exists(ApplicationData.Current.LocalFolder.Path + @"\library.db"))
            {
                OldItems = db.GetTracks().ToList();
                TracksCollection = new GroupedObservableCollection<string, Mediafile>(t => t.Title, OldItems, t => t.Title);
                PlaylistCollection = new ThreadSafeObservableCollection<Playlist>(OldItems.SelectMany(t => t.Playlists).DistinctBy(t => t.Name).ToList());
                foreach (var list in PlaylistCollection)
                {
                    var cmd = new ContextMenuCommand(AddToPlaylistCommand, list.Name);
                    Options.Add(cmd);

                    ShellVM.PlaylistsItems.Add(new SplitViewMenu.SimpleNavMenuItem
                    {
                        Arguments = db.PlaylistSort(list.Name),
                        Label = list.Name,
                        DestinationPage = typeof(Albums),
                        Symbol = Symbol.List
                    });
                    GC.Collect();
                }
            }
        }
        


        /// <summary>
        /// Asynchronously saves all the album arts in the library. 
        /// </summary>
        /// <param name="Data">ID3 tag of the song to get album art data from.</param>
        private async void SaveImages(ID3v2 Data)
        {
            try
            {
                var albumartFolder = ApplicationData.Current.LocalFolder;
                await albumartFolder.CreateFileAsync(@"AlbumArts\" + Mediafile.Title + ".jpg", CreationCollisionOption.ReplaceExisting);
                var albumart = await albumartFolder.GetFileAsync(@"AlbumArts\" + Mediafile.Title + ".jpg");
                using (var albumstream = await albumart.OpenStreamForWriteAsync())
                {
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(Data.AttachedPictureFrames[0].Data.AsRandomAccessStream());
                    WriteableBitmap bmp = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                    await bmp.SetSourceAsync(Data.AttachedPictureFrames[0].Data.AsRandomAccessStream());
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, albumstream.AsRandomAccessStream());
                    var pixelStream = bmp.PixelBuffer.AsStream();
                    byte[] pixels = new byte[bmp.PixelBuffer.Length];
                    await pixelStream.ReadAsync(pixels, 0, pixels.Length);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)bmp.PixelWidth, (uint)bmp.PixelHeight, 96, 96, pixels);
                    await encoder.FlushAsync();
                    pixelStream.Dispose();
                }
            }
            catch { }
        }
       public async Task<Mediafile> CreateMediafile(Stream stream)
        {
            var Mediafile = new Mediafile();
            ID3v2 Data = new ID3v2(true, stream);
            Mediafile._id = LiteDB.ObjectId.NewObjectId();
            Mediafile.Path = Path;
            Mediafile.Title = GetStringForNullOrEmptyProperty((await GetTextFrame(Data, "TIT2")), System.IO.Path.GetFileNameWithoutExtension(Path));
            Mediafile.LeadArtist = GetStringForNullOrEmptyProperty(await GetTextFrame(Data, "TPE1"), "Unknown Artist");
            Mediafile.Album = GetStringForNullOrEmptyProperty(await GetTextFrame(Data, "TALB"), "Unknown Album");
            Mediafile.State = PlayerState.Stopped;
            Mediafile.Genre = (await GetTextFrame(Data, "TCON")).Remove(0, (await GetTextFrame(Data, "TCON")).IndexOf(')') + 1);
            Mediafile.Date = DateTime.Now.Date.ToString("dd/MM/yyyy");
            if (Mediafile.Genre != null && Mediafile.Genre != "NaN")
            {
                Mediafile.Playlists.Add(new Playlist() { Name = "Hello" });
                GenreCollection.Add(Mediafile.Genre);
            }
            if (Data.AttachedPictureFrames.Count > 0)
            {
                Mediafile.Playlists.Add(new Playlist() { Name = "Top Songs" });
                //Mediafile.AttachedPicture = Data.AttachedPictureFrames[0].Picture;
                //SaveImages(Data);
                GC.Collect();
            }
            return Mediafile;
        }
        /// <summary>
        /// Loads songs from a specified folder into the library. <seealso cref="LoadCommand"/>
        /// </summary>
        public async void Load()
        {
            FolderPicker picker = new FolderPicker() { SuggestedStartLocation = PickerLocationId.MusicLibrary };

            picker.FileTypeFilter.Add(".mp3");
            StorageFolder folder = await picker.PickSingleFolderAsync();
            if (folder != null)
            {
                var filelist = await Macalifa.Common.DirectoryWalker.GetFiles(folder.Path);
                foreach (var x in filelist)
                {
                    StorageFile file = await StorageFile.GetFileFromPathAsync(x);
                    Path = file.Path;
                    using (var stream = await Dispatcher.RunTaskAsync(GetFileAsStream))
                    {
                        if (stream != null)
                        {
                            var path = file.Path;
                            if (TracksCollection.Elements.All(t => t.Path != path))
                            {
                                var m = await CreateMediafile(stream);
                                TracksCollection.AddItem(m);
                                //TracksCollection.Elements.Add(m);
                                //ByStringGroupedCollection.AddItem(Mediafile);
                                db.Insert(m);
                            }
                        }
                    }
                }
            }
        }
        #endregion
        public ThreadSafeObservableCollection<ContextMenuCommand> Options { get; set; } = new ThreadSafeObservableCollection<ContextMenuCommand>();

        void AddToPlaylist(object file)
        {
            var menu = file as MenuFlyoutItem;
            var s = menu.Tag as Mediafile;
            s.Playlists.Add(new Playlist() { Name = menu.Text });
            db.Update(s);
        }
        async void OpenSongLocation(object file)
        {
            var mp3File = file as Mediafile;
            StorageFolder folder = await (await StorageFile.GetFileFromPathAsync(mp3File.Path)).GetParentAsync();
            await Launcher.LaunchFolderAsync(folder);
        }
        void Init(object para)
        {
            var childern = para as UIElementCollection;
            var fileBox = childern.OfType<ListBox>().ToList()[0];
            FileListBox = fileBox;
        }
        
    }
}
