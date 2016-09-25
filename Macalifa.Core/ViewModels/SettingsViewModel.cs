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
            get { _isThemeDark = ApplicationData.Current.LocalSettings.Values["SelectedTheme"].ToString() == "Light" ? true : false; return _isThemeDark; }
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
                var filelist = Macalifa.Common.DirectoryWalker.GetFiles(folder.Path);               
                LibraryFoldersCollection.Add(folder);
                foreach (var x in filelist)
                {
                    StorageFile file = await StorageFile.GetFileFromPathAsync(x);
                    LibraryViewModel.Path = file.Path;
                    using (var stream = await LibVM.Dispatcher.RunTaskAsync(LibraryViewModel.GetFileAsStream))
                    {
                        if (stream != null)
                        {
                            var path = file.Path;
                            if (LibVM.TracksCollection.Elements.All(t => t.Path != path))
                            {
                                var m = await Methods.CreateMediafile(stream);
                                LibVM.TracksCollection.AddItem(m);
                                LibVM.db.Insert(m);
                            }
                        }
                    }
                }
            }
        }
    }
}
