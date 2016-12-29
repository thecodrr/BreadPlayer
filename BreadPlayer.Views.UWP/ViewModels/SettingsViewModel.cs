﻿/* 
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
using Windows.Storage.FileProperties;
using BreadPlayer.Extensions;
using Windows.Storage.Search;
using BreadPlayer.Messengers;
using BreadPlayer.Service;
using BreadPlayer.Common;
using System.Diagnostics;

namespace BreadPlayer.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        #region Properties
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

        int playbarLocation = 0;
        public int PlaybarLocation
        {
            get
            {
                return isPlaybarOnBottom ? 1 : 0;
            }
            set
            {
                Set(ref playbarLocation, value);
                IsPlaybarOnBottom = playbarLocation == 0 ? false : true;
                RoamingSettingsHelper.SaveSetting("IsPlaybarOnBottom", IsPlaybarOnBottom);
            }
        }
        bool isPlaybarOnBottom;
        public bool IsPlaybarOnBottom
        {
            get { return isPlaybarOnBottom; }
            set { Set(ref isPlaybarOnBottom, value); }
        }
        bool changeAccentByAlbumart;
        public bool ChangeAccentByAlbumArt
        {
            get { return changeAccentByAlbumart; }
            set { Set(ref changeAccentByAlbumart, value); }
        }
        #endregion

        #region MessageHandling
        private async void HandleLibraryLoadedMessage(Message message)
        {
            if (message.Payload is List<object>)
            {
                TracksCollection = (message.Payload as List<object>)[0] as GroupedObservableCollection<string, Mediafile>;
                if (new LibraryService(new DatabaseService()).SongCount == 0)
                {
                    await AutoLoadMusicLibraryAsync().ConfigureAwait(false);
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
            IsPlaybarOnBottom = RoamingSettingsHelper.GetSetting<bool>("IsPlaybarOnBottom", false);
            ChangeAccentByAlbumArt = RoamingSettingsHelper.GetSetting<bool>("ChangeAccentByAlbumArt", true);
            FileBatchSize = RoamingSettingsHelper.GetSetting<int>("FileBatchSize", 100);
            TimeOpened = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss");
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
            Messenger.Instance.NotifyColleagues(MessageTypes.MSG_DISPOSE);
            LibraryFoldersCollection.Clear();
            await ApplicationData.Current.ClearAsync();
            ResetCommand.IsEnabled = false;
            await Task.Delay(200);
            ResetCommand.IsEnabled = true;
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
            //LibVM.Database.RemoveFolder(LibraryFoldersCollection[0].Path);
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
        #endregion

        #endregion

        #region Methods

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
                    await NotificationManager.ShowAsync(" Song(s) Loaded", "Loading...");
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
        /// <summary>
        /// Add folder to Library asynchronously.
        /// </summary>
        /// <param name="queryResult">The query result after querying in a specific folder.</param>
        /// <returns></returns>
        public static async Task AddFolderToLibraryAsync(StorageFileQueryResult queryResult)
        {
            if (queryResult != null)
            {
                var stop = Stopwatch.StartNew();
                //we create two uints. 'index' for the index of current block/batch of files and 'stepSize' for the size of the block. This optimizes the loading operation tremendously.
                uint index = 0, stepSize = 200;
                //a list containing the files we recieved after querying using the two uints we created above.
                IReadOnlyList<StorageFile> files = await queryResult.GetFilesAsync(index, stepSize);
                //we move forward the index 100 steps because first 100 files are loaded when we called the above method.
                index += 200;

                //this is a temporary list to collect all the processed Mediafiles. We use List because it is fast. Faster than using ObservableCollection directly because of the events firing on every add.
                var tempList = new List<Mediafile>();

                //'count' is for total files got after querying.
                var count = await queryResult.GetItemCountAsync();
                if (count == 0)
                {
                    string error = "No songs found!";
                    await NotificationManager.ShowAsync(error);
                    return;
                }

                AlbumArtistViewModel model = new AlbumArtistViewModel();
                LibraryService service = new LibraryService(new DatabaseService());
                int failedCount = 0;
                //'i' is a variable for the index of currently processing file
                short i = 0;
                //using while loop until number of files become 0. This is to confirm that we process all files without leaving anything out.
                while (files.Count != 0)
                {
                    try
                    {
                        foreach (StorageFile file in files)
                        {
                            try
                            {
                                //we use 'if' conditional so that we don't add any duplicates
                                if (TracksCollection.Elements.All(t => t.Path != file.Path))
                                {
                                    //A null Mediafile which we will use afterwards.
                                    Mediafile mp3file = null;
                                    i++; //Notice here that we are increasing the 'i' variable by one for each file.
                                    //we send a message to anyone listening relaying that song count has to be updated.
                                    Messenger.Instance.NotifyColleagues(MessageTypes.MSG_UPDATE_SONG_COUNT, i);
                                    await Task.Run(async () =>
                                    {
                                        //here we load into 'mp3file' variable our processed Song. This is a long process, loading all the properties and the album art.
                                        mp3file = await CreateMediafile(file, false); //the core of the whole method.
                                        mp3file.FolderPath = Path.GetDirectoryName(file.Path);
                                        await SaveSingleFileAlbumArtAsync(mp3file).ConfigureAwait(false);
                                    });
                                    //this methods notifies the Player that one song is loaded. We use both 'count' and 'i' variable here to report current progress.
                                    await NotificationManager.ShowAsync(i.ToString() + "\\" + count.ToString() + " Song(s) Loaded", "Loading...");

                                    //we then add the processed song into 'tempList' very silently without anyone noticing and hence, efficiently.
                                    tempList.Add(mp3file);
                                }
                            }
                            catch (Exception ex)
                            {
                                //we catch and report any exception without distrubing the 'foreach flow'.
                                await NotificationManager.ShowAsync(ex.Message + " || Occured on: " + file.Path);
                                failedCount++;
                            }
                        }
                        //await SaveMultipleAlbumArtsAsync(tempList).ConfigureAwait(false);
                        //we send the message to load the album. This comes first so there is enough time to load all albums before new list come up.
                        Messenger.Instance.NotifyColleagues(MessageTypes.MSG_ADD_ALBUMS, tempList);
                        //now we add 100 songs directly into our TracksCollection which is an ObservableCollection. This is faster because only one event is invoked.
                        TracksCollection.AddRange(tempList);
                        //now we load 100 songs into database.
                        service.AddMediafiles(tempList);
                        //we clear the 'tempList' so it can come with only 100 songs again.
                        tempList.Clear();
                        //Since the no. of files in 'files' list is 100, only 100 files will be loaded after which we will step out of while loop.
                        //To avoid this, we create and run a task that loads the next 100 files. Stepping forward 100 steps without increasing the index.
                        files = await queryResult.GetFilesAsync(index, stepSize).AsTask().ConfigureAwait(false);
                        //consequently we have to increase the index by 100 so that songs are not repeated.
                        index += 200;
                    }
                    catch (Exception ex)
                    {
                        string message1 = ex.Message + "||" + ex.InnerException;
                        await NotificationManager.ShowAsync(message1);
                    }
                }
                stop.Stop();
                string message = string.Format("Library successfully loaded! Total Songs: {0}; Failed: {1}; Loaded: {2}; Time Taken: {3}", count, failedCount, i, stop.Elapsed.TotalSeconds);
                await NotificationManager.ShowAsync(message);
                service.Dispose();
                model = null;
            }
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
                        bool albumSaved = false;
                        StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView, 300, ThumbnailOptions.UseCurrentScale).AsTask().ConfigureAwait(false);
                        switch (thumbnail.Type)
                        {
                            case ThumbnailType.Image:
                                albumSaved = await SaveImagesAsync(thumbnail, mp3file).ConfigureAwait(false);
                                break;
                            case ThumbnailType.Icon:
                            default:
                                albumSaved = await SaveImagesAsync(file, mp3file).ConfigureAwait(false);
                                break;
                        }
                        mp3file.AttachedPicture = albumSaved ? albumartLocation : null;
                    }
                }
                catch
                {
                    await NotificationManager.ShowAsync("Failed to save album art of " + mp3file.OrginalFilename);
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
                await NotificationManager.ShowAsync(ex.Message);
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
        private async void QueryResult_ContentsChanged(IStorageQueryResultBase sender, object args)
        {
            await PerformWatcherWorkAsync(sender.Folder).ConfigureAwait(false);
        }
        #endregion
        
    }
}
