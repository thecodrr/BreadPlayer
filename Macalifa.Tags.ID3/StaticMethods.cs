using System;
using System.Collections.Generic;
using System.Text;
using Macalifa.Tags.ID3;

namespace Macalifa.Tags.ID3
{
    /// <summary>
    /// Static methods to use in application
    /// </summary>
    public static class StaticMethods
    {
        /// <summary>
        /// Get System.Text.Encoding according to specified TextEncoding argument
        /// </summary>
        /// <param name="TEncoding">TextEncoding to indicate</param>
        /// <returns>System.Text.Encoding</returns>
        public static Encoding GetEncoding(TextEncodings TEncoding)
        {
            switch (TEncoding)
            {
                case TextEncodings.UTF_16:
                    return Encoding.Unicode;
                case TextEncodings.UTF_16BE:
                    return Encoding.GetEncoding("UTF-16BE");
                case TextEncodings.UTF8:
                    return Encoding.UTF8;
                default:
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    return Encoding.GetEncoding("windows-1252");
            }
        }

        /// <summary>
        /// Indicate if specified text is Ascii
        /// </summary>
        /// <param name="Text">Text to detect</param>
        /// <returns>true if Text was ascii otherwise false</returns>
        public static bool IsAscii(string Text)
        {
            return (Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(Text)) == Text);
        }

    }
}
