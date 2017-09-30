using BreadPlayer.Common;
using BreadPlayer.Core.Common;
using BreadPlayer.Core.Engines.BASSEngine;
using BreadPlayer.Core.Engines.Interfaces;
using BreadPlayer.Core.Models;
using BreadPlayer.Database;
using BreadPlayer.Dialogs;
using BreadPlayer.Dispatcher;
using BreadPlayer.Extensions;
using BreadPlayer.Helpers;
using BreadPlayer.NotificationManager;
using BreadPlayer.Services;
using BreadPlayer.ViewModels;
using BreadPlayer.Web.Lastfm;
using System;
using System.IO;
using System.Threading.Tasks;
using TagLib;
using Windows.Foundation.Metadata;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BreadPlayer.Core
{
    public class SharedLogic
    {
        #region Ctor

        public SharedLogic()
        {
            //To define them all here is not good. This ctor is called multiple times.
            //should remove this to a better place.
            //#TODO Move these properties to a better place perhaps the App Ctor?
            InitializeCore.Dispatcher = new BreadDispatcher();
            InitializeCore.NotificationManager = NotificationManager;
            InitializeCore.EqualizerSettingsHelper = new SettingsHelper();
            InitializeCore.IsMobile = ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1);
            InitializeCore.IsMobile = Window.Current?.Bounds.Width <= 600;
        }

        #endregion Ctor

        #region Singletons (NEED IMPROVEMENTS)
        public static SharedLogic Instance => GSingleton<SharedLogic>.Instance.Singleton;
        /// <summary>
        /// This path is used around the codebase multiple times,
        /// so it is better to define it once and call it everywhere
        /// </summary>
        public string DatabasePath => Path.Combine(ApplicationData.Current.LocalFolder.Path, "BreadPlayerDB");

        /// <summary>
        /// These contextmenu items are used in the context menu for library
        /// items. This shouldn't be here.
        /// #TODO move this to the correct place & remove the singleton.
        /// </summary>
        public ThreadSafeObservableCollection<ContextMenuCommand> OptionItems => GSingleton<ThreadSafeObservableCollection<ContextMenuCommand>>.Instance.Singleton;

        /// <summary>
        /// The notification manager is used around the application many times,
        /// a global singleton is best for such a design.
        /// </summary>
        public BreadNotificationManager NotificationManager => GSingleton<BreadNotificationManager>.Instance.Singleton;// { get { return items; } set { Set(ref items, value); } }

        /// <summary>
        /// The singleton for player engine. Could this be better designed?
        /// Perhaps an Instance Singleton in the PlayerEngine itself?
        /// A singleton here makes most sense.
        /// </summary>
        private IPlayerEngine _player;

        public IPlayerEngine Player
        {
            get
            {
                if (_player == null)
                {
                    _player = new BassPlayerEngine(ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1),  SettingsHelper.GetRoamingSetting<bool>("CrossfadeEnabled", true));
                }

                return _player;
            }
        }

        /// <summary>
        /// The static SettingsVM Singleton. I am still not sure if this should be here.
        /// This is bad design plain and simple. Needs improvement.
        /// #TODO Clean this up and improve this to a better design.
        /// </summary>
        public SettingsViewModel SettingsVm => GSingleton<SettingsViewModel>.Instance.Singleton;

        //This is not a traditional singleton. Instead of making it readonly,
        //I have made this read/write capable. Can this be improved?
        private Lastfm _lastfmScrobbler;

        public Lastfm LastfmScrobbler
        {
            get => _lastfmScrobbler;
            set
            {
                if (_lastfmScrobbler == null)
                {
                    _lastfmScrobbler = value;
                }
            }
        }

        #endregion Singletons (NEED IMPROVEMENTS)

        #region ICommands

        #region Definitions

        private RelayCommand _navigateToArtistPageCommand;
        private RelayCommand _changeAlbumArtCommand;
        private RelayCommand _showPropertiesCommand;
        private RelayCommand _opensonglocationCommand;
        private RelayCommand _navigateCommand;

        public ICommand NavigateToAlbumPageCommand
        {
            get
            { if (_navigateCommand == null) { _navigateCommand = new RelayCommand(param => NavigateToAlbumPage(param)); } return _navigateCommand; }
        }/// <summary>

         /// Gets command for navigating to a page./>
         /// </summary>
        public ICommand NavigateToArtistPageCommand
        {
            get
            { if (_navigateToArtistPageCommand == null) { _navigateToArtistPageCommand = new RelayCommand(param => NavigateToArtistPage(param)); } return _navigateToArtistPageCommand; }
        }

        /// <summary>
        /// Gets command for showing properties of a mediafile. This calls the <see cref="Init(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand ShowPropertiesCommand
        {
            get
            { if (_showPropertiesCommand == null) { _showPropertiesCommand = new RelayCommand(param => ShowProperties(param)); } return _showPropertiesCommand; }
        }

        /// <summary>
        /// Gets command for open song location. This calls the <see cref="OpenSongLocation(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand OpenSongLocationCommand
        {
            get
            { if (_opensonglocationCommand == null) { _opensonglocationCommand = new RelayCommand(param => OpenSongLocation(param)); } return _opensonglocationCommand; }
        }/// <summary>

         /// Gets command for open song location. This calls the <see cref="OpenSongLocation(object)"/> method. <seealso cref="ICommand"/>
         /// </summary>
        public ICommand ChangeAlbumArtCommand
        {
            get
            { if (_changeAlbumArtCommand == null) { _changeAlbumArtCommand = new RelayCommand(param => ChangeAlbumArt(param)); } return _changeAlbumArtCommand; }
        }

        #endregion Definitions

        #region Implementation

        private async void ChangeAlbumArt(object para)
        {
            Mediafile mediaFile = para as Mediafile;
            if (para == null)
            {
                mediaFile = Player.CurrentlyPlayingFile;
            }

            FileOpenPicker albumArtPicker = new FileOpenPicker();
            albumArtPicker.FileTypeFilter.Add(".jpg");
            albumArtPicker.FileTypeFilter.Add(".png");
            var albumArt = await albumArtPicker.PickSingleFileAsync();
            if (albumArt != null)
            {
                TagLib.File tagFile = TagLib.File.Create(new SimpleFileAbstraction(await StorageFile.GetFileFromPathAsync(mediaFile.Path)));
                IPicture[] pictures = new IPicture[1];
                pictures[0] = new Picture(new SimpleFileAbstraction(albumArt));
                tagFile.Tag.Pictures = pictures;
                tagFile.Save();
                var createAlbumArt = TagReaderHelper.AlbumArtFileExists(mediaFile);
                await albumArt.CopyAsync(await StorageFolder.GetFolderFromPathAsync(ApplicationData.Current.LocalFolder.Path + "\\Albumarts\\"), createAlbumArt.FileName + Path.GetExtension(albumArt.Path), NameCollisionOption.ReplaceExisting);
                mediaFile.AttachedPicture = ApplicationData.Current.LocalFolder.Path + "\\Albumarts\\" + createAlbumArt.FileName + Path.GetExtension(albumArt.Path);
            }
        }

        private async void ShowProperties(object para)
        {
            Mediafile file = para is Mediafile ? para as Mediafile : Player.CurrentlyPlayingFile;
            TagDialog tag = new TagDialog(file);
            if (CoreWindow.GetForCurrentThread().Bounds.Width >= 501)
            {
                tag.MaxWidth = CoreWindow.GetForCurrentThread().Bounds.Width - 10;
            }

            await tag.ShowAsync();
        }

        private async void OpenSongLocation(object file)
        {
            var mp3File = file as Mediafile;
            if (mp3File == null)
            {
                mp3File = Player.CurrentlyPlayingFile;
            }

            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(mp3File.Path));
            if (folder != null)
            {
                var storageFile = StorageFile.GetFileFromPathAsync(mp3File.Path);
                var folderOptions = new FolderLauncherOptions();
                folderOptions.ItemsToSelect.Add(await storageFile);
                await Launcher.LaunchFolderAsync(folder, folderOptions);
            }
        }

        private async void NavigateToAlbumPage(object para)
        {
            //because the gridview for both artists and albums is the same we need to handle,
            //item selection this way.
            if (para is Artist artist)
            {
                NavigateToArtistPage(artist);
                return;
            }
            SplitViewMenu.SplitViewMenu.UnSelectAll();
            var album = para is Album ? (Album)para : await AlbumArtistService.GetAlbum(para.ToString()).ConfigureAwait(false);
            await BreadDispatcher.InvokeAsync(() =>
            {
                if (album != null)
                    NavigationService.Instance.Frame.Navigate(typeof(PlaylistView), album);
            });
        }

        public AlbumArtistService AlbumArtistService => new AlbumArtistService(new DocumentStoreDatabaseService(DatabasePath, "Artists"));

        private async void NavigateToArtistPage(object para)
        {
            SplitViewMenu.SplitViewMenu.UnSelectAll();
            var artist = para is Artist ? (Artist)para : await AlbumArtistService.GetArtist(para.ToString()).ConfigureAwait(false);
            await BreadDispatcher.InvokeAsync(() =>
            {
                if (artist != null)
                    NavigationService.Instance.Frame.Navigate(typeof(PlaylistView), artist);
            });
        }

        #endregion Implementation

        #endregion ICommands

        #region Clean up needed

        //these methods are badly designed and redundant.
        //should be removed and cleanup.
        //#TODO clean this up
        public bool AddMediafile(Mediafile file, int index = -1)
        {
            if (file == null)
            {
                return false;
            }

            var service = new LibraryService(new DocumentStoreDatabaseService(DatabasePath, "Tracks"));

            SettingsViewModel.TracksCollection.Elements.Insert(index == -1 ? SettingsViewModel.TracksCollection.Elements.Count : index, file);
            service.AddMediafile(file);
            return true;
        }

        public async Task<bool> RemoveMediafile(Mediafile file)
        {
            if (file == null)
            {
                return false;
            }
            var service = new LibraryService(new DocumentStoreDatabaseService(DatabasePath, "Tracks"));
            SettingsViewModel.TracksCollection.Elements.Remove(file);
            await service.RemoveMediafile(file);
            return true;
        }

        public async Task<bool> AskForPassword(Playlist playlist)
        {
            if (playlist.IsPrivate)
            {
                return await ShowPasswordDialog(playlist.Hash, playlist.Salt);
            }
            return true;
        }

        public async Task<bool> ShowPasswordDialog(string hash, string salt)
        {
            var dialog = new PasswordDialog
            {
                Title = "This is private property. \rPlease enter the correct password to proceed."
            };
            if (CoreWindow.GetForCurrentThread().Bounds.Width <= 501)
            {
                dialog.DialogWidth = CoreWindow.GetForCurrentThread().Bounds.Width - 50;
            }
            else
            {
                dialog.DialogWidth = CoreWindow.GetForCurrentThread().Bounds.Width - 100;
            }

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                if (PasswordStorage.VerifyPassword(dialog.Password, salt, hash))
                {
                    return true;
                }

                return await ShowPasswordDialog(hash, salt);
            }

            return false;
        }

        /// <summary>
        /// wrongly placed.
        /// #TODO remove this to a file/path extension
        /// </summary>
        /// <param name="path"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool VerifyFileExists(string path, int timeout)
        {
            var task = new Task<bool>(() =>
            {
                var fi = new FileInfo(path);
                return fi.Exists;
            });
            task.Start();
            return task.Wait(timeout) && task.Result;
        }

        /// <summary>
        /// this method is used to get the dominant color in an image.
        /// still not sure about this. Doesn't make sense for this to be here.
        /// #TODO make a color/image extension class and move this method into it.
        /// </summary>
        /// <param name="file">the image file</param>
        /// <returns>the dominant color</returns>
        public async Task<Color> GetDominantColor(StorageFile file)
        {
            using (var stream = await file.OpenAsync(FileAccessMode.Read))
            {
                try
                {
                    //Create a decoder for the image
                    var decoder = BitmapDecoder.CreateAsync(stream);
                    var colorThief = new ColorThiefDotNet.ColorThief();
                    var qColor = await colorThief.GetColor(await decoder);

                    //read the color
                    return Color.FromArgb(qColor.Color.A, qColor.Color.R, qColor.Color.G, qColor.Color.B);
                }
                catch
                {
                    return Colors.Transparent;
                }
            }
        }

        #endregion Clean up needed

    }
}