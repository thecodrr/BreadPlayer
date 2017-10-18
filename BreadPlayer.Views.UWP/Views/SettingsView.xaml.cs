using BreadPlayer.Models;
using BreadPlayer.Services;
using Microsoft.Advertising.WinRT.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BreadPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsView : Page
    {
        
        public SettingsView()
        {
            InitializeComponent();
        }

        private void OnSettingClicked(object sender, ItemClickEventArgs e)
        {
            NavigationService.Instance.Frame.Navigate((e.ClickedItem as SettingGroup).Page, e.ClickedItem);
        }
    }
}