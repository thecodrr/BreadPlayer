using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace BreadPlayer.Converters
{
    class StringToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string val = value.ToString();
            if (val == "No Repeat")
                return new SymbolIcon(Symbol.Sync);
            else if (val == "Repeat Song")
                return new SymbolIcon(Symbol.RepeatOne);
            else
                return new SymbolIcon(Symbol.RepeatAll);
        }
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
