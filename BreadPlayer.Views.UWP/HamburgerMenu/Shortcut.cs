using BreadPlayer;
using Windows.UI.Xaml.Controls;

namespace SplitViewMenu
{
    public class Shortcut : ObservableObject
    {
        public string Tooltip { get; set; }
        public string SymbolAsChar { get; set; }
        ICommand shortcutCommand;
        public ICommand ShortcutCommand
        {
            get => shortcutCommand; 
            set => Set(ref shortcutCommand, value);
        }
        public object ShortcutCommandParameter { get; set; }
        MenuFlyout shortcutFlyout;
        public MenuFlyout ShortcutFlyout { get => shortcutFlyout; set { Set(ref shortcutFlyout, value); } }
    }
}
