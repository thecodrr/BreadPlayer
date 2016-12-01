using BreadPlayer.Extensions;
using BreadPlayer.Models;
using BreadPlayer.Services;
using BreadPlayer.ViewModels;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Core;
using BreadPlayer.BreadNotificationManager;
using BreadPlayer.Service;
using System.Windows.Input;
using Windows.System;
using SplitViewMenu;
using BreadPlayer.Tags.ID3;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using BreadPlayer.Tags.ID3.ID3v2Frames.BinaryFrames;
using Windows.Storage.Streams;
using System.Diagnostics;

namespace BreadPlayer.Core
{
	public class SharedLogic
    {
        public System.Collections.ObjectModel.ObservableCollection<SimpleNavMenuItem> PlaylistsItems => GenericService<System.Collections.ObjectModel.ObservableCollection<SimpleNavMenuItem>>.Instance.GenericClass;
        public ThreadSafeObservableCollection<ContextMenuCommand> OptionItems => GenericService<ThreadSafeObservableCollection<ContextMenuCommand>>.Instance.GenericClass;// { get { return items; } set { Set(ref items, value); } }
        public static NotificationManager NotificationManager => GenericService<NotificationManager>.Instance.GenericClass;
        public static CoreBreadPlayer Player => GenericService<CoreBreadPlayer>.Instance.GenericClass;
        public static CoreDispatcher Dispatcher { get; set; } = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
        public static SettingsViewModel SettingsVM => GenericService<SettingsViewModel>.Instance.GenericClass;
      
        #region ICommands

        #region Definitions
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
        }
        #endregion

        #region Implementation
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
        public static String GetStringForNullOrEmptyProperty(string data, string setInstead)
        {
            return string.IsNullOrEmpty(data) ? setInstead : data;
        }

        /// <summary>
        /// Asynchronously saves all the album arts in the library. 
        /// </summary>
        /// <param name="Data">ID3 tag of the song to get album art data from.</param>
        public static async Task<bool> SaveImagesAsync(StorageFile file, Mediafile mp3file)
        {
            var albumartFolder = ApplicationData.Current.LocalFolder;
            var md5Path = (mp3file.Album + mp3file.LeadArtist).ToLower().ToSha1();
            if (!File.Exists(albumartFolder.Path + @"\AlbumArts\" + md5Path + ".jpg"))
            {
                TagLib.File tagFile = TagLib.File.Create(new SimpleFileAbstraction(file));
                if (tagFile.Tag.Pictures.Length >= 1)
                {
                    try
                    {
                        var albumart = await albumartFolder.CreateFileAsync(@"AlbumArts\" + md5Path + ".jpg", CreationCollisionOption.FailIfExists);

                        using (var albumstream = await albumart.OpenStreamForWriteAsync())
                        {
                            await albumstream.WriteAsync(tagFile.Tag.Pictures[0].Data.Data, 0, tagFile.Tag.Pictures[0].Data.Data.Length); 
                        }
                        return true;
                    }
                    catch (Exception ex) { Debug.Write(ex.Message + "||" + file.Path); return false; }
                }
            }
            return false;
        }
        /// <summary>
        /// Asynchronously saves all the album arts in the library. 
        /// </summary>
        /// <param name="Data">ID3 tag of the song to get album art data from.</param>
        public static async Task<bool> SaveImagesAsync(Windows.Storage.FileProperties.StorageItemThumbnail thumb, Mediafile file)
        {
            var albumartFolder = ApplicationData.Current.LocalFolder;
            var md5Path = (file.Album + file.LeadArtist).ToLower().ToSha1();

            if (!File.Exists(albumartFolder.Path + @"\AlbumArts\" + md5Path + ".jpg"))
            {
                try
                {
                    Windows.Storage.Streams.IBuffer buf;
                    Windows.Storage.Streams.Buffer inputBuffer = new Windows.Storage.Streams.Buffer(1024);
                    var albumart = await albumartFolder.CreateFileAsync(@"AlbumArts\" + md5Path + ".jpg", CreationCollisionOption.FailIfExists).AsTask().ConfigureAwait(false);
                    using (Windows.Storage.Streams.IRandomAccessStream albumstream = await albumart.OpenAsync(FileAccessMode.ReadWrite).AsTask().ConfigureAwait(false))
                    {

                        while ((buf = (await thumb.ReadAsync(inputBuffer, inputBuffer.Capacity, Windows.Storage.Streams.InputStreamOptions.None).AsTask().ConfigureAwait(false))).Length > 0)
                            await albumstream.WriteAsync(buf).AsTask().ConfigureAwait(false);
                    }
                    thumb.Dispose();
                    return true;
                }
                catch (Exception ex)
                {
                    await NotificationManager.ShowAsync(ex.Message + "||" + file.Path);
                    return false;
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
                SettingsViewModel.TracksCollection.Elements.Insert(index == -1 ? SettingsViewModel.TracksCollection.Elements.Count: index, file);
                service.AddMediafile(file);
                return true;
            }
            return false;
        }
        public static bool RemoveMediafile(Mediafile file)
        {
            if (file != null)
            {
                if (service == null)
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
                
                var albumartFolder = ApplicationData.Current.LocalFolder;
                var albumartLocation = albumartFolder.Path + @"\AlbumArts\" + (mediafile.Album + mediafile.LeadArtist).ToLower().ToSha1() + ".jpg";

                if (File.Exists(albumartLocation))
                {
                    mediafile.AttachedPicture = albumartLocation;
                }
            }
            catch (Exception ex)
            {
                await NotificationManager.ShowAsync(ex.Message + "||" + file.Path);
            }
            return mediafile;
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