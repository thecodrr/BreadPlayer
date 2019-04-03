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
using BreadPlayer.Core.Events;
using BreadPlayer.Core.Models;
using BreadPlayer.Core.PortableAPIs;
using BreadPlayer.Database;
using BreadPlayer.Dispatcher;
using BreadPlayer.Extensions;
using BreadPlayer.Helpers;
using BreadPlayer.Messengers;
using BreadPlayer.MomentoPattern;
using BreadPlayer.Services;
using BreadPlayer.Themes;
using Microsoft.Advertising.WinRT.UI;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.Media.Devices;
using Windows.Phone.Media.Devices;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Microsoft.Services.Store.Engagement;
using Windows.Phone.UI.Input;
using System.IO;
using BreadPlayer.Controls;
using BreadPlayer.Interfaces;

namespace BreadPlayer.ViewModels
{
    public class ShellViewModel : ObservableObject
    {
        #region Fields

        private SymbolIcon _playPauseIcon = new SymbolIcon(Symbol.Play);
        private SymbolIcon _repeatIcon = new SymbolIcon(Symbol.Sync);
        private Mediafile _songToStopAfter;
        private DispatcherTimer _timer;
        private LibraryService _service = new LibraryService(new KeyValueStoreDatabaseService(SharedLogic.Instance.DatabasePath, "Tracks"));
        private int _songCount;
        private string _audioDeviceId = MediaDevice.GetDefaultAudioRenderId(AudioDeviceRole.Default);
        private int _indexOfCurrentlyPlayingFile = -1;
        #endregion Fields

        #region Constructor

        public ShellViewModel()
        {
            NavigateToNowPlayingViewCommand = new RelayCommand(NavigateToNowPlayingView);
            WatchAnAdCommand = new DelegateCommand(WatchAnAd);
            IncreaseVolumeCommand = new DelegateCommand(IncreaseVolume);
            DecreaseVolumeCommand = new DelegateCommand(DecreaseVolume);
            SeekForwardCommand = new DelegateCommand(SeekForward);
            SeekBackwardCommand = new DelegateCommand(SeekBackward);
            MuteCommand = new DelegateCommand(Mute);
            Messenger.Instance.Register(MessageTypes.MsgPlaylistLoaded, new Action<Message>(HandleLibraryLoadedMessage));
            Messenger.Instance.Register(MessageTypes.MsgLibraryLoaded, new Action<Message>(HandleLibraryLoadedMessage));
            Messenger.Instance.Register(MessageTypes.MsgPlaySong, new Action<Message>(HandlePlaySongMessage));
            Messenger.Instance.Register(MessageTypes.MsgDispose, HandleDisposeMessage);
            Messenger.Instance.Register(MessageTypes.MsgExecuteCmd, new Action<Message>(HandleExecuteCmdMessage));
            Messenger.Instance.Register(MessageTypes.MsgUpdateSongCount, new Action<Message>(HandleEnablePlayMessage));
            Messenger.Instance.Register(MessageTypes.MsgStopAfterSong, new Action<Message>(HandleSaveSongToStopAfterMessage));
            PlayPauseIcon = new SymbolIcon(Symbol.Play);
            //PlaylistsItems = new ObservableCollection<SimpleNavMenuItem>();
            SharedLogic.Instance.Player.PlayerState = PlayerState.Stopped;
            DontUpdatePosition = false;
            _timer = new DispatcherTimer(new BreadDispatcher())
            {
                Interval = TimeSpan.FromMilliseconds(1)
            };
            _timer.Tick += Timer_Tick;
            _timer.Stop();
            SharedLogic.Instance.Player.MediaEnded += Player_MediaEnded;
            PropertyChanged += ShellViewModel_PropertyChanged;
            SharedLogic.Instance.Player.MediaAboutToEnd += Player_MediaAboutToEnd;
            SharedLogic.Instance.Player.MediaChanging += OnMediaChanging;

            //these events are for detecting when the default audio
            //device is changed in PC and Mobile.
            if (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1))
                AudioRoutingManager.GetDefault().AudioEndpointChanged += OnAudioEndpointChanged;
            else
                MediaDevice.DefaultAudioRenderDeviceChanged += OnDefaultAudioRenderDeviceChanged;
        }

        private async void OnMediaChanging(object sender, EventArgs e)
        {
            var lastPlayingFile = SharedLogic.Instance.Player.CurrentlyPlayingFile;
            if (IsPlayingFromNetwork && lastPlayingFile != null
                && await Task.Run(() => File.Exists(lastPlayingFile.Path)))
            {
                await (await StorageFile.GetFileFromPathAsync(lastPlayingFile.Path)).TryDeleteItemAsync();
            }
        }

        #endregion Constructor

        #region HandleMessages

        private void HandleEnablePlayMessage(Message message)
        {
            if (message.Payload is short count && count > 0)
            {
                message.HandledStatus = MessageHandledStatus.HandledContinue;
                PlayPauseCommand.IsEnabled = true;
            }
        }
        private Task<IEnumerable<Mediafile>> ToMediafileCollection(IEnumerable<DiskItem> folderFiles, MediaLocationType mediaLocationType)
        {
            return Task.Run<IEnumerable<Mediafile>>(async () =>
            {
                List<Mediafile> mediaFiles = new List<Mediafile>();
                foreach (var file in folderFiles)
                {
                    if (file.IsFile)
                    {
                        mediaFiles.Add(new Mediafile
                        {
                            Title = file.Title,
                            LeadArtist = file.Artist,
                            Path = file.Path,
                            MediaLocation = mediaLocationType,
                            Album = file.Album,
                            Size = file.Size
                        });
                    }
                }
                return mediaFiles;
            });
        }
        private async void HandleLibraryLoadedMessage(Message message)
        {
            message.HandledStatus = MessageHandledStatus.HandledContinue;
            if (message.Payload is ThreadSafeObservableCollection<Mediafile> tMediaFile)
            {
                PlaylistSongCollection = tMediaFile;
            }
            else if (message.Payload is object[] array)
            {
                NowPlayingQueue = new ThreadSafeObservableCollection<Mediafile>(await ToMediafileCollection((IEnumerable<DiskItem>)array[0], (MediaLocationType)array[1]).ConfigureAwait(false));
            }
            else
            {
                var listObject = message.Payload as List<object>;
                TracksCollection = listObject[0] as GroupedObservableCollection<IGroupKey, Mediafile>;
                TracksCollection.CollectionChanged += TracksCollection_CollectionChanged;
                IsSourceGrouped = (bool)listObject[1];
                _songCount = _service.SongCount;
                GetSettings();
            }
        }

        private async void TracksCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (TracksCollection.Elements.Count > 0)
            {
                PlayPauseCommand.IsEnabled = true;
            }
            if (TracksCollection.Elements.Count == _songCount)
            {
                SetNowPlayingSong();

                if (Shuffle)
                    await ReshuffleCollection();
                else
                {
                    UpcomingSong = await GetUpcomingSong();
                    PreviousSong = GetPreviousSong();
                }
            }
            TracksCollection.CollectionChanged -= TracksCollection_CollectionChanged;
        }

        private async void HandlePlaySongMessage(Message message)
        {
            if (message.Payload is List<object> obj)
            {
                message.HandledStatus = MessageHandledStatus.HandledCompleted;
                var file = obj[0] as Mediafile;
                var play = (bool)obj[1];
                IsPlayingFromPlaylist = (bool)obj[2];
                if (obj.Count > 3)
                {
                    IsPlayingFromNetwork = (bool)obj[3];
                }
                await Load(file, play);
            }
            else if (message.Payload is string path)
            {
                message.HandledStatus = MessageHandledStatus.HandledCompleted;
                var mediaFile = await TagReaderHelper.CreateMediafile(await StorageFile.GetFileFromPathAsync(path));
                await Load(mediaFile, true);
            }
            else if (message.Payload is StorageFile file)
            {
                message.HandledStatus = MessageHandledStatus.HandledCompleted;
                await Load(await FromNetworkStorageFile(file).ConfigureAwait(false), true);
            }
        }
        private async Task<Mediafile> FromNetworkStorageFile(StorageFile file)
        {
            var mediaFile = await TagReaderHelper.CreateMediafile(file);
            mediaFile.MediaLocation = MediaLocationType.Device;
            mediaFile.ByteArray = await (await file.OpenStreamForReadAsync()).ToByteArray();
            return mediaFile;
        }
        private async void HandleExecuteCmdMessage(Message message)
        {
            if (message.Payload == null) return;

            if (message.Payload is List<object> list)
            {
                double volume = 0;
                if ((double)list[3] == 50.0)
                {
                    volume = SettingsHelper.GetLocalSetting<double>("volume", 50.0);
                }
                else
                {
                    volume = (double)list[3];
                }
                Mediafile libraryMediaFile = null;
                if (list[0] is IReadOnlyList<IStorageItem> files)
                {
                    List<Mediafile> mediafileList = new List<Mediafile>(files.Count);
                    foreach (IStorageItem item in files)
                    {
                        if (item.IsOfType(StorageItemTypes.File))
                        {
                            mediafileList.Add(await TagReaderHelper.CreateMediafile(item as StorageFile));
                        }
                    }
                    NowPlayingQueue = new ThreadSafeObservableCollection<Mediafile>();
                    NowPlayingQueue.AddRange(mediafileList);
                    libraryMediaFile = NowPlayingQueue[0];
                }
                else
                {
                    var id = SettingsHelper.GetLocalSetting<long>("NowPlayingID", 0L);
                    libraryMediaFile = _service.GetMediafile(id);
                    if (libraryMediaFile == null)
                    {
                        var path = SettingsHelper.GetLocalSetting<string>("path", null);
                        if (path != null)
                        {
                            if (await Task.Run(() => File.Exists(path)))
                                libraryMediaFile = await TagReaderHelper.CreateMediafile(await StorageFile.GetFileFromPathAsync(path));
                        }
                    }
                }

                await Load(libraryMediaFile, (bool)list[2], (double)list[1], volume);
            }
            else
            {
                GetType().GetTypeInfo().GetDeclaredMethod(message.Payload as string)?.Invoke(this, new object[] { });
            }

            message.HandledStatus = MessageHandledStatus.HandledCompleted;
        }

        private async void HandleDisposeMessage()
        {
            Reset();
            await SharedLogic.Instance.Player.Stop();
        }

        private void HandleSaveSongToStopAfterMessage(Message songToStopAfter)
        {
            if (songToStopAfter.Payload is Mediafile mediaFile)
            {
                _songToStopAfter = mediaFile;
            }
        }

        #endregion HandleMessages

        #region Commands

        #region Definition

        private RelayCommand _openSongCommand;
        private DelegateCommand _playPreviousCommand;
        private DelegateCommand _playNextCommand;
        private DelegateCommand _playPauseCommand;
        private DelegateCommand _setRepeatCommand;
        private DelegateCommand _showEqualizerCommand;
        private DelegateCommand _shuffleAllCommand;

        public DelegateCommand ShuffleAllCommand { get { if (_shuffleAllCommand == null) { _shuffleAllCommand = new DelegateCommand(ShuffleAll); } return _shuffleAllCommand; } }

        public ICommand MuteCommand { get; set; }
        public ICommand IncreaseVolumeCommand { get; set; }
        public ICommand DecreaseVolumeCommand { get; set; }
        public ICommand SeekForwardCommand { get; set; }
        public ICommand SeekBackwardCommand { get; set; }

        /// <summary>
        /// Gets OpenSong command. This calls the <see cref="Open(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand OpenSongCommand
        {
            get
            { if (_openSongCommand == null) { _openSongCommand = new RelayCommand(param => Open(param)); } return _openSongCommand; }
        }

        public DelegateCommand PlayPauseCommand
        {
            get
            {
                if (_playPauseCommand == null)
                {
                    _playPauseCommand = new DelegateCommand(PlayPause)
                    {
                        IsEnabled = false
                    };
                }
                return _playPauseCommand;
            }
        }

        public DelegateCommand PlayNextCommand { get { if (_playNextCommand == null) { _playNextCommand = new DelegateCommand(PlayNext); } return _playNextCommand; } }
        public DelegateCommand PlayPreviousCommand { get { if (_playPreviousCommand == null) { _playPreviousCommand = new DelegateCommand(PlayPrevious); } return _playPreviousCommand; } }
        public DelegateCommand SetRepeatCommand { get { if (_setRepeatCommand == null) { _setRepeatCommand = new DelegateCommand(SetRepeat); } return _setRepeatCommand; } }
        public DelegateCommand ShowEqualizerCommand { get { if (_showEqualizerCommand == null) { _showEqualizerCommand = new DelegateCommand(ShowEqualizer); } return _showEqualizerCommand; } }
        public ICommand NavigateToNowPlayingViewCommand { get; set; }// { if (navigateToNowPlayingViewCommand == null) navigateToNowPlayingViewCommand = new DelegateCommand(NavigateToNowPlayingView); return navigateToNowPlayingViewCommand; } }
        public DelegateCommand WatchAnAdCommand { get; set; }// { if (navigateToNowPlayingViewCommand == null) navigateToNowPlayingViewCommand = new DelegateCommand(NavigateToNowPlayingView); return navigateToNowPlayingViewCommand; } }
        #endregion Definition

        #region Implementation
        private async void ShuffleAll()
        {
            if (TracksCollection.Count <= 0)
                return;
            Shuffle = true;
            await Load((await ShuffledCollection().ConfigureAwait(false))[0], true).ConfigureAwait(false);
        }
        private async void WatchAnAd()
        {
            if (!InternetConnectivityHelper.IsInternetConnected)
            {
                await NotifyAndDeselect("Seems you don't have an internet connection. Thanks for the support, though!");
                return;
            }
            StoreServicesCustomEventLogger logger = StoreServicesCustomEventLogger.GetDefault();

            InterstitialAd ad = new InterstitialAd();
            string myAppId = "9nblggh42srx";
            string myAdUnitId = "11701839";
            ad.Keywords = "music,software,apps";
            ad.AdReady += async (r, a) =>
            {
                if (InterstitialAdState.Ready == ad.State)
                {
                    logger.Log("WatchAnAdStarted");
                    ad.Show();
                    await SharedLogic.Instance.NotificationManager.ShowMessageBoxAsync("Please click at least one link to help me more! Thanks!", "Thank you so much!");
                }
            };
            ad.Completed += async (r, a) =>
            {
                logger.Log("WatchAnAdCompleted");
                await NotifyAndDeselect("Thanks!");
            };
            ad.Cancelled += async (r, a) =>
            {
                await NotifyAndDeselect("No worries!");
            };
            int retryCount = 0;
            ad.ErrorOccurred += async (r, a) =>
            {
                if (retryCount < 2)
                {
                    retryCount++;
                    await NotifyAndDeselect("Aw! An error occured. Trying something else. Please wait.");
                    myAdUnitId = "1100000276";
                    ad.RequestAd(AdType.Display, myAppId, myAdUnitId);
                }
                else
                {
                    await NotifyAndDeselect("Aw! An error occured. Thanks anyway.");
                }
            };
            ad.RequestAd(AdType.Video, myAppId, myAdUnitId);
            await NotifyAndDeselect("Please continue. The ad will be shown shortly.");

            async Task NotifyAndDeselect(string message)
            {
                await SharedLogic.Instance.NotificationManager.ShowMessageAsync(message);
                SplitViewMenu.SelectPrevious();
            }
        }
        private void Mute()
        {
            SharedLogic.Instance.Player.IsVolumeMuted = SharedLogic.Instance.Player.IsVolumeMuted ? false : true;
        }

        private void IncreaseVolume()
        {
            if (SharedLogic.Instance.Player.Volume < 100)
            {
                SharedLogic.Instance.Player.Volume++;
            }
        }

        private void DecreaseVolume()
        {
            if (SharedLogic.Instance.Player.Volume > 0)
            {
                SharedLogic.Instance.Player.Volume--;
            }
        }

        private void SeekForward()
        {
            DontUpdatePosition = true;
            CurrentPosition += 2;
            DontUpdatePosition = false;
        }

        private void SeekBackward()
        {
            DontUpdatePosition = true;
            CurrentPosition -= 2;
            DontUpdatePosition = false;
        }

        private void NavigateToNowPlayingView(object para)
        {
            NavigationService.Instance.UnregisterEvents();
            Messenger.Instance.NotifyColleagues(MessageTypes.MsgNavigate, new { pageType = typeof(NowPlayingView), parameter = "NowPlayingView" });
        }


        private void ShowEqualizer()
        {
            IsEqualizerVisible = IsEqualizerVisible ? false : true;
        }

        private void SetRepeat()
        {
            switch (Repeat)
            {
                case "No Repeat":
                    Repeat = "Repeat Song";
                    SharedLogic.Instance.Player.IsLoopingEnabled = true;
                    break;

                case "Repeat Song":
                    Repeat = "Repeat List";
                    SharedLogic.Instance.Player.IsLoopingEnabled = false;
                    break;

                case "Repeat List":
                    Repeat = "No Repeat";
                    SharedLogic.Instance.Player.IsLoopingEnabled = false;
                    break;

                default:
                    break;
            }
        }

        private async void PlayPause()
        {
            try
            {

                if (SharedLogic.Instance.Player.CurrentlyPlayingFile == null && TracksCollection?.Elements?.Count > 0)
                {
                    await Load(TracksCollection.Elements.First(), true);
                }
                else
                {
                    await BreadDispatcher.InvokeAsync(async () =>
                    {
                        switch (SharedLogic.Instance.Player.PlayerState)
                        {
                            case PlayerState.Playing:
                                await SharedLogic.Instance.Player.Pause();
                                _timer.Stop();
                                SharedLogic.Instance.Player.PlayerState = PlayerState.Stopped;
                                PlayPauseIcon = new SymbolIcon(Symbol.Play);
                                break;

                            case PlayerState.Paused:
                            case PlayerState.Ended:
                            case PlayerState.Stopped:
                                await SharedLogic.Instance.Player.Play();
                                _timer.Start();
                                PlayPauseIcon = new SymbolIcon(Symbol.Pause);
                                DontUpdatePosition = false;
                                break;
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                await SharedLogic.Instance.NotificationManager.ShowMessageAsync("Some error occured while playing the song. ERROR INFO: " + ex.Message);
            }
        }

        private void SetNowPlayingSong()
        {
            string path = SettingsHelper.GetLocalSetting<string>("path", "");

            if (TracksCollection.Elements.Any(t => t.State == PlayerState.Playing))
            {
                var sa = TracksCollection.Elements.Where(l => l.State == PlayerState.Playing);
                foreach (var mp3 in sa)
                {
                    mp3.State = PlayerState.Stopped;
                }
            }
            if (TracksCollection.Elements.Any(t => t.Path == path))
            {
                TracksCollection.Elements.GetSongByPath(path).State = PlayerState.Playing;
                UpdateCurrentlyPlayingSongIndex();
            }
        }

        public async Task<Mediafile> GetUpcomingSong(bool isNext = false, bool raiseException = true)
        {
            var playingCollection = GetPlayingCollection();
            NowPlayingQueue = playingCollection;
            if (playingCollection != null)
            {
                try
                {
                    Mediafile toPlayFile = null;
                    if (Shuffle)
                    {
                        //only recreate the shuffled list if the current list is null or
                        //if it has gotten stale.
                        if (_shuffledList.Count < playingCollection.Count || _shuffledList == null)
                        {
                            _shuffledList = await ShuffledCollection();
                        }

                        //just set the index of currently playing file to -1
                        //if we are reached the last file in the playing collection.
                        if (_indexOfCurrentlyPlayingFile > playingCollection.Count - 2)
                        {
                            _indexOfCurrentlyPlayingFile = -1;
                        }
                        toPlayFile = _shuffledList?.ElementAt(_indexOfCurrentlyPlayingFile + 1);
                    }
                    else if (IsSourceGrouped)
                    {
                        toPlayFile = GetNextOrPrevSongInGroup(false);
                    }
                    else
                    {
                        if (_indexOfCurrentlyPlayingFile + 1 <= playingCollection.Count)
                        {
                            toPlayFile = _indexOfCurrentlyPlayingFile <= playingCollection.Count - 2 && _indexOfCurrentlyPlayingFile != -1
                                        ? playingCollection[_indexOfCurrentlyPlayingFile + 1]
                                        : Repeat == "Repeat List" || isNext
                                        ? playingCollection.ElementAt(0)
                                        : null;
                        }
                    }
                    return toPlayFile;
                }
                catch (Exception ex)
                {
                    if (raiseException)
                    {
                        BLogger.E(string.Format("An error occured while trying to play next song. IsShuffle: {0} IsSourceGrouped: {1}", Shuffle, IsSourceGrouped), ex);
                        await SharedLogic.Instance.NotificationManager.ShowMessageAsync("An error occured while trying to play next song. Trying again...");
                        ClearPlayerState();
                        PlayNext();
                    }
                }
            }
            return null;
        }
        private Mediafile GetPreviousSong()
        {
            var playingCollection = GetPlayingCollection();
            NowPlayingQueue = playingCollection;
            if (playingCollection != null)
            {
                try
                {
                    Mediafile previousSong = null;
                    if (IsSourceGrouped)
                    {
                        GetNextOrPrevSongInGroup(true);
                    }
                    else
                    {
                        var previousSongIndex = _indexOfCurrentlyPlayingFile <= 0 ? _indexOfCurrentlyPlayingFile = playingCollection.Count - 1 : _indexOfCurrentlyPlayingFile - 1;
                        previousSong = Shuffle ? _shuffledList[_indexOfCurrentlyPlayingFile - 1] : playingCollection[previousSongIndex];
                    }
                    return previousSong;
                }
                catch { }
            }
            return null;
        }
        private void UpdateCurrentlyPlayingSongIndex()
        {
            var playingCollection = GetPlayingCollection();
            if (playingCollection != null)
            {
                _indexOfCurrentlyPlayingFile = playingCollection.GetCurrentlyPlayingIndex();
            }
        }
        private Mediafile GetNextOrPrevSongInGroup(bool prev = false)
        {
            try
            {
                //get current group (the group in which current song is playing).
                Grouping<IGroupKey, Mediafile> currentGroup = TracksCollection.GetCurrentlyPlayingGroup();
                if (currentGroup == null)
                    return null;
                //get the index of the song playing in the currentGroup (with reference to the currentGroup)
                int currentSongIndex = currentGroup.GetPlayingSongIndexInGroup();

                //get next song index (depending on the parameters).
                int nextSongIndex = prev ? currentSongIndex - 1 : currentSongIndex + 1;

                //get condition for next/prev group.
                bool nextGroupCondition = nextSongIndex.Equals(prev ? -1 : currentGroup.Count);

                //get next/prev group index
                int nextGroupIndex = prev ? TracksCollection.IndexOf(currentGroup) - 1 : TracksCollection.IndexOf(currentGroup) + 1;
                if (nextGroupIndex >= TracksCollection.Count - 1)
                {
                    nextGroupIndex = 0;
                }
                //get next/prev group.
                Grouping<IGroupKey, Mediafile> nextGroup = nextGroupCondition ? TracksCollection.ElementAt(nextGroupIndex) : currentGroup;

                //get nextSong index depending on if the group is new or old. 
                int toPlaySongIndex = nextGroup.Equals(currentGroup) ? nextSongIndex : 0;

                if ((nextGroup.Count - 1) >= toPlaySongIndex)
                    return nextGroup.ElementAt(toPlaySongIndex);

                return null;
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }

        private async void PlayNext()
        {
            if (SharedLogic.Instance.Player.CurrentlyPlayingFile != null)
            {
                PreviousSong = SharedLogic.Instance.Player.CurrentlyPlayingFile;
            }

            Mediafile toPlayFile = UpcomingSong;
            if (toPlayFile == null)
            {
                PlayPause();
            }
            else
            {
                await PlayFile(toPlayFile);
            }
        }

        private ThreadSafeObservableCollection<Mediafile> GetPlayingCollection()
        {
            if (Shuffle)
            {
                return _shuffledList;
            }
            else
            {
                if (PlaylistSongCollection?.IsPlayingCollection() == true || IsPlayingFromPlaylist)
                {
                    return PlaylistSongCollection;
                }
                else if (TracksCollection?.Elements?.IsPlayingCollection() == true)
                {
                    return TracksCollection.Elements;
                }
            }
            if (NowPlayingQueue?.Any() == true)
            {
                return NowPlayingQueue;
            }
            return null;
        }

        private async void PlayPrevious()
        {
            if (CurrentPosition > 5)
            {
                DontUpdatePosition = true;
                CurrentPosition = 0;
                DontUpdatePosition = false;
                return;
            }
            if (PreviousSong != null)
            {
                await PlayFile(PreviousSong);
            }
        }

        private async void Open(object para)
        {
            FileOpenPicker openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.MusicLibrary
            };
            openPicker.FileTypeFilter.Add(".mp3");
            openPicker.FileTypeFilter.Add(".wav");
            openPicker.FileTypeFilter.Add(".ogg");
            openPicker.FileTypeFilter.Add(".flac");
            openPicker.FileTypeFilter.Add(".m4a");
            openPicker.FileTypeFilter.Add(".aif");
            openPicker.FileTypeFilter.Add(".wma");
            openPicker.FileTypeFilter.Add(".aac");
            openPicker.FileTypeFilter.Add(".mp4");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(file);
                var mp3File = await TagReaderHelper.CreateMediafile(file);
                if (SharedLogic.Instance.Player.PlayerState == PlayerState.Paused || SharedLogic.Instance.Player.PlayerState == PlayerState.Stopped)
                {
                    await Load(mp3File);
                }
                else
                {
                    await Load(mp3File, true);
                }
            }
        }

        #endregion Implementation

        #endregion Commands

        #region Events

        private int eventCount = 0; //used in AudioEndpointChangedEvent
        private async void OnAudioEndpointChanged(AudioRoutingManager sender, object args)
        {
            var currentEndpoint = sender.GetAudioEndpoint();
            //when this event is initialized, it is invoked 2 times.
            //to avoid changing the device at that time, we use this statement.
            if (eventCount > 1)
            {
                BLogger.I($"Switching audio render device to [{currentEndpoint.ToString()}].");
                await SharedLogic.Instance.Player.ChangeDevice(currentEndpoint.ToString());
            }
            //increase the event count
            eventCount += 1;
        }

        private async void OnDefaultAudioRenderDeviceChanged(object sender, DefaultAudioRenderDeviceChangedEventArgs args)
        {
            // If we have a device ID but Play is disabled, enable it. This handles when an audio device has been been reset.
            if (!String.IsNullOrEmpty(args.Id) && !PlayPauseCommand.IsEnabled)
                PlayPauseCommand.IsEnabled = true;
            if (args.Role != AudioDeviceRole.Default || args.Id == _audioDeviceId)
                return;
            // If no device ID is supplied we cannot play media.
            if (String.IsNullOrEmpty(args.Id))
            {
                if (SharedLogic.Instance.Player.PlayerState == PlayerState.Playing)
                {
                    PlayPause();
                }
                PlayPauseCommand.IsEnabled = false;
                BLogger.I("Audio device disabled or not found.");
                await SharedLogic.Instance.NotificationManager.ShowMessageAsync("It appears your audio device has been disabled or stopped functioning. Please re-enable your audio device to continue. You may need to restart BreadPlayer to resume playback.");
            }
            else
            {
                PlayPauseCommand.IsEnabled = true;
                var device = await DeviceInformation.CreateFromIdAsync(args.Id);
                if (!String.IsNullOrEmpty(_audioDeviceId))
                {
                    var oldDevice = await DeviceInformation.CreateFromIdAsync(_audioDeviceId);
                    BLogger.I($"Switching audio render device from [{oldDevice.Name}] to [{device.Name}]");
                }
                else
                {
                    BLogger.I($"New audio render device connected. {device.Name}");
                }
                _audioDeviceId = args.Id;

                await SharedLogic.Instance.Player.ChangeDevice(device.Name);
            }
        }

        private async void Player_MediaAboutToEnd(object sender, MediaAboutToEndEventArgs e)
        {
            if (UpcomingSong == null)
            {
                UpcomingSong = await GetUpcomingSong(true);
            }
            if (Repeat != "Repeat Song" && UpcomingSong != null && SharedLogic.Instance.SettingsVm.CoreSettingsVM.UpcomingSongNotifcationsEnabled)
            {
                SharedLogic.Instance.NotificationManager.SendUpcomingSongNotification(UpcomingSong);
                await SharedLogic.Instance.NotificationManager.ShowMessageAsync("Upcoming Song: " + UpcomingSong.Title + " by " + UpcomingSong.LeadArtist, 15);
            }
        }

        private async void ShellViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Shuffle" && Shuffle)
            {
                //on startup there is a chance that the main library will be null.
                await ReshuffleCollection();
            }
        }
        private async Task ReshuffleCollection()
        {
            if (TracksCollection?.Elements?.Any() == true)
            {
                _shuffledList = await ShuffledCollection().ConfigureAwait(false);
                _indexOfCurrentlyPlayingFile = -1;
                UpcomingSong = await GetUpcomingSong().ConfigureAwait(false);
                PreviousSong = GetPreviousSong();
            }
        }

        private async void Player_MediaEnded(object sender, MediaEndedEventArgs e)
        {
            var lastPlayingSong = SharedLogic.Instance.Player.CurrentlyPlayingFile;
            if (Repeat == "Repeat List")
            {
                PlayNext();
            }
            else
            {
                await BreadDispatcher.InvokeAsync(() =>
                {
                    DontUpdatePosition = true;
                    CurrentPosition = 0;

                    SharedLogic.Instance.Player.PlayerState = Repeat == "Repeat Song" ? PlayerState.Stopped : PlayerState.Playing;
                    if (Repeat == "No Repeat" && GetPlayingCollection() != null && GetPlayingCollection().Any())
                    {
                        PlayNext();
                    }
                    else
                        PlayPause();
                });
            }
            await BreadDispatcher.InvokeAsync(async () =>
            {
                if (TracksCollection.Elements.Contains(lastPlayingSong))
                {
                    lastPlayingSong.PlayCount++;
                    lastPlayingSong.LastPlayed = DateTime.Now;
                    await _service.UpdateMediafile(lastPlayingSong);
                }
                await ScrobblePlayingSong(lastPlayingSong);
            });
        }

        private void Timer_Tick(object sender, object e)
        {
            double pos = 0;
            if (SharedLogic.Instance.Player != null)
            {
                pos = SharedLogic.Instance.Player.Position;
            }
            if (!DontUpdatePosition)
            {
                CurrentPosition = pos;
            }
        }

        #endregion Events

        #region Properties

        private bool _isPlaybarHidden;
        public bool IsPlaybarHidden
        {
            get => _isPlaybarHidden;
            set => Set(ref _isPlaybarHidden, value);
        }

        private bool _isEqualizerVisible;

        public bool IsEqualizerVisible
        {
            get => _isEqualizerVisible;
            set => Set(ref _isEqualizerVisible, value);
        }

        private bool _isVolumeSliderVisible;

        public bool IsVolumeSliderVisible
        {
            get => _isVolumeSliderVisible;
            set => Set(ref _isVolumeSliderVisible, value);
        }

        private bool _isPlayingFromPlaylist;

        public bool IsPlayingFromPlaylist { get; set; }
        public bool IsPlayingFromNetwork { get; set; }

        private bool _isSourceGrouped;

        public bool IsSourceGrouped { get; set; }

        public GroupedObservableCollection<IGroupKey, Mediafile> TracksCollection
        { get; set; }

        public ThreadSafeObservableCollection<Mediafile> PlaylistSongCollection
        { get; set; }

        private ThreadSafeObservableCollection<Mediafile> _nowPlayingQueue;

        public ThreadSafeObservableCollection<Mediafile> NowPlayingQueue
        {
            get => _nowPlayingQueue;
            set => Set(ref _nowPlayingQueue, value);
        }

        private string _repeat = "No Repeat";

        public string Repeat
        {
            get => _repeat;
            set
            {
                Set(ref _repeat, value);
                SettingsHelper.SaveRoamingSetting("Repeat", _repeat);
            }
        }

        private bool _shuffle;

        public bool Shuffle
        {
            get => _shuffle;
            set
            {
                Set(ref _shuffle, value);
                SettingsHelper.SaveRoamingSetting("Shuffle", _shuffle);
            }
        }

        public bool DontUpdatePosition { get; set; }
        private double _currentPosition;

        public double CurrentPosition
        {
            get => _currentPosition;
            set
            {
                Set(ref _currentPosition, value);
                if (DontUpdatePosition)
                {
                    SharedLogic.Instance.Player.Position = _currentPosition;
                }
            }
        }

        public SymbolIcon PlayPauseIcon
        {
            get => _playPauseIcon;
            set
            {
                if (_playPauseIcon == null)
                {
                    _playPauseIcon = new SymbolIcon(Symbol.Play);
                }

                Set(ref _playPauseIcon, value);
            }
        }

        private Mediafile _upcomingsong = new Mediafile(); //we init beforehand so no null exception occurs

        public Mediafile UpcomingSong
        {
            get => _upcomingsong;
            set => Set(ref _upcomingsong, value);
        }

        private Mediafile _previoussong = new Mediafile(); //we init beforehand so no null exception occurs

        public Mediafile PreviousSong
        {
            get => _previoussong;
            set => Set(ref _previoussong, value);
        }

        #endregion Properties

        #region Reset

        public void Reset()
        {
            PlaylistSongCollection?.Clear();
            TracksCollection?.Clear();
            DontUpdatePosition = true;
            CurrentPosition = 0;
            UpcomingSong = null;
            _timer.Stop();
            _shuffledList?.Clear();
            PlayPauseIcon = new SymbolIcon(Symbol.Play);
        }

        #endregion Reset

        #region Methods

        private async Task ScrobblePlayingSong(Mediafile song)
        {
            if (SharedLogic.Instance.LastfmScrobbler != null)
            {
                var scrobble = await SharedLogic.Instance.LastfmScrobbler.Scrobble(song.LeadArtist, song.Album, song.Title);
                if (scrobble.Success)
                {
                    await SharedLogic.Instance.NotificationManager.ShowMessageAsync("Song successfully scrobbled.", 4);
                }
                else
                {
                    await SharedLogic.Instance.NotificationManager.ShowMessageBoxAsync(string.Format("Failed to scrobble this song due to {0}. Exception details: {1}.", scrobble.Status.ToString(), scrobble?.Exception?.Message), "Failed to scrobble this song");
                }
            }
        }

        private void GetSettings()
        {
            Shuffle = SettingsHelper.GetRoamingSetting<bool>("Shuffle", false);
            Repeat = SettingsHelper.GetRoamingSetting<string>("Repeat", "No Repeat");
        }

        private async Task PlayFile(Mediafile toPlayFile, bool play = false)
        {
            if (SharedLogic.Instance.Player.PlayerState == PlayerState.Paused || SharedLogic.Instance.Player.PlayerState == PlayerState.Stopped)
            {
                await Load(toPlayFile);
            }
            else
            {
                Messenger.Instance.NotifyColleagues(MessageTypes.MsgPlaySong, toPlayFile);
            }
        }

        private ThreadSafeObservableCollection<Mediafile> _shuffledList;

        public async Task<ThreadSafeObservableCollection<Mediafile>> ShuffledCollection()
        {
            var shuffled = new ThreadSafeObservableCollection<Mediafile>();
            await BreadDispatcher.InvokeAsync(() =>
            {
                shuffled.AddRange(TracksCollection.Elements.ToList());
                shuffled.Shuffle();
            });
            return shuffled;
        }

        private void ClearPlayerState()
        {
            if (TracksCollection != null)
            {
                if (IsSourceGrouped)
                {
                    foreach (var song in TracksCollection.SelectMany(t => t.Select(a => a).Where(b => b.State == PlayerState.Playing)))
                    {
                        song.State = PlayerState.Stopped;
                    }
                }
                if (TracksCollection.Elements.IsPlayingCollection())
                {
                    TracksCollection.Elements.FirstOrDefault(t => t.State == PlayerState.Playing).State = PlayerState.Stopped;
                }
                else if (NowPlayingQueue?.Any(t => t.State == PlayerState.Playing) == true)
                {
                    NowPlayingQueue.FirstOrDefault(t => t.State == PlayerState.Playing).State = PlayerState.Stopped;
                }

                if (PlaylistSongCollection.IsPlayingCollection())
                {
                    PlaylistSongCollection.FirstOrDefault(t => t.State == PlayerState.Playing).State = PlayerState.Stopped;
                }
            }
        }

        private bool IsSongToStopAfter()
        {
            if (_songToStopAfter != null
                && (_songToStopAfter.CompareTo(PreviousSong) == 0
                || _songToStopAfter.CompareTo(SharedLogic.Instance.Player.CurrentlyPlayingFile) == 0))
            {
                PlayPause();
                _songToStopAfter = null;
                PreviousSong = null;
                UpcomingSong = null;
                return true;
            }
            return false;
        }

        private ApplicationView applicationView = ApplicationView.GetForCurrentView();

        private async Task UpdateUi(Mediafile mediaFile)
        {
            applicationView.Title = string.Format("Listening to {0} by {1}", mediaFile.Title, mediaFile.LeadArtist);

            ThemeManager.SetThemeColor(SharedLogic.Instance.Player.CurrentlyPlayingFile?.AttachedPicture);
            CoreWindowLogic.UpdateSmtc();
            CoreWindowLogic.SaveSettings();
            CoreWindowLogic.UpdateTile(mediaFile);
            if (SharedLogic.Instance.SettingsVm.CoreSettingsVM.ReplaceLockscreenWithAlbumArt)
            {
                await LockscreenHelper.ChangeLockscreenImage(mediaFile);
            }
            UpdateCurrentlyPlayingSongIndex();
            UpcomingSong = await GetUpcomingSong(true);
            PreviousSong = GetPreviousSong();
        }
        private Task<bool> DynamicLoadMusicAsync(Mediafile mediafile)
        {
            switch (mediafile.MediaLocation)
            {
                case MediaLocationType.Device:
                case MediaLocationType.Network:
                    return SharedLogic.Instance.Player.LoadStreamAsync(mediafile, mediafile.ByteArray);
                case MediaLocationType.Internet:
                    return SharedLogic.Instance.Player.LoadURLAsync(mediafile, mediafile.Path);
                default:
                case MediaLocationType.Local:
                    return SharedLogic.Instance.Player.LoadLocalFileAsync(mediafile);
            }
        }
        public async Task Load(Mediafile mp3File, bool play = false, double currentPos = 0, double vol = 50)
        {
            if (mp3File == null) return;
            if (IsSongToStopAfter()) return;

            ClearPlayerState();

            mp3File.State = PlayerState.Playing;
            SharedLogic.Instance.Player.Volume = SharedLogic.Instance.Player.Volume == 50 ? vol : SharedLogic.Instance.Player.Volume;
            if (await DynamicLoadMusicAsync(mp3File))
            {
                PlayPauseCommand.IsEnabled = true;
                if (play)
                {
                    PlayPauseCommand.Execute(null);

                    //navigate to now playing view automatically if on mobile.
                    if (InitializeSwitch.IsMobile)
                    {
                        NavigateToNowPlayingView("NowPlayingView");
                    }
                }
                else
                {
                    DontUpdatePosition = true;
                    CurrentPosition = currentPos;
                }

                await UpdateUi(mp3File);
            }
            else
            {
                BLogger.I("Failed to load file. Loading next file...");
                await UpdateUi(mp3File);
                await Load(UpcomingSong, true);
            }
        }

        #endregion Methods
    }
}