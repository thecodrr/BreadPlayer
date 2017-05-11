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
        public Color NewColor => newColor;
        public Color OldColor => oldColor;
    }
}
