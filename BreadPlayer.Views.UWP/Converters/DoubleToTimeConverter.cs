﻿/* 
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
using Windows.Foundation.Metadata;

namespace BreadPlayer.Converters
{
	public class DoubleToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string FormattedTime = "00:00";
            if (value is double)
            {
                double time = (double)value;
                FormattedTime = TimeSpan.FromSeconds(time).ToString(@"mm\:ss");                
            }
            return FormattedTime;
        }
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class WidthToHalfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double width = (double)value;
            if (value is double)
            {
                if (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1))
                    return width;
                else
                {
                    if (Windows.UI.Core.CoreWindow.GetForCurrentThread().Bounds.Width <= 501)
                    {
                        // tag.MaxWidth = CoreWindow.GetForCurrentThread().Bounds.Width - 10;
                        //tag.ItemWidth = CoreWindow.GetForCurrentThread().Bounds.Width - 50;
                        return width;
                    }
                    return (width / 2) - 20;
                }
                    
            }
            return width;
        }
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
