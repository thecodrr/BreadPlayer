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
using BreadPlayer.Core.Models;
using BreadPlayer.Extensions;
using BreadPlayer.Helpers;
using BreadPlayer.Messengers;
using BreadPlayer.ViewModels;
using SplitViewMenu;
using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

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
            NowPlayingItem.Command = _shellVm.NavigateToNowPlayingViewCommand;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown += (sender, args) =>
            GlobalPageKeyDown?.Invoke(sender, args);
            if (SettingsHelper.GetLocalSetting<bool>("IsFirstTime", true))
            {
                string releaseNotes = "𝐖𝐡𝐚𝐭'𝐬 𝐅𝐢𝐱𝐞𝐝:\n\n" +
                    "• Fixed issue where library import took too much time.\n" +
                    "• Fixed issue where many album arts were not loaded.\n" +
                    "• Fixed other bugs.\n\n" +
                    "𝐖𝐡𝐚𝐭'𝐬 𝐍𝐞𝐰:\n\n" +
                    "• Added ability to ignore DRM-Protected songs. (𝑒𝑥𝑝𝑟𝑖𝑚𝑒𝑛𝑡𝑎𝑙)\n" +
                    "• Added sorting by tracknumber for album songs.\n";
                await SharedLogic.NotificationManager.ShowMessageBoxAsync(releaseNotes, "What's new in v2.6.2");
                SettingsHelper.SaveLocalSetting("IsFirstTime", false);
            }
            if (e.Parameter is IReadOnlyList<IStorageItem> files)
            {
                Messenger.Instance.NotifyColleagues(MessageTypes.MsgExecuteCmd, new List<object> { files, 0.0, true, 50.0 });
            }

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown -= (sender, args) => GlobalPageKeyDown?.Invoke(sender, args);
        }

        private bool _isPressed;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            positionSlider.InitEvents(() => { positionSlider.UpdatePosition(_shellVm); }, () => { _shellVm.DontUpdatePosition = true; });
            Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;
            Window.Current.CoreWindow.PointerReleased += CoreWindow_PointerReleased;
            NowPlayingFrame.RegisterPropertyChangedCallback(VisibilityProperty, (d, obj) =>
            {
                if (NowPlayingFrame.Visibility == Visibility.Visible)
                {
                    CoreWindow.GetForCurrentThread().PointerReleased += OnNowPlayingHide;
                }
            });
            equalizerOverlayGrid.RegisterPropertyChangedCallback(VisibilityProperty, (d, obj) =>
            {
                if (equalizerOverlayGrid.Visibility == Visibility.Visible)
                {
                    CoreWindow.GetForCurrentThread().PointerReleased += OnEqualizerHide;
                }
            });

        }

        private void OnEqualizerHide(CoreWindow sender, PointerEventArgs args)
        {
            if (_shellVm.IsEqualizerVisible && equalizerOverlayGrid.GetBoundingRect().Contains(args.CurrentPoint.Position) && !equalizerGrid.GetBoundingRect().Contains(args.CurrentPoint.Position))
            {
                _shellVm.IsEqualizerVisible = false;
                equalizerBtn.IsChecked = false;
                CoreWindow.GetForCurrentThread().PointerReleased -= OnEqualizerHide;
            }
        }

        private void OnNowPlayingHide(CoreWindow sender, PointerEventArgs args)
        {
            if (_shellVm.IsPlaybarHidden && !NowPlayingFrame.GetBoundingRect().Contains(args.CurrentPoint.Position))
            {
                _shellVm.IsPlaybarHidden = false;
                CoreWindow.GetForCurrentThread().PointerReleased -= OnNowPlayingHide;
            }
        }

        private void CoreWindow_PointerReleased(CoreWindow sender, PointerEventArgs args)
        {
            if (_isPressed && !positionSlider.IsDragging())
            {
                positionSlider.UpdatePosition(_shellVm, true);
                _isPressed = false;
            }
        }

        private void CoreWindow_PointerPressed(CoreWindow sender, PointerEventArgs args)
        {
            if (positionSlider.GetBoundingRect().Contains(args.CurrentPoint.Position) && !positionSlider.IsDragging())
            {
                _isPressed = true;
                _shellVm.DontUpdatePosition = true;
            }
        }
    }
}