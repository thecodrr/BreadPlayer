using BreadPlayer.Extensions;
using BreadPlayer.Models;
using BreadPlayer.Services;
using BreadPlayer.ViewModels;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Core;
using BreadPlayer.Service;
using System.Windows.Input;
using Windows.System;
using SplitViewMenu;
using Windows.Graphics.Imaging;
using Windows.UI;
using Windows.UI.Xaml.Media;
using BreadPlayer.NotificationManager;
using Windows.Storage.FileProperties;
using BreadPlayer.Web.Lastfm;
using Windows.Storage.Pickers;
using BreadPlayer.Dialogs;
using Windows.UI.Xaml.Controls;
using BreadPlayer.Common;

namespace BreadPlayer.Core
{
    public class SharedLogic
    {
        public SharedLogic()
        {
            InitializeCore.Dispatcher = new Dispatcher.BreadDispatcher(Dispatcher);
            InitializeCore.NotificationManager = NotificationManager;
            InitializeCore.EqualizerSettingsHelper = new RoamingSettingsHelper();
        }
        public System.Collections.ObjectModel.ObservableCollection<SimpleNavMenuItem> PlaylistsItems => GenericService<System.Collections.ObjectModel.ObservableCollection<SimpleNavMenuItem>>.Instance.GenericClass;
        public ThreadSafeObservableCollection<ContextMenuCommand> OptionItems => GenericService<ThreadSafeObservableCollection<ContextMenuCommand>>.Instance.GenericClass;// { get { return items; } set { Set(ref items, value); } }
        public static BreadNotificationManager NotificationManager => GenericService<BreadNotificationManager>.Instance.GenericClass;// { get { return items; } set { Set(ref items, value); } }
        public static CoreBreadPlayer Player => GenericService<CoreBreadPlayer>.Instance.GenericClass;
        public static CoreDispatcher Dispatcher { get; set; } = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
        public static SettingsViewModel SettingsVM => GenericService<SettingsViewModel>.Instance.GenericClass;
        public Lastfm LastfmScrobbler { get; set; }
        public static Windows.UI.Xaml.Thickness DynamicMargin
        {
            get
            {
                if (CoreWindow.GetForCurrentThread().Bounds.Width < 600)
                {
                    return new Windows.UI.Xaml.Thickness(28, 0, 0, 0);
                }
                else
                    return new Windows.UI.Xaml.Thickness(48, 0, 0, 0);
            }
        }
        #region ICommands

        #region Definitions
        RelayCommand _changeAlbumArtCommand;
        RelayCommand _showPropertiesCommand;
        RelayCommand _opensonglocationCommand;
        /// <summary>
        /// Gets command for showing properties of a mediafile. This calls the <see cref="Init(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand ShowPropertiesCommand
        {
            get
            { if (_showPropertiesCommand == null) { _showPropertiesCommand = new RelayCommand(param => this.ShowProperties(param)); } return _showPropertiesCommand; }
        }
        /// <summary>
        /// Gets command for open song location. This calls the <see cref="OpenSongLocation(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand OpenSongLocationCommand
        {
            get
            { if (_opensonglocationCommand == null) { _opensonglocationCommand = new RelayCommand(param => this.OpenSongLocation(param)); } return _opensonglocationCommand; }
        }/// <summary>
         /// Gets command for open song location. This calls the <see cref="OpenSongLocation(object)"/> method. <seealso cref="ICommand"/>
         /// </summary>
        public ICommand ChangeAlbumArtCommand
        {
            get
            { if (_changeAlbumArtCommand == null) { _changeAlbumArtCommand = new RelayCommand(param => this.ChangeAlbumArt(param)); } return _changeAlbumArtCommand; }
        }
        #endregion

        #region Implementation
        private async void ChangeAlbumArt(object para)
        {
            Mediafile mediaFile = null;
            if (para == null)
                mediaFile = Player.CurrentlyPlayingFile;
            FileOpenPicker albumArtPicker = new FileOpenPicker();
            albumArtPicker.FileTypeFilter.Add(".jpg");
            albumArtPicker.FileTypeFilter.Add(".png");
            var albumArt = await albumArtPicker.PickSingleFileAsync();
            if (albumArt != null)
            {
                TagLib.File tagFile = TagLib.File.Create(new SimpleFileAbstraction(await StorageFile.GetFileFromPathAsync(mediaFile.Path)));
                TagLib.IPicture[] pictures = new TagLib.IPicture[1];
                pictures[0] = new TagLib.Picture(new SimpleFileAbstraction(albumArt));
                tagFile.Tag.Pictures = pictures;
                tagFile.Save();
                var createAlbumArt = AlbumArtFileExists(mediaFile);
                await albumArt.CopyAsync(await StorageFolder.GetFolderFromPathAsync(ApplicationData.Current.LocalFolder.Path + "\\Albumarts\\"), createAlbumArt.FileName + Path.GetExtension(albumArt.Path), NameCollisionOption.ReplaceExisting);
                mediaFile.AttachedPicture = ApplicationData.Current.LocalFolder.Path + "\\Albumarts\\" + createAlbumArt.FileName + Path.GetExtension(albumArt.Path);
            }         
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
        async void OpenSongLocation(object file)
        {
            var mp3File = file as Mediafile;
            if (mp3File == null)
                mp3File = Player.CurrentlyPlayingFile;
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(System.IO.Path.GetDirectoryName(mp3File.Path));
            if (folder != null)
            {
                StorageFile storageFile = await StorageFile.GetFileFromPathAsync(mp3File.Path);
                var folderOptions = new FolderLauncherOptions();
                folderOptions.ItemsToSelect.Add(storageFile);
                await Launcher.LaunchFolderAsync(folder, folderOptions);                
            }
        }

        #endregion

        #endregion

        public async Task SplitList(GroupedObservableCollection<string, Mediafile> collection, int nSize = 30)
        {
            for (int i = 0; i < service.SongCount; i += nSize)
            {
                collection.AddRange(await service.GetRangeOfMediafiles(i, Math.Min(nSize, service.SongCount - i)).ConfigureAwait(false), false, false);
            }
        }

        public static string GetStringForNullOrEmptyProperty(string data, string setInstead)
        {
            return string.IsNullOrEmpty(data) ? setInstead : data;
        }

        public static async Task<Color> GetDominantColor(StorageFile file)
        {
            using (var stream = await file.OpenAsync(FileAccessMode.Read))
            {
                try
                {
                    //Create a decoder for the image
                    var decoder = await BitmapDecoder.CreateAsync(stream);
                    var colorThief = new ColorThiefDotNet.ColorThief();
                    var qColor = await colorThief.GetColor(decoder);

                    //read the color 
                    return Color.FromArgb(qColor.Color.A, qColor.Color.R, qColor.Color.G, qColor.Color.B);
                }
                catch { return (App.Current.Resources["PhoneAccentBrush"] as SolidColorBrush).Color; }
            }
        }
        
        private static (bool NotExists, string FileName) AlbumArtFileExists(Mediafile file)
        {
            var albumartFolder = ApplicationData.Current.LocalFolder;
            var md5Path = (file.Album + file.LeadArtist).ToLower().ToSha1();
            if (!File.Exists(albumartFolder.Path + @"\AlbumArts\" + md5Path + ".jpg"))
            {
                //var albumart = await albumartFolder.CreateFileAsync(@"AlbumArts\" + md5Path + ".jpg", CreationCollisionOption.FailIfExists).AsTask().ConfigureAwait(false);
                return (true, md5Path);
            }
            return (false, md5Path);
        }
      
        /// <summary>
        /// Asynchronously saves all the album arts in the library. 
        /// </summary>
        /// <param name="Data">ID3 tag of the song to get album art data from.</param>
        public static async Task<bool> SaveImagesAsync(StorageFile file, Mediafile mediafile)
        {
            using (StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView, 300, ThumbnailOptions.UseCurrentScale))
            {
                var albumArt = AlbumArtFileExists(mediafile);
                if (albumArt.NotExists)
                {
                    try
                    {
                      
                        switch (thumbnail.Type)
                        {
                            case ThumbnailType.Image:
                                {
                                    var albumart = await ApplicationData.Current.LocalFolder.CreateFileAsync(@"AlbumArts\" + albumArt.FileName + ".jpg", CreationCollisionOption.FailIfExists);

                                    Windows.Storage.Streams.IBuffer buf;
                                    Windows.Storage.Streams.Buffer inputBuffer = new Windows.Storage.Streams.Buffer(1024);
                                    using (Windows.Storage.Streams.IRandomAccessStream albumstream = await albumart.OpenAsync(FileAccessMode.ReadWrite))
                                    {
                                        while ((buf = (await thumbnail.ReadAsync(inputBuffer, inputBuffer.Capacity, Windows.Storage.Streams.InputStreamOptions.None))).Length > 0)
                                            await albumstream.WriteAsync(buf).AsTask().ConfigureAwait(false);
                                    }
                                    thumbnail.Dispose();
                                    return true;
                                }
                            //case ThumbnailType.Icon:
                            //default:
                            //    TagLib.File tagFile = TagLib.File.Create(new SimpleFileAbstraction(file));
                            //    if (tagFile.Tag.Pictures.Length >= 1)
                            //    {
                            //        var albumart = await ApplicationData.Current.LocalFolder.CreateFileAsync(@"AlbumArts\" + albumArt.FileName + ".jpg", CreationCollisionOption.FailIfExists);

                            //        using (var albumstream = await albumart.OpenStreamForWriteAsync())
                            //        {
                            //            await albumstream.WriteAsync(tagFile.Tag.Pictures[0].Data.Data, 0, tagFile.Tag.Pictures[0].Data.Data.Length);
                            //        }
                            //        return true;
                            //    }
                            //    break;
                        }
                    }
                    catch (Exception ex)
                    {
                        await NotificationManager.ShowMessageAsync(ex.Message + "||" + file.Path);
                        return false;
                    }
                }

            }
            return false;
        }

        static LibraryService service = new LibraryService(new DatabaseService());
        public static bool AddMediafile(Mediafile file, int index = -1)
        {
            if (file != null)
            {
                if (service == null)
                    service = new LibraryService(new DatabaseService());
                SettingsViewModel.TracksCollection.Elements.Insert(index == -1 ? SettingsViewModel.TracksCollection.Elements.Count : index, file);
                service.AddMediafile(file);
                return true;
            }
            return false;
        }
        public static bool RemoveMediafile(Mediafile file)
        {
            if (file != null)
            {
                service = new LibraryService(new DatabaseService());
                SettingsViewModel.TracksCollection.Elements.Remove(file);
                service.RemoveMediafile(file);
                return true;
            }
            return false;
        }
        public static bool VerifyFileExists(string path, int timeout)
        {
            var task = new Task<bool>(() =>
            {
                var fi = new FileInfo(path);
                return fi.Exists;
            });
            task.Start();
            return task.Wait(timeout) && task.Result;
        }
        public static async Task<Mediafile> CreateMediafile(StorageFile file, bool cache = false)
        {

            var mediafile = new Mediafile();
            try
            {               
                if (cache == true)
                {
                    Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(file);
                }
                mediafile._id = LiteDB.ObjectId.NewObjectId();
                mediafile.Path = file.Path;
                mediafile.OrginalFilename = file.DisplayName;
                mediafile.State = PlayerState.Stopped;
                var properties = await file.Properties.GetMusicPropertiesAsync().AsTask().ConfigureAwait(false);
                mediafile.Title = GetStringForNullOrEmptyProperty(properties.Title, System.IO.Path.GetFileNameWithoutExtension(file.Path));
                mediafile.Album = GetStringForNullOrEmptyProperty(properties.Album, "Unknown Album");
                mediafile.LeadArtist = GetStringForNullOrEmptyProperty(properties.Artist, "Unknown Artist");
                mediafile.Genre = string.Join(",", properties.Genre);
                mediafile.Year = properties.Year.ToString();
                mediafile.TrackNumber = properties.TrackNumber.ToString();
                mediafile.Length = GetStringForNullOrEmptyProperty(properties.Duration.ToString(@"mm\:ss"), "00:00");
                mediafile.AddedDate = DateTime.Now.ToString();
                var albumartFolder = ApplicationData.Current.LocalFolder;
                var albumartLocation = albumartFolder.Path + @"\AlbumArts\" + (mediafile.Album + mediafile.LeadArtist).ToLower().ToSha1() + ".jpg";

                if (File.Exists(albumartLocation))
                {
                    mediafile.AttachedPicture = albumartLocation;
                }
            }
            catch (Exception ex)
            {
                await NotificationManager.ShowMessageAsync(ex.Message + "||" + file.Path);
            }
            return mediafile;
        }
        public async Task<bool> AskForPassword(Playlist playlist)
        {
            if (playlist.IsPrivate)
            {
                if (await ShowPasswordDialog(playlist.Hash, playlist.Salt))
                {
                    return true;
                }
            }
            else
                return true;
            return false;
        }
        public async Task<bool> ShowPasswordDialog(string hash, string salt)
        {
            var dialog = new PasswordDialog()
            {
                Title = "This is private property. \rPlease enter the correct password to proceed.",
            };
            if (CoreWindow.GetForCurrentThread().Bounds.Width <= 501)
                dialog.DialogWidth = CoreWindow.GetForCurrentThread().Bounds.Width - 50;
            else
                dialog.DialogWidth = CoreWindow.GetForCurrentThread().Bounds.Width - 100;
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                if (Common.PasswordStorage.VerifyPassword(dialog.Password, salt, hash))
                    return true;
                else
                    return await ShowPasswordDialog(hash, salt);
            }

            return false;
        }
    }

    public class SimpleFileAbstraction : TagLib.File.IFileAbstraction
    {
        private StorageFile file;

        public SimpleFileAbstraction(StorageFile file)
        {
            this.file = file;
        }

        public string Name
        {
            get { return file.Name; }
        }

        public System.IO.Stream ReadStream
        {
            get { return file.OpenStreamForReadAsync().Result; }
        }

        public System.IO.Stream WriteStream
        {
            get { return file.OpenStreamForWriteAsync().Result; }
        }

        public void CloseStream(System.IO.Stream stream)
        {
            stream.Position = 0;
        }
    }
}
