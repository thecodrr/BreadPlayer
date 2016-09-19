/* 
	Macalifa. A music player made for Windows 10 store.
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
using Macalifa.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Runtime.CompilerServices;
using System.ServiceModel.Channels;
using Macalifa.Models;

namespace Macalifa.Extensions
{
    public class BindableFlyout : MenuFlyout
    {
        
        public ThreadSafeObservableCollection<ContextMenuCommand> ItemsSource
        {
            get { return (ThreadSafeObservableCollection<ContextMenuCommand>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(ThreadSafeObservableCollection<ContextMenuCommand>), typeof(BindableFlyout), new PropertyMetadata(null, (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
            {
                var obj = o as BindableFlyout;
                obj.Setup(obj);
            }
        ));

        public object DataContext
        {
            get { return GetValue(DataContextProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty DataContextProperty =
            DependencyProperty.Register("DataContext", typeof(object), typeof(BindableFlyout), new PropertyMetadata(null, (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
            {
                var obj = o as BindableFlyout;
                obj.Setup(obj);
            }
        ));
        private void Setup(BindableFlyout menuFlyout)
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                return;
            if (menuFlyout.ItemsSource == null)
                return;

            ItemsSource.CollectionChanged += ItemsSource_CollectionChanged;
            menuFlyout.Items.Clear();
            foreach (var menuItem in menuFlyout.ItemsSource)
            {
                var item = new MenuFlyoutItem()
                {
                    Text = menuItem.Text,
                    Command = menuItem.Command
                };
                 item.CommandParameter =  menuItem.CommandParameter == null ? item : menuItem.CommandParameter;
                item.Tag = menuFlyout.DataContext as Mediafile;
                menuFlyout.Items.Add(item);
            }
        }

        private void ItemsSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Setup(this);
        }
    }

    public class ContextMenuCommand
    {
        public ContextMenuCommand(ICommand command, string text, object cmdPara = null)
        {
            Command = command;
            Text = text;
            CommandParameter = cmdPara;
        }

        public string Text
        {
            get; private set;
        }

        public ICommand Command
        {
            get; private set;
        }
        public object CommandParameter
        {
            get; private set;
        }
    }

    public static class MenuExtension 
    {
        public static List<MenuFlyoutItemBase> GetMyItems(DependencyObject obj)
        {
            return (List<MenuFlyoutItemBase>)obj.GetValue(MyItemsProperty);
        }
        public static void SetMyItems(DependencyObject obj, List<MenuFlyoutItemBase> value)
        {
            obj.SetValue(MyItemsProperty, value);
        }
        public static readonly DependencyProperty MyItemsProperty =
            DependencyProperty.Register("MyItems",
                typeof(List<MenuFlyoutItemBase>),
                typeof(MenuExtension),
                new PropertyMetadata(new List<MenuFlyoutItemBase>(), (sender, e) => {
                    var menu = sender as MenuFlyout;
                    menu.Items.Clear();
                    foreach (var item in e.NewValue as List<MenuFlyoutItemBase>)
                    {
                        menu.Items.Add(item);
                    }
                }));

    }
}
