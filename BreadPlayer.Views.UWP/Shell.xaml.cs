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
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using BreadPlayer.Common;
using BreadPlayer.Core;
using BreadPlayer.Core.Models;
using BreadPlayer.Extensions;
using BreadPlayer.Helpers;
using BreadPlayer.Messengers;
using BreadPlayer.ViewModels;
using SplitViewMenu;
using BreadPlayer.Core.Common;
using Windows.UI.ViewManagement;

namespace BreadPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shell : Page
    {
        public event EventHandler<KeyEventArgs> GlobalPageKeyDown;
        private ShellViewModel _shellVm;
        private List<Mediafile> _oldFiles = new List<Mediafile>();
        public Shell()
        {
            InitializeComponent();
            //SurfaceLoader.Initialize(ElementCompositionPreview.GetElementVisual(this).Compositor);
            new CoreWindowLogic();
            _shellVm = DataContext as ShellViewModel;
            LibraryItem.Shortcuts.Add(new Shortcut
            {
                SymbolAsChar = "\uE762",
                Tooltip = "Enable Multiselection",
                ShortcutCommand = (Application.Current.Resources["LibVM"] as LibraryViewModel).ChangeSelectionModeCommand
            });
            NowPlayingItem.Command = new DelegateCommand(() => 
            {
                _shellVm.IsPlaybarHidden = true;
                ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            });          
        }
        private void VisualStateGroup_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (NowPlayingFrame.CurrentSourcePageType != typeof(NowPlayingView))
                NowPlayingFrame.Navigate(typeof(NowPlayingView));
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown += (sender, args) =>
            GlobalPageKeyDown?.Invoke(sender, args);
            if (RoamingSettingsHelper.GetSetting<bool>("IsFirstTime", true))
            {
                string releaseNotes = "FIXES:\r\n\r\n" + 
                    "Fix play on tap for all touch devices.\n"+
                    "Fixed all crashes.\r\n\r\n" + 
                    "NEW THINGS:\r\n\r\n" +
                    "Removed enter to full screen on startup.\n" + 
                    "Removed back button from shortcuts.\n";
                await SharedLogic.NotificationManager.ShowMessageBoxAsync(releaseNotes, "What's new in v2.4.0");
                RoamingSettingsHelper.SaveSetting("IsFirstTime", false);
            }
            if (e.Parameter is StorageFile)
            {
                Messenger.Instance.NotifyColleagues(MessageTypes.MsgExecuteCmd, new List<object> { e.Parameter, 0.0, true, 50.0 });
            }

            base.OnNavigatedTo(e);
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown -= (sender, args) => GlobalPageKeyDown?.Invoke(sender, args);
        }

        private bool _isPressed;
        private bool _isProgBarPressed;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            positionSlider.InitEvents(() => { positionSlider.UpdatePosition(positionProgressBar, _shellVm); }, () => { _shellVm.DontUpdatePosition = true; });
            Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;
            Window.Current.CoreWindow.PointerMoved += CoreWindow_PointerMoved;
            Window.Current.CoreWindow.PointerReleased += CoreWindow_PointerReleased;
        }

        private void CoreWindow_PointerReleased(CoreWindow sender, PointerEventArgs args)
        {
            if (_isPressed && !positionSlider.IsDragging())
            {
                positionSlider.UpdatePosition(positionProgressBar, _shellVm, true);
                _isPressed = false;
            }
            else if (_isProgBarPressed)
            {
                positionProgressBar.ZoomAnimate((int)positionProgressBar.ActualHeight, (int)positionProgressBar.ActualHeight - 4, "Height");
                _isProgBarPressed = false;
                positionSlider.UpdatePosition(positionProgressBar, _shellVm, true, true);
            }
        }

        private void CoreWindow_PointerMoved(CoreWindow sender, PointerEventArgs args)
        {
            if (_isProgBarPressed)
            {
                double mousePosition = args.CurrentPoint.Position.X;
                double ratio = mousePosition / positionProgressBar.ActualWidth;
                double progressBarValue = ratio * positionProgressBar.Maximum;
                positionProgressBar.Value = progressBarValue;
            }
        }

        private void CoreWindow_PointerPressed(CoreWindow sender, PointerEventArgs args)
        {
            if (positionSlider.GetBoundingRect().Contains(args.CurrentPoint.Position) && !positionSlider.IsDragging())
            {
                _isPressed = true;
                _shellVm.DontUpdatePosition = true;
            }
            if (seekRect.GetBoundingRect().Contains(args.CurrentPoint.Position))
            {
                positionProgressBar.ZoomAnimate((int)positionProgressBar.ActualHeight, (int)positionProgressBar.ActualHeight + 4, "Height");
                _shellVm.DontUpdatePosition = true;
                _isProgBarPressed = true;
            }
        }

      
    }
}
