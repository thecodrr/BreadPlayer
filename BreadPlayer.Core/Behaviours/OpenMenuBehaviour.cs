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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml.Controls.Primitives;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using BreadPlayer.Extensions;
using BreadPlayer.Models;
using System.Reflection;
using Windows.Foundation;
using SplitViewMenu;

namespace BreadPlayer.Behaviours
{
    public class OpenMenuFlyoutAction : DependencyObject, IAction
    {
        public object Parameter
        {
            get { return GetValue(ParameterProperty); }
            set { SetValue(ParameterProperty, value); }
        }
        public static readonly DependencyProperty ParameterProperty =
           DependencyProperty.Register("Flyout", typeof(object), typeof(OpenMenuFlyoutAction), new PropertyMetadata(null, (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
           {
           }
        ));
        public object Execute(object sender, object parameter)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            var navList = senderElement.GetFirstAncestorOfType<NavMenuListView>() != null && senderElement.GetFirstAncestorOfType<NavMenuListView>().Name == "PlaylistsMenuList" ? senderElement.GetFirstAncestorOfType<NavMenuListView>() : null;
            ListBoxItem item = senderElement.GetFirstAncestorOfType<ListBoxItem>();
            var ListBox = item.GetFirstAncestorOfType<ListBox>();
            // if (item != null) item.IsSelected = true;
            ListViewItem listItem = senderElement.Tag is ContentPresenter ? (senderElement.Tag as ContentPresenter).Tag as ListViewItem : null;
            if(listItem != null) listItem.IsSelected = true;
            //var items = //((Parameter as Binding).Path as ListViewItem);
            if (Parameter.ToString() == "BindableFlyout")
            {
                if (ListBox?.SelectedItems.Count == 1) { ListBox.SelectedIndex = -1; }
                if (item != null) item.IsSelected = true;
                var flyout = (senderElement as Button).GetAncestorsOfType<Grid>().Where(t => t.Name == "LayoutRoot").ToList()[0].Resources["PlaylistsFlyout"] as BindableFlyout;
                flyout.ShowAt(senderElement);
            }

            else if (Parameter.ToString() == "Playlist")
            {
                if(navList != null)
                {
                    var flyout = senderElement.Resources["Flyout"] as MenuFlyout; //.GetFirstDescendantOfType<Grid>().Resources["Flyout"] as MenuFlyout;
                    flyout.ShowAt(senderElement, senderElement.GetPointerPosition());
                }
            }
            else
            {
                ListBox.SelectedIndex = -1;
                if (item != null) item.IsSelected = true;
                var flyout = senderElement.Resources["Flyout"] as MenuFlyout; //.GetFirstDescendantOfType<Grid>().Resources["Flyout"] as MenuFlyout;
                flyout.ShowAt(senderElement, senderElement.GetPointerPosition());
            }
           
            return null;
        }

        
    }
}