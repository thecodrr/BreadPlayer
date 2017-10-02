using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace BreadPlayer.Converters
{
    public class SelectedItemToValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //if (value is string)
            //{
            //    var combobox = parameter as ComboBox;
            //    var toSelectItem = combobox.Items.FirstOrDefault(t => (t as ComboBoxItem).Content == value) as ComboBoxItem;
            //    return toSelectItem;
            //}
            return value;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            if (value is ComboBoxItem comboBox)
            {
                return comboBox.Content.ToString();
            }
            return 0;
        }
    }
}