using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml.Controls.Primitives;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Macalifa.Extensions;
using Macalifa.Models;
using System.Reflection;
using Windows.Foundation;

namespace Macalifa.Behaviours
{
    public class OpenMenuFlyoutAction : DependencyObject, IAction
    {
        public object Parameter
        {
            get { return GetValue(ParameterProperty); }
            set { SetValue(ParameterProperty, value); }
        }
        public static readonly DependencyProperty ParameterProperty =
           DependencyProperty.Register("Flyout", typeof(object), typeof(OpenMenuFlyoutAction), new PropertyMetadata(null, (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
           {
           }
        ));
        public object Execute(object sender, object parameter)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            ListBoxItem item = senderElement.GetFirstAncestorOfType<ListBoxItem>();
            item.IsSelected = true;
            if (Parameter.ToString() == "BindableFlyout")
            {
                var flyout = (senderElement as Button).GetAncestorsOfType<Grid>().Where(t => t.Name == "LayoutRoot").ToList()[0].Resources["PlaylistsFlyout"] as BindableFlyout;
                flyout.ShowAt(senderElement);
            }
            else
            {
                var flyout = senderElement.Resources["Flyout"] as MenuFlyout; //.GetFirstDescendantOfType<Grid>().Resources["Flyout"] as MenuFlyout;
                flyout.ShowAt(senderElement, senderElement.GetPointerPosition());
            }
            GC.Collect();
            return null;
        }

        
    }
}
