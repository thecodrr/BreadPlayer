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
using Microsoft.Xaml.Interactivity;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace BreadPlayer.Behaviours
{
  
    public class InvokeCommandByKeyDown : DependencyObject, IAction
    {
        public ICommand Command
        {
            get { return (ICommand)this.GetValue(CommandProperty); }
            set { this.SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(InvokeCommandByKeyDown), new PropertyMetadata(null));
        public object CommandParameter
        {
            get { return this.GetValue(CommandParameterProperty); }
            set { this.SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
           DependencyProperty.Register("CommandParameter", typeof(object), typeof(InvokeCommandByKeyDown), new PropertyMetadata(null));

        public VirtualKey PressedKey
        {
            get { return (VirtualKey)this.GetValue(PressedKeyProperty); }
            set { this.SetValue(PressedKeyProperty, value); }
        }

        public static readonly DependencyProperty PressedKeyProperty =
            DependencyProperty.Register("PressedKey", typeof(VirtualKey), typeof(InvokeCommandByKeyDown), new PropertyMetadata(VirtualKey.None));
        public bool DoubleKeyCommand
        {
            get { return (bool)this.GetValue(DoubleKeyCommandProperty); }
            set { this.SetValue(DoubleKeyCommandProperty, value); }
        }

        public static readonly DependencyProperty DoubleKeyCommandProperty =
            DependencyProperty.Register("DoubleKeyCommand?", typeof(bool), typeof(InvokeCommandByKeyDown), new PropertyMetadata(VirtualKey.None));

        public object Execute(object sender, object parameter)
        {
            if ((sender as ListView).SelectedItems.Count > 0 && parameter is KeyRoutedEventArgs keyParam)
            {
                if (!this.DoubleKeyCommand && keyParam.Key == this.PressedKey)
                {
                    var p = this.CommandParameter;
                    this.Command.Execute(p);
                    keyParam.Handled = true;
                }
                else if (Window.Current.CoreWindow.GetKeyState(VirtualKey.Control) == CoreVirtualKeyStates.Down
                    && keyParam.Key == PressedKey)
                {
                    var p = this.CommandParameter as BreadPlayer.Models.Mediafile;
                    this.Command.Execute(p);
                    keyParam.Handled = true;
                }
            }
            return null;
        }
    }
}