﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace BreadPlayer.Extensions
{
    public static class SolidColorBrushExtensions
    {
        public static void FadeInOutBrush(this SolidColorBrush oldBrush, Color newColor, double to)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(5);
            timer.Start();
            timer.Tick += (sender, e) =>
            {
                if (oldBrush.Opacity > to)
                    oldBrush.Opacity -= 0.01;
                else if (oldBrush.Opacity <= to)
                {
                    timer.Stop();
                    oldBrush.Opacity = to;
                    (oldBrush).Color = newColor;
                    FadeInBrush(oldBrush, timer, 1);
                }
            };
        }

        static void FadeInBrush(this SolidColorBrush brush, DispatcherTimer timer, double to)
        {
            timer.Start();
            timer.Tick += (sender, e) =>
            {
                if (brush.Opacity < 1)
                    brush.Opacity += 0.01;
                else if (brush.Opacity >= to)
                {
                    timer.Stop();
                    brush.Opacity = 1;
                }
            };
        }
    }
}
