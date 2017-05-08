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
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;
using BreadPlayer.Core;
using System.ComponentModel;
using BreadPlayer.Models;
using BreadPlayer.Extensions;
using System.Collections.Generic;
using BreadPlayer.MomentoPattern;
using BreadPlayer.Messengers;
using BreadPlayer.Common;
using BreadPlayer.Database;
using System.Reflection;
using Windows.Graphics.Display;
using BreadPlayer.Helpers;
using Windows.UI.ViewManagement;

namespace BreadPlayer.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        #region Fields
        private SymbolIcon _playPauseIcon = new SymbolIcon(Symbol.Play);
        private SymbolIcon _repeatIcon = new SymbolIcon(Symbol.Sync);
        private Mediafile _songToStopAfter;
        DispatcherTimer timer;
        UndoRedoStack<Mediafile> history = new UndoRedoStack<Mediafile>();
        LibraryService service = new LibraryService(new KeyValueStoreDatabaseService(Core.SharedLogic.DatabasePath, "Tracks", "TracksText"));
        int SongCount = 0;
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
            Messenger.Instance.Register(Messengers.MessageTypes.MSG_PLAYLIST_LOADED, new Action<Message>(HandleLibraryLoadedMessage));
            Messenger.Instance.Register(Messengers.MessageTypes.MSG_LIBRARY_LOADED, new Action<Message>(HandleLibraryLoadedMessage));
            Messenger.Instance.Register(MessageTypes.MSG_PLAY_SONG, new Action<Message>(HandlePlaySongMessage));
            Messenger.Instance.Register(MessageTypes.MSG_DISPOSE, new Action(HandleDisposeMessage));
            Messenger.Instance.Register(MessageTypes.MSG_EXECUTE_CMD, new Action<Message>(HandleExecuteCmdMessage));
            Messenger.Instance.Register(MessageTypes.MSG_UPDATE_SONG_COUNT, new Action<Message>(HandleEnablePlayMessage));
            Messenger.Instance.Register(MessageTypes.MSG_STOP_AFTER_SONG, new Action<Message>(HandleSaveSongToStopAfterMessage));
            PlayPauseIcon = new SymbolIcon(Symbol.Play);
            //PlaylistsItems = new ObservableCollection<SimpleNavMenuItem>();
            Player.PlayerState = PlayerState.Stopped;
            DontUpdatePosition = false;
            this.timer = new DispatcherTimer(new BreadPlayer.Dispatcher.BreadDispatcher(SharedLogic.Dispatcher));
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
            this.timer.Stop();
            Player.MediaEnded += Player_MediaEnded;
            this.PropertyChanged += ShellViewModel_PropertyChanged;
            Player.MediaAboutToEnd += Player_MediaAboutToEnd;
        }
        #endregion

        #region HandleMessages
        void HandleEnablePlayMessage(Message message)
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
                SongCount = service.SongCount;
                TracksCollection.CollectionChanged += TracksCollection_CollectionChanged;
                GetSettings();
            }
        }

        private async void TracksCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (TracksCollection.Elements.Count > 0)
            {
                PlayPauseCommand.IsEnabled = true;
            }
            if (TracksCollection.Elements.Count == SongCount)
            {
                SetNowPlayingSong();
                UpcomingSong = await GetUpcomingSong();
            }
            TracksCollection.CollectionChanged -= TracksCollection_CollectionChanged;
        }

        async void HandlePlaySongMessage(Message message)
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
        async void HandleExecuteCmdMessage(Message message)
        {
            if (message.Payload != null)
            {
                if (message.Payload is List<object> list)
                {
                    double volume = 0;
                    if ((double)list[3] == 50.0)
                        volume = RoamingSettingsHelper.GetSetting<double>("volume", 50.0);
                    else
                        volume = (double)list[3];
                    await Load(await SharedLogic.CreateMediafile(list[0] as StorageFile), (bool)list[2], (double)list[1], volume);
                }
                else
                    this.GetType().GetTypeInfo().GetDeclaredMethod(message.Payload as string)?.Invoke(this, new object[] { });
                message.HandledStatus = MessageHandledStatus.HandledCompleted;
            }
        }
        async void HandleDisposeMessage()
        {
            Reset();
            await Player.Stop();
        }

        void HandleSaveSongToStopAfterMessage(Message songToStopAfter)
        {
            if (songToStopAfter.Payload is Mediafile mediaFile)
            {
                _songToStopAfter = mediaFile;
            }
        }

        #endregion

        #region Commands

        #region Definition
        RelayCommand _openSongCommand;
        DelegateCommand _playPreviousCommand;
        DelegateCommand _playNextCommand;
        DelegateCommand _playPauseCommand;
        DelegateCommand _setRepeatCommand;
        DelegateCommand showEqualizerCommand;

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
            { if (_openSongCommand == null) { _openSongCommand = new RelayCommand(param => this.Open(param)); } return _openSongCommand; }
        }
        public DelegateCommand PlayPauseCommand { get { if (_playPauseCommand == null) { _playPauseCommand = new DelegateCommand(PlayPause); _playPauseCommand.IsEnabled = false; } return _playPauseCommand; } }
        public DelegateCommand PlayNextCommand { get { if (_playNextCommand == null) _playNextCommand = new DelegateCommand(PlayNext); return _playNextCommand; } }
        public DelegateCommand PlayPreviousCommand { get { if (_playPreviousCommand == null) _playPreviousCommand = new DelegateCommand(PlayPrevious); return _playPreviousCommand; } }
        public DelegateCommand SetRepeatCommand { get { if (_setRepeatCommand == null) _setRepeatCommand = new DelegateCommand(SetRepeat); return _setRepeatCommand; } }
        public DelegateCommand ShowEqualizerCommand { get { if (showEqualizerCommand == null) showEqualizerCommand = new DelegateCommand(ShowEqualizer); return showEqualizerCommand; } }
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
                Player.Volume++;
        }
        private void DecreaseVolume()
        {
            if (Player.Volume > 0)
                Player.Volume--;
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
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
        }
        private void ShowEqualizer()
        {
            IsEqualizerVisible = IsEqualizerVisible ? false : true;
            DisplayInformation.AutoRotationPreferences = DisplayInformation.AutoRotationPreferences == DisplayOrientations.Landscape ? DisplayOrientations.Portrait : DisplayOrientations.Landscape;
        }
        void SetRepeat()
        {
            switch (Repeat)
            {
                case "No Repeat":
                    Repeat = "Repeat Song";
                    break;
                case "Repeat Song":
                    Repeat = "Repeat List";
                    break;
                case "Repeat List":
                    Repeat = "No Repeat";
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
                    await Load(TracksCollection.Elements.First(), true);
                else
                {
                    await SharedLogic.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        switch (Player.PlayerState)
                        {
                            case PlayerState.Playing:
                                await Player.Pause();
                                timer.Stop();
                                Player.PlayerState = PlayerState.Stopped;
                                PlayPauseIcon = new SymbolIcon(Symbol.Play);
                                break;
                            case PlayerState.Paused:
                            case PlayerState.Ended:
                            case PlayerState.Stopped:
                                await Player.Play();
                                timer.Start();
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
                    foreach (var mp3 in sa) mp3.State = PlayerState.Stopped;
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
                    int IndexOfCurrentlyPlayingFile = -1;
                    if (playingCollection.Any(t => t.State == PlayerState.Playing))
                        IndexOfCurrentlyPlayingFile = playingCollection.IndexOf(playingCollection.FirstOrDefault(t => t.State == PlayerState.Playing));
                    Mediafile toPlayFile = null;
                    if (Shuffle)
                    {
                        if (ShuffledList.Count < playingCollection.Count || ShuffledList == null)
                        {
                            ShuffledList = await ShuffledCollection();
                            IndexOfCurrentlyPlayingFile = 0;
                        }
                        toPlayFile = ShuffledList?.ElementAt(IndexOfCurrentlyPlayingFile + 1);
                    }
                    else if (IsSourceGrouped)
                    {
                        toPlayFile = GetNextSongInGroup();
                    }
                    else
                    {
                        toPlayFile = IndexOfCurrentlyPlayingFile <= playingCollection.Count - 2 && IndexOfCurrentlyPlayingFile != -1 ? playingCollection.ElementAt(IndexOfCurrentlyPlayingFile + 1) : Repeat == "Repeat List" || isNext ? playingCollection.ElementAt(0) : null;
                    }
                    return toPlayFile;
                }
                catch (Exception ex)
                {
                    BLogger.Logger.Error("An error occured while trying to play next song.", ex);
                    await NotificationManager.ShowMessageAsync("An error occured while trying to play next song. Trying again...");
                    TracksCollection?.Elements.Where(t => t.State == PlayerState.Playing).ToList().ForEach(new Action<Mediafile>((Mediafile file) => { file.State = PlayerState.Stopped; }));
                    PlaylistSongCollection?.Where(t => t.State == PlayerState.Playing).ToList().ForEach(new Action<Mediafile>((Mediafile file) => { file.State = PlayerState.Stopped; }));
                    PlayNext();
                }
            }
            return null;
        }
        Mediafile GetNextSongInGroup()
        {
            var currentGroup = TracksCollection.FirstOrDefault(t => t.Any(c => c.Path == Player.CurrentlyPlayingFile.Path));
            var currentSongIndex = currentGroup.IndexOf(currentGroup.FirstOrDefault(t => t.Path == Player.CurrentlyPlayingFile.Path));
            var nextGroup = currentSongIndex + 1 == currentGroup.Count ? TracksCollection.ElementAt(TracksCollection.IndexOf(currentGroup) + 1) : currentGroup;
            var toPlaySongIndex = nextGroup == currentGroup ? currentSongIndex + 1 : 0;
            return nextGroup.ElementAt(toPlaySongIndex);
        }

        async void PlayNext()
        {
            if (Player.CurrentlyPlayingFile != null)
            {
                PreviousSong = Player.CurrentlyPlayingFile;
                history.Do(Player.CurrentlyPlayingFile);
            }

            Mediafile toPlayFile = await GetUpcomingSong(true);
            if (toPlayFile == null)
            {
                PlayPause();
            }
            else
                await PlayFile(toPlayFile);
        }
        private ThreadSafeObservableCollection<Mediafile> GetPlayingCollection()
        {
            if (PlaylistSongCollection?.Any(t => t.State == PlayerState.Playing) == true || IsPlayingFromPlaylist)
            {
                return PlaylistSongCollection;
            }
            else if (TracksCollection?.Elements.Any(t => t.State == PlayerState.Playing) == true)
            {
                return TracksCollection.Elements;
            }
            return null;
        }
        async void PlayPrevious()
        {
            if (CurrentPosition > 5)
            {
                DontUpdatePosition = true;
                CurrentPosition = 0;
                DontUpdatePosition = false;
                return;
            }
            var file = history.Undo(null);
            PreviousSong = history.SemiUndo(null);
            if (file != null)
                await PlayFile(file);
        }
        async void Open(object para)
        {
            var picker = new FileOpenPicker();
            FileOpenPicker openPicker = new FileOpenPicker()
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
                var mp3file = await SharedLogic.CreateMediafile(file, true);
                if (Player.PlayerState == PlayerState.Paused || Player.PlayerState == PlayerState.Stopped)
                    await Load(mp3file);
                else
                {
                    await Load(mp3file, true);
                }
            }

        }
        #endregion

        #endregion

        #region Events  
        private async void Player_MediaAboutToEnd(object sender, Core.Events.MediaAboutToEndEventArgs e)
        {
            if (UpcomingSong == null)
                UpcomingSong = await GetUpcomingSong(true);
            NotificationManager.SendUpcomingSongNotification(UpcomingSong);
            await NotificationManager.ShowMessageAsync("Upcoming Song: " + UpcomingSong.Title + " by " + UpcomingSong.LeadArtist, 15);
        }

        private async void ShellViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Shuffle" && Shuffle == true)
            {
                ShuffledList = await ShuffledCollection().ConfigureAwait(false);
                UpcomingSong = await GetUpcomingSong().ConfigureAwait(false);
            }
        }
        private async void Player_MediaEnded(object sender, Events.MediaEndedEventArgs e)
        {
            var lastPlayingSong = Player.CurrentlyPlayingFile;
            if (Repeat == "Repeat List")
            {
                PlayNext();
            }
            else
            {
                await SharedLogic.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    DontUpdatePosition = true;
                    CurrentPosition = 0;
                    Player.PlayerState = Repeat == "Repeat Song" ? PlayerState.Stopped : PlayerState.Playing;
                    if (Repeat == "No Repeat" && GetPlayingCollection() != null && GetPlayingCollection().Any())
                        PlayNext();
                    else
                        PlayPause();
                });
            }
            await SharedLogic.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                lastPlayingSong.PlayCount++;
                lastPlayingSong.LastPlayed = DateTime.Now.ToString();
                TracksCollection.Elements.First(T => T.Path == lastPlayingSong.Path).PlayCount++;
                TracksCollection.Elements.First(T => T.Path == lastPlayingSong.Path).LastPlayed = DateTime.Now.ToString();
                await service.UpdateMediafile(lastPlayingSong);

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
            if (!this.DontUpdatePosition)
            {
                CurrentPosition = pos;
            }
        }

        #endregion

        #region Properties
        bool isPlaybarHidden;
        public bool IsPlaybarHidden
        {
            get { return isPlaybarHidden; }
            set { Set(ref isPlaybarHidden, value); }
        }
        bool isEqualizerVisible;
        public bool IsEqualizerVisible
        {
            get { return isEqualizerVisible; }
            set { Set(ref isEqualizerVisible, value); }
        }
        bool isVolumeSliderVisible;
        public bool IsVolumeSliderVisible
        {
            get { return isVolumeSliderVisible; }
            set { Set(ref isVolumeSliderVisible, value); }
        }
        bool isPlayingFromPlaylist;
        public bool IsPlayingFromPlaylist
        {
            get { return isPlayingFromPlaylist; }
            set { Set(ref isPlayingFromPlaylist, value); }
        }
        bool isSourceGrouped;
        public bool IsSourceGrouped
        {
            get { return isSourceGrouped; }
            set { Set(ref isSourceGrouped, value); }
        }
        public GroupedObservableCollection<string, Mediafile> TracksCollection
        { get; set; }
        public ThreadSafeObservableCollection<Mediafile> PlaylistSongCollection
        { get; set; }
        ThreadSafeObservableCollection<Mediafile> nowPlayingQueue;      
        public ThreadSafeObservableCollection<Mediafile> NowPlayingQueue
        {
            get => nowPlayingQueue;
            set => Set(ref nowPlayingQueue, value);
        }
        string queryWord = "";
        public string QueryWord
        {
            get { return queryWord; }
            set { Set(ref queryWord, value); }
        }
        string _repeat = "No Repeat";
        public string Repeat
        {
            get { return _repeat; }
            set
            {
                Set(ref _repeat, value);
                ApplicationData.Current.RoamingSettings.Values["Repeat"] = Repeat;
            }
        }
       
        bool _shuffle = false;
        public bool Shuffle
        {
            get { return _shuffle; }
            set
            {
                Set(ref _shuffle, value);
                ApplicationData.Current.RoamingSettings.Values["Shuffle"] = Shuffle;
            }
        }

        public bool DontUpdatePosition { get; set; }
        double _currentPosition;
        public double CurrentPosition
        {
            get { return this._currentPosition; }
            set
            {
                Set(ref _currentPosition, value);
                if (DontUpdatePosition)
                    Player.Position = _currentPosition;
            }
        }
        public SymbolIcon PlayPauseIcon
        {
            get
            {
                return _playPauseIcon;
            }
            set
            {
                if (_playPauseIcon == null)
                    _playPauseIcon = new SymbolIcon(Symbol.Play);
                Set(ref _playPauseIcon, value);
            }
        }

        Mediafile upcomingsong = new Mediafile(); //we init beforehand so no null exception occurs
        public Mediafile UpcomingSong
        {
            get { return upcomingsong; }
            set { Set(ref upcomingsong, value); }
        }

        Mediafile previoussong = new Mediafile(); //we init beforehand so no null exception occurs
        public Mediafile PreviousSong
        {
            get { return previoussong; }
            set { Set(ref previoussong, value); }
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
            timer.Stop();
            ShuffledList?.Clear();
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
                    await NotificationManager.ShowMessageAsync("Song successfully scrobbled.", 4);
                else
                    await NotificationManager.ShowMessageBoxAsync(string.Format("Failed to scrobble this song due to {0}. Exception details: {1}.", scrobble.Status.ToString(), scrobble?.Exception?.Message), "Failed to scrobble this song");
            }
        }
      
        private void GetSettings()
        {
            Shuffle = RoamingSettingsHelper.GetSetting<bool>("Shuffle", false);
            Repeat = RoamingSettingsHelper.GetSetting<string>("Repeat", "No Repeat");
        }
        async Task PlayFile(Mediafile toPlayFile, bool play = false)
        {
            if (Player.PlayerState == PlayerState.Paused || Player.PlayerState == PlayerState.Stopped)
            {
                await Load(toPlayFile);
            } 
            else
            {
                Messenger.Instance.NotifyColleagues(MessageTypes.MSG_PLAY_SONG, toPlayFile);
            }
        }

        ThreadSafeObservableCollection<Mediafile> ShuffledList;
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
                    songCollectionWithPlayingState.AddRange(TracksCollection.Elements.Where(t => t.State == PlayerState.Playing));
                if (PlaylistSongCollection != null && PlaylistSongCollection.Any())
                    songCollectionWithPlayingState.AddRange(PlaylistSongCollection.Where(t => t.State == PlayerState.Playing));
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
        private async Task UpdateUI(Mediafile mediaFile)
        {
            Themes.ThemeManager.SetThemeColor(Player.CurrentlyPlayingFile?.AttachedPicture);
            CoreWindowLogic.UpdateSmtc();
            CoreWindowLogic.UpdateTile(mediaFile);
            if (SharedLogic.SettingsVM.ReplaceLockscreenWithAlbumArt)
                await LockscreenHelper.ChangeLockscreenImage(mediaFile);
            UpcomingSong = await GetUpcomingSong();
            if (InitializeCore.IsMobile)
                NavigateToNowPlayingView();
        }
        public async Task Load(Mediafile mp3file, bool play = false, double currentPos = 0, double vol = 50)
        {
            ClearPlayerState();
            if (mp3file != null)
            {
                if (IsSongToStopAfter())
                    return;
                if (play == true)
                    Player.IgnoreErrors = true;
                mp3file.State = PlayerState.Playing;
                Player.Volume = Player.Volume == 50 ? vol : Player.Volume;
                if (await Player.Load(mp3file))
                {
                    PlayPauseCommand.IsEnabled = true;
                    if (play)
                    {
                        PlayPauseCommand.Execute(null);
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
                    int indexoferrorfile = playingCollection.IndexOf(playingCollection.FirstOrDefault(t => t.Path == mp3file.Path));
                    Player.IgnoreErrors = false;
                    await Load(await GetUpcomingSong(), true);
                }

                await UpdateUI(mp3file);
            }
        }       
        #endregion

    }
}
