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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using BreadPlayer.Core;
using Windows.Storage.AccessCache;
using BreadPlayer.Models;
using BreadPlayer.PlaylistBus;
using BreadPlayer.Extensions;
using Windows.Storage.Search;
using BreadPlayer.Messengers;
using BreadPlayer.Service;
using BreadPlayer.Common;
using System.Diagnostics;
using Windows.UI.Core;
using BreadPlayer.Dialogs;

namespace BreadPlayer.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        #region Properties
        string uiTextType;
        public string UITextType
        {
            get { return uiTextType; }
            set
            {
                Set(ref uiTextType, value);
                RoamingSettingsHelper.SaveSetting("UITextType", uiTextType);
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
                    _isThemeDark = false;
                return _isThemeDark;
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
            get
            {
                if (_LibraryFoldersCollection == null)
                {
                    _LibraryFoldersCollection = new ThreadSafeObservableCollection<StorageFolder>();
                }
                return _LibraryFoldersCollection;
            }
            set { Set(ref _LibraryFoldersCollection, value); }
        }
        public static GroupedObservableCollection<string, Mediafile> TracksCollection
        { get; set; }
        string timeClosed;
        public string TimeClosed
        {
            get { return timeClosed; }
            set { Set(ref timeClosed, value); }
        }
        string timeOpened;
        public string TimeOpened
        {
            get { return timeOpened; }
            set { Set(ref timeOpened, value); }
        }
        List<StorageFile> modifiedFiles = new List<StorageFile>();
        public List<StorageFile> ModifiedFiles
        {
            get { return modifiedFiles; }
            set { Set(ref modifiedFiles, value); }
        }
        int filebatchsize;
        public int FileBatchSize
        {
            get { return filebatchsize; }
            set
            {
                Set(ref filebatchsize, value);
                RoamingSettingsHelper.SaveSetting("FileBatchSize", FileBatchSize);
            }
        }

        bool changeAccentByAlbumart;
        public bool ChangeAccentByAlbumArt
        {
            get { return changeAccentByAlbumart; }
            set
            {
                Set(ref changeAccentByAlbumart, value);
                if (value == false)
                    Themes.ThemeManager.SetThemeColor(null);
                else
                    Themes.ThemeManager.SetThemeColor("default");
                RoamingSettingsHelper.SaveSetting("ChangeAccentByAlbumArt", changeAccentByAlbumart);
            }
        }

        bool sendReportOnEveryStartup;
        public bool SendReportOnEveryStartup
        {
            get { return sendReportOnEveryStartup; }
            set
            {
                Set(ref sendReportOnEveryStartup, value);
                RoamingSettingsHelper.SaveSetting("SendReportOnEveryStartup", sendReportOnEveryStartup);
            }
        }
        #endregion

        #region MessageHandling
        private async void HandleLibraryLoadedMessage(Message message)
        {
            if (message.Payload is List<object> list)
            {
                TracksCollection = list[0] as GroupedObservableCollection<string, Mediafile>;
                if (new LibraryService(new KeyValueStoreDatabaseService()).SongCount == 0)
                {
                    //await AutoLoadMusicLibraryAsync().ConfigureAwait(false);
                }
                if (TracksCollection != null)
                {
                    message.HandledStatus = MessageHandledStatus.HandledContinue;
                }
            }
        }
        #endregion

        #region Ctor  
        public SettingsViewModel()
        {
            ChangeAccentByAlbumArt = RoamingSettingsHelper.GetSetting<bool>("ChangeAccentByAlbumArt", true);
            FileBatchSize = RoamingSettingsHelper.GetSetting<int>("FileBatchSize", 100);
            TimeOpened = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss");
            SendReportOnEveryStartup = RoamingSettingsHelper.GetSetting<bool>("SendReportOnEveryStartup", true);
            UITextType = RoamingSettingsHelper.GetSetting<string>("UITextType", "Normal");
            Messengers.Messenger.Instance.Register(Messengers.MessageTypes.MSG_LIBRARY_LOADED, new Action<Message>(HandleLibraryLoadedMessage));
        }
        #endregion

        #region Commands

        #region Definitions   
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

        DelegateCommand _resetCommand;
        /// <summary>
        /// Gets load library command. This calls the <see cref="Load"/> method.
        /// </summary>
        public DelegateCommand ResetCommand { get { if (_resetCommand == null) { _resetCommand = new DelegateCommand(Reset); } return _resetCommand; } }

        #endregion

        #region Implementation
        private async void Reset()
        {
            try
            {
                Messenger.Instance.NotifyColleagues(MessageTypes.MSG_DISPOSE);
                LibraryFoldersCollection.Clear();
                await ApplicationData.Current.ClearAsync();
                ResetCommand.IsEnabled = false;
                await Task.Delay(200);
                ResetCommand.IsEnabled = true;
            }
            catch (Exception ex)
            {
                BLogger.Logger.Error("Error occured while resetting the player.", ex);
            }
        }
        private async void ImportPlaylists()
        {
            var picker = new FileOpenPicker();
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
            openPicker.FileTypeFilter.Add(".m3u");
            openPicker.FileTypeFilter.Add(".pls");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                IPlaylist playlist = null;
                if (Path.GetExtension(file.Path) == ".m3u") playlist = new M3U();
                else playlist = new PLS();
                var plist = new Playlist() { Name = file.DisplayName };
                Messenger.Instance.NotifyColleagues(MessageTypes.MSG_ADD_PLAYLIST, plist);
                await playlist.LoadPlaylist(file).ConfigureAwait(false);
            }
        }
        /// <summary>
        /// Loads songs from a specified folder into the library. <seealso cref="LoadCommand"/>
        /// </summary>
        public async void Load()
        {
            try
            {  //LibVM.Database.RemoveFolder(LibraryFoldersCollection[0].Path);
                FolderPicker picker = new FolderPicker();
                picker.FileTypeFilter.Add(".mp3");
                picker.FileTypeFilter.Add(".wav");
                picker.FileTypeFilter.Add(".ogg");
                picker.FileTypeFilter.Add(".flac");
                picker.FileTypeFilter.Add(".m4a");
                picker.FileTypeFilter.Add(".aif");
                picker.FileTypeFilter.Add(".wma");
                picker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
                picker.ViewMode = PickerViewMode.List;
                picker.CommitButtonText = "Import";
                StorageFolder folder = await picker.PickSingleFolderAsync();
                if (folder != null)
                {
                    Messenger.Instance.NotifyColleagues(MessageTypes.MSG_UPDATE_SONG_COUNT, (short)2);
                    LibraryFoldersCollection.Add(folder);
                    StorageApplicationPermissions.FutureAccessList.Add(folder);
                    //Get query options with which we search for files in the specified folder
                    var options = Common.DirectoryWalker.GetQueryOptions();
                    //this is the query result which we recieve after querying in the folder
                    StorageFileQueryResult queryResult = folder.CreateFileQueryWithOptions(options);
                    //the event for files changed
                    queryResult.ContentsChanged += QueryResult_ContentsChanged;
                    await AddFolderToLibraryAsync(queryResult);
                }
            }
            catch(UnauthorizedAccessException)
            {
                await NotificationManager.ShowMessageAsync("You are not authorized to access this folder. Please choose another folder or try again.");
            }
        }
        #endregion

        #endregion

        #region Methods
        
        #region LoadFoldersCommand
        public async Task LoadFolders()
        {
            if (LibraryFoldersCollection.Count <= 0)
            {
                var folderPaths = RoamingSettingsHelper.GetSetting<string>("folders", null);
                if (folderPaths != null)
                {
                    foreach (var folder in folderPaths.Split('|'))
                    {
                        if (!string.IsNullOrEmpty(folder))
                        {
                            var storageFolder = await StorageFolder.GetFolderFromPathAsync(folder);
                            LibraryFoldersCollection.Add(storageFolder);
                        }
                    }
                }
            }
        }
        #endregion
       
        #region Add Methods
        /// <summary>
        /// Adds modified files got from querying in the <see cref="LibraryFoldersCollection"/>. The query parameters include time range from <see cref="TimeClosed"/> to <seealso cref="TimeOpened"/>.
        /// </summary>
        private async Task AddModifiedFilesAsync()
        {
            TimeClosed = RoamingSettingsHelper.GetSetting<string>("timeclosed", "0");
            ModifiedFiles = await Common.DirectoryWalker.GetModifiedFiles(LibraryFoldersCollection, TimeClosed);
            if (ModifiedFiles.Any())
                RenameAddOrDeleteFiles(ModifiedFiles);
        }
        /// <summary>
        /// Adds storage files into library.
        /// </summary>
        /// <param name="files">List containing StorageFile(s).</param>
        /// <returns></returns>
        public async static Task AddStorageFilesToLibraryAsync(IEnumerable<StorageFile> files)
        {
            foreach (var file in files)
            {
                Mediafile mp3file = null;
                int index = -1;
                if (file != null)
                {
                    if (TracksCollection.Elements.Any(t => t.Path == file.Path))
                    {
                        index = TracksCollection.Elements.IndexOf(TracksCollection.Elements.First(t => t.Path == file.Path));
                        RemoveMediafile(TracksCollection.Elements.First(t => t.Path == file.Path));
                    }
                    //this methods notifies the Player that one song is loaded. We use both 'count' and 'i' variable here to report current progress.
                    await NotificationManager.ShowMessageAsync(" Song(s) Loaded");
                    await Task.Run(async () =>
                    {
                        //here we load into 'mp3file' variable our processed Song. This is a long process, loading all the properties and the album art.
                        mp3file = await SharedLogic.CreateMediafile(file, false); //the core of the whole method.
                        await SaveSingleFileAlbumArtAsync(mp3file, file).ConfigureAwait(false);
                    });
                    AddMediafile(mp3file, index);
                }
            }
        }
        #endregion

        #region Load Methods
        /// <summary>
        /// Auto loads the User's Music Libary on first load.
        /// </summary>
        private async Task AutoLoadMusicLibraryAsync()
        {
            try
            {
                var options = Common.DirectoryWalker.GetQueryOptions();
                //this is the query result which we recieve after querying in the folder
                StorageFileQueryResult queryResult = KnownFolders.MusicLibrary.CreateFileQueryWithOptions(options);
                //the event for files changed
                queryResult.ContentsChanged += QueryResult_ContentsChanged;
                if (await queryResult.GetItemCountAsync() > 0)
                {
                    await AddFolderToLibraryAsync(queryResult);
                }
            }
            catch (Exception ex)
            {
                BLogger.Logger.Error("Auto Loading of library failed.", ex);
                await NotificationManager.ShowMessageAsync(ex.Message);
            }
        }
        /// <summary>
        /// Add folder to Library asynchronously.
        /// </summary>
        /// <param name="queryResult">The query result after querying in a specific folder.</param>
        /// <returns></returns>
        public async Task AddFolderToLibraryAsync(StorageFileQueryResult queryResult)
        {
            if (queryResult != null)
            {
                //so that no new event is raised. We want to focus on loading.
                isLibraryLoading = true;

                //this is a temporary list to collect all the processed Mediafiles. We use List because it is fast. Faster than using ObservableCollection directly because of the events firing on every add.
                var tempList = new List<Mediafile>();
               
                //'count' is for total files got after querying.
                var count = await queryResult.GetItemCountAsync().AsTask().ConfigureAwait(false);
                if (count == 0)
                {
                    string error = "No songs found!";
                    BLogger.Logger.Error("No songs were found!");
                    await NotificationManager.ShowMessageAsync(error);
                    return;
                }
                LibraryService service = new LibraryService(new KeyValueStoreDatabaseService());
                int failedCount = 0;
                //'i' is a variable for the index of currently processing file
                short i = 0;

                Stopwatch watch = Stopwatch.StartNew();
                try
                {
                    foreach (StorageFile file in await queryResult.GetFilesAsync())
                    {
                        try
                        {
                            //A null Mediafile which we will use afterwards.
                            Mediafile mp3file = null;
                            i++; //Notice here that we are increasing the 'i' variable by one for each file.
                                 //we send a message to anyone listening relaying that song count has to be updated.
                            Messenger.Instance.NotifyColleagues(MessageTypes.MSG_UPDATE_SONG_COUNT, i);
                            //here we load into 'mp3file' variable our processed Song. This is a long process, loading all the properties and the album art.
                            mp3file = await CreateMediafile(file, false); //the core of the whole method.
                            mp3file.FolderPath = Path.GetDirectoryName(file.Path);
                            await SaveSingleFileAlbumArtAsync(mp3file, file).ConfigureAwait(false);

                            //this methods notifies the Player that one song is loaded. We use both 'count' and 'i' variable here to report current progress.
                            await NotificationManager.ShowMessageAsync(i.ToString() + "\\" + count.ToString() + " Song(s) Loaded", 0);
                            //we then add the processed song into 'tempList' very silently without anyone noticing and hence, efficiently.
                            tempList.Add(mp3file);
                            mp3file = null;
                        }
                        catch (Exception ex)
                        {
                            // BLogger.Logger.Error("Loading of a song in folder failed.", ex);
                            //we catch and report any exception without distrubing the 'foreach flow'.
                            await NotificationManager.ShowMessageAsync(ex.Message + " || Occured on: " + file.Path);
                            failedCount++;
                        }
                    }

                    // BLogger.Logger.Info(string.Format("{0} out of {1} songs loaded. {2} is iteration count.", tempList.Count, count, i));
                }
                catch (Exception ex)
                {
                    //BLogger.Logger.Error("Failed to import songs in library.", ex);
                    string message1 = ex.Message + "||" + ex.InnerException;
                    await NotificationManager.ShowMessageAsync(message1);
                }

                //now we add 100 songs directly into our TracksCollection which is an ObservableCollection. This is faster because only one event is invoked.
                //tempList.Sort();
                TracksCollection.AddRange(tempList);
                //now we load 100 songs into database.
                await service.AddMediafiles(tempList);
                service.Dispose();

                watch.Stop();
                var secs = watch.Elapsed.TotalSeconds;

                AlbumArtistViewModel vm = new AlbumArtistViewModel();
                Messenger.Instance.NotifyColleagues(MessageTypes.MSG_UPDATE_SONG_COUNT, "Done!");
                Messenger.Instance.NotifyColleagues(MessageTypes.MSG_ADD_ALBUMS, tempList);
                vm = null;
                //we send the message to load the album. This comes first so there is enough time to load all albums before new list come up.
                isLibraryLoading = false;
                string message = string.Format("Songs successfully imported! Total Songs: {0}; Failed: {1}; Loaded: {2}", count, failedCount, i);

                BLogger.Logger.Info(message);
                await NotificationManager.ShowMessageAsync(message);   
                await DeleteDuplicates(TracksCollection.Elements).ConfigureAwait(false);               
                tempList.Clear();
            }
        }
        async Task DeleteDuplicates(IEnumerable<Mediafile> source)
        {
            var duplicateFiles = source.Where(s => source.Count(x => x.OrginalFilename == s.OrginalFilename) > 1);
            if (duplicateFiles.Count() > 0)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    await ShowMessageBox((selectedDuplicates) =>
                    {
                        if (selectedDuplicates.Any())
                        {
                            foreach (var duplicate in selectedDuplicates)
                            {
                                var duplicateIndex = 0;
                                var duplicateCount = TracksCollection.Elements.Count(t => t.OrginalFilename == duplicate.OrginalFilename);
                                if (duplicateCount > 2)
                                {
                                    for (int i = 0; i < duplicateCount - 1; i++)
                                    {
                                        duplicateIndex = TracksCollection.Elements.IndexOf(TracksCollection.Elements.FirstOrDefault(t => t.OrginalFilename == duplicate.OrginalFilename));
                                        if (duplicateIndex > -1)
                                            RemoveMediafile(TracksCollection.Elements.ElementAt(duplicateIndex));
                                    }
                                }
                                else
                                    RemoveMediafile(duplicate);
                            }
                        }
                    }, duplicateFiles);
                });
            }
        }
        async Task ShowMessageBox(Action<IEnumerable<Mediafile>> action, IEnumerable<Mediafile> Duplicates)
        {
            try
            {
                DuplicatesDialog dialog = new DuplicatesDialog();
                dialog.Duplicates = Duplicates.DistinctBy(t => t.OrginalFilename);
                dialog.Title = string.Format("Please choose the duplicates you would like to delete. Total duplicates: {0} duplicate songs?", Duplicates.Count());
                if (CoreWindow.GetForCurrentThread().Bounds.Width <= 501)
                    dialog.DialogWidth = CoreWindow.GetForCurrentThread().Bounds.Width - 50;
                else
                    dialog.DialogWidth = CoreWindow.GetForCurrentThread().Bounds.Width - 100;

                var result = await dialog.ShowAsync();
                if (result == Windows.UI.Xaml.Controls.ContentDialogResult.Primary)
                {
                    action.Invoke(dialog.SelectedDuplicates);
                }
            }
            catch (UnauthorizedAccessException) { }
        }
        #endregion

        #region AlbumArt Methods
        public static async Task SaveSingleFileAlbumArtAsync(Mediafile mp3file, StorageFile file = null)
        {
            if (mp3file != null)
            {
                try
                {
                    if (file == null)
                        file = await StorageFile.GetFileFromPathAsync(mp3file.Path);

                    var albumartFolder = ApplicationData.Current.LocalFolder;
                    var albumartLocation = albumartFolder.Path + @"\AlbumArts\" + (mp3file.Album + mp3file.LeadArtist).ToLower().ToSha1() + ".jpg";

                    if (!VerifyFileExists(albumartLocation, 300))
                    {
                        bool albumSaved = await SaveImagesAsync(file, mp3file);                       
                        mp3file.AttachedPicture = albumSaved ? albumartLocation : null;
                    }
                    file = null;
                }
                catch (Exception ex)
                {
                    BLogger.Logger.Info("Failed to save albumart.", ex);
                    await NotificationManager.ShowMessageAsync("Failed to save album art of " + mp3file.OrginalFilename);
                }
            }
        }

        private static async Task SaveMultipleAlbumArtsAsync(IEnumerable<Mediafile> files)
        {
            foreach(var file in files)
            {
                await SaveSingleFileAlbumArtAsync(file);
            }
        }
        #endregion

        #region Folder Watecher Methods
        /// <summary>
        /// This is still experimental. As you can see, there are more lines of code then necessary.
        /// </summary>
        /// <param name="files"></param>
        public static async void RenameAddOrDeleteFiles(IEnumerable<StorageFile> files)
        {
            try
            {
                string folder = "";
                foreach (var file in files)
                {
                    var props = await file.Properties.GetMusicPropertiesAsync();
                    folder = Path.GetDirectoryName(file.Path);
                    //FOR RENAMING (we check all the tracks to see if we have a track that is the same as the changed file except for its path)
                    if (TracksCollection.Elements.ToArray().Any(t => t.Path != file.Path && t.Title == props.Title && t.LeadArtist == props.Artist && t.Album == props.Album && t.Length == props.Duration.ToString(@"mm\:ss")))
                    {
                        var mp3File = TracksCollection.Elements.FirstOrDefault(t => t.Path != file.Path && t.Title == props.Title && t.LeadArtist == props.Artist && t.Album == props.Album && t.Length == props.Duration.ToString(@"mm\:ss"));
                        mp3File.Path = file.Path;
                    }
                    //FOR ADDITION (we check all the files to see if we already have the file or not)
                    if (TracksCollection.Elements.ToArray().All(t => t.Path != file.Path))
                    {
                        var mediafile = await CreateMediafile(file, false);
                        AddMediafile(mediafile);
                        await SaveSingleFileAlbumArtAsync(mediafile);
                    }
                }
                //FOR DELETE (we only want to iterate through songs which are in the changed folder)
                var deletedFiles = new List<Mediafile>();
                foreach (var file in TracksCollection.Elements.ToArray().Where(t => t.FolderPath == folder))
                {
                    if (!File.Exists(file.Path))
                    {
                        deletedFiles.Add(TracksCollection.Elements.FirstOrDefault(t => t.Path == file.Path));
                    }
                }
                if (deletedFiles.Any())
                    foreach (var deletedFile in deletedFiles)
                        TracksCollection.Elements.Remove(deletedFile);
            }
            catch (Exception ex)
            {
                BLogger.Logger.Error("Some error occured while renaming, deleting or editting the modified files.", ex);
                await NotificationManager.ShowMessageAsync(ex.Message);
            }
        }

        public async static Task PerformWatcherWorkAsync(StorageFolder folder)
        {
            StorageFileQueryResult modifiedqueryResult = folder.CreateFileQueryWithOptions(Common.DirectoryWalker.GetQueryOptions("datemodified:>" + SettingsVM.TimeOpened));
            var files = await modifiedqueryResult.GetFilesAsync();
            if (await modifiedqueryResult.GetItemCountAsync() > 0)
            {
                await AddStorageFilesToLibraryAsync(await modifiedqueryResult.GetFilesAsync());
            }
            //since there were no modifed files returned yet the event was raised, this means that some file was renamed or deleted. To acknowledge that change we need to reload everything in the modified folder
            else
            {
                //this is the query result which we recieve after querying in the folder
                StorageFileQueryResult queryResult = folder.CreateFileQueryWithOptions(Common.DirectoryWalker.GetQueryOptions());
                files = await queryResult.GetFilesAsync();
                RenameAddOrDeleteFiles(files);
            }
        }



        #endregion

        #endregion

        #region Events
        bool isLibraryLoading;
        private async void QueryResult_ContentsChanged(IStorageQueryResultBase sender, object args)
        {
            if (!isLibraryLoading)
            {
                await PerformWatcherWorkAsync(sender.Folder).ConfigureAwait(false);
            }
        }
        #endregion

    }
}
