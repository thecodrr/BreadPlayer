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
using Macalifa;
using System;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace SplitViewMenu
{
    public sealed class SimpleNavMenuItem : ViewModelBase, INavigationMenuItem
    {
        string label;
        public string Label { get { return label; } set { Set(ref label, value); } }
        public Symbol Symbol { get; set; }
        public char SymbolAsChar => (char) Symbol;
        public string FontGlyph { get; set; }
        object args;
        public object Arguments { get { return args; } set { Set(ref args, value); } }

        public Type DestinationPage { get; set; }

        void Select(object param) { }

    }
}