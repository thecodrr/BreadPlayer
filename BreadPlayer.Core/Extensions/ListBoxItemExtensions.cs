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
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BreadPlayer.Extensions
{
    /// <summary>
    /// Attached properties for use with a ListBoxItem.
    /// </summary>
    /// <remarks>
    /// Note that the IsEnabled property is to be used on an element inside of a ListBox and not the ListBoxItem itself.
    /// </remarks>
    public static class ListBoxItemExtensions
    {

        #region IsSelected
        /// <summary>
        /// IsSelected Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.RegisterAttached(
                "IsSelected",
                typeof(bool),
                typeof(ListBoxItemExtensions),
                new PropertyMetadata(false, OnIsSelectedChanged));

        /// <summary>
        /// Gets the IsSelected property. This dependency property 
        /// indicates whether the first ListBoxItem found in ancestors is selected.
        /// </summary>
        /// <remarks>
        /// Note that the IsSelected property is to be used on an element inside of a ListBox and not the ListBoxItem itself.
        /// Setting this property will update the IsSelected property of the ListBoxItem making it easier to
        /// disable selection of ListBoxItems in the databound collection scenarios.
        /// </remarks>
        public static bool GetIsSelected(DependencyObject d)
        {
            return (bool)d.GetValue(IsSelectedProperty);
        }

        /// <summary>
        /// Sets the IsSelected property. This dependency property 
        /// indicates whether the first ListBoxItem found in ancestors is selected.
        /// </summary>
        /// <remarks>
        /// Note that the IsSelected property is to be used on an element inside of a ListBox and not the ListBoxItem itself.
        /// Setting this property will update the IsSelected property of the ListBoxItem making it easier to
        /// disable selection of ListBoxItems in the databound collection scenarios.
        /// </remarks>
        public static void SetIsSelected(DependencyObject d, bool value)
        {
            d.SetValue(IsSelectedProperty, value);
        }

        /// <summary>
        /// Handles changes to the IsSelected property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static async void OnIsSelectedChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool oldIsSelected = (bool)e.OldValue;
            bool newIsSelected = (bool)d.GetValue(IsSelectedProperty);

            //if (!d.IsInVisualTree())
            //    await ((FrameworkElement)d).WaitForLoadedAsync();

            var ListBoxItem =
                d as ListBoxItem ??
                d.GetAncestors().OfType<ListBoxItem>().FirstOrDefault();
            if (ListBoxItem == null)
                return;
            //throw new InvalidOperationException("ListBoxItemExtensions.IsSelected can only be set on a ListBoxItem or its descendant in the visual tree");
            ListBoxItem.IsSelected = newIsSelected;
        }
        #endregion
    }
}