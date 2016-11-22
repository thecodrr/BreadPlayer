using BreadPlayer.Extensions;
using BreadPlayer.Models;
using BreadPlayer.Services;
using BreadPlayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using BreadPlayer;
using Windows.UI.Core;
using ManagedBass;
using ManagedBass.Tags;
using Windows.Storage.FileProperties;
using BreadPlayer.BreadNotificationManager;
using BreadPlayer.Service;
using System.Windows.Input;
using Windows.System;

namespace BreadPlayer.Core
{
    public class SharedLogic : ObservableObject
    {
        public ThreadSafeObservableCollection<ContextMenuCommand> OptionItems => GenericService<ThreadSafeObservableCollection<ContextMenuCommand>>.Instance.GenericClass;// { get { return items; } set { Set(ref items, value); } }
        public static NotificationManager NotificationManager => GenericService<NotificationManager>.Instance.GenericClass;
        public static LibraryViewModel LibVM => GenericService<LibraryViewModel>.Instance.GenericClass;
        public static ShellViewModel ShellVM => GenericService<ShellViewModel>.Instance.GenericClass;
        public static CoreBreadPlayer Player => GenericService<CoreBreadPlayer>.Instance.GenericClass;
        public static PlaylistViewModel PlaylistVM => GenericService<PlaylistViewModel>.Instance.GenericClass;
        public static AlbumArtistViewModel AlbumArtistVM => GenericService<AlbumArtistViewModel>.Instance.GenericClass;
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
        public static async Task SaveImagesAsync(Windows.Storage.FileProperties.StorageItemThumbnail thumb, Mediafile file)
        {
            var albumartFolder = ApplicationData.Current.LocalFolder;
            var md5Path = (file.Album + file.LeadArtist).ToLower().ToSha1();

            if (!File.Exists(albumartFolder.Path + @"\AlbumArts\" + md5Path + ".jpg"))
            {
                Windows.Storage.Streams.IBuffer buf;
                Windows.Storage.Streams.Buffer inputBuffer = new Windows.Storage.Streams.Buffer(1024);
                var albumart = await albumartFolder.CreateFileAsync(@"AlbumArts\" + md5Path + ".jpg", CreationCollisionOption.FailIfExists).AsTask().ConfigureAwait(false);
                using (Windows.Storage.Streams.IRandomAccessStream albumstream = await albumart.OpenAsync(FileAccessMode.ReadWrite).AsTask().ConfigureAwait(false))
                {
                    try
                    {
                        while ((buf = (await thumb.ReadAsync(inputBuffer, inputBuffer.Capacity, Windows.Storage.Streams.InputStreamOptions.None).AsTask().ConfigureAwait(false))).Length > 0)
                            await albumstream.WriteAsync(buf).AsTask().ConfigureAwait(false);

                    }
                    catch (Exception ex) { await NotificationManager.ShowAsync(ex.Message + "||" + file.Path); }

                }
                thumb.Dispose();
            }
        }

       // static LibraryService service = new LibraryService(new DatabaseService());
        public static bool AddMediafile(Mediafile file, int index = -1)
        {
            if (file != null)
            {
                SettingsViewModel.TracksCollection.Elements.Insert(index == -1 ? SettingsViewModel.TracksCollection.Elements.Count: index, file);
              //  service.AddMediafile(file);
                return true;
            }
            return false;
        }
        public static bool RemoveMediafile(Mediafile file)
        {
            if (file != null)
            {
                SettingsViewModel.TracksCollection.Elements.Remove(file);
               // service.RemoveMediafile(file);
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
           
            var Mediafile = new Mediafile();
            try
            {
                if (cache == true)
                    Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(file);

                Mediafile._id = LiteDB.ObjectId.NewObjectId();
                Mediafile.Path = file.Path;
                Mediafile.OrginalFilename = file.DisplayName;
                Mediafile.State = PlayerState.Stopped;
                var properties = await file.Properties.GetMusicPropertiesAsync().AsTask().ConfigureAwait(false);
                Mediafile.Title = GetStringForNullOrEmptyProperty(properties.Title, System.IO.Path.GetFileNameWithoutExtension(file.Path));
                Mediafile.Album = GetStringForNullOrEmptyProperty(properties.Album, "Unknown Album");
                Mediafile.LeadArtist = GetStringForNullOrEmptyProperty(properties.Artist, "Unknown Artist");
                Mediafile.Genre = string.Join(",", properties.Genre);
                Mediafile.Year = properties.Year.ToString();
                Mediafile.TrackNumber = properties.TrackNumber.ToString();
                Mediafile.Length = GetStringForNullOrEmptyProperty(properties.Duration.ToString(@"mm\:ss"), "00:00");

                var albumartFolder = ApplicationData.Current.LocalFolder;
                var albumartLocation = albumartFolder.Path + @"\AlbumArts\" + (Mediafile.Album + Mediafile.LeadArtist).ToLower().ToSha1() + ".jpg";

                if (File.Exists(albumartLocation))
                {
                    Mediafile.AttachedPicture = albumartLocation;
                }
            }
            catch(Exception ex) { await NotificationManager.ShowAsync(ex.Message + "||" + file.Path); }
            return Mediafile;
        }
    }

}
