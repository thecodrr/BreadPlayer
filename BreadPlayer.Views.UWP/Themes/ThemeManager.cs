using BreadPlayer.Core;
using BreadPlayer.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace BreadPlayer.Themes
{
    public class ThemeManager
    {
        private static readonly string[] brushKeys = new[]
        {
            //wp
            "PhoneAccentBrush",
            // windows
                   
                   "SystemControlDisabledAccentBrush" ,
                   "SystemControlForegroundAccentBrush" ,
                   "SystemControlHighlightAccentBrush" ,
                   "SystemControlHighlightAltAccentBrush",
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
                   "SystemAccentColor1",
                   "HoverBrush2",
                   "AppBarToggleHover",
                   "SystemControlBackgroundAccentBrush",
                   "PlaybarBrush"

        };

        public static async void SetThemeColor(string albumartPath)
        {
            await SharedLogic.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                if (SharedLogic.SettingsVM.ChangeAccentByAlbumArt == false)
                {
                    ChangeColor(GetAccentColor());
                    return;
                }
                if (App.Current.RequestedTheme == ApplicationTheme.Light && SharedLogic.SettingsVM.ChangeAccentByAlbumArt)
                {
                    try
                    {
                        Color color;
                        if (!string.IsNullOrEmpty(albumartPath) && albumartPath != "default")
                            color = await SharedLogic.GetDominantColor(await StorageFile.GetFileFromPathAsync(albumartPath));
                        else if (albumartPath == "default" && SharedLogic.Player.CurrentlyPlayingFile != null)
                            color = await SharedLogic.GetDominantColor(await StorageFile.GetFileFromPathAsync(SharedLogic.Player.CurrentlyPlayingFile.AttachedPicture));
                        else
                            color = GetAccentColor();
                        ChangeColor(color);
                    }
                    catch (Exception ex)
                    {
                        BLogger.Logger.Error("Failed to update accent.", ex);
                        await Core.SharedLogic.NotificationManager.ShowMessageAsync(ex.Message);
                    }
                    //ThemeChanged.Invoke(null, new Events.ThemeChangedEventArgs(oldColor, color));
                }
            });
        }
        private static void ChangeColor(Color color)
        {
            var oldColor = GetThemeResource<SolidColorBrush>("SystemControlBackgroundAccentBrush").Color;
            if (oldColor == color)
                return;
            ChangeTitleBarColor(color);
            GetThemeResource<SolidColorBrush>("SystemControlBackgroundAccentBrush").AnimateBrush(oldColor, color, "(SolidColorBrush.Color)");
            foreach (var brushKey in brushKeys)
            {
                if (Application.Current.Resources.ContainsKey(brushKey))
                {
                    ((SolidColorBrush)App.Current.Resources[brushKey]).Color = color;
                }
            }
        }
        private static void ChangeTitleBarColor(Color color)
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1))
            {
                var statusBar = StatusBar.GetForCurrentView();
                statusBar.BackgroundColor = color;
                statusBar.BackgroundOpacity = 1;
            }
            else
            {
                ApplicationView.GetForCurrentView().TitleBar.BackgroundColor = color;
                ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = color;
            }
        }
        private static Color GetAccentColor()
        {
            return ((Color)App.Current.Resources["SystemAccentColor"]);
        }
        private static T GetThemeResource<T>(string key)
        {
            return ((T)App.Current.Resources[key]);
        }

        // public static event OnThemeChanged ThemeChanged;
    }
    // public delegate void OnThemeChanged(object sender, Events.ThemeChangedEventArgs e);
}
