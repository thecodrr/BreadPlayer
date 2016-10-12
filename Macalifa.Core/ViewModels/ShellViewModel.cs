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
using Macalifa.Core;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SplitViewMenu;
using Macalifa.Services;
using Macalifa.Models;
using System.Windows.Input;
using Macalifa.Extensions;
using System.Collections.Generic;
using Macalifa.MomentoPattern;
using Windows.Media;
using System.Diagnostics;

namespace Macalifa.ViewModels
{
   public class ShellViewModel : ViewModelBase
    {
        #region Fields
        private SymbolIcon _playPauseIcon = new SymbolIcon(Symbol.Play);
        DispatcherTimer timer;
        UndoRedoStack<Mediafile> history = new UndoRedoStack<Mediafile>();
        #endregion

        #region Commands

        #region Definition
        RelayCommand _openSongCommand;
        DelegateCommand _playPreviousCommand;
        DelegateCommand _playNextCommand;
        DelegateCommand _playPauseCommand;
        /// <summary>
        /// Gets OpenSong command. This calls the <see cref="Open(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand OpenSongCommand
        {
            get
            { if (_openSongCommand == null) { _openSongCommand = new RelayCommand(param => this.Open(param)); } return _openSongCommand; }
        }
        public DelegateCommand PlayPauseCommand { get { if (_playPauseCommand == null) { _playPauseCommand = new DelegateCommand(PlayPause); _playPauseCommand.IsEnabled = false; } return _playPauseCommand; }}
        public DelegateCommand PlayNextCommand { get { if (_playNextCommand == null) _playNextCommand = new DelegateCommand(PlayNext); return _playNextCommand; } }
        public DelegateCommand PlayPreviousCommand { get { if (_playPreviousCommand == null) _playPreviousCommand = new DelegateCommand(PlayPrevious); return _playPreviousCommand; } }
        #endregion

        #region Implementation
        private async void PlayPause()
        {
            switch (Player.PlayerState)
            {
                case PlayerState.Playing:
                    await Player.Pause();
                    timer.Stop();
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
        void PlayNext()
        {
            if (Player.CurrentlyPlayingFile != null)
                history.Do(Player.CurrentlyPlayingFile);
            int IndexOfCurrentlyPlayingFile = LibVM.FileListBox.Items.IndexOf(LibVM.FileListBox.Items.Single(t => (t as Mediafile).State == PlayerState.Playing));
            Mediafile toPlayFile = null;
            if (Shuffle)
            {
                toPlayFile = ShuffledList.ElementAt(IndexOfCurrentlyPlayingFile + 1);
            }
            else
            {
                toPlayFile = IndexOfCurrentlyPlayingFile <= LibVM.FileListBox.Items.Count - 2 ? LibVM.FileListBox.Items.ElementAt(IndexOfCurrentlyPlayingFile + 1) as Mediafile : LibVM.FileListBox.Items.ElementAt(0) as Mediafile;
            }
            PlayFile(toPlayFile);
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
            StorageFile file = await openPicker.PickSingleFileAsync();            
            var mp3file = await CoreMethods.CreateMediafile(file);
            if (Player.PlayerState == PlayerState.Paused || Player.PlayerState == PlayerState.Stopped)
                Load(mp3file);
            else
            {
                Load(mp3file, true);
            }
        }
        #endregion

        #endregion

        #region Events
        private async void Player_MediaEnded(object sender, Events.MediaEndedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                DontUpdatePosition = true;
                CurrentPosition = 0;
                Player.PlayerState = Repeat ? PlayerState.Stopped : PlayerState.Playing;
                PlayPause();
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
        bool _repeat = false;
        public bool Repeat
        {
            get { return _repeat; }
            set { Set(ref _repeat, value); }
        }
        bool _shuffle = false;
        public bool Shuffle
        {
            get { return _shuffle; }
            set { Set(ref _shuffle, value); if(_shuffle == true)ShuffledList = ShuffledCollection(); }
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
        #endregion

        #region Methods
        void PlayFile(Mediafile toPlayFile)
        {
            if (Player.PlayerState == PlayerState.Paused || Player.PlayerState == PlayerState.Stopped)
                Load(toPlayFile);
            else
            {
                LibVM.PlayCommand.Execute(toPlayFile);
            }
        }

        ThreadSafeObservableCollection<Mediafile> ShuffledList;
        public ThreadSafeObservableCollection<Mediafile> ShuffledCollection()
        {
            var shuffled = new ThreadSafeObservableCollection<Mediafile>(LibVM.TracksCollection.Elements);
            shuffled.Shuffle();
            return shuffled;
        }
        private async void Load(Mediafile mp3file,  bool play = false, double currentPos = 0, double vol = 50)
        {
            if (mp3file != null)
            {
                if (await Player.Load(mp3file))
                {
                    PlayPauseCommand.IsEnabled = true;
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
                }
            }
        }
        public async void Play(StorageFile para, Mediafile mp3File = null, double currentPos = 0, bool play = true, double vol = 50)
        {
            if(para != null)
            {
                mp3File = await CreateMediafile(para);               
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
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += Timer_Tick;
            this.timer.Stop();
            Player.MediaEnded += Player_MediaEnded;
           
        }
        RelayCommand searchCommand;
        public RelayCommand SearchCommand
        {
            get { if (searchCommand == null) searchCommand = new RelayCommand(param => Search(param)); return searchCommand; }
        }
        public async void Search(object para)
        {
            await CoreMethods.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (para.ToString().Length > 0)
                {
                    LibVM.TracksCollection.Clear();
                    LibVM.TracksCollection.AddRange(LibVM.db.Query(para.ToString()));
                }               
            });
        }
        #endregion
        public async void ShowMessage(string msg)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(msg);
            await dialog.ShowAsync();
        }
    }


}
