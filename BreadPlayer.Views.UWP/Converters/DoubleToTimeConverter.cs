/*
	BreadPlayer. A music player made for Windows 10 store.
    Copyright (C) 2016  theweavrs (Abdullah Atta)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.Xaml.Data;

namespace BreadPlayer.Converters
{
    public class DoubleToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string formattedTime = "00:00";
            if (value is double time)
            {
                if (TimeSpan.FromSeconds(time).TotalMinutes > 60)
                {
                    formattedTime = TimeSpan.FromSeconds(time).ToString(@"hh\:mm\:ss");
                }
                else
                {
                    formattedTime = TimeSpan.FromSeconds(time).ToString(@"mm\:ss");
                }
            }
            return formattedTime;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            return string.Empty;
        }
    }

    public class WidthToHalfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double width)
            {
                if (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1))
                {
                    return width;
                }

                if (CoreWindow.GetForCurrentThread().Bounds.Width <= 501)
                {
                    // tag.MaxWidth = CoreWindow.GetForCurrentThread().Bounds.Width - 10;
                    //tag.ItemWidth = CoreWindow.GetForCurrentThread().Bounds.Width - 50;
                    return width;
                }
                return (width / 2) - 20;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            return 0;
        }
    }
}