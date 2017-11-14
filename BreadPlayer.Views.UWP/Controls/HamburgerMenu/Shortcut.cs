using BreadPlayer.Core.Common;
using BreadPlayer.Interfaces;
using Windows.UI.Xaml.Controls;

namespace BreadPlayer.Controls
{
    public class Shortcut : ObservableObject
    {
        public string Tooltip { get; set; }
        public string SymbolAsChar { get; set; }
        private ICommand _shortcutCommand;

        public ICommand ShortcutCommand
        {
            get => _shortcutCommand;
            set => Set(ref _shortcutCommand, value);
        }

        public object ShortcutCommandParameter { get; set; }
        private MenuFlyout _shortcutFlyout;

        public MenuFlyout ShortcutFlyout
        {
            get => _shortcutFlyout; set => Set(ref _shortcutFlyout, value);
        }
    }
}