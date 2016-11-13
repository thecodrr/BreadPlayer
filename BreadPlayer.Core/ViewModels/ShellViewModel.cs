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
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;
using System.IO;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using BreadPlayer.Core;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SplitViewMenu;
using BreadPlayer.Services;
using BreadPlayer.Models;
using System.Windows.Input;
using BreadPlayer.Extensions;
using System.Collections.Generic;
using BreadPlayer.MomentoPattern;
using Windows.Media;
using System.Diagnostics;
using Windows.UI.Xaml.Input;
using Extensions;

namespace BreadPlayer.ViewModels
{
   public class ShellViewModel : ViewModelBase, IDisposable
    {
        #region Fields
        private SymbolIcon _playPauseIcon = new SymbolIcon(Symbol.Play);
        private SymbolIcon _repeatIcon = new SymbolIcon(Symbol.Sync);
        DispatcherTimer timer;
        UndoRedoStack<Mediafile> history = new UndoRedoStack<Mediafile>();
        #endregion

        #region Commands

        #region Definition
        RelayCommand _openSongCommand;
        DelegateCommand _playPreviousCommand;
        DelegateCommand _playNextCommand;
        DelegateCommand _playPauseCommand;
        DelegateCommand _setRepeatCommand;
        /// <summary>
        /// Gets OpenSong command. This calls the <see cref="Open(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand OpenSongCommand
        {
            get
            { if (_openSongCommand == null) { _openSongCommand = new RelayCommand(param => this.Open(param)); } return _openSongCommand; }
        }
        public DelegateCommand PlayPauseCommand { get { if (_playPauseCommand == null) { _playPauseCommand = new DelegateCommand(PlayPause); _playPauseCommand.IsEnabled = LibVM.TracksCollection.Count > 0 ? true : false; } return _playPauseCommand; }}
        public DelegateCommand PlayNextCommand { get { if (_playNextCommand == null) _playNextCommand = new DelegateCommand(PlayNext); return _playNextCommand; } }
        public DelegateCommand PlayPreviousCommand { get { if (_playPreviousCommand == null) _playPreviousCommand = new DelegateCommand(PlayPrevious); return _playPreviousCommand; } }
        public DelegateCommand SetRepeatCommand { get { if (_setRepeatCommand == null) _setRepeatCommand = new DelegateCommand(SetRepeat); return _setRepeatCommand; } }

        #endregion

        #region Implementation
        void SetRepeat()
        {
            if(Repeat == "No Repeat")
            {
                Repeat = "Repeat Song";
            }
                else if(Repeat == "Repeat Song")
            {
                Repeat = "Repeat List";
            }
                else if(Repeat == "Repeat List")
            {
                Repeat = "No Repeat";
            }
        }
        private async void PlayPause()
        {
            if (Player.CurrentlyPlayingFile == null && LibVM.TracksCollection.Elements.Count > 0)
                Play(null, LibVM.TracksCollection.Elements.First());
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
        public async Task<Mediafile> GetUpcomingSong()
        {
            if (LibVM.TracksCollection.Elements.Any())
            {
                try
                {
                    int IndexOfCurrentlyPlayingFile = -1;
                    if (GetPlayingCollection().Any(t => t.State == PlayerState.Playing))
                        IndexOfCurrentlyPlayingFile = GetPlayingCollection().IndexOf(GetPlayingCollection().SingleOrDefault(t => t.State == PlayerState.Playing));
                    Mediafile toPlayFile = null;
                    if (Shuffle)
                    {
                        if (!ShuffledList.Any())
                        {
                            ShuffledList = await ShuffledCollection().ConfigureAwait(false);
                            IndexOfCurrentlyPlayingFile = 0;
                        }
                        toPlayFile = ShuffledList?.ElementAt(IndexOfCurrentlyPlayingFile + 1);
                    }
                    else
                    {
                        toPlayFile = IndexOfCurrentlyPlayingFile <= GetPlayingCollection().Count - 2 && IndexOfCurrentlyPlayingFile != -1 ? GetPlayingCollection().ElementAt(IndexOfCurrentlyPlayingFile + 1) : Repeat == "Repeat List" ? GetPlayingCollection().ElementAt(0) : null;
                    }
                    return toPlayFile;
                }
                catch (Exception ex)
                {
                    await NotificationManager.ShowAsync(ex.Message);
                    return null;
                }
            }
            return null;
        }
        async void PlayNext()
        {
            if (LibVM.TracksCollection.Elements.Any())
            {
                if (Player.CurrentlyPlayingFile != null)
                    history.Do(Player.CurrentlyPlayingFile);

                Mediafile toPlayFile = await GetUpcomingSong();
                if (toPlayFile == null)
                {
                    PlayPause();
                }
                else
                   PlayFile(toPlayFile);

            }
        }
        ThreadSafeObservableCollection<Mediafile> GetPlayingCollection()
        {
            if (PlaylistVM.Songs.Elements.Any(t => t.State == PlayerState.Playing))
            {
                return PlaylistVM.Songs.Elements;
            }
            else
            {
                return LibVM.TracksCollection.Elements;
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
            if(file != null) PlayFile(file);
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
            if(file != null)
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
                    if (Repeat == "No Repeat" && GetPlayingCollection().Any())
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
        string _repeat = "No Repeat";
        public string Repeat
        {
            get { return _repeat; }
            set
            {
                Set(ref _repeat, value);

            }
        }
        bool _isplaybarvisible = true;
        public bool IsPlayBarVisible
        {
            get { return _isplaybarvisible; }
            set { Set(ref _isplaybarvisible, value); }
        }
        
        bool _shuffle = false;
        public bool Shuffle
        {
            get { return _shuffle; }
            set { Set(ref _shuffle, value); }
        }
        public ObservableCollection<SimpleNavMenuItem> PlaylistsItems { get; }
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
            set { Set(ref upcomingsong, value);}
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            DontUpdatePosition = true;
            CurrentPosition = 0;
            UpcomingSong = null;
            timer.Stop();
            ShuffledList?.Clear();
            PlayPauseIcon = new SymbolIcon(Symbol.Play);
        }
        #endregion

        #region Methods

        void PlayFile(Mediafile toPlayFile, bool play = false)
        {
            if (Player.PlayerState == PlayerState.Paused || Player.PlayerState == PlayerState.Stopped)
            {
                Load(toPlayFile);
            }
            else
            {
                LibVM.PlayCommand.Execute(toPlayFile);
            }
        }

        ThreadSafeObservableCollection<Mediafile> ShuffledList;
        public async Task<ThreadSafeObservableCollection<Mediafile>> ShuffledCollection()
        {
            var shuffled = new ThreadSafeObservableCollection<Mediafile>();
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                shuffled.AddRange(LibVM.TracksCollection.Elements.ToList());
                shuffled.Shuffle();
            });         
            return shuffled;
        }
        async void ChangePlayingSongState(PlayerState compareValue, PlayerState state)
        {
            try
            {
                List<Mediafile> mp3 = new List<Mediafile>();
                mp3.AddRange(PlaylistVM?.Songs?.Elements.Where(t => t.State == compareValue));
                mp3.AddRange(LibVM?.TracksCollection?.Elements?.Where(t => t.State == compareValue));
                mp3.AddRange(LibVM?.RecentlyPlayedCollection?.Where(t => t.State == compareValue));
                if (mp3 != null)
                    foreach (Mediafile song in mp3)
                    {
                        if (song != null)
                            song.State = state;
                    }
            }
            catch (Exception ex)
            {
                await NotificationManager.ShowAsync(ex.Message);
            }
        }
        public async void Load(Mediafile mp3file,  bool play = false, double currentPos = 0, double vol = 50)
        {
            if (mp3file != null)
            {
                if (play == true)
                    Player.IgnoreErrors = true;
                ChangePlayingSongState(PlayerState.Playing, PlayerState.Stopped);
                if (await Player.Load(mp3file))
                {                   
                    PlayPauseCommand.IsEnabled = true;
                    mp3file.State = PlayerState.Playing;
                    var mp3 = LibVM.TracksCollection?.Elements?.SingleOrDefault(t => t.Path == Player.CurrentlyPlayingFile?.Path);
                    if (mp3 != null) mp3.State = PlayerState.Playing;
                    if (play)
                    {                       
                        PlayPauseCommand.Execute(null);
                    }
                    else
                    {
                        Player.Volume = vol;
                        DontUpdatePosition = true;
                        CurrentPosition = currentPos;
                    }
                   UpcomingSong = await GetUpcomingSong();
                }
                else
                {
                    mp3file.State = PlayerState.Playing;
                    int indexoferrorfile = GetPlayingCollection().IndexOf(GetPlayingCollection().FirstOrDefault(t => t.Path == mp3file.Path));
                    Player.IgnoreErrors = false;
                    Load(await GetUpcomingSong(), true);                    
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

        #region Constructor
        public ShellViewModel()
        {
            PlayPauseIcon = new SymbolIcon(Symbol.Play);
            PlaylistsItems = new ObservableCollection<SimpleNavMenuItem>();
            Player.PlayerState = PlayerState.Stopped;
            DontUpdatePosition = false;
            this.timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
            this.timer.Stop();
            Player.MediaEnded += Player_MediaEnded;
            this.PropertyChanged += ShellViewModel_PropertyChanged;
        }

        string queryWord = "";
        public string QueryWord
        {
            get { return queryWord; }
            set { Set(ref queryWord, value); }
        }
        RelayCommand searchCommand;
        public RelayCommand SearchCommand
        {
            get { if (searchCommand == null) searchCommand = new RelayCommand(param => Search(param)); return searchCommand; }
        }
        ThreadSafeObservableCollection<Mediafile> cache;
        public void Search(object para)
        {
            if (QueryWord.Length == 0 && LibVM.TracksCollection.Elements.Count < LibVM.SongCount)
            {
               Reload().ConfigureAwait(false);                
            }
            if(para is KeyRoutedEventArgs)
            { 
            if (((KeyRoutedEventArgs)para).Key == Windows.System.VirtualKey.Enter)
            {
                Search().ConfigureAwait(false);
            }
            }
        }
        public void Search(object para, AutoSuggestBoxQuerySubmittedEventArgs e)
        {
             Search().ConfigureAwait(false);
        }
        async Task Reload()
        {
            if (cache == null)
            {
                LibVM.TracksCollection.Clear();
                LibVM.TracksCollection.AddRange(await LibVM.Database.GetTracks().ConfigureAwait(false), true);               
                cache = new ThreadSafeObservableCollection<Mediafile>(LibVM.TracksCollection.Elements);
                LibVM.SongCount = LibVM.TracksCollection.Elements.Count;
            }
            else
            {
                LibVM.TracksCollection.Clear();
                LibVM.TracksCollection.AddRange(cache, true);
            }
        }
        async Task<object> Search()
        {
            if (QueryWord.Length > 0)
            {
                LibVM.TracksCollection.Clear();
                LibVM.TracksCollection.AddRange(await LibVM.Database.Query(QueryWord).ConfigureAwait(false), true);
            }
            return null;
        }
        #endregion

        /// <summary>
        /// This is for ease of testing only.
        /// </summary>
        /// <param name="msg">The message to display.</param>
        public async void ShowMessage(string msg)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(msg);
            await dialog.ShowAsync();
        }
    }


}
