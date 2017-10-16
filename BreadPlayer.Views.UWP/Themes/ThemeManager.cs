using BreadPlayer.Common;
using BreadPlayer.Core;
using BreadPlayer.Dispatcher;
using BreadPlayer.Extensions;
using BreadPlayer.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace BreadPlayer.Themes
{
    public class ThemeManager : ObservableObject
    {
        private static readonly string[] BrushKeys = {
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
                   "SystemControlBackgroundAccentBrush",
                   "PlaybarBrush"
        };

        public static async void SetThemeColor(string albumartPath)
        {
            await BreadDispatcher.InvokeAsync(async () =>
            {
                if (!SharedLogic.Instance.VerifyFileExists(albumartPath, 100))
                    return;
                if (SharedLogic.Instance.SettingsVm.PersonalizationVM.ChangeAccentByAlbumArt == false || albumartPath == null)
                {
                    ChangeColor(GetAccentColor());
                    return;
                }
                if (SettingsHelper.GetLocalSetting<string>("SelectedTheme", "Light") == "Light" && SharedLogic.Instance.SettingsVm.PersonalizationVM.ChangeAccentByAlbumArt)
                {
                    try
                    {
                        Color color;
                        if (!string.IsNullOrEmpty(albumartPath) && albumartPath != "default")
                        {
                            color = await SharedLogic.Instance.GetDominantColor(await StorageFile.GetFileFromPathAsync(albumartPath));
                        }
                        else if (albumartPath == "default" && SharedLogic.Instance.Player.CurrentlyPlayingFile != null)
                        {
                            color = await SharedLogic.Instance.GetDominantColor(await StorageFile.GetFileFromPathAsync(SharedLogic.Instance.Player.CurrentlyPlayingFile.AttachedPicture));
                        }
                        else
                        {
                            color = GetAccentColor();
                        }

                        ChangeColor(color);
                    }
                    catch (Exception ex)
                    {
                        BLogger.E("Failed to update accent.", ex);
                        await SharedLogic.Instance.NotificationManager.ShowMessageAsync(ex.Message);
                    }
                    //ThemeChanged?.Invoke(null, new Events.ThemeChangedEventArgs(oldColor, color));
                }
                else
                {
                    ChangeColor(GetAccentColor());
                }
            });
        }

        private static void ChangeColor(Color color)
        {
            ChangeTitleBarColor(color);
            var oldColor = GetThemeResource<SolidColorBrush>("PlaybarBrush").Color;
            if (oldColor == color)
            {
                return;
            }

            GetThemeResource<SolidColorBrush>("PlaybarBrush").AnimateBrush(oldColor, color, "(SolidColorBrush.Color)");
            AdjustForeground(color);
            foreach (var brushKey in BrushKeys)
            {
                if (Application.Current.Resources.ContainsKey(brushKey))
                {
                    ((SolidColorBrush)Application.Current.Resources[brushKey]).Color = color;
                }
            }
        }

        private static void ChangeTitleBarColor(Color color)
        {
            if (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1))
            {
                var statusBar = StatusBar.GetForCurrentView();
                statusBar.BackgroundColor = color;
                statusBar.BackgroundOpacity = 1;
            }
            else
            {
                ApplicationView.GetForCurrentView().TitleBar.BackgroundColor = Colors.Transparent;
                ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Colors.Transparent;
                ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = Colors.Black;
                ApplicationView.GetForCurrentView().TitleBar.ButtonHoverBackgroundColor = color;
                ApplicationView.GetForCurrentView().TitleBar.ButtonPressedForegroundColor = Colors.White;
            }
        }

        private static Color GetAccentColor()
        {
            return ((Color)Application.Current.Resources["SystemAccentColor"]);
        }

        private static T GetThemeResource<T>(string key)
        {
            return ((T)Application.Current.Resources[key]);
        }

        private static void AdjustForeground(Color accentColor)
        {
            GetThemeResource<SolidColorBrush>("TextBrush").Color = accentColor.ToForeground(); //.AnimateBrush(foregroundColor.Color, foreg, "(SolidColorBrush.Color)");
            GetThemeResource<SolidColorBrush>("AccentHoverBrush").Color = accentColor.ToHoverColor();
        }
    }
}