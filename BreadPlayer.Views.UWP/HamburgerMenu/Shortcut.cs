using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace SplitViewMenu
{
    public class Shortcut : ObservableObject
    {
        public string SymbolAsChar { get; set; }
        public BreadPlayer.DelegateCommand ShortcutCommand { get; set; }
        public object ShortcutCommandParameter { get; set; }
        MenuFlyout shortcutFlyout;
        public MenuFlyout ShortcutFlyout { get { return shortcutFlyout; } set { Set(ref shortcutFlyout, value); } }
    }
}
