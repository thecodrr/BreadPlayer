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
using Windows.UI.Xaml;
using Microsoft.Xaml.Interactivity;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Xaml.Input;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace BreadPlayer.Behaviours
{
    class InvokeCommandByKeyDown : DependencyObject, IAction
    {
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(InvokeCommandByKeyDown), new PropertyMetadata(null));
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }
        public static readonly DependencyProperty CommandParameterProperty =
           DependencyProperty.Register("CommandParameter", typeof(object), typeof(InvokeCommandByKeyDown), new PropertyMetadata(null));

        public VirtualKey PressedKey
        {
            get { return (VirtualKey)GetValue(PressedKeyProperty); }
            set { SetValue(PressedKeyProperty, value); }
        }

        public static readonly DependencyProperty PressedKeyProperty =
            DependencyProperty.Register("PressedKey", typeof(VirtualKey), typeof(InvokeCommandByKeyDown), new PropertyMetadata(VirtualKey.None));
        public bool DoubleKeyCommand
        {
            get { return (bool)GetValue(DoubleKeyCommandProperty); }
            set { SetValue(DoubleKeyCommandProperty, value); }
        }

        public static readonly DependencyProperty DoubleKeyCommandProperty =
            DependencyProperty.Register("DoubleKeyCommand?", typeof(bool), typeof(InvokeCommandByKeyDown), new PropertyMetadata(VirtualKey.None));

        public object Execute(object sender, object parameter)
        {

            var ctrl = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
            ListView fileBox = sender as ListView;
            if (fileBox.SelectedItems.Count > 0)
            {
                KeyRoutedEventArgs keyPrarm = parameter as KeyRoutedEventArgs;
                if (keyPrarm != null)
                {
                    if (!DoubleKeyCommand)
                    {
                        if (keyPrarm.Key == PressedKey)
                        {
                            var p = CommandParameter;
                            Command.Execute(p);
                            keyPrarm.Handled = true;
                        }
                    }
                    else
                    {
                        if(ctrl == CoreVirtualKeyStates.Down)
                            if (keyPrarm.Key == PressedKey)
                            {
                                var p = CommandParameter as BreadPlayer.Models.Mediafile;
                                Command.Execute(p);
                                keyPrarm.Handled = true;
                            }
                    }
                }
               
            }
            return null;
        }
    }
}