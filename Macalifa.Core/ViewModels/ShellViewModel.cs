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

namespace Macalifa.ViewModels
{
   public class ShellViewModel : ViewModelBase
    {
        #region Fields
        LibraryViewModel LibVM => LibraryViewService.Instance.LibVM;
        MacalifaPlayer player => MacalifaPlayerService.Instance.Player;
        private IconElement _playPauseIcon = new SymbolIcon(Symbol.Play);
        private string s = "0 %";
        private double _volume = 100;
        DispatcherTimer timer;
        double pos = 0;
        CoreDispatcher Dispatcher;
        #endregion

        #region Commands

        #region Definition
        RelayCommand _openSongCommand;
        /// <summary>
        /// Gets OpenSong command. This calls the <see cref="Open(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand OpenSongCommand
        {
            get
            { if (_openSongCommand == null) { _openSongCommand = new RelayCommand(param => this.Open(param)); } return _openSongCommand; }
        }
        public DelegateCommand LoadCommand { get; private set; }
        public DelegateCommand PlayPauseCommand { get; private set; }
        public DelegateCommand PlayNextCommand { get; private set; }
        public DelegateCommand PlayPreviousCommand { get; private set; }
        #endregion

        #region Implementation
        private async void PlayPause()
        {
            switch (player.PlayerState)
            {
                case PlayerState.Playing:
                    await player.Pause();
                    timer.Stop();
                    PlayPauseIcon = new SymbolIcon(Symbol.Play);
                    break;
                case PlayerState.Paused:
                case PlayerState.Ended:
                case PlayerState.Stopped:
                    await player.Play();
                    timer.Start();
                    PlayPauseIcon = new SymbolIcon(Symbol.Pause);
                    DontUpdatePosition = false;
                    break;
            }
        }
        void PlayNext()
        {
            Mediafile toPlayFile = null;

            if (Shuffle)
            {
                if (ShuffledList == null)
                    ShuffledList = ShuffledCollection();
                var playlingFile = LibVM.TracksCollection.Elements.Single(t => t.State == PlayerState.Playing);
                var index = LibVM.TracksCollection.Elements.IndexOf(playlingFile) + 1;
                toPlayFile = ShuffledList.ElementAt(index);
            }
            else
            {
                LibVM.FileListBox.SelectedIndex = LibVM.FileListBox.SelectedIndex <= LibVM.FileListBox.Items.Count - 2 ? LibVM.FileListBox.SelectedIndex + 1 : 0;
                toPlayFile = LibVM.FileListBox.SelectedItem as Mediafile;
            }
            PlayFile(toPlayFile);

        }
        void PlayPrevious()
        {
        }
        async void Open(object para)
        {
            var picker = new FileOpenPicker();
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
            openPicker.FileTypeFilter.Add(".mp3");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (player.PlayerState == PlayerState.Paused || player.PlayerState == PlayerState.Stopped)
                Load(file);
            else
            {
                Load(file, true);
            }

        }
        #endregion
        #endregion

        #region Events
        private async void Player_MediaEnded(object sender, Events.MediaEndedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                DontUpdatePosition = true;
                CurrentPosition = 0;
                player.PlayerState = PlayerState.Stopped;
                if (!Repeat)
                {
                    await player.Pause();
                    timer.Stop();
                    PlayPauseIcon = new SymbolIcon(Symbol.Play);
                    DontUpdatePosition = false;
                }
                else
                {
                    timer.Start();
                    DontUpdatePosition = false;
                    PlayPauseCommand.Execute(null);
                }

            });
        }

        private void Timer_Tick(object sender, object e)
        {
            if (this.player != null)
            {
                pos = player.Position;
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
            set { Set(ref _shuffle, value); }
        }
        public ObservableCollection<SimpleNavMenuItem> TopMenuItems { get; }
        public ObservableCollection<SimpleNavMenuItem> BottomMenuItems { get; }
        public ObservableCollection<SimpleNavMenuItem> PlaylistsItems { get; }
        public Type InitialPage { get; }
        public bool DontUpdatePosition { get; set; }
        double _currentPosition;
        public double CurrentPosition
        {
            get { return this._currentPosition; }
            set
            {
                Set(ref _currentPosition, value);
                if (DontUpdatePosition)
                    player.Position = _currentPosition;
            }
        }
        Macalifa.Core.Tags _tags;
        public Macalifa.Core.Tags Tags
        {
            get
            {
                return _tags;
            }
            set
            {
                Set(ref _tags, value);
            }
        }
        public double Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                Set(ref _volume, value);
                player.Volume = _volume;
            }
        }
        double _length = 0;
        public double Length
        {
            get { return _length; }
            set { Set(ref _length, value); }
        }
        public String S
        {
            get
            {
                return s;
            }
            set
            {
                Set(ref s, value);
            }
        }
        public IconElement PlayPauseIcon
        {
            get
            {
                return _playPauseIcon;
            }
            set
            {
                Set(ref _playPauseIcon, value);
            }
        }
        #endregion

        #region Methods
        async void PlayFile(Mediafile toPlayFile)
        {
            if (player.PlayerState == PlayerState.Paused || player.PlayerState == PlayerState.Stopped)
                Load(await StorageFile.GetFileFromPathAsync(toPlayFile.Path));
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
        private async void Load(object mp3file,  bool play = false, double currentPos = 0, double vol = 50)
        {          
            StorageFile file = mp3file as StorageFile;
            if (file != null && file.FileType == ".mp3")
            {
                await player.Load(file.Path);
                Length = player.Length;                
                PlayPauseCommand.IsEnabled = true;
                if (play)
                {
                    PlayPauseCommand.Execute(null);
                }
                else
                {
                    Volume = vol;
                    DontUpdatePosition = true;
                    CurrentPosition = currentPos;
                }
                Tags = player.Tags;
            }
            GC.Collect();
        }
        public void Play(object para, double currentPos = 0, bool play = true, double vol = 50)
        {
            Load(para, play, currentPos, vol);
        }
        #endregion

        #region Constructor
        public ShellViewModel()
        {
            Dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;            
            PlayPauseCommand = new DelegateCommand(PlayPause) { IsEnabled = false };
            PlayNextCommand = new DelegateCommand(PlayNext);
            PlayPreviousCommand = new DelegateCommand(PlayPrevious);
            TopMenuItems = new ObservableCollection<SimpleNavMenuItem>();
            BottomMenuItems = new ObservableCollection<SimpleNavMenuItem>();
            PlaylistsItems = new ObservableCollection<SimpleNavMenuItem>();
            InitialPage = typeof(LibraryView);
            player.PlayerState = PlayerState.Stopped;
            DontUpdatePosition = false;
            this.timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += Timer_Tick;
            this.timer.Stop();
            player.MediaEnded += Player_MediaEnded;
          
        }
        #endregion        

    }


}
