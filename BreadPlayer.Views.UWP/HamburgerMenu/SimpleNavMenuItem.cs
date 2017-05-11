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
using BreadPlayer;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BreadPlayer.Core.Common;

namespace SplitViewMenu
{
	public class SimpleNavMenuItem :  ViewModelBase, INavigationMenuItem
    {
        public SimpleNavMenuItem()
        {

        }

        private string label;
        public string Label { get => label;
            set => Set(ref label, value);
        }
        public Symbol Symbol { get; set; }
        public char SymbolAsChar => (char) Symbol;
        public string FontGlyph { get; set; }
        private object args;
        public object Arguments { get => args;
            set => Set(ref args, value);
        }
        public Visibility HeaderVisibility
        {
            get; set;
        } = Visibility.Visible;

        private ElementTheme shortcutTheme;
        public ElementTheme ShortcutTheme
        {
            get => shortcutTheme;
            set => Set(ref shortcutTheme, value);
        }
        public ICommand Command { get; set; }

        private List<Shortcut> shortcuts = new List<Shortcut>()
        {
            new Shortcut() { SymbolAsChar = "\xE00E", Tooltip = "Go Back",
                ShortcutCommand = new DelegateCommand(() => BreadPlayer.Services.NavigationService.Instance.NavigateBack()) },
            new Shortcut() { SymbolAsChar = "\xE149",
                ShortcutCommand = new DelegateCommand(() =>
                {
                    BreadPlayer.Services.NavigationService.Instance.Reload(SplitViewMenu.GetParameterFromSelectedItem());
                }),
                Tooltip = "Refresh",
            },
            new Shortcut { SymbolAsChar = "\xE80F",
                ShortcutCommand = new DelegateCommand(() =>
                {
                    BreadPlayer.Services.NavigationService.Instance.NavigateToHome();
                }),
                Tooltip = "Go Home",
            },
            new Shortcut() { SymbolAsChar = "\xE094", Tooltip = "Search Tracks", ShortcutCommand = SplitViewMenu.SearchClickedCommand() }
        };
        public List<Shortcut> Shortcuts
        {
            get => shortcuts;
            set => Set(ref shortcuts, value);
        }
        public string Tooltip { get; set; }

        public Type DestinationPage { get; set; }

        private void Select(object param) { }
    }
}
