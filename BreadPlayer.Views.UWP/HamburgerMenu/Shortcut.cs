using BreadPlayer;
using Windows.UI.Xaml.Controls;
using BreadPlayer.Core.Common;

namespace SplitViewMenu
{
    public class Shortcut : ObservableObject
    {
        public string Tooltip { get; set; }
        public string SymbolAsChar { get; set; }
        private ICommand shortcutCommand;
        public ICommand ShortcutCommand
        {
            get => shortcutCommand; 
            set => Set(ref shortcutCommand, value);
        }
        public object ShortcutCommandParameter { get; set; }
        private MenuFlyout shortcutFlyout;
        public MenuFlyout ShortcutFlyout { get => shortcutFlyout; set => Set(ref shortcutFlyout, value);
        }
    }
}
