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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace BreadPlayer.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool flag = false;
            switch (value)
            {
                case bool boolValue:
                    flag = boolValue;
                    break;

                case int intValue:
                    flag = (Int16)intValue < 1;
                    break;

                case double doubleValue:
                    flag = (Int16)doubleValue > 0;
                    break;

                case ImageSource imageSource:
                    flag = ((BitmapImage)imageSource).UriSource == null;
                    break;

                case string stringValue:
                    flag = stringValue.Length < 0 || stringValue == null;
                    break;

                case null:
                    flag = true;
                    break;

                default:
                    flag = true;
                    break;
            }
            if(parameter?.ToString() == "Inverse")
            {
                flag = !flag;
            }
            return (flag ? Visibility.Visible : Visibility.Collapsed);
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            return (Visibility)value != Visibility.Collapsed;
        }
    }
}