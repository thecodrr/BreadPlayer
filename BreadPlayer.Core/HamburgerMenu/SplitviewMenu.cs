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
using BreadPlayer.Models;
using BreadPlayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;

namespace SplitViewMenu
{
	public sealed class SplitViewMenu : Control
    {
        public static NavigationService NavService { get; protected set; }
        internal static readonly DependencyProperty MenuItemDataTemplateSelectorProperty =
            DependencyProperty.Register("MenuItemDataTemplateSelector", typeof (DataTemplateSelector),
                typeof (SplitViewMenu), new PropertyMetadata(null));

        internal static readonly DependencyProperty NavMenuItemTemplateProperty =
            DependencyProperty.Register("NavMenuItemTemplate", typeof (DataTemplate), typeof (SplitViewMenu),
                new PropertyMetadata(null));

        internal static readonly DependencyProperty NavMenuItemContainerStyleProperty =
            DependencyProperty.Register("NavMenuItemContainerStyle", typeof (Style), typeof (SplitViewMenu),
                new PropertyMetadata(null));

        internal static readonly DependencyProperty InitialPageProperty =
            DependencyProperty.Register("InitialPage", typeof (Type), typeof (SplitViewMenu),
                new PropertyMetadata(null));

        internal static readonly DependencyProperty TopNavigationItemsProperty =
            DependencyProperty.Register("TopNavigationItems", typeof (List<INavigationMenuItem>),
                typeof (SplitViewMenu),
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
        private static NavMenuListView _playlistsMenuListView;
        private static Frame _pageFrame;
        private static SplitView _splitView;
        private static ToggleButton TogglePaneButton;
        public SplitViewMenu()
        {
            DefaultStyleKey = typeof (SplitViewMenu);
            Loaded += OnSplitViewMenuLoaded;
            
        }

        public DataTemplateSelector MenuItemDataTemplateSelector
        {
            get { return (DataTemplateSelector) GetValue(MenuItemDataTemplateSelectorProperty); }
            set { SetValue(MenuItemDataTemplateSelectorProperty, value); }
        }

        public DataTemplate NavMenuItemTemplate
        {
            get { return (DataTemplate) GetValue(NavMenuItemTemplateProperty); }
            set { SetValue(NavMenuItemTemplateProperty, value); }
        }

        public Style NavMenuItemContainerStyle
        {
            get { return (Style) GetValue(NavMenuItemContainerStyleProperty); }
            set { SetValue(NavMenuItemContainerStyleProperty, value); }
        }

        public Type InitialPage
        {
            get { return (Type) GetValue(InitialPageProperty); }
            set { SetValue(InitialPageProperty, value); }
        }

        public List<INavigationMenuItem> TopNavigationItems
        {
            get { return (List<INavigationMenuItem>) GetValue(TopNavigationItemsProperty); }
            set { SetValue(TopNavigationItemsProperty, value); }
        }
        public List<INavigationMenuItem> BottomNavigationItems
        {
            get { return (List<INavigationMenuItem>)GetValue(BottomNavigationItemsProperty); }
            set { SetValue(BottomNavigationItemsProperty, value); }
        }
        public List<INavigationMenuItem> PlaylistsItems
        {
            get { return (List<INavigationMenuItem>)GetValue(PlaylistsItemsProperty); }
            set { SetValue(PlaylistsItemsProperty, value); }
        }
        private void OnSplitViewMenuLoaded(object sender, RoutedEventArgs e)
        {
            if (InitialPage == null || _pageFrame == null)
                return;
            _pageFrame.Navigate(InitialPage);
        }

        private static void OnTopNavigationItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var menu = (SplitViewMenu) d;
            if (_navTopMenuListView != null)
                _navTopMenuListView.ItemsSource = e.NewValue;
        }
        private static void OnBottomNavigationItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var menu = (SplitViewMenu)d;
            if (_navBottomMenuListView != null)
                _navBottomMenuListView.ItemsSource = e.NewValue;
        }
        private static void OnPlaylistsItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var menu = (SplitViewMenu)d;
            if (_playlistsMenuListView != null)
                _playlistsMenuListView.ItemsSource = e.NewValue;
        }
        protected override void OnApplyTemplate()
        {
            _splitView = GetTemplateChild("RootSplitView") as SplitView;
            _pageFrame = GetTemplateChild("PageFrame") as Frame;
            NavService = new NavigationService(ref _pageFrame);
            _navTopMenuListView = GetTemplateChild("NavTopMenuList") as NavMenuListView;
            _navBottomMenuListView = GetTemplateChild("NavBottomMenuList") as NavMenuListView;
            _playlistsMenuListView = GetTemplateChild("PlaylistsMenuList") as NavMenuListView;
            _backButton = GetTemplateChild("BackButton") as Button;
            TogglePaneButton = GetTemplateChild("TogglePaneButton") as ToggleButton;
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
            if (_playlistsMenuListView != null)
            {
                _playlistsMenuListView.ItemInvoked += OnNavMenuItemInvoked;
                _playlistsMenuListView.ContainerContentChanging += OnContainerContextChanging;
                _playlistsMenuListView.SelectionChanged += _playlistsMenuListView_SelectionChanged; ;
            }
            if (_backButton != null)
            {
                _backButton.Click += OnBackButtonClick;
            }

            if (_pageFrame != null)
            {
                _pageFrame.Navigating += OnNavigatingToPage;
                _pageFrame.Navigated += OnNavigatedToPage;
            }
        }
        

        static INavigationMenuItem LastItem = new SimpleNavMenuItem();
        private void _playlistsMenuListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_navTopMenuListView.SelectedIndex > -1 || _navBottomMenuListView.SelectedIndex > -1)
            {
                _navTopMenuListView.SelectedIndex = -1;
                _navBottomMenuListView.SelectedIndex = -1;
                LastItem =  new SimpleNavMenuItem();
            }
            else
            {
                if (e.RemovedItems.Count > 0 && _navTopMenuListView.SelectedIndex == -1)
                    LastItem = e.RemovedItems[0] as INavigationMenuItem;
            }
        }

        private void _navBottomMenuListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                _navTopMenuListView.SelectedIndex = -1;
                _playlistsMenuListView.SelectedIndex = -1;
        }

        private void _navTopMenuListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _navBottomMenuListView.SelectedIndex = -1;
                _playlistsMenuListView.SelectedIndex = -1;
        }

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            var ignored = false;
            BackRequested(ref ignored);
        }
        public void BackRequested(ref bool handled)
        {
            if (_pageFrame == null)
                return;
            if (!_pageFrame.CanGoBack || handled)
                return;
            handled = true;
            _pageFrame.GoBack();
        }

        private static void OnContainerContextChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (!args.InRecycleQueue && args.Item is INavigationMenuItem)
            {
                args.ItemContainer.SetValue(AutomationProperties.NameProperty, ((INavigationMenuItem) args.Item).Label);
            }
            else
            {
                args.ItemContainer.ClearValue(AutomationProperties.NameProperty);
            }
        }

        private void OnNavigatedToPage(object sender, NavigationEventArgs e)
        {
            var page = e.Content as Page;
            if (page != null && e.Content != null)
            {
                var control = page;
                control.Loaded += PageLoaded;
            }
          
        }
        public static void UnSelectAll()
        {
            LastItem = null;
            _navBottomMenuListView.SelectedIndex = -1;
            _navTopMenuListView.SelectedIndex = -1;
            _playlistsMenuListView.SelectedIndex = -1;
        }
        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            ((Page) sender).Focus(FocusState.Programmatic);
            ((Page) sender).Loaded -= PageLoaded;
        }
        INavigationMenuItem GetItemFromList(Type sourcePagetype)
        {
            if (sourcePagetype == typeof(LibraryView) || sourcePagetype == typeof(PlaylistView))
            {
                return null;
            }
            else if (sourcePagetype == typeof(SettingsView))
            {
                return BottomNavigationItems.SingleOrDefault(p => p.DestinationPage == sourcePagetype);
            }
            else
                return null;
        }
        NavMenuListView GetParentListViewFromItem(SimpleNavMenuItem item)
        {
            if (item.DestinationPage == typeof(LibraryView))
            {
                return _navTopMenuListView;
            }
            else if (item.DestinationPage == typeof(PlaylistView))
            {
                return _playlistsMenuListView;
            }
            else
            {
                return _navBottomMenuListView;
            }
        }
        private void OnNavigatingToPage(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode != NavigationMode.Back || !TopNavigationItems.Any())
                return;           
            var item = GetItemFromList(e.SourcePageType); 
            if (item == null && _pageFrame.BackStackDepth > 0)
            {
                foreach (var entry in _pageFrame.BackStack.Reverse())
                {
                    if(entry.SourcePageType == typeof(PlaylistView))
                    {
                        var para = entry.Parameter as Dictionary<Playlist, IEnumerable<Mediafile>>; //get previous entry's parameter
                        item = PlaylistsItems.SingleOrDefault(p => p.Label == para.First().Key.Name); //search for the item in PlaylistItems with the same label as in parameters Name.
                    }
                    else if(entry.SourcePageType == typeof(LibraryView))
                    {
                        var para = entry.Parameter;
                         if(para != null)
                            item = TopNavigationItems.SingleOrDefault(t => t.Arguments == para);
                    }
                    if (item != null) 
                        break;  //if item is successfully got break the loop. We got what we needed.
                }
            }
            if(item != null)
            {
                var container = (ListViewItem)GetParentListViewFromItem(item as SimpleNavMenuItem).ContainerFromItem(item);
                if (container != null)
                    container.IsTabStop = false;
                GetParentListViewFromItem(item as SimpleNavMenuItem).SetSelectedItem(container);
                container.IsSelected = true;
                if (container != null)
                    container.IsTabStop = true;
            }
        }

        private void OnNavMenuItemInvoked(object sender, ListViewItem e)
        {         
            var item = (INavigationMenuItem) ((NavMenuListView) sender).ItemFromContainer(e);
         
            if (((NavMenuListView)sender).Name != "PlaylistsMenuList" && ((NavMenuListView)sender).Tag.ToString() != "NavTopMenuList")
            {
                if (item?.DestinationPage != null &&
              item.DestinationPage != _pageFrame.CurrentSourcePageType)
                {
                    _pageFrame.Navigate(item.DestinationPage, item.Arguments);
                }
            }
            else
            {
                if (item?.DestinationPage != null &&
              item.Label != LastItem?.Label)
                {
                    _pageFrame.Navigate(item.DestinationPage, item.Arguments);
                   
                }
            }
            LastItem = item;
        if(_splitView.DisplayMode == SplitViewDisplayMode.Inline)
            {
                TogglePaneButton.IsChecked = false;
            }
        }

       
    }
}