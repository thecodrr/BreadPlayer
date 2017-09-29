using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace BreadPlayer.Extensions
{
    public static class DependencyObjectExtensions
    {
        public static void AnimateBrush(this DependencyObject objAnimate, Color fromColor, Color toColor, string propPath)
        {
            ColorAnimation col = new ColorAnimation()
            {
                From = fromColor,// ((SolidColorBrush)App.Current.Resources["SystemControlBackgroundAccentBrush"]).Color;
                To = toColor,
                Duration = new Duration(TimeSpan.FromSeconds(2))
            };
            Storyboard zgo = new Storyboard();
            Storyboard.SetTarget(col, objAnimate);// (SolidColorBrush)App.Current.Resources["SystemControlBackgroundAccentBrush"]);
            Storyboard.SetTargetProperty(col, propPath);// "(SolidColorBrush.Color)");
            zgo.Children.Add(col);
            zgo.Begin();
        }

        public static void ZoomAnimate(this DependencyObject obj, int from, int to, string targetPath)
        {
            Storyboard board = new Storyboard();
            var zoomAnimate = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = TimeSpan.FromMilliseconds(200),
                FillBehavior = FillBehavior.HoldEnd,
                EnableDependentAnimation = true
            };
            Storyboard.SetTarget(zoomAnimate, obj);// (SolidColorBrush)App.Current.Resources["SystemControlBackgroundAccentBrush"]);
            Storyboard.SetTargetProperty(zoomAnimate, targetPath);// "(SolidColorBrush.Color)");
            board.Children.Add(zoomAnimate);
            board.Begin();
            board.Completed += (send, eventArgs) => { board = null; };
        }
    }
}