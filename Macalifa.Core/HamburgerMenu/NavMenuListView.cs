using System;
using System.Linq;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace SplitViewMenu
{
    public sealed class NavMenuListView : ListView
    {
        private SplitView _splitViewHost;

        public NavMenuListView()
        {
            SelectionMode = ListViewSelectionMode.Single;
            IsItemClickEnabled = true;
            Loaded += OnLoaded;
            ItemClick += ItemClickedHandler;
        }

        private void OnLoaded(object o, RoutedEventArgs e)
        {
            var parent = VisualTreeHelper.GetParent(this);
            while (parent != null && !(parent is SplitView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            if (parent == null)
                return;
            _splitViewHost = (SplitView) parent;

            _splitViewHost.RegisterPropertyChangedCallback(SplitView.IsPaneOpenProperty,
                (_, __) => { OnPaneToggled(); });

            OnPaneToggled();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            for (var i = 0; i < ItemContainerTransitions.Count; i++)
            {
                if (ItemContainerTransitions[i] is EntranceThemeTransition)
                {
                    ItemContainerTransitions.RemoveAt(i);
                }
            }
        }

        public void SetSelectedItem(ListViewItem item)
        {
            if (Items == null)
                return;
            foreach (
                var cont in
                    Items.Select(i => (ListViewItem) ContainerFromItem(i)).Where(cont => cont != null && cont != item))
            {
                cont.IsSelected = false;
            }
            if (item != null)
                item.IsSelected = true;
        }

        public event EventHandler<ListViewItem> ItemInvoked;

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            var focusedItem = FocusManager.GetFocusedElement();

            switch (e.Key)
            {
                case VirtualKey.Up:
                    TryMoveFocus(FocusNavigationDirection.Up);
                    e.Handled = true;
                    break;

                case VirtualKey.Down:
                    TryMoveFocus(FocusNavigationDirection.Down);
                    e.Handled = true;
                    break;

                case VirtualKey.Tab:
                    var shiftKeyState = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Shift);
                    var shiftKeyDown = (shiftKeyState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
                    var item = focusedItem as ListViewItem;
                    if (item != null && Items != null)
                    {
                        var currentItem = item;
                        var onlastitem = IndexFromContainer(currentItem) == Items.Count - 1;
                        var onfirstitem = IndexFromContainer(currentItem) == 0;

                        if (!shiftKeyDown)
                        {
                            TryMoveFocus(onlastitem ? FocusNavigationDirection.Next : FocusNavigationDirection.Down);
                        }
                        else
                        {
                            TryMoveFocus(onfirstitem ? FocusNavigationDirection.Previous : FocusNavigationDirection.Up);
                        }
                    }
                    else if (focusedItem is Control)
                    {
                        TryMoveFocus(!shiftKeyDown ? FocusNavigationDirection.Down : FocusNavigationDirection.Up);
                    }

                    e.Handled = true;
                    break;

                case VirtualKey.Space:
                case VirtualKey.Enter:
                    // Fire our event using the item with current keyboard focus
                    InvokeItem(focusedItem);
                    e.Handled = true;
                    break;

                default:
                    base.OnKeyDown(e);
                    break;
            }
        }

        private void TryMoveFocus(FocusNavigationDirection direction)
        {
            if (direction == FocusNavigationDirection.Next || direction == FocusNavigationDirection.Previous)
            {
                FocusManager.TryMoveFocus(direction);
            }
            else
            {
                var control = FocusManager.FindNextFocusableElement(direction) as Control;
                control?.Focus(FocusState.Programmatic);
            }
        }

        private void ItemClickedHandler(object sender, ItemClickEventArgs e)
        {
            var item = ContainerFromItem(e.ClickedItem);
            InvokeItem(item);
        }

        private void InvokeItem(object focusedItem)
        {
            SetSelectedItem(focusedItem as ListViewItem);
            var handler = ItemInvoked;
            handler?.Invoke(this, focusedItem as ListViewItem);

            if (!_splitViewHost.IsPaneOpen ||
                (_splitViewHost.DisplayMode != SplitViewDisplayMode.CompactOverlay &&
                 _splitViewHost.DisplayMode != SplitViewDisplayMode.Overlay))
                return;
            _splitViewHost.IsPaneOpen = false;
            var item = focusedItem as ListViewItem;
            item?.Focus(FocusState.Programmatic);
        }

        private void OnPaneToggled()
        {
            if (ItemsPanelRoot == null)
                return;
            if (_splitViewHost.IsPaneOpen)
            {
                ItemsPanelRoot.ClearValue(WidthProperty);
                ItemsPanelRoot.ClearValue(HorizontalAlignmentProperty);
            }
            else if (_splitViewHost.DisplayMode == SplitViewDisplayMode.CompactInline ||
                     _splitViewHost.DisplayMode == SplitViewDisplayMode.CompactOverlay)
            {
                ItemsPanelRoot.SetValue(WidthProperty, _splitViewHost.CompactPaneLength);
                ItemsPanelRoot.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Left);
            }
        }
    }
}