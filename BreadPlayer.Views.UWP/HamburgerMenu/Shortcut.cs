using BreadPlayer;
using Windows.UI.Xaml.Controls;

namespace SplitViewMenu
{
    public class Shortcut : ObservableObject
    {
        public string SymbolAsChar { get; set; }
        public ICommand ShortcutCommand
        {
            get;
            set;
        }
        public object ShortcutCommandParameter { get; set; }
        MenuFlyout shortcutFlyout;
        public MenuFlyout ShortcutFlyout { get { return shortcutFlyout; } set { Set(ref shortcutFlyout, value); } }
    }
}
