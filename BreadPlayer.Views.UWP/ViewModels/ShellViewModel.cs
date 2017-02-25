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
using System.Windows.Input;
using BreadPlayer.Extensions;
using System.Collections.Generic;
using BreadPlayer.MomentoPattern;
using BreadPlayer.Messengers;
using BreadPlayer.Common;
using BreadPlayer.Service;
using System.Reflection;

namespace BreadPlayer.ViewModels
{
	public class ShellViewModel : ViewModelBase
    {
        #region Fields
        private SymbolIcon _playPauseIcon = new SymbolIcon(Symbol.Play);
        private SymbolIcon _repeatIcon = new SymbolIcon(Symbol.Sync);
        DispatcherTimer timer;
        UndoRedoStack<Mediafile> history = new UndoRedoStack<Mediafile>();
        LibraryService service = new LibraryService(new DatabaseService());
        int SongCount = 0;
        #endregion

        #region Constructor
        public ShellViewModel()
        {
            Messenger.Instance.Register(Messengers.MessageTypes.MSG_PLAYLIST_LOADED, new Action<Message>(HandleLibraryLoadedMessage));
            Messenger.Instance.Register(Messengers.MessageTypes.MSG_LIBRARY_LOADED, new Action<Message>(HandleLibraryLoadedMessage));
            Messenger.Instance.Register(MessageTypes.MSG_PLAY_SONG, new Action<Message>(HandlePlaySongMessage));
            Messenger.Instance.Register(MessageTypes.MSG_DISPOSE, new Action(HandleDisposeMessage));
            Messenger.Instance.Register(MessageTypes.MSG_EXECUTE_CMD, new Action<Message>(HandleExecuteCmdMessage));
            Messenger.Instance.Register(MessageTypes.MSG_UPDATE_SONG_COUNT, new Action<Message>(HandleEnablePlayMessage));
            SearchCommand.IsEnabled = false;
            PlayPauseIcon = new SymbolIcon(Symbol.Play);
            //PlaylistsItems = new ObservableCollection<SimpleNavMenuItem>();
            Player.PlayerState = PlayerState.Stopped;
            DontUpdatePosition = false;
            this.timer = new DispatcherTimer(new BreadPlayer.Dispatcher.BreadDispatcher(Dispatcher));
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
            this.timer.Stop();
            Player.MediaEnded += Player_MediaEnded;
            this.PropertyChanged += ShellViewModel_PropertyChanged;
        }

        #endregion

        #region HandleMessages
        void HandleEnablePlayMessage(Message message)
        {
            if (message.Payload is short)
            {
                var count = (short)message.Payload;
                if (count > 0)
                {
                    message.HandledStatus = MessageHandledStatus.HandledContinue;
                    PlayPauseCommand.IsEnabled = true;
                }
            }
        }
        private async void HandleLibraryLoadedMessage(Message message)
        {
            if (message.Payload is ThreadSafeObservableCollection<Mediafile>)
            {
                message.HandledStatus = MessageHandledStatus.HandledCompleted;
                PlaylistSongCollection = message.Payload as ThreadSafeObservableCollection<Mediafile>;
            }
            else
            {
                var listObject = message.Payload as List<object>;
                TracksCollection = listObject[0] as GroupedObservableCollection<string, Mediafile>;
                IsSourceGrouped = (bool)listObject[1];
                TracksCollection.CollectionChanged += TracksCollection_CollectionChanged;
                UpcomingSong = await GetUpcomingSong().ConfigureAwait(false);
                SongCount = TracksCollection.Elements.Count;
                GetSettings();
            }
        }

        private void TracksCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (TracksCollection.Elements.Count > 0)
            {
                searchCommand.IsEnabled = true;
                PlayPauseCommand.IsEnabled = true;
            }
        }

        void HandlePlaySongMessage(Message message)
        {
            var obj = message.Payload as List<object>;
            if(obj != null)
            {
                message.HandledStatus = MessageHandledStatus.HandledCompleted;
                var file = obj[0] as Mediafile;
                var play = (bool)obj[1];
                IsPlayingFromPlaylist = (bool)obj[2];
                Load(file, play);
            }
        }
        void HandleExecuteCmdMessage(Message message)
        {
            if (message.Payload != null)
            {
                if(message.Payload is List<object>)
                {
                    var list = message.Payload as List<object>;
                    double volume = 0;
                    if ((double)list[3] == 50.0)
                        volume = RoamingSettingsHelper.GetSetting<double>("volume", 50.0);
                    else
                        volume = (double)list[3];
                    Play(list[0] as StorageFile, null, (double)list[1], (bool)list[2], volume);
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
        #endregion

        #region Commands

        #region Definition
        RelayCommand _openSongCommand;
        DelegateCommand _playPreviousCommand;
        DelegateCommand _playNextCommand;
        DelegateCommand _playPauseCommand;
        DelegateCommand _setRepeatCommand;
        RelayCommand searchCommand;
        public RelayCommand SearchCommand
        {
            get { if (searchCommand == null) searchCommand = new RelayCommand(param => Search(param)); return searchCommand; }
        }
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

        #endregion

        #region Implementation 
        ThreadSafeObservableCollection<Mediafile> cache;
        public void Search(object para)
        {
            try
            {
                DispatcherTimer timer = new  DispatcherTimer(new BreadPlayer.Dispatcher.BreadDispatcher(Dispatcher));
                if (QueryWord.Length < 2 && TracksCollection.Elements.Count < SongCount)
                {
                    Messenger.Instance.NotifyColleagues(MessageTypes.MSG_SEARCH_STARTED, "Music Library");
                    Reload().ConfigureAwait(false);
                }
                else if (QueryWord.Length > 2)
                {
                    timer.Interval = TimeSpan.FromMilliseconds(200);
                    timer.Start();
                    timer.Tick += (sender, args) =>
                    {
                        Messenger.Instance.NotifyColleagues(MessageTypes.MSG_SEARCH_STARTED, "Results for \"" + QueryWord + "\"");
                        Search().ConfigureAwait(false);
                        timer.Stop();
                    };
                }
            }
            catch (Exception ex)
            {
                BLogger.Logger.Error("Error occured while searching. Search string length: " + QueryWord.Length, ex);
            }
        }
        void SetRepeat()
        {
            if (Repeat == "No Repeat")
            {
                Repeat = "Repeat Song";
            }
            else if (Repeat == "Repeat Song")
            {
                Repeat = "Repeat List";
            }
            else if (Repeat == "Repeat List")
            {
                Repeat = "No Repeat";
            }
        }
        private async void PlayPause()
        {
            if (Player.CurrentlyPlayingFile == null && TracksCollection.Elements.Count > 0)
                Play(null, TracksCollection.Elements.First());
            else
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
            }
        }
        public async Task<Mediafile> GetUpcomingSong(bool isNext = false)
        {
            var playingCollection = GetPlayingCollection();
            if (playingCollection != null && playingCollection.Any())
            {
                try
                {                    
                    int IndexOfCurrentlyPlayingFile = -1;
                    if (playingCollection.Any(t => t.State == PlayerState.Playing))
                        IndexOfCurrentlyPlayingFile = playingCollection.IndexOf(playingCollection.SingleOrDefault(t => t.State == PlayerState.Playing));
                    Mediafile toPlayFile = null;
                    if (Shuffle)
                    {
                        if (ShuffledList.Count < playingCollection.Count)
                        {
                            ShuffledList = await ShuffledCollection();
                            IndexOfCurrentlyPlayingFile = 0;
                        }
                        toPlayFile = ShuffledList?.ElementAt(IndexOfCurrentlyPlayingFile + 1);
                    }
                    else if(IsSourceGrouped)
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

                    return null;
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
		PreviousSong= Player.CurrentlyPlayingFile;
		history.Do(Player.CurrentlyPlayingFile);
            }
                
            Mediafile toPlayFile = await GetUpcomingSong(true);
            if (toPlayFile == null)
            {
                PlayPause();
            }
            else
                PlayFile(toPlayFile);
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
            else
            {
                return null;
            }
        }
        void PlayPrevious()
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
            if (file != null) PlayFile(file);
        }
        async void Open(object para)
        {
            var picker = new FileOpenPicker();
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;            
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
                var mp3file = await CreateMediafile(file, true);
                if (Player.PlayerState == PlayerState.Paused || Player.PlayerState == PlayerState.Stopped)
                    Load(mp3file);
                else
                {
                    Load(mp3file, true);
                }
            }

        }
        #endregion

        #endregion

        #region Events  
        
        private async void ShellViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Shuffle")
            {
                if (Shuffle == true)
                {
                    ShuffledList = await ShuffledCollection().ConfigureAwait(false);
                    UpcomingSong = await GetUpcomingSong().ConfigureAwait(false);
                }
            }
        }
        private async void Player_MediaEnded(object sender, Events.MediaEndedEventArgs e)
        {

            if (Repeat == "Repeat List")
            {
                PlayNext();
            }
            else
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
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
        bool _isplaybarvisible = true;
        public bool IsPlayBarVisible
        {
            get { return _isplaybarvisible; }
            set
            {
                Set(ref _isplaybarvisible, value);
                ApplicationData.Current.RoamingSettings.Values["IsPlayBarVisible"] = IsPlayBarVisible;
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
        async Task Reload()
        {
            if (cache == null || TracksCollection.Elements.Count < service.SongCount)
            {
                TracksCollection.Clear();
                await SplitList(400).ConfigureAwait(false);
                cache = new ThreadSafeObservableCollection<Mediafile>(TracksCollection.Elements);
            }
            else
            {
                TracksCollection.Clear();
                TracksCollection.AddRange(cache, false, true);
            }
        }
        public async Task SplitList(int nSize = 30)
        {
            for (int i = 0; i < service.SongCount; i += nSize)
            {
                TracksCollection.AddRange(await service.GetRangeOfMediafiles(i, Math.Min(nSize, service.SongCount - i)).ConfigureAwait(false), false, false);
            }
        }
        async Task<object> Search()
        {
            if (QueryWord.Length > 2)
            {
                TracksCollection.Clear();
                TracksCollection.AddRange((await service.Query("Title", QueryWord).ConfigureAwait(false)), false, false);
                if (TracksCollection.Elements.Count <= 0)
                    Messenger.Instance.NotifyColleagues(MessageTypes.MSG_SEARCH_STARTED, "Nothing found for keyword \"" + QueryWord + "\"");
            }
            return null;
        }
        private void GetSettings()
        {
            Shuffle = RoamingSettingsHelper.GetSetting<bool>("Shuffle", false);
            IsPlayBarVisible = RoamingSettingsHelper.GetSetting<bool>("IsPlayBarVisible", true);
            Repeat = RoamingSettingsHelper.GetSetting<string>("Repeat", "No Repeat");
        }
        void PlayFile(Mediafile toPlayFile, bool play = false)
        {
            if (Player.PlayerState == PlayerState.Paused || Player.PlayerState == PlayerState.Stopped)
            {
                Load(toPlayFile);
            }
            else
            {
                Messenger.Instance.NotifyColleagues(MessageTypes.MSG_PLAY_SONG, toPlayFile);
                //LibVM.PlayCommand.Execute(toPlayFile);
            }
        }

        ThreadSafeObservableCollection<Mediafile> ShuffledList;
        public async Task<ThreadSafeObservableCollection<Mediafile>> ShuffledCollection()
        {
            var shuffled = new ThreadSafeObservableCollection<Mediafile>();
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                shuffled.AddRange(TracksCollection.Elements.ToList());
                shuffled.Shuffle();
            });
            return shuffled;
        }

        public async void Load(Mediafile mp3file, bool play = false, double currentPos = 0, double vol = 50)
        {
            if (mp3file != null)
            {
                try
                {
                    if (play == true)
                        Player.IgnoreErrors = true;

                    if (await Player.Load(mp3file))
                    {
                        TracksCollection?.Elements.Where(t => t.State == PlayerState.Playing).ToList().ForEach(new Action<Mediafile>((Mediafile file) => { file.State = PlayerState.Stopped; service.UpdateMediafile(file); }));
                        PlaylistSongCollection?.Where(t => t.State == PlayerState.Playing).ToList().ForEach(new Action<Mediafile>((Mediafile file) => { file.State = PlayerState.Stopped; }));
                        PlayPauseCommand.IsEnabled = true;
                        mp3file.State = PlayerState.Playing;
                        if (Player.Volume == 50)
                            Player.Volume = vol;
                        if (play)
                        {
                            PlayPauseCommand.Execute(null);
                        }
                        else
                        {
                            DontUpdatePosition = true;
                            CurrentPosition = currentPos;
                        }
                        if (GetPlayingCollection() != null)
                            UpcomingSong = await GetUpcomingSong();

                        Themes.ThemeManager.SetThemeColor(Player.CurrentlyPlayingFile.AttachedPicture);
                    }
                    else
                    {
                        BLogger.Logger.Error("Failed to load file. Loading next file...");
                        TracksCollection?.Elements.Where(t => t.State == PlayerState.Playing).ToList().ForEach(new Action<Mediafile>((Mediafile file) => { file.State = PlayerState.Stopped; service.UpdateMediafile(file); }));
                        PlaylistSongCollection?.Where(t => t.State == PlayerState.Playing).ToList().ForEach(new Action<Mediafile>((Mediafile file) => { file.State = PlayerState.Stopped; }));
                        mp3file.State = PlayerState.Playing;
                        int indexoferrorfile = GetPlayingCollection().IndexOf(GetPlayingCollection().FirstOrDefault(t => t.Path == mp3file.Path));
                        Player.IgnoreErrors = false;
                        Load(await GetUpcomingSong(), true);
                    }
                }
                catch (Exception ex)
                {
                    BLogger.Logger.Error("Failed to load file.", ex);
                }
            }
        }
        public async void Play(StorageFile para, Mediafile mp3File = null, double currentPos = 0, bool play = true, double vol = 50)
        {
            if (para != null)
            {
                mp3File = await CreateMediafile(para, true);
            }
            else if (para == null && mp3File == null)
            {
                return;
            }
            Load(mp3File, play, currentPos, vol);
        }
        #endregion

    }


}
