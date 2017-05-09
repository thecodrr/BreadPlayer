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
using BreadPlayer.Extensions;
using BreadPlayer.Helpers;
using BreadPlayer.Models;
using BreadPlayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace BreadPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shell : Page
    {
        public event EventHandler<KeyEventArgs> GlobalPageKeyDown;
        ShellViewModel ShellVM;
        List<Mediafile> OldFiles = new List<Mediafile>();
        public Shell()
        {
            this.InitializeComponent();
            //SurfaceLoader.Initialize(ElementCompositionPreview.GetElementVisual(this).Compositor);
            new CoreWindowLogic();
            ShellVM = DataContext as ShellViewModel;
            LibraryItem.Shortcuts.Add(new SplitViewMenu.Shortcut()
            {
                SymbolAsChar = "\uE762",
                Tooltip = "Enable Multiselection",
                ShortcutCommand = (App.Current.Resources["LibVM"] as LibraryViewModel).ChangeSelectionModeCommand,
            });
            NowPlayingItem.Command = ShellVM.NavigateToNowPlayingViewCommand;
          
        }
           
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            ConfigSaver saver = new ConfigSaver();
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("equalizerPresets.json", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, saver.SaveSettings());
            Window.Current.CoreWindow.KeyDown += (sender, args) =>
            GlobalPageKeyDown?.Invoke(sender, args);
            if (RoamingSettingsHelper.GetSetting<bool>("IsFirstTime", true))
            {
                string releaseNotes = "FIXES:\r\n\r\n" + 
                    "We fixed the startup crash.\n" +
                    "Fixed crash when changing theme.\n" +
                    "Fixed crash when play songs from an album.\n" +
                    "Fixed other various bugs and crashes.\r\n\r\n" + 
                    "NEW THINGS:\r\n\r\n" +
                    "Added 'Now Playing' Screen with artist and album info fetching.\r\n\r\n" +
                    "IMPROVEMENTS:\r\n\r\n" +
                    "Greatly improved performance and navigation.\n" +
                    "Improved Equalizer UI.\nImproved Blur\n" +
                    "Improved UI (less glitches etc.)\r\n";
                await SharedLogic.NotificationManager.ShowMessageBoxAsync(releaseNotes, "What's new in v2.3.0");
                RoamingSettingsHelper.SaveSetting("IsFirstTime", false);
            }
            if (e.Parameter is StorageFile)
                Messengers.Messenger.Instance.NotifyColleagues(Messengers.MessageTypes.MSG_EXECUTE_CMD, new List<object> { e.Parameter, 0.0, true, 50.0 });
            
            base.OnNavigatedTo(e);
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown -= (sender, args) => GlobalPageKeyDown?.Invoke(sender, args);
        }
        bool isPressed;
        bool isProgBarPressed = false;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            positionSlider.InitEvents(() => { positionSlider.UpdatePosition(positionProgressBar, ShellVM); }, () => { ShellVM.DontUpdatePosition = true; });
            Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;
            Window.Current.CoreWindow.PointerMoved += CoreWindow_PointerMoved;
            Window.Current.CoreWindow.PointerReleased += CoreWindow_PointerReleased;
        }

        private void CoreWindow_PointerReleased(CoreWindow sender, PointerEventArgs args)
        {
            if (isPressed && !positionSlider.IsDragging())
            {
                positionSlider.UpdatePosition(positionProgressBar, ShellVM, true);
                isPressed = false;
            }
            else if (isProgBarPressed)
            {
                positionProgressBar.ZoomAnimate((int)positionProgressBar.ActualHeight, (int)positionProgressBar.ActualHeight - 4, "Height");
                isProgBarPressed = false;
                positionSlider.UpdatePosition(positionProgressBar, ShellVM, true, true);
            }
        }

        private void CoreWindow_PointerMoved(CoreWindow sender, PointerEventArgs args)
        {
            if (isProgBarPressed)
            {
                double MousePosition = args.CurrentPoint.Position.X;
                double ratio = MousePosition / positionProgressBar.ActualWidth;
                double ProgressBarValue = ratio * positionProgressBar.Maximum;
                positionProgressBar.Value = ProgressBarValue;
            }
        }

        private void CoreWindow_PointerPressed(CoreWindow sender, PointerEventArgs args)
        {
            if (positionSlider.GetBoundingRect().Contains(args.CurrentPoint.Position) && !positionSlider.IsDragging())
            {
                isPressed = true;
                ShellVM.DontUpdatePosition = true;
            }
            if (seekRect.GetBoundingRect().Contains(args.CurrentPoint.Position))
            {
                positionProgressBar.ZoomAnimate((int)positionProgressBar.ActualHeight, (int)positionProgressBar.ActualHeight + 4, "Height");
                ShellVM.DontUpdatePosition = true;
                isProgBarPressed = true;
            }
        }           
    }
}
