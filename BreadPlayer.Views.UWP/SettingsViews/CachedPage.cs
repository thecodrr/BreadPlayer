using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace BreadPlayer.SettingsViews
{
    public class CachedPage : Page
    {
        public CachedPage()
        {
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;
        }
    }
}
