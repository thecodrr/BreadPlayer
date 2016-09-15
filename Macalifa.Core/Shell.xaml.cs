using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Macalifa.Core;
using Macalifa.Services;
using Macalifa.Common;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using Windows.Media;
using Macalifa.ViewModels;
using Windows.Storage;
using Macalifa.Models;
using Macalifa.Extensions;
using System.Threading.Tasks;

namespace Macalifa
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shell : Page
    {
        static MacalifaPlayer Player => MacalifaPlayerService.Instance.Player;
        static ShellViewModel ShellVM => ShellViewService.Instance.ShellVM;

        LibraryViewService LibVM => LibraryViewService.Instance;
        List<Mediafile> OldFiles = new List<Mediafile>();
        SystemMediaTransportControls _smtc;
        public Shell()
        {
            this.InitializeComponent();

            // ShellViewModel vm = new ShellViewModel(Dispatcher);
            ShellVM.TopMenuItems.Add(new SplitViewMenu.SimpleNavMenuItem
            {
                Label = "Library",
                DestinationPage = typeof(LibraryView),
                Symbol = Symbol.Library
            });
            ShellVM.TopMenuItems.Add(new SplitViewMenu.SimpleNavMenuItem
            {
                Label = "Albums",
                DestinationPage = typeof(Albums),
                Symbol = Symbol.Mute
            });
            ShellVM.BottomMenuItems.Add(new SplitViewMenu.SimpleNavMenuItem
            {
                Label = "Settings",
                DestinationPage = typeof(LibraryView),
                Symbol = Symbol.Bookmarks
            });
            ShellVM.BottomMenuItems.Add(new SplitViewMenu.SimpleNavMenuItem
            {
                Label = "Albums",
                DestinationPage = typeof(Albums),
                Symbol = Symbol.Emoji
            });
            ShellVM.PlaylistsItems.Add(new SplitViewMenu.SimpleNavMenuItem
            {
                Arguments = "Hello",
                Label = "Hello",
                DestinationPage = typeof(Albums),
                Symbol = Symbol.List
            });
            this.DataContext = ShellVM;

            _smtc = SystemMediaTransportControls.GetForCurrentView();
            _smtc.IsEnabled = true;
            _smtc.IsPlayEnabled = true;
            _smtc.IsPauseEnabled = true;
            _smtc.ButtonPressed += _smtc_ButtonPressed;

        }


        public async static void Play(object para, double currentPos = 0, bool play = true, double vol = 50)
        {
            if (para is StorageFile)
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync((para as StorageFile).Path);
                if (file != null && file.FileType == ".mp3")
                {
                    await Player.Load(file.Path);
                    ShellVM.Length = Player.Length;
                    ShellVM.PlayPauseCommand.IsEnabled = true;
                    if (play)
                    {
                        ShellVM.PlayPauseCommand.Execute(null);
                    }
                    else
                    {
                        ShellVM.Volume = vol;
                        ShellVM.DontUpdatePosition = true;
                        ShellVM.CurrentPosition = currentPos;
                    }
                    ShellVM.Tags = Player.Tags;
                }

            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            Play(e.Parameter);
            base.OnNavigatedTo(e);
        }
        private void Player_MediaStateChanged(object sender, Events.MediaStateChangedEventArgs e)
        {
            if (_smtc != null)
                switch (e.NewState)
                {
                    case PlayerState.Playing:
                        _smtc.PlaybackStatus = MediaPlaybackStatus.Playing;
                        break;
                    case PlayerState.Paused:
                        _smtc.PlaybackStatus = MediaPlaybackStatus.Paused;
                        break;
                    case PlayerState.Stopped:
                        _smtc.PlaybackStatus = MediaPlaybackStatus.Stopped;
                        break;
                    default:
                        break;
                }
        }

        private async void _smtc_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                    {
                        await Player.Play();
                    });
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                    {
                        await Player.Pause();
                    });
                    break;
                default:
                    break;
            }
        }

        private void VolSliderThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            var vm = this.DataContext as ShellViewModel;
            vm.DontUpdatePosition = true;
        }

        private void VolSliderThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            var vm = this.DataContext as ShellViewModel;
            if (vm != null)
            {

                vm.CurrentPosition = positionSlider.Value;
                vm.DontUpdatePosition = false;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Thumb volSliderThumb = FrameworkElementExtensions.FindChildOfType<Thumb>(positionSlider);
            volSliderThumb.DragCompleted += VolSliderThumb_DragCompleted;
            volSliderThumb.DragStarted += VolSliderThumb_DragStarted;

            Player.MediaStateChanged += Player_MediaStateChanged;
            
          
        }

        private async void positionSlider_Tapped(object sender, TappedRoutedEventArgs e)
        {         
            var vm = this.DataContext as ShellViewModel;
            vm.DontUpdatePosition = true;
            if (vm != null)
            {
                vm.CurrentPosition = positionSlider.Value;
            }
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(2));
            vm.DontUpdatePosition = false;

        }
        async void ShowMessage(string msg)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(msg);
            await dialog.ShowAsync();
        }

        private async void He_KeyUp(object sender, KeyRoutedEventArgs e)
        {
          
                await Task.Run(async () =>
{
    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
    {
        var numbers = LibVM.LibVM.TracksCollection.Elements;
        numbers.Shuffle();
        string ss = "";
        foreach (var s in numbers)
        {
            ss += s.Title + "\r\n";
        }
        ShowMessage(ss);

        if (He.Text.Length > 0)
        {
           
            var list = LibVM.LibVM.OldItems.Where(w => w.Title.ToUpper().Contains(He.Text.ToUpper())).ToList();
            var col = new GroupedObservableCollection<string, Mediafile>(t => t.Title.Remove(1), list, a => a.Title.Remove(1));
            LibVM.LibVM.TracksCollection = null;
            LibVM.LibVM.TracksCollection = col;
            
        }     
        else
        {

            var col = new GroupedObservableCollection<string, Mediafile>(t => t.Title.Remove(1), LibVM.LibVM.OldItems, a => a.Title.Remove(1));
            LibVM.LibVM.TracksCollection = null;
            LibVM.LibVM.TracksCollection = col;

            GC.Collect();
        }
    });
});
           
        }
    }
}