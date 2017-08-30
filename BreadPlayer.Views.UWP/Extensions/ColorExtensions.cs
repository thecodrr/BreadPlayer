using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace BreadPlayer.Extensions
{
    public static class ColorExtensions
    {
        public static string ToHexString(this Color c)
        {
            return "#" + c.A.ToString("X2") + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
      
        private static Color FromHexString(this string hexColor)
        {
            //Remove # if present
            if (hexColor.IndexOf('#') != -1)
                hexColor = hexColor.Replace("#", "");

            byte alpha = 0;
            byte red = 0;
            byte green = 0;
            byte blue = 0;
            
            if (hexColor.Length == 8)
            {
                //#AARRGGBB
                alpha = byte.Parse(hexColor.Substring(0, 2), NumberStyles.AllowHexSpecifier);
                red = byte.Parse(hexColor.Substring(2,2), NumberStyles.AllowHexSpecifier);
                green = byte.Parse(hexColor.Substring(4, 2), NumberStyles.AllowHexSpecifier);
                blue = byte.Parse(hexColor.Substring(6, 2), NumberStyles.AllowHexSpecifier);
            }
            else if (hexColor.Length == 3)
            {
                //#RGB
                red = byte.Parse(hexColor[0].ToString() + hexColor[0].ToString(), NumberStyles.AllowHexSpecifier);
                green = byte.Parse(hexColor[1].ToString() + hexColor[1].ToString(), NumberStyles.AllowHexSpecifier);
                blue = byte.Parse(hexColor[2].ToString() + hexColor[2].ToString(), NumberStyles.AllowHexSpecifier);
            }

            return Color.FromArgb(alpha,red, green, blue);
        }
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
            return IsDark(color) ? Colors.White : Color.FromArgb(255, 11, 11, 11);
        }
        public static Color ToHoverColor(this Color color)
        {
            return IsDark(color) ? Colors.LightGray : Color.FromArgb(255, 59,59,59);
        }
    }
}
