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

using BreadPlayer.Extensions;
using Microsoft.Xaml.Interactivity;
using BreadPlayer.Controls;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace BreadPlayer.Behaviours
{
    public class OpenMenuFlyoutAction : DependencyObject, IAction
    {
        public object Parameter
        {
            get => GetValue(ParameterProperty);
            set => SetValue(ParameterProperty, value);
        }

        public static readonly DependencyProperty ParameterProperty =
           DependencyProperty.Register("Flyout", typeof(object),
               typeof(OpenMenuFlyoutAction),
               new PropertyMetadata(null));

        public object Execute(object sender, object parameter)
        {
            if (parameter is RightTappedRoutedEventArgs eventArgs)
            {
                if (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1))
                {
                    return null;
                }
                OpenMenu(sender, eventArgs.GetPosition(sender as FrameworkElement));
            }
            else
            {
                OpenMenu(sender, null);
            }

            return null;
        }

        public void OpenMenu(object sender, Point? position)
        {
            if (sender == null) return;
            try
            {
                FrameworkElement senderElement = sender as FrameworkElement;
                var navList = senderElement.GetFirstAncestorOfType<NavMenuListView>() != null && senderElement.GetFirstAncestorOfType<NavMenuListView>().Name == "PlaylistsMenuList" ? senderElement.GetFirstAncestorOfType<NavMenuListView>() : null;
                SelectorItem item = senderElement.GetFirstAncestorOfType<SelectorItem>();
                ListViewBase listView = item?.GetFirstAncestorOfType<ListViewBase>();

                // if (item != null) item.IsSelected = true;
                ListViewItem listItem = senderElement.Tag is ContentPresenter contentPresenter ? contentPresenter.Tag as ListViewItem : null;
                if (listItem != null)
                {
                    listItem.IsSelected = true;
                }
                //var items = //((Parameter as Binding).Path as ListViewItem);
                if (Parameter is BindableFlyout bindableFlyout)
                {
                    if (listView?.SelectedItems.Count == 1) { listView.SelectedIndex = -1; }
                    if (item != null)
                    {
                        item.IsSelected = true;
                    }

                    bindableFlyout.ShowAt(senderElement);
                }
                else if (Parameter.ToString() == "Playlist")
                {
                    if (navList != null)
                    {
                        var flyout = senderElement.Resources["Flyout"] as MenuFlyout; //.GetFirstDescendantOfType<Grid>().Resources["Flyout"] as MenuFlyout;
                        flyout.ShowAt(senderElement, position.Value);
                    }
                }
                else
                {
                    if (listView != null && listView.SelectedItems.Count < 2)
                    {
                        listView.SelectedIndex = -1;
                        if (item != null)
                        {
                            item.IsSelected = true;
                        }
                    }
                    var flyout = Parameter as MenuFlyout; //.GetFirstDescendantOfType<Grid>().Resources["Flyout"] as MenuFlyout;
                    
                    if (position != null)
                    {
                        flyout.ShowAt(senderElement, position.Value);
                    }
                    else
                    {
                        flyout.ShowAt(senderElement);
                    }
                }
            }
            catch { }
        }
    }
}