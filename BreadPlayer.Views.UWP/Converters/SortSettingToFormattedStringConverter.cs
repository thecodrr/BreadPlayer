using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace BreadPlayer.Converters
{
    class SortSettingToFormattedStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string sort = value as String;
            if (sort == "LeadArtist")
            {
                return "Artist";
            }
            else if (sort == "TrackNumber")
            {
                return "Track No.";
            }
            else if (sort == "Length")
            {
                return "Song Length";
            }
            else if (sort == "FolderPath")
            {
                return "Folder";
            }
            else if(sort == "Title")
            {
                return "A to Z";
            }
            else
                return sort;
        }
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            return string.Empty;
        }
    }
}
