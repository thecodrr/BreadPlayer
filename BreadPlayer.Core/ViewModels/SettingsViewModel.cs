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

namespace BreadPlayer.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel()
        {

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
            IPlaylist playlist = null;
            if(Path.GetExtension(file.Path) == ".m3u") playlist = new M3U();
            else playlist = new PLS();
            var dict = await playlist.LoadPlaylist(file);
            LibVM.AddPlaylist(dict, file.DisplayName, "");
        }
        bool _isThemeDark;
        public bool IsThemeDark
        {
            get
            {
               if(ApplicationData.Current.LocalSettings.Values["SelectedTheme"] != null) _isThemeDark = ApplicationData.Current.LocalSettings.Values["SelectedTheme"].ToString() == "Light" ? true : false; return _isThemeDark; //ApplicationData.Current.LocalSettings.Values["SelectedTheme"].ToString() == "Light" ? true : false; 
            }
            set
            {
                Set(ref _isThemeDark, value);
                ApplicationData.Current.LocalSettings.Values["SelectedTheme"] = _isThemeDark == true ? "Light" : "Dark";
            }
        }

        public ThreadSafeObservableCollection<StorageFolder> _LibraryFoldersCollection;
        public ThreadSafeObservableCollection<StorageFolder> LibraryFoldersCollection
        {
            get { if (_LibraryFoldersCollection == null) { _LibraryFoldersCollection = new ThreadSafeObservableCollection<StorageFolder>(); } return _LibraryFoldersCollection; }
            set { Set(ref _LibraryFoldersCollection, value); }
        }
        /// <summary>
        /// Loads songs from a specified folder into the library. <seealso cref="LoadCommand"/>
        /// </summary>
        public async void Load()
        {
            FolderPicker picker = new FolderPicker() { SuggestedStartLocation = PickerLocationId.MusicLibrary };
            CoreMethods Methods = new CoreMethods();
            picker.FileTypeFilter.Add(".mp3");
            StorageFolder folder = await picker.PickSingleFolderAsync();
            LibraryFoldersCollection.Add(folder);
            if (folder != null)
            {
                if (StorageApplicationPermissions.FutureAccessList.Entries.Count <= 999)
                    StorageApplicationPermissions.FutureAccessList.Clear();
                StorageApplicationPermissions.FutureAccessList.Add(folder);
                var filelist = await BreadPlayer.Common.DirectoryWalker.GetFiles(folder.Path); //folder.GetFilesAsync(Windows.Storage.Search.CommonFileQuery.OrderByName); //
                var tempList = new List<Mediafile>();
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(2);
                timer.Start();
                var stop = System.Diagnostics.Stopwatch.StartNew();
                double i = 0;
                var count = filelist.Count();
                foreach (var x in filelist)
                {
                    i++;
                    double percent = (i / count) * 100;
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { LibVM.Progress = percent; });
                    Mediafile mp3file = null;
                    StorageFile file = await StorageFile.GetFileFromPathAsync(x);
                    string path = file.Path;

                    if (LibVM.TracksCollection.Elements.All(t => t.Path != path))
                    {
                        try
                        {

                            mp3file = await CoreMethods.CreateMediafile(file);
                            tempList.Add(mp3file);
                        }
                        catch { }
                      
                            timer.Tick += (sender, e) =>
                            {
                                LibVM.TracksCollection.AddRange(tempList);
                                LibVM.db.Insert(tempList);
                                tempList.Clear();
                            };
                        
                    }
                }
                AlbumArtistVM.AddAlbums();
                stop.Stop();
                ShowMessage(stop.ElapsedMilliseconds.ToString() + "    " + LibVM.TracksCollection.Elements.Count.ToString());
            }
        }
        public async void ShowMessage(string msg)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(msg);
            await dialog.ShowAsync();
        }
    }
}
