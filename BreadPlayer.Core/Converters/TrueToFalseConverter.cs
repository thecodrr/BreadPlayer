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
using Windows.UI.Xaml.Data;

namespace BreadPlayer.Converters
{
	public class TrueToFalseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is Windows.UI.Xaml.Controls.SplitViewDisplayMode)
            { 
            Windows.UI.Xaml.Controls.SplitViewDisplayMode mode = (Windows.UI.Xaml.Controls.SplitViewDisplayMode)value;
            if (mode == Windows.UI.Xaml.Controls.SplitViewDisplayMode.Overlay || mode == Windows.UI.Xaml.Controls.SplitViewDisplayMode.CompactOverlay)
                return true;
            else
                return false;
            }
            else if(value is bool)
            {
                if (parameter == null)
                {
                    if ((bool)value == true)
                        return false;
                    else
                        return true;
                }
            }
           
            return false;
        }
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class BrushToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Windows.UI.Xaml.Visibility mode = (Windows.UI.Xaml.Visibility)value;

            return mode.ToString();
        }
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
