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

using BreadPlayer.Core.Common;
using BreadPlayer.Core.Models;
using BreadPlayer.Interfaces;
using Microsoft.Xaml.Interactivity;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace BreadPlayer.Behaviours
{
    public class InvokeCommandByKeyDown : DependencyObject, IAction
    {
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(InvokeCommandByKeyDown), new PropertyMetadata(null));

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public static readonly DependencyProperty CommandParameterProperty =
           DependencyProperty.Register("CommandParameter", typeof(object), typeof(InvokeCommandByKeyDown), new PropertyMetadata(null));

        public VirtualKey PressedKey
        {
            get => (VirtualKey)GetValue(PressedKeyProperty);
            set => SetValue(PressedKeyProperty, value);
        }

        public static readonly DependencyProperty PressedKeyProperty =
            DependencyProperty.Register("PressedKey", typeof(VirtualKey), typeof(InvokeCommandByKeyDown), new PropertyMetadata(VirtualKey.None));

        public int PressedKeyCode
        {
            get => (int)GetValue(PressedKeyCodeProperty);
            set => SetValue(PressedKeyCodeProperty, value);
        }

        public static readonly DependencyProperty PressedKeyCodeProperty =
            DependencyProperty.Register("PressedKeyCode", typeof(int), typeof(InvokeCommandByKeyDown), new PropertyMetadata(0));

        public bool DoubleKeyCommand
        {
            get => (bool)GetValue(DoubleKeyCommandProperty);
            set => SetValue(DoubleKeyCommandProperty, value);
        }

        public static readonly DependencyProperty DoubleKeyCommandProperty =
            DependencyProperty.Register("DoubleKeyCommand?", typeof(bool), typeof(InvokeCommandByKeyDown), new PropertyMetadata(VirtualKey.None));

        public object Execute(object sender, object parameter)
        {
            if (parameter is KeyEventArgs keyParam)
            {
                InvokeCommand(keyParam.VirtualKey);
                keyParam.Handled = true;
            }
            else if (parameter is KeyRoutedEventArgs routed)
            {
                InvokeCommand(routed.Key);
                routed.Handled = true;
            }
            return null;
        }

        private void InvokeCommand(VirtualKey paramKey)
        {
            if ((int)paramKey == (PressedKeyCode == 0 ? (int)PressedKey : PressedKeyCode))
            {
                if (!DoubleKeyCommand)
                {
                    var p = CommandParameter;
                    Command.Execute(p);
                }
                else if (DoubleKeyCommand && IsControlPressed())
                {
                    var p = CommandParameter as Mediafile;
                    Command.Execute(p);
                }
            }
        }

        private bool IsControlPressed()
        {
            return Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Control) == CoreVirtualKeyStates.Down
                || Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.LeftControl) == CoreVirtualKeyStates.Down
                || Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.RightControl) == CoreVirtualKeyStates.Down;
        }
    }
}