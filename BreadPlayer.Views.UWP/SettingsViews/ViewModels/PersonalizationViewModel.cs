using BreadPlayer.Common;
using BreadPlayer.Core;
using BreadPlayer.Helpers;
using BreadPlayer.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.SettingsViews.ViewModels
{
    public class PersonalizationViewModel : ObservableObject
    {
        private bool _enableBlur;

        private bool _isThemeDark;

        private bool _changeAccentByAlbumart;

        private string _backgroundOverlayColor;
        public PersonalizationViewModel()
        {
            _changeAccentByAlbumart = SettingsHelper.GetRoamingSetting<bool>("ChangeAccentByAlbumArt", true);
            _isThemeDark = SettingsHelper.GetLocalSetting<string>("SelectedTheme", null) == "Light" ? true : false;
            _enableBlur = SettingsHelper.GetLocalSetting<bool>("EnableBlur", !InitializeSwitch.IsMobile);
            _backgroundOverlayColor = SettingsHelper.GetLocalSetting<string>("BackgroundOverlayColor", "Album art color");
        }

        public string BackgroundOverlayColor
        {
            get => _backgroundOverlayColor;
            set
            {
                Set(ref _backgroundOverlayColor, value);
                SettingsHelper.SaveLocalSetting("BackgroundOverlayColor", value);
            }
        }

        public bool EnableBlur
        {
            get => _enableBlur;
            set
            {
                Set(ref _enableBlur, value);
                SettingsHelper.SaveLocalSetting("EnableBlur", value);
            }
        }
        public bool IsThemeDark
        {
            get => _isThemeDark;
            set
            {
                Set(ref _isThemeDark, value);
                SettingsHelper.SaveLocalSetting("SelectedTheme", _isThemeDark ? "Light" : "Dark");
                // SharedLogic.InitializeTheme();
            }
        }
        
        public bool ChangeAccentByAlbumArt
        {
            get => _changeAccentByAlbumart;
            set
            {
                Set(ref _changeAccentByAlbumart, value);
                if (value == false)
                {
                    ThemeManager.SetThemeColor(null);
                }
                else
                {
                    ThemeManager.SetThemeColor("default");
                }

                SettingsHelper.SaveRoamingSetting("ChangeAccentByAlbumArt", _changeAccentByAlbumart);
            }
        }
    }
}
