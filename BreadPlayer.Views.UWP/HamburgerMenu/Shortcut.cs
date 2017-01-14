using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SplitViewMenu
{
    public class Shortcut
    {
        public string SymbolAsChar { get; set; }
        public ICommand ShortcutCommand { get; set; }
        public object ShortcutCommandParameter { get; set; }
    }
}
