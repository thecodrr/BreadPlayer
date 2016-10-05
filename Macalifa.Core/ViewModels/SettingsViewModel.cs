/* 
	Macalifa. A music player made for Windows 10 store.
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
using Macalifa.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Macalifa.Core;
using Windows.UI.Xaml;
using Windows.UI;
using Windows.UI.Xaml.Media;
using System.Windows.Input;
using Windows.Storage.AccessCache;
using Windows.UI.Core;
using Macalifa.Models;
using ManagedBass;

namespace Macalifa.ViewModels
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

        bool _isThemeDark;
        public bool IsThemeDark
        {
            get
            {
                _isThemeDark = false; return _isThemeDark; //ApplicationData.Current.LocalSettings.Values["SelectedTheme"].ToString() == "Light" ? true : false; 
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
        LibraryViewModel LibVM => LibraryViewService.Instance.LibVM;
        /// <summary>
        /// Loads songs from a specified folder into the library. <seealso cref="LoadCommand"/>
        /// </summary>
        public async void Load()
        {
            FolderPicker picker = new FolderPicker() { SuggestedStartLocation = PickerLocationId.MusicLibrary };
            CoreMethods Methods = new CoreMethods();
            picker.FileTypeFilter.Add(".mp3");
            StorageFolder folder = await picker.PickSingleFolderAsync();
            if (folder != null)
            {
                StorageApplicationPermissions.FutureAccessList.Add(folder);
                var filelist = Macalifa.Common.DirectoryWalker.GetFiles(folder.Path); //folder.GetFilesAsync(Windows.Storage.Search.CommonFileQuery.OrderByName); //

                LibraryFoldersCollection.Add(folder);
                foreach (var x in filelist)
                {
                    Mediafile mp3file = null;
                    StorageFile file = await StorageFile.GetFileFromPathAsync(x);
                    LibraryViewModel.Path = file.Path;
                    var path = file.Path;
                    if (LibVM.TracksCollection.Elements.All(t => t.Path != path))
                    {
                        using (var stream = await CoreWindow.GetForCurrentThread().Dispatcher.RunTaskAsync(LibraryViewModel.GetFileAsStream))
                        {
                            if (stream != null)
                            {
                                mp3file = await Methods.CreateMediafile(stream, file);
                            }
                        }
                        GetLength(mp3file);
                        LibVM.TracksCollection.AddItem(mp3file);
                        LibVM.db.Insert(mp3file);
                        LibVM.AddAlbums();
                    }
                }
                }
            }

        async void GetLength(Mediafile f)
        {
            string sPath = f.Path;
            int handle = 0;
            await Task.Run(() =>
            {
                Bass.MusicFree(handle);
                Bass.ChannelStop(handle);
                handle = Bass.CreateStream(sPath);
            });
            f.Length = Bass.ChannelBytes2Seconds(handle, Bass.ChannelGetLength(handle)).ToString();
        }
    }
}
