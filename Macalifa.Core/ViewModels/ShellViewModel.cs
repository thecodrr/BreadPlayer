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
        RelayCommand _openSongCommand;
        #endregion
        /// <summary>
        /// Gets Play command. This calls the <see cref="Play(object)"/> method. <seealso cref="ICommand"/>
        /// </summary>
        public ICommand OpenSongCommand
        {
            get
            { if (_openSongCommand == null) { _openSongCommand = new RelayCommand(param => this.Open(param)); } return _openSongCommand; }
        }
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
            Repeat = false;
            player.MediaEnded += Player_MediaEnded;
          
        }
        ThreadSafeObservableCollection<int> ShuffledList;
        void PlayNext()
        {
            
            //LibVM.FileListBox.SelectedIndex = LibVM.FileListBox.SelectedIndex <= LibVM.FileListBox.Items.Count - 2? LibVM.FileListBox.SelectedIndex + 1 : 0 ;
            //LibVM.PlayCommand.Execute(LibVM.FileListBox.SelectedItem);
        }
        async void PlayPrevious()
        {
            ShuffledList = ShuffledCollection();
            var playlingFile = LibVM.TracksCollection.Elements.Single(t => t.State == PlayerState.Playing);
            var index = ShuffledList[LibVM.TracksCollection.Elements.IndexOf(playlingFile)];//ShuffledList.IndexOf(playlingFile) + 1;
            var toPlayFile = LibVM.TracksCollection.Elements.ElementAt(index) as Mediafile;           
            if (player.PlayerState == PlayerState.Paused || player.PlayerState == PlayerState.Stopped)
                Load(await StorageFile.GetFileFromPathAsync(toPlayFile.Path));
            else
            {
                LibVM.PlayCommand.Execute(toPlayFile);
            }
            //toPlayFile.State = PlayerState.Playing;
        }
        public ThreadSafeObservableCollection<int> ShuffledCollection()
        {
            var numbers = new ThreadSafeObservableCollection<int>(Enumerable.Range(0, LibVM.TracksCollection.Count));
            numbers.Shuffle();
            return numbers;
        }
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
        #endregion


        bool _repeat;
        public bool Repeat
        {
            get { return _repeat; }
            set { Set(ref _repeat, value, "Repeat");}
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
                if(DontUpdatePosition)
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

        //private void PlayerOnPlaybackStopped(object sender, StoppedEventArgs stoppedEventArgs)
        //{
        //    LoadCommand.IsEnabled = true;
        //    StopCommand.IsEnabled = false;
        //    PlayPauseCommand.IsEnabled = false;
        //    if (reader != null)
        //    {
        //        reader.Position = 0;
        //    }
        //}
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
        private async void Load(object mp3file, bool play = false)
        {
            var mp3 = LibVM.TracksCollection.Elements.SingleOrDefault(t => t.State == PlayerState.Playing);
            if (mp3 != null) mp3.State = PlayerState.Stopped;
            StorageFile file = mp3file  as StorageFile;
            if (file != null)
            {
                await player.Load(file.Path);
                Length = player.Length;
                CurrentPosition = 0;
                Tags = player.Tags;
                if (play)
                {
                    PlayPauseCommand.IsEnabled = true;
                    PlayPauseCommand.Execute(null);
                }
            }
            GC.Collect();
        }

        public DelegateCommand LoadCommand { get; private set; }
        public DelegateCommand PlayPauseCommand { get; private set; }
        public DelegateCommand PlayNextCommand { get; private set; }
        public DelegateCommand PlayPreviousCommand { get; private set; }

    }


}
