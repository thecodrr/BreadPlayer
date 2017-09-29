using Windows.UI;

namespace BreadPlayer.Events
{
    public class ThemeChangedEventArgs
    {
        private Color _oldColor;
        private Color _newColor;

        public ThemeChangedEventArgs(Color old, Color newClr)
        {
            _oldColor = old;
            _newColor = newClr;
        }

        public Color NewColor => _newColor;
        public Color OldColor => _oldColor;
    }
}