using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SplitViewMenu
{
    public sealed class SplitViewMenu : Control
    {
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
            DependencyProperty.Register("TopNavigationItems", typeof (IEnumerable<INavigationMenuItem>),
                typeof (SplitViewMenu),
                new PropertyMetadata(Enumerable.Empty<INavigationMenuItem>(), OnTopNavigationItemsPropertyChanged));

        internal static readonly DependencyProperty BottomNavigationItemsProperty =
           DependencyProperty.Register("BottomNavigationItems", typeof(IEnumerable<INavigationMenuItem>),
               typeof(SplitViewMenu),
               new PropertyMetadata(Enumerable.Empty<INavigationMenuItem>(), OnBottomNavigationItemsPropertyChanged));

        internal static readonly DependencyProperty PlaylistsItemsProperty =
          DependencyProperty.Register("PlaylistsItems", typeof(IEnumerable<INavigationMenuItem>),
              typeof(SplitViewMenu),
              new PropertyMetadata(Enumerable.Empty<INavigationMenuItem>(), OnPlaylistsItemsPropertyChanged));


        private Button _backButton;
        private NavMenuListView _navTopMenuListView;
        private NavMenuListView _navBottomMenuListView;
        private NavMenuListView _playlistsMenuListView;
        private Frame _pageFrame;

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

        public IEnumerable<INavigationMenuItem> TopNavigationItems
        {
            get { return (IEnumerable<INavigationMenuItem>) GetValue(TopNavigationItemsProperty); }
            set { SetValue(TopNavigationItemsProperty, value); }
        }
        public IEnumerable<INavigationMenuItem> BottomNavigationItems
        {
            get { return (IEnumerable<INavigationMenuItem>)GetValue(BottomNavigationItemsProperty); }
            set { SetValue(BottomNavigationItemsProperty, value); }
        }
        public IEnumerable<INavigationMenuItem> PlaylistsItems
        {
            get { return (IEnumerable<INavigationMenuItem>)GetValue(PlaylistsItemsProperty); }
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
            if (menu._navTopMenuListView != null)
                menu._navTopMenuListView.ItemsSource = e.NewValue;
        }
        private static void OnBottomNavigationItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var menu = (SplitViewMenu)d;
            if (menu._navBottomMenuListView != null)
                menu._navBottomMenuListView.ItemsSource = e.NewValue;
        }
        private static void OnPlaylistsItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var menu = (SplitViewMenu)d;
            if (menu._playlistsMenuListView != null)
                menu._playlistsMenuListView.ItemsSource = e.NewValue;
        }
        protected override void OnApplyTemplate()
        {
            _pageFrame = GetTemplateChild("PageFrame") as Frame;
            _navTopMenuListView = GetTemplateChild("NavTopMenuList") as NavMenuListView;
            _navBottomMenuListView = GetTemplateChild("NavBottomMenuList") as NavMenuListView;
            _playlistsMenuListView = GetTemplateChild("PlaylistsMenuList") as NavMenuListView;
            _backButton = GetTemplateChild("BackButton") as Button;

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
        INavigationMenuItem LastItem = new SimpleNavMenuItem();
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

        private void BackRequested(ref bool handled)
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

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            ((Page) sender).Focus(FocusState.Programmatic);
            ((Page) sender).Loaded -= PageLoaded;
        }

        private void OnNavigatingToPage(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode != NavigationMode.Back || !TopNavigationItems.Any())
                return;
            var item = TopNavigationItems.SingleOrDefault(p => p.DestinationPage == e.SourcePageType);
            if (item == null && _pageFrame.BackStackDepth > 0)
            {
                foreach (var entry in _pageFrame.BackStack.Reverse())
                {
                    item = TopNavigationItems.SingleOrDefault(p => p.DestinationPage == entry.SourcePageType);
                    if (item != null)
                        break;
                }
            }

            var container = (ListViewItem)_navTopMenuListView.ContainerFromItem(item);
            if (container != null)
                container.IsTabStop = false;
            _navTopMenuListView.SetSelectedItem(container);
            if (container != null)
                container.IsTabStop = true;
        }

        private void OnNavMenuItemInvoked(object sender, ListViewItem e)
        {
            var item = (INavigationMenuItem) ((NavMenuListView) sender).ItemFromContainer(e);
            if(((NavMenuListView)sender).Name != "PlaylistsMenuList")
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
              item.Label != LastItem.Label)
                {
                    _pageFrame.Navigate(item.DestinationPage, item.Arguments);
                    LastItem = item;
                }
            }

            GC.Collect();
        }

       
    }
}