using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using BreadPlayer.Common;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BreadPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WelcomeSplashScreen : Page
    {
        Frame frame;
        public WelcomeSplashScreen(Frame pageFrame)
        {
            this.InitializeComponent();
            frame = pageFrame;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RoamingSettingsHelper.SaveSetting("FirstRun", false);
            frame.Navigate(typeof(Shell));
        }
    }
}
