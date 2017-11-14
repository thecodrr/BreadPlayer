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

using BreadPlayer.Core;
using BreadPlayer.Core.Common;
using BreadPlayer.Core.Models;
using BreadPlayer.Dispatcher;
using BreadPlayer.Interfaces;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BreadPlayer.Extensions
{
    public class BindableFlyout : MenuFlyout
    {
        public ThreadSafeObservableCollection<ContextMenuCommand> ItemsSource
        {
            get => (ThreadSafeObservableCollection<ContextMenuCommand>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(ThreadSafeObservableCollection<ContextMenuCommand>), typeof(BindableFlyout), new PropertyMetadata(null, (o, args) =>
            {
                var obj = o as BindableFlyout;
                obj.Setup(obj);
                obj.ItemsSource.CollectionChanged += (e, a) =>
                {
                    obj.Setup(obj);
                };
            }
        ));

        public object DataContext
        {
            get => GetValue(DataContextProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty DataContextProperty =
            DependencyProperty.Register("DataContext", typeof(object), typeof(BindableFlyout), new PropertyMetadata(null, (o, args) =>
            {
                var obj = o as BindableFlyout;
                obj.Setup(obj);
            }
        ));

        private void Setup(BindableFlyout menuFlyout)
        {
            if (DesignMode.DesignModeEnabled)
            {
                return;
            }

            if (menuFlyout.ItemsSource == null)
            {
                return;
            }

            menuFlyout.Items.Clear();
            foreach (var menuItem in menuFlyout.ItemsSource)
            {
                menuItem.PropertyChanged += (s, a) =>
                {
                    Setup(this);
                };
                var item = new MenuFlyoutIconItem
                {
                    Text = menuItem.Text,
                    Command = menuItem.Command,
                    Glyph = menuItem.CommandIcon
                };

                item.CommandParameter = menuItem.CommandParameter == null ? item : menuItem.CommandParameter;
                item.Tag = menuFlyout.DataContext as Mediafile;
                if (menuFlyout.Items.Count == 1)
                {
                    menuFlyout.Items.Add(new MenuFlyoutSeparator());
                }
                menuFlyout.Items.Add(item);
            }
        }
    }

    public class MenuFlyoutIconItem : MenuFlyoutItem
    {
        public string Glyph
        {
            get { return (string)GetValue(GlyphProperty); }
            set { SetValue(GlyphProperty, value); }
        }

        public static readonly DependencyProperty GlyphProperty =
            DependencyProperty.Register("Glyph", typeof(string), typeof(MenuFlyoutIconItem), new PropertyMetadata(null));
    }

    public class ContextMenuCommand : ObservableObject
    {
        public ContextMenuCommand(ICommand command, string text, string glyph = "\uE93C", object cmdPara = null)
        {
            Command = command;
            Text = text;
            CommandParameter = cmdPara;
            CommandIcon = glyph;
        }

        private string _text;

        public string Text
        {
            get => _text;
            set => Set(ref _text, value);
        }

        public string CommandIcon { get; set; }

        public ICommand Command
        {
            get; private set;
        }

        public object CommandParameter
        {
            get; private set;
        }
    }

    public class CustomFlyout : MenuFlyout
    {
        public object Tag
        {
            get => GetValue(TagProperty);
            set => SetValue(TagProperty, value);
        }

        public static readonly DependencyProperty TagProperty =
            DependencyProperty.Register("Tag", typeof(object), typeof(CustomFlyout), new PropertyMetadata(null, (o, args) =>
            {
                (o as CustomFlyout).Tag = args.NewValue;
            }
        ));
    }

    public static class FlyoutMenuExtension
    {
        public static ThreadSafeObservableCollection<ContextMenuCommand> GetMyItems(DependencyObject obj)
        {
            return (ThreadSafeObservableCollection<ContextMenuCommand>)obj.GetValue(MyItemsProperty);
        }

        public static void SetMyItems(DependencyObject obj, ThreadSafeObservableCollection<ContextMenuCommand> value)
        {
            obj.SetValue(MyItemsProperty, value);
        }

        private static void AddMenuItems(MenuFlyoutSubItem menuFlyoutSubItem, MenuFlyout menuFlyout = null)
        {
            foreach (var menuItem in SharedLogic.Instance.OptionItems)
            {
                var item = new MenuFlyoutItem
                {
                    Text = menuItem.Text,
                    Command = menuItem.Command
                };
                item.CommandParameter = menuItem.CommandParameter ?? item;
                if (menuFlyout != null && menuFlyout.GetType() != typeof(CustomFlyout))
                {
                    item.Tag = "Current";
                }

                if (menuFlyoutSubItem.Items.Count == 1)
                {
                    menuFlyoutSubItem.Items.Add(new MenuFlyoutSeparator());
                }
                menuFlyoutSubItem.Items.Add(item);
            }
        }

        private async static void Setup(MenuFlyout menuFlyout)
        {
            if (menuFlyout == null) return;

            await BreadDispatcher.InvokeAsync(() =>
            {
                menuFlyout.Items.Clear();
                MenuFlyoutSubItem addTo = new MenuFlyoutSubItem { Text = "Add to" };
                MenuFlyoutItem properties = new MenuFlyoutItem { Text = "Properties", Command = SharedLogic.Instance.ShowPropertiesCommand, CommandParameter = null };
                MenuFlyoutItem openLoc = new MenuFlyoutItem { Text = "Open Song Location", Command = SharedLogic.Instance.OpenSongLocationCommand, CommandParameter = null };
                MenuFlyoutItem changeAlbumArt = new MenuFlyoutItem { Text = "Change Album Art", Command = SharedLogic.Instance.ChangeAlbumArtCommand, CommandParameter = null };

                menuFlyout.Items.Add(addTo);
                menuFlyout.Items.Add(changeAlbumArt);
                menuFlyout.Items.Add(openLoc);
                menuFlyout.Items.Add(properties);
                AddMenuItems(addTo, menuFlyout);
            });
            //SharedLogic.Instance.Player.PropertyChanged += Player_PropertyChanged;
        }

        private static void Player_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentlyPlayingFile")
            {
                Refresh();
            }
        }

        /// <summary>
        /// Fix this later. Not very essential right now.
        /// </summary>
        private static void Refresh()
        {
            //MenuFlyoutSubItem removeFrom = new MenuFlyoutSubItem() { Text = "Remove from" };
            //if (Menu != null && Menu.Items.Any() && Menu.GetType() != typeof(CustomFlyout))
            //{
            //    if (Menu.Items.Any(t => (t as MenuFlyoutSubItem)?.Text == removeFrom.Text))
            //        Menu.Items.RemoveAt(Menu.Items.IndexOf(Menu.Items.First(t => (t as MenuFlyoutSubItem)?.Text == removeFrom.Text)));
            //    if (CoreMethods.Player.CurrentlyPlayingFile != null)
            //        using(LibraryService service = new LibraryService(new DatabaseService()))
            //        {
            //            if (service.CheckExists<Mediafile>(LiteDB.Query.EQ("Path", CoreMethods.Player.CurrentlyPlayingFile.Path), new Common.TracksCollection()))
            //            {
            //                var file = Core.CoreMethods.LibVM.Database.tracks.FindOne(t => t.Path == CoreMethods.Player.CurrentlyPlayingFile.Path);
            //                if (file.Playlists.Count > 0)
            //                {
            //                    Menu.Items.Add(removeFrom);
            //                    foreach (var list in file.Playlists)
            //                    {
            //                        var item = new MenuFlyoutItem() { Text = list.Name };//, Command = CoreMethods.PlaylistVM.DeleteCommand };
            //                        item.CommandParameter = item;
            //                        item.Click += Item_Click;
            //                        removeFrom.Items.Add(item);
            //                    }
            //                }
            //            }
            //        }
            //}
        }

        private static void Item_Click(object sender, RoutedEventArgs e)
        {
            //var item = sender as MenuFlyoutItem;
            //var parent = Menu.Items[1] as MenuFlyoutSubItem;
            //parent.Items.Remove(parent.Items.First(t => (t as MenuFlyoutItem).Text == item.Text));
        }

        private static MenuFlyout _menu;
        private static List<MenuFlyout> _fly = new List<MenuFlyout>();

        public static readonly DependencyProperty MyItemsProperty =
            DependencyProperty.Register("MyItems",
                typeof(List<MenuFlyoutItemBase>),
                typeof(FlyoutMenuExtension),
                new PropertyMetadata(new ThreadSafeObservableCollection<ContextMenuCommand>(), (sender, e) =>
                {
                    if (sender is MenuFlyout menuFlyout)
                    {
                        _menu = menuFlyout;
                        Setup(menuFlyout);
                        SharedLogic.Instance.OptionItems.CollectionChanged += OptionItems_CollectionChanged;
                    }
                }));

        private static void OptionItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Setup(_menu);
        }
    }
}