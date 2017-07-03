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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
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
using BreadPlayer.Themes;

namespace BreadPlayer.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        #region Fields
        private SymbolIcon _playPauseIcon = new SymbolIcon(Symbol.Play);
        private SymbolIcon _repeatIcon = new SymbolIcon(Symbol.Sync);
        private Mediafile _songToStopAfter;
        private DispatcherTimer _timer;
        private UndoRedoStack<Mediafile> _history = new UndoRedoStack<Mediafile>();
        private LibraryService _service = new LibraryService(new KeyValueStoreDatabaseService(SharedLogic.DatabasePath, "Tracks", "TracksText"));
        private int _songCount;
        #endregion

        #region Constructor
        public ShellViewModel()
        {
            NavigateToNowPlayingViewCommand = new DelegateCommand(NavigateToNowPlayingView);
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
            Player.PlayerState = PlayerState.Stopped;
            DontUpdatePosition = false;
            _timer = new DispatcherTimer(new BreadDispatcher(SharedLogic.Dispatcher))
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            _timer.Tick += Timer_Tick;
            _timer.Stop();
            Player.MediaEnded += Player_MediaEnded;
            PropertyChanged += ShellViewModel_PropertyChanged;
            Player.MediaAboutToEnd += Player_MediaAboutToEnd;
        }
        #endregion

        #region HandleMessages

        private void HandleEnablePlayMessage(Message message)
        {
            if (message.Payload is short count && count > 0)
            {
                message.HandledStatus = MessageHandledStatus.HandledContinue;
                PlayPauseCommand.IsEnabled = true;
            }
        }
        private void HandleLibraryLoadedMessage(Message message)
        {
            if (message.Payload is ThreadSafeObservableCollection<Mediafile> tMediaFile)
            {
                message.HandledStatus = MessageHandledStatus.HandledCompleted;
                PlaylistSongCollection = tMediaFile;
            }
            else
            {
                var listObject = message.Payload as List<object>;
                TracksCollection = listObject[0] as GroupedObservableCollection<string, Mediafile>;
                IsSourceGrouped = (bool)listObject[1];
                _songCount = _service.SongCount;
                TracksCollection.CollectionChanged += TracksCollection_CollectionChanged;
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
                UpcomingSong = await GetUpcomingSong();
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
                await Load(file, play);
            }
        }

        private async void HandleExecuteCmdMessage(Message message)
        {
            if (message.Payload != null)
            {
                if (message.Payload is List<object> list)
                {
                    double volume = 0;
                    if ((double)list[3] == 50.0)
                    {
                        volume = RoamingSettingsHelper.GetSetting<double>("volume", 50.0);
                    }
                    else
                    {
                        volume = (double)list[3];
                    }

                    await Load(await SharedLogic.CreateMediafile(list[0] as StorageFile), (bool)list[2], (double)list[1], volume);
                }
                else
                {
                    GetType().GetTypeInfo().GetDeclaredMethod(message.Payload as string)?.Invoke(this, new object[] { });
                }

                message.HandledStatus = MessageHandledStatus.HandledCompleted;
            }
        }

        private async void HandleDisposeMessage()
        {
            Reset();
            await Player.Stop();
        }

        private void HandleSaveSongToStopAfterMessage(Message songToStopAfter)
        {
            if (songToStopAfter.Payload is Mediafile mediaFile)
            {
                _songToStopAfter = mediaFile;
            }
        }

        #endregion

        #region Commands

        #region Definition

        private RelayCommand _openSongCommand;
        private DelegateCommand _playPreviousCommand;
        private DelegateCommand _playNextCommand;
        private DelegateCommand _playPauseCommand;
        private DelegateCommand _setRepeatCommand;
        private DelegateCommand _showEqualizerCommand;

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
        public DelegateCommand PlayPauseCommand { get { if (_playPauseCommand == null) {
                    _playPauseCommand = new DelegateCommand(PlayPause)
                    {
                        IsEnabled = false
                    };
                } return _playPauseCommand; } }
        public DelegateCommand PlayNextCommand { get { if (_playNextCommand == null) { _playNextCommand = new DelegateCommand(PlayNext); } return _playNextCommand; } }
        public DelegateCommand PlayPreviousCommand { get { if (_playPreviousCommand == null) { _playPreviousCommand = new DelegateCommand(PlayPrevious); } return _playPreviousCommand; } }
        public DelegateCommand SetRepeatCommand { get { if (_setRepeatCommand == null) { _setRepeatCommand = new DelegateCommand(SetRepeat); } return _setRepeatCommand; } }
        public DelegateCommand ShowEqualizerCommand { get { if (_showEqualizerCommand == null) { _showEqualizerCommand = new DelegateCommand(ShowEqualizer); } return _showEqualizerCommand; } }
        public DelegateCommand NavigateToNowPlayingViewCommand { get; set; }// { if (navigateToNowPlayingViewCommand == null) navigateToNowPlayingViewCommand = new DelegateCommand(NavigateToNowPlayingView); return navigateToNowPlayingViewCommand; } }

        #endregion

        #region Implementation 
        private void Mute()
        {
            Player.IsVolumeMuted = Player.IsVolumeMuted ? false : true;
        }
        private void IncreaseVolume()
        {
            if(Player.Volume < 100)
            {
                Player.Volume++;
            }
        }
        private void DecreaseVolume()
        {
            if (Player.Volume > 0)
            {
                Player.Volume--;
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
        private void NavigateToNowPlayingView()
        {
            IsPlaybarHidden = true;
            if(!InitializeCore.IsMobile)
                ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
        }
        private void ShowEqualizer()
        {
            IsEqualizerVisible = IsEqualizerVisible ? false : true;
            DisplayInformation.AutoRotationPreferences = DisplayInformation.AutoRotationPreferences == DisplayOrientations.Landscape ? DisplayOrientations.Portrait : DisplayOrientations.Landscape;
        }

        private void SetRepeat()
        {
            switch (Repeat)
            {
                case "No Repeat":
                    Repeat = "Repeat Song";
                    Player.IsLoopingEnabled = true;
                    break;
                case "Repeat Song":
                    Repeat = "Repeat List";
                    Player.IsLoopingEnabled = false;
                    break;
                case "Repeat List":
                    Repeat = "No Repeat";
                    Player.IsLoopingEnabled = false;
                    break;
                default:
                    break;
            }
        }
        private async void PlayPause()
        {
            try
            {
                if (Player.CurrentlyPlayingFile == null && TracksCollection.Elements.Count > 0)
                {
                    await Load(TracksCollection.Elements.First(), true);
                }
                else
                {
                    await SharedLogic.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        switch (Player.PlayerState)
                        {
                            case PlayerState.Playing:
                                await Player.Pause();
                                _timer.Stop();
                                Player.PlayerState = PlayerState.Stopped;
                                PlayPauseIcon = new SymbolIcon(Symbol.Play);
                                break;
                            case PlayerState.Paused:
                            case PlayerState.Ended:
                            case PlayerState.Stopped:
                                await Player.Play();
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
                await NotificationManager.ShowMessageAsync("Some error occured while playing the song. ERROR INFO: " + ex.Message);
            }
        }
        private void SetNowPlayingSong()
        {
            string path = RoamingSettingsHelper.GetSetting<string>("path", "");
            if (!TracksCollection.Elements.Any(t => t.Path == path && t.State == PlayerState.Playing))
            {
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
                    TracksCollection.Elements.FirstOrDefault(t => t.Path == path).State = PlayerState.Playing;
                }
            }
        }
        public async Task<Mediafile> GetUpcomingSong(bool isNext = false)
        {
            var playingCollection = GetPlayingCollection();
            NowPlayingQueue = playingCollection;
            if (playingCollection != null && playingCollection.Any())
            {
                try
                {
                    int indexOfCurrentlyPlayingFile = -1;
                    if (playingCollection.Any(t => t.State == PlayerState.Playing))
                    {
                        indexOfCurrentlyPlayingFile = playingCollection.IndexOf(playingCollection.FirstOrDefault(t => t.State == PlayerState.Playing));
                    }

                    Mediafile toPlayFile = null;
                    if (Shuffle)
                    {
                        if (_shuffledList.Count < playingCollection.Count || _shuffledList == null)
                        {
                            _shuffledList = await ShuffledCollection();
                            indexOfCurrentlyPlayingFile = 0;
                        }
                        toPlayFile = _shuffledList?.ElementAt(indexOfCurrentlyPlayingFile + 1);
                    }
                    else if (IsSourceGrouped)
                    {
                        toPlayFile = GetNextSongInGroup();
                    }
                    else
                    {
                        toPlayFile = indexOfCurrentlyPlayingFile <= playingCollection.Count - 2 && indexOfCurrentlyPlayingFile != -1 ? playingCollection.ElementAt(indexOfCurrentlyPlayingFile + 1) : Repeat == "Repeat List" || isNext ? playingCollection.ElementAt(0) : null;
                    }
                    return toPlayFile;
                }
                catch (Exception ex)
                {
                    BLogger.Logger.Error("An error occured while trying to play next song.", ex);
                    await NotificationManager.ShowMessageAsync("An error occured while trying to play next song. Trying again...");
                    TracksCollection?.Elements.Where(t => t.State == PlayerState.Playing).ToList().ForEach(file => { file.State = PlayerState.Stopped; });
                    PlaylistSongCollection?.Where(t => t.State == PlayerState.Playing).ToList().ForEach(file => { file.State = PlayerState.Stopped; });
                    PlayNext();
                }
            }
            return null;
        }

        private Mediafile GetNextSongInGroup()
        {
            var currentGroup = TracksCollection.FirstOrDefault(t => t.Any(c => c.Path == Player.CurrentlyPlayingFile.Path));
            var currentSongIndex = currentGroup.IndexOf(currentGroup.FirstOrDefault(t => t.Path == Player.CurrentlyPlayingFile.Path));
            var nextGroup = currentSongIndex + 1 == currentGroup.Count ? TracksCollection.ElementAt(TracksCollection.IndexOf(currentGroup) + 1) : currentGroup;
            var toPlaySongIndex = nextGroup == currentGroup ? currentSongIndex + 1 : 0;
            return nextGroup.ElementAt(toPlaySongIndex);
        }

        private async void PlayNext()
        {
            if (Player.CurrentlyPlayingFile != null)
            {
                PreviousSong = Player.CurrentlyPlayingFile;
                _history.Do(Player.CurrentlyPlayingFile);
            }

            Mediafile toPlayFile = await GetUpcomingSong(true);
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
            if (PlaylistSongCollection?.Any(t => t.State == PlayerState.Playing) == true || IsPlayingFromPlaylist)
            {
                return PlaylistSongCollection;
            }
            if (TracksCollection?.Elements.Any(t => t.State == PlayerState.Playing) == true)
            {
                return TracksCollection.Elements;
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
            var file = _history.Undo(null);
            PreviousSong = _history.SemiUndo(null);
            if (file != null)
            {
                await PlayFile(file);
            }
        }

        private async void Open(object para)
        {
            var picker = new FileOpenPicker();
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
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                var mp3File = await SharedLogic.CreateMediafile(file, true);
                if (Player.PlayerState == PlayerState.Paused || Player.PlayerState == PlayerState.Stopped)
                {
                    await Load(mp3File);
                }
                else
                {
                    await Load(mp3File, true);
                }
            }

        }
        #endregion

        #endregion

        #region Events  
        private async void Player_MediaAboutToEnd(object sender, MediaAboutToEndEventArgs e)
        {
            if (UpcomingSong == null)
            {
                UpcomingSong = await GetUpcomingSong(true);
            }

            NotificationManager.SendUpcomingSongNotification(UpcomingSong);
            await NotificationManager.ShowMessageAsync("Upcoming Song: " + UpcomingSong.Title + " by " + UpcomingSong.LeadArtist, 15);
        }

        private async void ShellViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Shuffle" && Shuffle)
            {
                _shuffledList = await ShuffledCollection().ConfigureAwait(false);
                UpcomingSong = await GetUpcomingSong().ConfigureAwait(false);
            }
        }
        private async void Player_MediaEnded(object sender, MediaEndedEventArgs e)
        {
            var lastPlayingSong = Player.CurrentlyPlayingFile;
            if (Repeat == "Repeat List")
            {
                PlayNext();
            }
            else
            {
                await SharedLogic.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    DontUpdatePosition = true;
                    CurrentPosition = 0;
                    Player.PlayerState = Repeat == "Repeat Song" ? PlayerState.Stopped : PlayerState.Playing;
                    if (Repeat == "No Repeat" && GetPlayingCollection() != null && GetPlayingCollection().Any())
                    {
                        PlayNext();
                    }
                    else
                        PlayPause();
                });
            }
            await SharedLogic.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                lastPlayingSong.PlayCount++;
                lastPlayingSong.LastPlayed = DateTime.Now.ToString(CultureInfo.CurrentCulture);
                TracksCollection.Elements.First(T => T.Path == lastPlayingSong.Path).PlayCount++;
                TracksCollection.Elements.First(T => T.Path == lastPlayingSong.Path).LastPlayed = DateTime.Now.ToString();
                await _service.UpdateMediafile(lastPlayingSong);

                await ScrobblePlayingSong();
            });
        }
        private void Timer_Tick(object sender, object e)
        {
            double pos = 0;
            if (Player != null)
            {
                pos = Player.Position;
            }
            if (!DontUpdatePosition)
            {
                CurrentPosition = pos;
            }
        }

        #endregion

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
        public bool IsPlayingFromPlaylist
        {
            get => _isPlayingFromPlaylist;
            set => Set(ref _isPlayingFromPlaylist, value);
        }

        private bool _isSourceGrouped;
        public bool IsSourceGrouped
        {
            get => _isSourceGrouped;
            set => Set(ref _isSourceGrouped, value);
        }
        public GroupedObservableCollection<string, Mediafile> TracksCollection
        { get; set; }
        public ThreadSafeObservableCollection<Mediafile> PlaylistSongCollection
        { get; set; }

        private ThreadSafeObservableCollection<Mediafile> _nowPlayingQueue;      
        public ThreadSafeObservableCollection<Mediafile> NowPlayingQueue
        {
            get => _nowPlayingQueue;
            set => Set(ref _nowPlayingQueue, value);
        }

        private string _queryWord = "";
        public string QueryWord
        {
            get => _queryWord;
            set => Set(ref _queryWord, value);
        }

        private string _repeat = "No Repeat";
        public string Repeat
        {
            get => _repeat;
            set
            {
                Set(ref _repeat, value);
                ApplicationData.Current.RoamingSettings.Values["Repeat"] = Repeat;
            }
        }

        private bool _shuffle;
        public bool Shuffle
        {
            get => _shuffle;
            set
            {
                Set(ref _shuffle, value);
                ApplicationData.Current.RoamingSettings.Values["Shuffle"] = Shuffle;
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
                    Player.Position = _currentPosition;
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
        #endregion

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
        #endregion

        #region Methods
        private async Task ScrobblePlayingSong()
        {
            if (SharedLogic.LastfmScrobbler != null)
            {
                var scrobble = await SharedLogic.LastfmScrobbler.Scrobble(Player.CurrentlyPlayingFile.LeadArtist, Player.CurrentlyPlayingFile.Album, Player.CurrentlyPlayingFile.Title);
                if (scrobble.Success)
                {
                    await NotificationManager.ShowMessageAsync("Song successfully scrobbled.", 4);
                }
                else
                {
                    await NotificationManager.ShowMessageBoxAsync(string.Format("Failed to scrobble this song due to {0}. Exception details: {1}.", scrobble.Status.ToString(), scrobble?.Exception?.Message), "Failed to scrobble this song");
                }
            }
        }
      
        private void GetSettings()
        {
            Shuffle = RoamingSettingsHelper.GetSetting<bool>("Shuffle", false);
            Repeat = RoamingSettingsHelper.GetSetting<string>("Repeat", "No Repeat");
        }

        private async Task PlayFile(Mediafile toPlayFile, bool play = false)
        {
            if (Player.PlayerState == PlayerState.Paused || Player.PlayerState == PlayerState.Stopped)
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
            await SharedLogic.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
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
                List<Mediafile> songCollectionWithPlayingState = new List<Mediafile>();
                if (IsSourceGrouped)
                {
                    songCollectionWithPlayingState.AddRange(TracksCollection.SelectMany(t => t.Select(a => a).Where(b => b.State == PlayerState.Playing)));
                }
                if (TracksCollection.Elements.Any())
                {
                    songCollectionWithPlayingState.AddRange(TracksCollection.Elements.Where(t => t.State == PlayerState.Playing));
                }

                if (PlaylistSongCollection != null && PlaylistSongCollection.Any())
                {
                    songCollectionWithPlayingState.AddRange(PlaylistSongCollection.Where(t => t.State == PlayerState.Playing));
                }

                foreach (var song in songCollectionWithPlayingState)
                {
                    song.State = PlayerState.Stopped;
                }
            }
        }
        private bool IsSongToStopAfter()
        {
            if (_songToStopAfter != null && (_songToStopAfter.CompareTo(PreviousSong) == 0 || _songToStopAfter.CompareTo(Player.CurrentlyPlayingFile) == 0))
            {
                PlayPause();
                _songToStopAfter = null;
                PreviousSong = null;
                UpcomingSong = null;
                return true;
            }
            return false;
        }
        private async Task UpdateUi(Mediafile mediaFile)
        {
            ThemeManager.SetThemeColor(Player.CurrentlyPlayingFile?.AttachedPicture);
            CoreWindowLogic.UpdateSmtc();
            CoreWindowLogic.UpdateTile(mediaFile);
            if (SharedLogic.SettingsVm.ReplaceLockscreenWithAlbumArt)
            {
                await LockscreenHelper.ChangeLockscreenImage(mediaFile);
            }

            UpcomingSong = await GetUpcomingSong();
        }
        public async Task Load(Mediafile mp3File, bool play = false, double currentPos = 0, double vol = 50)
        {
            ClearPlayerState();
            if (mp3File != null)
            {
                if (IsSongToStopAfter())
                {
                    return;
                }

                if (play)
                {
                    Player.IgnoreErrors = true;
                }

                mp3File.State = PlayerState.Playing;
                Player.Volume = Player.Volume == 50 ? vol : Player.Volume;
                if (await Player.Load(mp3File))
                {
                    PlayPauseCommand.IsEnabled = true;
                    if (play)
                    {
                        PlayPauseCommand.Execute(null);

                        //navigate to now playing view automatically if on mobile.
                        if (InitializeCore.IsMobile)
                        {
                            NavigateToNowPlayingView();
                        }
                    }
                    else
                    {
                        DontUpdatePosition = true;
                        CurrentPosition = currentPos;
                    }
                }
                else
                {
                    BLogger.Logger.Error("Failed to load file. Loading next file...");
                    var playingCollection = GetPlayingCollection();
                    int indexoferrorfile = playingCollection.IndexOf(playingCollection.FirstOrDefault(t => t.Path == mp3File.Path));
                    Player.IgnoreErrors = false;
                    await Load(await GetUpcomingSong(), true);
                }

                await UpdateUi(mp3File);
            }
        }       
        #endregion

    }
}
