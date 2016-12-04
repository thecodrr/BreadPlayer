using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace BreadPlayer.Themes
{
    public static class ThemeManager
    {
        private static readonly string[] brushKeys = new[]
        {
            //wp
            "PhoneAccentBrush",
            // windows
                   
                   "SystemControlBackgroundAccentBrush",
                   "SystemControlDisabledAccentBrush" ,
                   "SystemControlForegroundAccentBrush" ,
                   "SystemControlHighlightAccentBrush" ,
                   "SystemControlHighlightAltAccentBrush" ,
                   "SystemControlHighlightAltListAccentHighBrush" ,
                   "SystemControlHighlightAltListAccentLowBrush" ,
                   "SystemControlHighlightAltListAccentMediumBrush",
                   "SystemControlHighlightListAccentHighBrush" ,
                   "SystemControlHighlightListAccentLowBrush" ,
                   "SystemControlHighlightListAccentMediumBrush",
                   "SystemControlHyperlinkTextBrush" ,
                   "ContentDialogBorderThemeBrush" ,
                   "JumpListDefaultEnabledBackground" ,
                   "HoverBrush" ,
                   "AppBarToggleHover",

        };


        public static void SetThemeColor(Color color)
        {
            App.Current.Resources["SystemAccentColor"] = color;
            foreach (var brushKey in brushKeys)
            {
                if (Application.Current.Resources.ContainsKey(brushKey))
                {
                    ((SolidColorBrush)App.Current.Resources[brushKey]).Color = color;
                }
            }

#if WINDOWS_PHONE_APP
            var statusBar = StatusBar.GetForCurrentView();
            statusBar.ForegroundColor = color;
#else
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = color;
            titleBar.ButtonBackgroundColor = color;           
#endif

        }

        public static Color GetThemeColor()
        {
            return ((SolidColorBrush)App.Current.Resources["SystemControlBackgroundAccentBrush"]).Color;
        }


    }
}
