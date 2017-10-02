using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace BreadPlayer.Converters
{
    internal class StringToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string val = value.ToString();
            SymbolIcon symbol = null;
            if (val == "No Repeat")
            {
                symbol = new SymbolIcon(Symbol.Sync);
            }
            else if (val == "Repeat Song")
            {
                symbol = new SymbolIcon(Symbol.RepeatOne);
            }
            else
            {
                symbol = new SymbolIcon(Symbol.RepeatAll);
            }

            if (parameter?.ToString() == "char")
            {
                return (char)(symbol.Symbol);
            }

            return symbol;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            return value;
        }
    }
}