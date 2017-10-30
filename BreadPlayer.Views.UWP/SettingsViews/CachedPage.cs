using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace BreadPlayer.SettingsViews
{
    public class CachedPage : Page
    {
        public CachedPage()
        {
            NavigationCacheMode = NavigationCacheMode.Required;
        }
    }
}
