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
using BreadPlayer.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using BreadPlayer.Core;
using Windows.UI.Xaml;
using Windows.UI;
using Windows.UI.Xaml.Media;
using System.Windows.Input;
using Windows.Storage.AccessCache;
using Windows.UI.Core;
using BreadPlayer.Models;
using ManagedBass;
using ManagedBass.Tags;
using System.Diagnostics;
using BreadPlayer.PlaylistBus;
using Windows.Storage.FileProperties;
using BreadPlayer.Extensions;
using Windows.Storage.Search;

namespace BreadPlayer.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel()
        {

        }
        DelegateCommand _resetCommand;
        /// <summary>
        /// Gets load library command. This calls the <see cref="Load"/> method.
        /// </summary>
        public DelegateCommand ResetCommand { get { if (_resetCommand == null) { _resetCommand = new DelegateCommand(Reset); } return _resetCommand; } }

        async void Reset()
        {
            LibVM.Dispose();
            Player.Dispose();
            await Player.Init();
            ShellVM.Dispose();
            AlbumArtistVM.Dispose();
            LibraryFoldersCollection.Clear();
            if (File.Exists(ApplicationData.Current.LocalFolder.Path + @"\breadplayer.db"))
            {
                var libFile = await StorageFile.GetFileFromPathAsync(ApplicationData.Current.LocalFolder.Path + @"\breadplayer.db");
                await libFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                LibVM.db = new Database.QueryMethods();
            }
            if (File.Exists(ApplicationData.Current.TemporaryFolder.Path + @"\lastplaying.mc"))
            {
                var saveFile = await StorageFile.GetFileFromPathAsync(ApplicationData.Current.TemporaryFolder.Path + @"\lastplaying.mc");
                await saveFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
            if(Directory.Exists(ApplicationData.Current.LocalFolder.Path + @"\Albumarts"))
            {
                var saveFile = await StorageFolder.GetFolderFromPathAsync(ApplicationData.Current.LocalFolder.Path + @"\Albumarts");
                await saveFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
        }
        DelegateCommand _loadCommand;
        /// <summary>
        /// Gets load library command. This calls the <see cref="Load"/> method.
        /// </summary>
        public DelegateCommand LoadCommand { get { if (_loadCommand == null) { _loadCommand = new DelegateCommand(Load); } return _loadCommand; } }

        DelegateCommand _importPlaylistCommand;
        /// <summary>
        /// Gets load library command. This calls the <see cref="Load"/> method.
        /// </summary>
        public DelegateCommand ImportPlaylistCommand { get { if (_importPlaylistCommand == null) { _importPlaylistCommand = new DelegateCommand(ImportPlaylists); } return _importPlaylistCommand; } }
        async void ImportPlaylists()
        {
            var picker = new FileOpenPicker();
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
            openPicker.FileTypeFilter.Add(".m3u");
            openPicker.FileTypeFilter.Add(".pls");
            StorageFile file = await openPicker.PickSingleFileAsync();          
            if(file != null)
            {
                IPlaylist playlist = null;
                if (Path.GetExtension(file.Path) == ".m3u") playlist = new M3U();
                else playlist = new PLS();
                var dict = await playlist.LoadPlaylist(file);
                LibVM.AddPlaylist(dict, file.DisplayName, "");
            }
        }
        bool _isThemeDark;
        public bool IsThemeDark
        {
            get
            {
                if (ApplicationData.Current.LocalSettings.Values["SelectedTheme"] != null)
                    _isThemeDark = ApplicationData.Current.LocalSettings.Values["SelectedTheme"].ToString() == "Light" ? true : false;
                else
                    _isThemeDark = true;
                return _isThemeDark;
                //ApplicationData.Current.LocalSettings.Values["SelectedTheme"].ToString() == "Light" ? true : false; 
            }
            set
            {
                Set(ref _isThemeDark, value);
                ApplicationData.Current.LocalSettings.Values["SelectedTheme"] = _isThemeDark == true ? "Light" : "Dark";
            }
        }
        public ThreadSafeObservableCollection<StorageFolder> _LibraryFoldersCollection ;
        public ThreadSafeObservableCollection<StorageFolder> LibraryFoldersCollection
        {
            get {
                if (_LibraryFoldersCollection == null)
                {
                    _LibraryFoldersCollection = new ThreadSafeObservableCollection<StorageFolder>();
                }
                return _LibraryFoldersCollection; }
            set { Set(ref _LibraryFoldersCollection, value); }
        }
        /// <summary>
        /// Loads songs from a specified folder into the library. <seealso cref="LoadCommand"/>
        /// </summary>
        public async void Load()
        {
            var stop = System.Diagnostics.Stopwatch.StartNew();
            FolderPicker picker = new FolderPicker() { SuggestedStartLocation = PickerLocationId.MusicLibrary };
            CoreMethods Methods = new CoreMethods();
            picker.FileTypeFilter.Add(".mp3");
            picker.FileTypeFilter.Add(".wav");
            picker.FileTypeFilter.Add(".ogg");
            picker.FileTypeFilter.Add(".flac");
            picker.FileTypeFilter.Add(".m4a");
            picker.FileTypeFilter.Add(".aif");
            picker.FileTypeFilter.Add(".wma");
            StorageFolder folder = await picker.PickSingleFolderAsync();            
            if (folder != null)
            {
                LibraryFoldersCollection.Add(folder);
                StorageApplicationPermissions.FutureAccessList.Add(folder);
                QueryOptions options = new QueryOptions(CommonFileQuery.OrderByName, new String[] { ".mp3" });
                options.FileTypeFilter.Add(".wav");
                options.FileTypeFilter.Add(".ogg");
                options.FileTypeFilter.Add(".flac");
                options.FileTypeFilter.Add(".m4a");
                options.FileTypeFilter.Add(".aif");
                options.FileTypeFilter.Add(".wma");
                options.FolderDepth = FolderDepth.Deep;
                options.SetThumbnailPrefetch(ThumbnailMode.MusicView, 300, ThumbnailOptions.UseCurrentScale);
                options.IndexerOption = IndexerOption.UseIndexerWhenAvailable;
                options.SetPropertyPrefetch(PropertyPrefetchOptions.MusicProperties, new String[] { "System.Music.AlbumTitle", "System.Music.Artist", "System.Music.Genre" });
                StorageFileQueryResult queryResult = folder.CreateFileQueryWithOptions(options);
                uint index = 0, stepSize = 50;
                IReadOnlyList<StorageFile> files = await queryResult.GetFilesAsync(index, stepSize);
                index +=50;
                var tempList = new List<Mediafile>();
                double i = 0;
                var count = await queryResult.GetItemCountAsync();
                while (files.Count != 0)
                {
                    var fileTask = queryResult.GetFilesAsync(index, stepSize).AsTask();                   
                    Mediafile mp3file = null;
                    foreach (StorageFile file in files)
                    {
                        i++;
                        LibVM.SongCount++;
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => 
                        {
                                await NotificationManager.ShowAsync(i.ToString() + "\\" + count.ToString() + " Song(s) Loaded", "Loading...");
                         });
                        string path = file.Path;
                        if (LibVM.TracksCollection.Elements.All(t => t.Path != path))
                        {
                            try
                            {
                                mp3file = await CoreMethods.CreateMediafile(file, false);
                                tempList.Add(mp3file);
                            }
                            catch { }
                        }
                        ShellVM.PlayPauseCommand.IsEnabled = true;
                    }
                    LibVM.TracksCollection.Elements.AddRange(tempList);
                    LibVM.db.Insert(tempList);
                    tempList.Clear();
                    files = await fileTask.ConfigureAwait(false);
                    index += 50;
                }
                await AlbumArtistVM.AddAlbums().ConfigureAwait(false);
                stop.Stop();
                await NotificationManager.ShowAsync(i.ToString() + " Song(s) loaded in " + Convert.ToInt32(stop.Elapsed.TotalSeconds).ToString() + " seconds", "Library loaded!");
            }
        }
        public async void ShowMessage(string msg)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(msg);
            await dialog.ShowAsync();
        }
    }
}
