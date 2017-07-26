using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace BreadPlayer.Extensions
{
    public static class ColorExtensions
    {
        public static bool IsDark(this Color accentColor)
        {
            var brightness = (int)Math.Sqrt(
               accentColor.R * accentColor.R * .241 +
               accentColor.G * accentColor.G * .691 +
               accentColor.B * accentColor.B * .068);
            return brightness < 130; // 
        }
        public static Color ToForeground(this Color color)
        {
            return IsDark(color) ? Colors.White : Color.FromArgb(255.ToByte(), 11.ToByte(), 11.ToByte(), 11.ToByte());
        }
        public static Color ToHoverColor(this Color color)
        {
            return IsDark(color) ? Colors.LightGray : Colors.DarkGray;
        }
    }
}
