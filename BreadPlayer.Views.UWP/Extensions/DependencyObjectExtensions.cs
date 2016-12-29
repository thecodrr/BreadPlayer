using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace BreadPlayer.Extensions
{
    public static class DependencyObjectExtensions
    {
        public static void AnimateBrush(this DependencyObject objAnimate, Color fromColor, Color toColor, string propPath)
        {
            ColorAnimation col = new ColorAnimation();
            col.From = fromColor;// ((SolidColorBrush)App.Current.Resources["SystemControlBackgroundAccentBrush"]).Color;
            col.To = toColor;
            col.Duration = new Duration(TimeSpan.FromSeconds(1));

            Storyboard zgo = new Storyboard();
            Storyboard.SetTarget(col, objAnimate);// (SolidColorBrush)App.Current.Resources["SystemControlBackgroundAccentBrush"]);
            Storyboard.SetTargetProperty(col, propPath);// "(SolidColorBrush.Color)");
            zgo.Children.Add(col);
            zgo.Begin();
        }
    }
}
