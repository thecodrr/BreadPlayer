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

using BreadPlayer;
using BreadPlayer.Core.Common;
using BreadPlayer.Core.Models;
using BreadPlayer.Models;
using BreadPlayer.Services;
using BreadPlayer.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace BreadPlayer.Controls
{
    public sealed class SplitViewMenu : Control
    {
        public event EventHandler<EventArgs> SplitViewMenuLoaded;
        public static NavigationService NavService { get; set; }

        internal static readonly DependencyProperty MenuItemDataTemplateSelectorProperty =
            DependencyProperty.Register("MenuItemDataTemplateSelector", typeof(DataTemplateSelector),
                typeof(SplitViewMenu), new PropertyMetadata(null));

        internal static readonly DependencyProperty NavMenuItemTemplateProperty =
            DependencyProperty.Register("NavMenuItemTemplate", typeof(DataTemplate), typeof(SplitViewMenu),
                new PropertyMetadata(null));

        internal static readonly DependencyProperty NavMenuItemContainerStyleProperty =
            DependencyProperty.Register("NavMenuItemContainerStyle", typeof(Style), typeof(SplitViewMenu),
                new PropertyMetadata(null));

        internal static readonly DependencyProperty InitialPageProperty =
            DependencyProperty.Register("InitialPage", typeof(Type), typeof(SplitViewMenu),
                new PropertyMetadata(null));

        internal static readonly DependencyProperty TopNavigationItemsProperty =
            DependencyProperty.Register("TopNavigationItems", typeof(List<INavigationMenuItem>),
                typeof(SplitViewMenu),
                new PropertyMetadata(new List<INavigationMenuItem>(), OnTopNavigationItemsPropertyChanged));

        internal static readonly DependencyProperty BottomNavigationItemsProperty =
           DependencyProperty.Register("BottomNavigationItems", typeof(List<INavigationMenuItem>),
               typeof(SplitViewMenu),
               new PropertyMetadata(new List<INavigationMenuItem>(), OnBottomNavigationItemsPropertyChanged));

        internal static readonly DependencyProperty PlaylistsItemsProperty =
          DependencyProperty.Register("PlaylistsItems", typeof(List<INavigationMenuItem>),
              typeof(SplitViewMenu),
              new PropertyMetadata(new List<INavigationMenuItem>(), OnPlaylistsItemsPropertyChanged));

        private Button _backButton;
        private static NavMenuListView _navTopMenuListView;
        private static NavMenuListView _navBottomMenuListView;

        //private static NavMenuListView _playlistsMenuListView;
        private static bool _focusPageOnLoad = true;

        private static Frame _pageFrame;
        private static SplitView _splitView;
        private static ToggleButton _togglePaneButton;
        private static AutoSuggestBox _searchBox;
        private static TextBlock _headerText;
        private static ItemsControl _shortcuts;

        public SplitViewMenu()
        {
            DefaultStyleKey = typeof(SplitViewMenu);
            Loaded += OnSplitViewMenuLoaded;
        }

        public DataTemplateSelector MenuItemDataTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(MenuItemDataTemplateSelectorProperty);
            set => SetValue(MenuItemDataTemplateSelectorProperty, value);
        }

        public DataTemplate NavMenuItemTemplate
        {
            get => (DataTemplate)GetValue(NavMenuItemTemplateProperty);
            set => SetValue(NavMenuItemTemplateProperty, value);
        }

        public Style NavMenuItemContainerStyle
        {
            get => (Style)GetValue(NavMenuItemContainerStyleProperty);
            set => SetValue(NavMenuItemContainerStyleProperty, value);
        }

        public Type InitialPage
        {
            get => (Type)GetValue(InitialPageProperty);
            set => SetValue(InitialPageProperty, value);
        }

        public static bool IsSearchBarVisible { get; set; }

        public List<INavigationMenuItem> TopNavigationItems
        {
            get => (List<INavigationMenuItem>)GetValue(TopNavigationItemsProperty);
            set => SetValue(TopNavigationItemsProperty, value);
        }

        public List<INavigationMenuItem> BottomNavigationItems
        {
            get => (List<INavigationMenuItem>)GetValue(BottomNavigationItemsProperty);
            set => SetValue(BottomNavigationItemsProperty, value);
        }

        public List<INavigationMenuItem> PlaylistsItems
        {
            get => (List<INavigationMenuItem>)GetValue(PlaylistsItemsProperty);
            set => SetValue(PlaylistsItemsProperty, value);
        }

        private void OnSplitViewMenuLoaded(object sender, RoutedEventArgs e)
        {
            if (InitialPage == null || _pageFrame == null)
            {
                return;
            }
            _pageFrame.Navigate(InitialPage);
        }

        private static void OnTopNavigationItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (_navTopMenuListView != null)
            {
                _navTopMenuListView.ItemsSource = e.NewValue;
            }
        }

        private static void OnBottomNavigationItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (_navBottomMenuListView != null)
            {
                _navBottomMenuListView.ItemsSource = e.NewValue;
            }
        }

        private static void OnPlaylistsItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //if (_playlistsMenuListView != null)
            //{
            //    _playlistsMenuListView.ItemsSource = e.NewValue;
            //}
        }

        protected async override void OnApplyTemplate()
        {
            _splitView = GetTemplateChild("RootSplitView") as SplitView;

            _pageFrame = GetTemplateChild("PageFrame") as Frame;
            NavService = new NavigationService(ref _pageFrame, typeof(LibraryView));
            _searchBox = GetTemplateChild("searchBox") as AutoSuggestBox;
            _navTopMenuListView = GetTemplateChild("NavTopMenuList") as NavMenuListView;
            _navBottomMenuListView = GetTemplateChild("NavBottomMenuList") as NavMenuListView;
            //_playlistsMenuListView = GetTemplateChild("PlaylistsMenuList") as NavMenuListView;
            _backButton = GetTemplateChild("BackButton") as Button;
            _headerText = GetTemplateChild("headerText") as TextBlock;
            _togglePaneButton = GetTemplateChild("TogglePaneButton") as ToggleButton;
            _shortcuts = GetTemplateChild("Shortcuts") as ItemsControl;
            await UpdateHeaderAndShortCuts(GetItemFromList(InitialPage) as SimpleNavMenuItem);
           
            if (_navTopMenuListView != null)
            {
                _navTopMenuListView.ItemInvoked += OnNavMenuItemInvoked;
                _navTopMenuListView.ContainerContentChanging += OnContainerContextChanging;
                _navTopMenuListView.SelectionChanged += _navTopMenuListView_SelectionChanged;
            }
            if (_navBottomMenuListView != null)
            {
                _navBottomMenuListView.ItemInvoked += OnNavMenuItemInvoked;
                _navBottomMenuListView.ContainerContentChanging += OnContainerContextChanging;
                _navBottomMenuListView.SelectionChanged += _navBottomMenuListView_SelectionChanged;
            }
            //if (_playlistsMenuListView != null)
            //{
            //    _playlistsMenuListView.ItemInvoked += OnNavMenuItemInvoked;
            //    _playlistsMenuListView.ContainerContentChanging += OnContainerContextChanging;
            //    _playlistsMenuListView.SelectionChanged += _playlistsMenuListView_SelectionChanged; ;
            //}
            if (_searchBox != null)
            {
                _searchBox.KeyUp += OnSearchBoxKeyPressed;
                _searchBox.QuerySubmitted += OnQuerySubmitted;
            }
            if (_pageFrame != null)
            {
                _pageFrame.Navigating += OnNavigatingToPage;
                _pageFrame.Navigated += OnNavigatedToPage;
            }

            var item = GetItemFromList(InitialPage);
            GetParentListViewFromItem(item).SelectedItem = item;
            SplitViewMenuLoaded?.Invoke(this, new EventArgs());
        }

        private void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            _focusPageOnLoad = false;
            if (sender.Text.Any())
            {
                UnSelectAll();
                if(_pageFrame.CurrentSourcePageType != typeof(SearchResultsView))
                    _pageFrame.Navigate(typeof(SearchResultsView));
                Messengers.Messenger.Instance.NotifyColleagues(Messengers.MessageTypes.MsgSearch, sender.Text);
            }
        }

        private void OnSearchBoxKeyPressed(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                
            }
        }

        public static DelegateCommand SearchClickedCommand()
        {
            DelegateCommand cmd = new DelegateCommand(() =>
            {
                if (_searchBox.Visibility == Visibility.Collapsed)
                {
                    var searchClickedStoryboard = (_splitView.Resources["SearchButtonClickedStoryBoard"] as Storyboard);//.Begin();
                    searchClickedStoryboard.Begin();
                    _searchBox.Focus(FocusState.Programmatic);
                }
                else if (_searchBox.Visibility == Visibility.Visible)
                {
                    var fadeStoryboard = (_splitView.Resources["SearchButtonClickedFadeStoryboard"] as Storyboard);//.Begin();
                    fadeStoryboard.Begin();
                }
            });
            return cmd;
        }

        private static INavigationMenuItem _lastItem = new SimpleNavMenuItem();
        //private void _playlistsMenuListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (_navTopMenuListView.SelectedIndex > -1 || _navBottomMenuListView.SelectedIndex > -1)
        //    {
        //        _navTopMenuListView.SelectedIndex = -1;
        //        _navBottomMenuListView.SelectedIndex = -1;
        //        _lastItem = new SimpleNavMenuItem();
        //    }
        //    else
        //    {
        //        if (e.RemovedItems.Count > 0 && _navTopMenuListView.SelectedIndex == -1)
        //        {
        //            _lastItem = e.RemovedItems[0] as INavigationMenuItem;
        //        }
        //    }
        //}

        private void _navBottomMenuListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _navTopMenuListView.SelectedIndex = -1;
        }

        private void _navTopMenuListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _navBottomMenuListView.SelectedIndex = -1;
        }
        
        private static void OnContainerContextChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (!args.InRecycleQueue && args.Item is INavigationMenuItem navigationMenu)
            {
                args.ItemContainer.SetValue(AutomationProperties.NameProperty, navigationMenu.Label);
            }
            else
            {
                args.ItemContainer.ClearValue(AutomationProperties.NameProperty);
            }
        }

        private async void OnNavigatedToPage(object sender, NavigationEventArgs e)
        {
            if (e.Content is Page page && e.Content != null)
            {
                var control = page;
                control.Loaded += PageLoaded;
            }
            if (e.Parameter?.ToString() == "Home")
            {
                _navTopMenuListView.SelectedIndex = 0;
                await UpdateHeaderAndShortCuts(_navTopMenuListView.Items[0] as SimpleNavMenuItem);
            }
            else if (e.Parameter is Album || e.Parameter is Artist || e.Parameter is Playlist)
            {
                await UpdateHeaderAndShortCuts(new SimpleNavMenuItem { HeaderVisibility = Visibility.Collapsed, ShortcutTheme = ElementTheme.Dark });
            }
            else if (e.Parameter is SettingGroup settingGroup)
            {
                await UpdateHeaderAndShortCuts(new SimpleNavMenuItem { Label = "Settings 🡒 " + settingGroup.Title });
            }
            else if (e.Parameter is Query query)
            {
                await UpdateHeaderAndShortCuts(new SimpleNavMenuItem { Label = "Search results for \"" + query.QueryWord + "\"" });
            }
            else if (e.Parameter is ValueTuple<Query, string> parameter)
            {
                await UpdateHeaderAndShortCuts(new SimpleNavMenuItem { Label = $"{parameter.Item2} for \"" + parameter.Item1.QueryWord + "\"" });
            }
        }

        public static void UnSelectAll()
        {
            _lastItem = null;
            if (_navBottomMenuListView != null)
                _navBottomMenuListView.SelectedIndex = -1;
            if (_navTopMenuListView != null)
                _navTopMenuListView.SelectedIndex = -1;
            //_playlistsMenuListView.SelectedIndex = -1;
        }

        public static void SelectHome()
        {
            _navTopMenuListView.SelectedIndex = 3;
        }

        public static void SelectPrevious()
        {
            if (_lastItem?.Label != null)
            {
                var listView = GetParentListViewFromItem(_lastItem);
                var item = listView.Items.First(t => (t as INavigationMenuItem).Label == _lastItem.Label);
                var index = listView.IndexFromContainer((ListViewItem)listView.ContainerFromItem(item));
                listView.SetValue(ListView.SelectedIndexProperty, index);
            }
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            if (_focusPageOnLoad)
                ((Page)sender).Focus(FocusState.Programmatic);
            _focusPageOnLoad = true;
            ((Page)sender).Loaded -= PageLoaded;
        }

        public static object GetParameterFromSelectedItem()
        {
            return _lastItem.Arguments;
        }

        private INavigationMenuItem GetItemFromList(Type sourcePagetype)
        {
            if (sourcePagetype == typeof(LibraryView) || sourcePagetype == typeof(PlaylistView))
            {
                return TopNavigationItems[0];
            }
            if (sourcePagetype == typeof(SettingsView))
            {
                return BottomNavigationItems.SingleOrDefault(p => p.DestinationPage == sourcePagetype);
            }
            return null;
        }

        private static NavMenuListView GetParentListViewFromItem(INavigationMenuItem item)
        {
            if (item.DestinationPage == typeof(LibraryView))
            {
                return _navTopMenuListView;
            }            
            //if (item.DestinationPage == typeof(PlaylistView))
            //{
            //    return _playlistsMenuListView;
            //}
            return _navBottomMenuListView;
        }

        private async void OnNavigatingToPage(object sender, NavigatingCancelEventArgs e)
        {
            try
            {
                if (e.NavigationMode != NavigationMode.Back)
                {
                    return;
                }

                var item = GetItemFromList(e.SourcePageType);
                if (item == null && _pageFrame.BackStackDepth > 0)
                {
                    foreach (var entry in _pageFrame.BackStack.Reverse())
                    {
                        if (entry.SourcePageType == typeof(LibraryView))
                        {
                            item = TopNavigationItems[0];
                        }
                        else if (entry.SourcePageType == typeof(SettingsView))
                        {
                            item = BottomNavigationItems[1];
                        }
                        if (item != null)
                        {
                            break;  //if item is successfully got break the loop. We got what we needed.
                        }
                    }
                }
                if (item != null)
                {
                    await UpdateHeaderAndShortCuts(item as SimpleNavMenuItem);
                    var container = (ListViewItem)GetParentListViewFromItem(item as SimpleNavMenuItem).ContainerFromItem(item);
                    if (container != null)
                    {
                        container.IsTabStop = false;
                        GetParentListViewFromItem(item as SimpleNavMenuItem).SetSelectedItem(container);
                        container.IsSelected = true;
                        container.IsTabStop = true;
                    }
                }
            }
            catch (Exception ex)
            {
                BLogger.E("Error while navigating back.", ex);
            }
        }

        private async Task UpdateHeaderAndShortCuts(SimpleNavMenuItem item)
        {
            if (item != null)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    _headerText.DataContext = item;
                    _shortcuts.DataContext = item.Shortcuts;
                    _shortcuts.ItemsSource = item.Shortcuts;
                });
            }
        }

        private async void OnNavMenuItemInvoked(object sender, ListViewItem e)
        {
            var item = (INavigationMenuItem)((NavMenuListView)sender).ItemFromContainer(e);
            if ((item as SimpleNavMenuItem).Command == null)
            {
                await UpdateHeaderAndShortCuts(item as SimpleNavMenuItem);
                _pageFrame.Navigate(item.DestinationPage, item.Arguments);                
                _lastItem = item;
            }
            else
            {
                (item as SimpleNavMenuItem).Command.Execute(null);
            }
        }
    }
}