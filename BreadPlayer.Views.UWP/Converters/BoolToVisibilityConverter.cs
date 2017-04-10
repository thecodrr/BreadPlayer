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

namespace BreadPlayer.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool flag = false;
            if (value is bool boolValue)
            {
                flag = boolValue;
            }
            else if (value is bool?)
            {
                flag = (bool?)value ?? false;
            }
            else if (value is int)
            {
                flag = System.Convert.ToInt16(value) <= 1;
            }
            else if (value is double)
            {
                flag = System.Convert.ToInt16(value) > 1;
            }
            else if (value is ImageSource imageSource)
            {
                flag = imageSource != null;
            }
            else if(value is string stringValue)
            {
                flag = stringValue.Length > 0;
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
