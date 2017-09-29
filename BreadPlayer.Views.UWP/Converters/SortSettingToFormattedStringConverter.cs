using System;
using Windows.UI.Xaml.Data;

namespace BreadPlayer.Converters
{
    internal class SortSettingToFormattedStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string sort = value as string;
            switch (sort)
            {
                case "LeadArtist":
                    return "Artist";

                case "TrackNumber":
                    return "Track No.";

                case "Length":
                    return "Song Length";

                case "FolderPath":
                    return "Folder";

                case "Title":
                    return "A to Z";

                default:
                    return sort;
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            return string.Empty;
        }
    }
}