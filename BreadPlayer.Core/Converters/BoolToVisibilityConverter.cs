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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using System.Globalization;
using Windows.UI.Xaml.Media;

namespace BreadPlayer.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool flag = false;
            if (value is bool)
            {
                flag = (bool)value;
            }
            else if (value is bool?)
            {
                bool? nullable = (bool?)value;
                flag = nullable.HasValue ? nullable.Value : false;
            }
            else if(value is int)
            {
                if (System.Convert.ToInt16(value) <= 0)
                    flag = true;
                else
                    flag = false;
            }
            else if(value is double)
            {
                if (System.Convert.ToInt16(value) <= 0)
                    flag = false;
                else
                    flag = true;
            }
            else if(value is ImageSource)
            {
                if (((ImageSource)value)== null)
                    flag = false;
                else
                    flag = true;
            }
            else if(value is string)
            {
                if (((string)value).Length <= 0)
                    flag = false;
                else
                    flag = true;
            }
            return (flag ? Visibility.Visible : Visibility.Collapsed);
        }
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
