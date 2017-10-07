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

using BreadPlayer.Common;
using BreadPlayer.Core;
using BreadPlayer.Core.Common;
using BreadPlayer.Core.Extensions;
using BreadPlayer.Core.Models;
using BreadPlayer.Database;
using BreadPlayer.Dispatcher;
using BreadPlayer.Extensions;
using BreadPlayer.Helpers;
using BreadPlayer.Messengers;
using BreadPlayer.Models;
using BreadPlayer.PlaylistBus;
using BreadPlayer.Services;
using BreadPlayer.SettingsViews;
using BreadPlayer.SettingsViews.ViewModels;
using BreadPlayer.Themes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Search;
using Windows.System.Display;
using Windows.UI.Popups;

namespace BreadPlayer.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        #region Properties

        public AccountsViewModel AccountSettingsVM { get; set; }
        public AudioSettingsViewModel AudioSettingsVM { get; set; }
        public PersonalizationViewModel PersonalizationVM { get; set; }
        private ThreadSafeObservableCollection<SettingGroup> settingsCollection;

        public ThreadSafeObservableCollection<SettingGroup> SettingsCollection
        {
            get => settingsCollection;
            set => Set(ref settingsCollection, value);
        }
        private bool _preventScreenFromLocking;

        public bool PreventScreenFromLocking
        {
            get => _preventScreenFromLocking;
            set
            {
                Set(ref _preventScreenFromLocking, value);
                if (value)
                {
                    KeepScreenActive();
                }
                else
                {
                    ReleaseDisplayRequest();
                }
            }
        }

        private bool _replaceLockscreenWithAlbumArt;

        public bool ReplaceLockscreenWithAlbumArt
        {
            get => _replaceLockscreenWithAlbumArt;
            set
            {
                Set(ref _replaceLockscreenWithAlbumArt, value);
                SettingsHelper.SaveLocalSetting("ReplaceLockscreenWithAlbumArt", value);
            }
        }       

        private ThreadSafeObservableCollection<StorageFolder> _LibraryFoldersCollection;
        public ThreadSafeObservableCollection<StorageFolder> LibraryFoldersCollection
        {
            get
            {
                if (_LibraryFoldersCollection == null)
                {
                    _LibraryFoldersCollection = new ThreadSafeObservableCollection<StorageFolder>();
                }
                return _LibraryFoldersCollection;
            }
            set => Set(ref _LibraryFoldersCollection, value);
        }

        public static GroupedObservableCollection<IGroupKey, Mediafile> TracksCollection
        { get; set; }
        private LibraryService LibraryService { get; set; }
        private StorageLibraryService StorageLibraryService { get; set; }
        #endregion Properties

        #region MessageHandling

        private async void HandleLibraryLoadedMessage(Message message)
        {
            if (message.Payload is List<object> list)
            {
                TracksCollection = list[0] as GroupedObservableCollection<IGroupKey, Mediafile>;
                if (LibraryService.SongCount <= 0)
                {
                    await AutoLoadMusicLibraryAsync().ConfigureAwait(false);
                }
                if (TracksCollection != null)
                {
                    message.HandledStatus = MessageHandledStatus.HandledContinue;
                }
            }
            Messenger.Instance.DeRegister(MessageTypes.MsgLibraryLoaded, new Action<Message>(HandleLibraryLoadedMessage));
        }

        #endregion MessageHandling


        #region Ctor

        public SettingsViewModel()
        {
            InitSettingsCollection();
            AccountSettingsVM = new AccountsViewModel();
            AudioSettingsVM = new AudioSettingsViewModel();
            PersonalizationVM = new PersonalizationViewModel();

            LibraryService = new LibraryService(new DocumentStoreDatabaseService(SharedLogic.Instance.DatabasePath, "Tracks"));
            PropertyChanged += SettingsViewModel_PropertyChanged;
            _replaceLockscreenWithAlbumArt = SettingsHelper.GetLocalSetting<bool>("replaceLockscreenWithAlbumArt", false);
            Messenger.Instance.Register(MessageTypes.MsgLibraryLoaded, new Action<Message>(HandleLibraryLoadedMessage));
            StorageLibraryService = new StorageLibraryService();
            StorageLibraryService.StorageItemsUpdated += StorageLibraryService_StorageItemsUpdated;
            LoadFolders();
        }

        public void InitSettingsCollection()
        {
            SettingsCollection = new ThreadSafeObservableCollection<SettingGroup>()
            {
                new SettingGroup("\uE771", SharedLogic.Instance.ResourceLoader.GetString("personlizationSettingsText"), SharedLogic.Instance.ResourceLoader.GetString("personalizationSettingsSubtitle"), typeof(PersonlizationView)),
                new SettingGroup("\uE910", SharedLogic.Instance.ResourceLoader.GetString("accountsSettingsText"), SharedLogic.Instance.ResourceLoader.GetString("accountsSettingsSubtitle"), typeof(AccountsView)),
                new SettingGroup("\uE144", SharedLogic.Instance.ResourceLoader.GetString("keyboardSettingsText"), SharedLogic.Instance.ResourceLoader.GetString("keyboardSettingsSubtitle"), typeof(KeyboardSettingsView)),
                new SettingGroup("\uE770", SharedLogic.Instance.ResourceLoader.GetString("coreSettingsText"), SharedLogic.Instance.ResourceLoader.GetString("coreSettingsSubtitle"), typeof(CoreSettingsView)),
                new SettingGroup("\uE7F6", SharedLogic.Instance.ResourceLoader.GetString("audioSettingsText"), SharedLogic.Instance.ResourceLoader.GetString("audioSettingsSubtitle"), typeof(AudioSettingsView)),
                new SettingGroup("\uE779", SharedLogic.Instance.ResourceLoader.GetString("contactSettingsText"), SharedLogic.Instance.ResourceLoader.GetString("contactSettingsSubtitle"), typeof(ContactView)),
                new SettingGroup("\uE946", SharedLogic.Instance.ResourceLoader.GetString("aboutSettingsText"), SharedLogic.Instance.ResourceLoader.GetString("aboutSettingsSubtitle"), typeof(AboutView)),
                new SettingGroup("\uE789", SharedLogic.Instance.ResourceLoader.GetString("contributeSettingsText"), SharedLogic.Instance.ResourceLoader.GetString("contributeSettingsSubtitle"), typeof(ContributeView)),
            };
        }

        #endregion Ctor

        #region Commands

        #region Definitions

        private DelegateCommand _resetCommand;

        /// <summary>
        /// Gets load library command. This calls the <see cref="Load"/> method.
        /// </summary>
        public DelegateCommand ResetCommand { get { if (_resetCommand == null) { _resetCommand = new DelegateCommand(Reset); } return _resetCommand; } }

        #endregion Definitions

        #region Implementation
        private async void Reset()
        {
            try
            {
                Messenger.Instance.NotifyColleagues(MessageTypes.MsgDispose);
                LibraryFoldersCollection.Clear();
                await ApplicationData.Current.ClearAsync();
                ResetCommand.IsEnabled = false;
                await Task.Delay(200);
                ResetCommand.IsEnabled = true;
                LibraryService = new LibraryService(new DocumentStoreDatabaseService(SharedLogic.Instance.DatabasePath, "Tracks"));
            }
            catch (Exception ex)
            {
                BLogger.E("Error occured while resetting the player.", ex);
            }
        }
        #endregion Implementation

        #endregion Commands

        #region Methods

        #region General Settings Methods

        private DisplayRequest _displayRequest;

        private void KeepScreenActive()
        {
            if (_displayRequest == null)
            {
                _displayRequest = new DisplayRequest();
                // This call activates a display-required request. If successful,
                // the screen is guaranteed not to turn off automatically due to user inactivity.
                _displayRequest.RequestActive();
            }
        }

        private void ReleaseDisplayRequest()
        {
            // This call de-activates the display-required request. If successful, the screen
            // might be turned off automatically due to a user inactivity, depending on the
            // power policy settings of the system. The requestRelease method throws an exception
            // if it is called before a successful requestActive call on this object.
            if (_displayRequest != null)
            {
                _displayRequest.RequestRelease();
                _displayRequest = null;
            }
        }

        #endregion General Settings Methods

        #region LoadFoldersCommand

        public async Task LoadFolders()
        {
            if (LibraryFoldersCollection.Count <= 0)
            {
                var folderPaths = SettingsHelper.GetLocalSetting<string>("folders", null);
                if (folderPaths != null)
                {
                    foreach (var folder in folderPaths.Split('|'))
                    {
                        if (!string.IsNullOrEmpty(folder))
                        {
                            try
                            {
                                var storageFolder = await StorageFolder.GetFolderFromPathAsync(folder);
                                LibraryFoldersCollection.Add(storageFolder);
                            }
                            catch (FileNotFoundException)
                            { }
                            catch (UnauthorizedAccessException)
                            { }
                        }
                    }
                }
            }
        }

        #endregion LoadFoldersCommand

        #region Add Methods

        /// <summary>
        /// Adds storage files into library.
        /// </summary>
        /// <param name="files">List containing StorageFile(s).</param>
        /// <returns></returns>
        public async static Task AddStorageFilesToLibraryAsync(IEnumerable<StorageFile> files)
        {
            foreach (var file in files)
            {
                Mediafile mp3File = null;
                int index = -1;
                if (file != null)
                {
                    if (TracksCollection.Elements.Any(t => t.Path == file.Path))
                    {
                        index = TracksCollection.Elements.IndexOf(TracksCollection.Elements.First(t => t.Path == file.Path));
                        await SharedLogic.Instance.RemoveMediafile(TracksCollection.Elements.First(t => t.Path == file.Path));
                    }
                    //this methods notifies the Player that one song is loaded. We use both 'count' and 'i' variable here to report current progress.
                    await SharedLogic.Instance.NotificationManager.ShowMessageAsync(" Song(s) Loaded");
                    await Task.Run(async () =>
                    {
                        //here we load into 'mp3file' variable our processed Song. This is a long process, loading all the properties and the album art.
                        mp3File = await TagReaderHelper.CreateMediafile(file, false); //the core of the whole method.
                        await LibraryHelper.SaveSingleFileAlbumArtAsync(mp3File, file).ConfigureAwait(false);
                    });
                    SharedLogic.Instance.AddMediafile(mp3File, index);
                }
            }
        }

        #endregion Add Methods

        #region Load Methods

        private async Task LoadFolderAsync(StorageFolder folder)
        {
            if (folder == null)
                return;
            Messenger.Instance.NotifyColleagues(MessageTypes.MsgUpdateSongCount, (short)2);
            LibraryFoldersCollection.Add(folder);
            await LibraryHelper.ImportFolderIntoLibraryAsync(folder);
        }

        /// <summary>
        /// Auto loads the User's Music Libary on first load.
        /// </summary>
        private async Task AutoLoadMusicLibraryAsync()
        {
            try
            {
                if (KnownFolders.MusicLibrary != null)
                    await LoadFolderAsync(KnownFolders.MusicLibrary);
            }
            catch (Exception ex)
            {
                BLogger.E("Auto Loading of library failed.", ex);
                await SharedLogic.Instance.NotificationManager.ShowMessageAsync(ex.Message);
            }
        }
        #endregion Load Methods

        #endregion Methods

        #region Events

        private async void SettingsViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ReplaceLockscreenWithAlbumArt")
            {
                if (ReplaceLockscreenWithAlbumArt)
                {
                    _replaceLockscreenWithAlbumArt = await LockscreenHelper.SaveCurrentLockscreenImage();
                }
                else
                {
                    await LockscreenHelper.ResetLockscreenImage();
                }
            }
        }

        private async void StorageLibraryService_StorageItemsUpdated(object sender, StorageItemsUpdatedEventArgs e)
        {
            //tell the reader that we accept the changes
            //so that the same files are not served to us again.
            await (sender as StorageLibraryChangeTracker).GetChangeReader().AcceptChangesAsync();

            //check if there are any updated items.
            if (e.UpdatedItems != null && e.UpdatedItems.Any())
            {
                foreach (var item in e.UpdatedItems)
                {
                    //sometimes, the breadplayer log is also included in updated files,
                    //this is to avoid accidently causing a crash.
                    if (Path.GetExtension(item.Path) == ".log")
                    {
                        return;
                    }
                    switch (item.ChangeType)
                    {
                        //file has been created
                        //NOTE: sometimes a altered file also has this ChangeType, so we check for both.
                        case StorageLibraryChangeType.Created:
                            await item.UpdateChangedItem(TracksCollection.Elements, LibraryService);
                            break;
                        //file has been deleted.
                        //NOTE: ChangeType for a file replaced is also this.
                        //TODO: Add logic for a file that is replaced.
                        case StorageLibraryChangeType.Deleted:
                            await item.RemoveItem(TracksCollection.Elements, LibraryService);
                            break;
                        //file was moved or renamed.
                        //NOTE: Logic for moved is the same as renamed (i.e. the path is changed.)
                        case StorageLibraryChangeType.MovedOrRenamed:
                            if (item.IsOfType(StorageItemTypes.File))
                            {
                                if (item.IsItemInLibrary(TracksCollection.Elements, out Mediafile renamedItem))
                                {
                                    renamedItem.Path = item.Path;
                                    if (await LibraryService.UpdateMediafile(renamedItem))
                                    {
                                        await SharedLogic.Instance.NotificationManager.ShowMessageAsync(string.Format("Mediafile Updated. File Path: {0}", renamedItem.Path), 5);
                                    }
                                }
                            }
                            else
                            {
                                await item.RenameFolder(TracksCollection.Elements, LibraryService);
                            }
                            break;
                        //this is almost never invoked but just in case,
                        //we implement RemoveItem logic here.
                        case StorageLibraryChangeType.MovedOutOfLibrary:
                            await item.RemoveItem(TracksCollection.Elements, LibraryService);
                            break;
                        //this is also never invoked but just in case,
                        //we implement AddNewItem logic here.
                        case StorageLibraryChangeType.MovedIntoLibrary:
                            await item.AddNewItem();
                            break;
                        //file's content was changed in some manner. Can be a tag change.
                        case StorageLibraryChangeType.ContentsChanged:
                            await item.UpdateChangedItem(TracksCollection.Elements, LibraryService);
                            break;
                        //TODO: Find a way to invoke this and then implement logic accordingly.
                        case StorageLibraryChangeType.ContentsReplaced:
                            break;
                        //Change was lost. According to the docs, we should Reset the Tracker here.
                        case StorageLibraryChangeType.ChangeTrackingLost:
                            (sender as StorageLibraryChangeTracker).Reset();
                            break;
                    }
                }
            }
        }

        #endregion Events
    }
}