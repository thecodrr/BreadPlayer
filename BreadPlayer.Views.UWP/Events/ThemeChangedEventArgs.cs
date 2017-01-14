using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace BreadPlayer.Events
{
    public class ThemeChangedEventArgs
    {
        private Color oldColor;
        private Color newColor;
        public ThemeChangedEventArgs(Color old, Color newClr)
        {
            oldColor = old;
            newColor = newClr;
        }
        public Color NewColor { get { return newColor; } }
        public Color OldColor { get { return oldColor; } }
    }
}
