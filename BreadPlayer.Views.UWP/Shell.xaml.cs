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
using BreadPlayer.Core.Models;
using BreadPlayer.Dialogs;
using BreadPlayer.Extensions;
using BreadPlayer.Helpers;
using BreadPlayer.Messengers;
using BreadPlayer.Services;
using BreadPlayer.ViewModels;
using BreadPlayer.Controls;
using System;
using System.Collections.Generic;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace BreadPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shell : Page
    {
        private bool _isPressed;

        private List<Mediafile> _oldFiles = new List<Mediafile>();

        private ShellViewModel _shellVm;

        private string _arguments = null;
        public Shell()
        {
            InitializeComponent();
            new CoreWindowLogic();
            _shellVm = DataContext as ShellViewModel;
            SetupMenu();
            if (SharedLogic.Instance.SettingsVm.PersonalizationVM.BackgroundOverlayColor == "Auto")
            {
                backgroundBorder.Background = Application.Current.Resources["BackgroundOverlay"] as SolidColorBrush;
                backgroundBorder.Opacity = 0.6;
            }
            else
            {
                backgroundBorder.Background = Application.Current.Resources["PlaybarBrush"] as SolidColorBrush;
                backgroundBorder.Opacity = 0.8;
            }
        }
        private void SetupMenu()
        {
            Messenger.Instance.Register(MessageTypes.MsgNavigate, new Action<Message>(HandleNavigationMessage));
            LibraryItem.Shortcuts.Add(new Shortcut
            {
                SymbolAsChar = "\uE762",
                Tooltip = "Enable Multiselection",
                ShortcutCommand = (Application.Current.Resources["LibVM"] as LibraryViewModel).ChangeSelectionModeCommand
            });
            NowPlayingItem.Command = _shellVm.NavigateToNowPlayingViewCommand;
            watchAdMenuItem.Command = new DelegateCommand(async () => 
            {
                DonateDialog dialog = new DonateDialog();
                await dialog.ShowAsync();
            });
        }
        private async void HandleNavigationMessage(Message message)
        {
            if (message.Payload != null)
            {
                dynamic payload = message.Payload;
                if (!InitializeSwitch.IsMobile)
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        NowPlayingFrame.Width = payload.parameter is string ? 700 : 900;
                    });
                }
                if (NowPlayingFrame.CurrentSourcePageType != payload.pageType)
                {
                    NowPlayingFrame.Navigate(payload.pageType, payload.parameter, new Windows.UI.Xaml.Media.Animation.SlideNavigationTransitionInfo());
                }
                else if(payload.pageType == typeof(PlaylistView))
                {
                    NowPlayingFrame.Navigate(payload.pageType, payload.parameter, new Windows.UI.Xaml.Media.Animation.SlideNavigationTransitionInfo());
                }
                if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                {
                    Windows.Phone.UI.Input.HardwareButtons.BackPressed += BackButtonPressed;
                }
                _shellVm.IsPlaybarHidden = true;
            }
        }
        private void BackButtonPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true;
            if (NowPlayingFrame.BackStackDepth <= 1)
            {
                NowPlayingFrame.BackStack.Clear();
                Windows.Phone.UI.Input.HardwareButtons.BackPressed -= BackButtonPressed;
                NavigationService.Instance.RegisterEvents();
                _shellVm.IsPlaybarHidden = false;
            }
            else
            {
                NowPlayingFrame.GoBack(new Windows.UI.Xaml.Media.Animation.SlideNavigationTransitionInfo());
            }
        }
        private void HamburgerMenu_SplitViewMenuLoaded(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_arguments))
                CoreWindowLogic.LoadAppWithArguments(_arguments);
        }       

        public event EventHandler<KeyEventArgs> GlobalPageKeyDown;
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown += (sender, args) =>
            GlobalPageKeyDown?.Invoke(sender, args);
            if (SettingsHelper.GetLocalSetting<bool>("IsFirstTime", true))
            {
                await WhatsNewDialogHelper.ShowWhatsNewDialogAsync();
                SettingsHelper.SaveLocalSetting("IsFirstTime", false);
            }
            if (e.Parameter is IReadOnlyList<IStorageItem> files)
            {
                Messenger.Instance.NotifyColleagues(MessageTypes.MsgExecuteCmd, new List<object> { files, 0.0, true, 50.0 });
            }
            else if(e.Parameter is string arguments && !string.IsNullOrEmpty(e.Parameter.ToString()))
            {
                hamburgerMenu.SplitViewMenuLoaded += HamburgerMenu_SplitViewMenuLoaded;
                _arguments = arguments;
            }
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown -= (sender, args) => GlobalPageKeyDown?.Invoke(sender, args);
        }
        private void CoreWindow_PointerPressed(CoreWindow sender, PointerEventArgs args)
        {
            if (positionSlider.GetBoundingRect().Contains(args.CurrentPoint.Position) && !positionSlider.IsDragging() && SharedLogic.Instance.Player.IsSeekable)
            {
                _isPressed = true;
                _shellVm.DontUpdatePosition = true;
            }
        }

        private void CoreWindow_PointerReleased(CoreWindow sender, PointerEventArgs args)
        {
            if (_isPressed && !positionSlider.IsDragging() && SharedLogic.Instance.Player.IsSeekable)
            {
                positionSlider.UpdatePosition(_shellVm, true);
                _isPressed = false;
            }
        }
        
        private void OnNowPlayingHide(CoreWindow sender, PointerEventArgs args)
        {
            if (!NowPlayingFrame.GetBoundingRect().Contains(args.CurrentPoint.Position))
                HideNowPlaying();
        }
        private void HideEquilizer()
        {
            if (_shellVm.IsEqualizerVisible)
            {
                _shellVm.IsEqualizerVisible = false;
                equalizerBtn.IsChecked = false;
            }
        }
        private void OnEqualizerHide(object sender, RoutedEventArgs e)
        {
            HideEquilizer();
        }
        private void HideNowPlaying()
        {
            if (_shellVm.IsPlaybarHidden)
            {
                _shellVm.IsPlaybarHidden = false;
                CoreWindow.GetForCurrentThread().PointerReleased -= OnNowPlayingHide;
            }
        }
        private void OnKeyUp(CoreWindow sender, KeyEventArgs args)
        {
            if (args.VirtualKey == Windows.System.VirtualKey.Escape)
            {
                HideNowPlaying();
                HideEquilizer();
            }
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            positionSlider.InitEvents(() => { positionSlider.UpdatePosition(_shellVm); }, () => { _shellVm.DontUpdatePosition = true; });
            Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;
            Window.Current.CoreWindow.PointerReleased += CoreWindow_PointerReleased;
            CoreWindow.GetForCurrentThread().KeyUp += OnKeyUp;
            NowPlayingGrid.RegisterPropertyChangedCallback(OpacityProperty, (d, obj) =>
            {
                if (NowPlayingGrid.Opacity == 1)
                {
                    CoreWindow.GetForCurrentThread().PointerReleased += OnNowPlayingHide;                   
                }
            });
        }
    }
}