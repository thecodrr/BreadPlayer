using BreadPlayer.SettingsViews.ViewModels;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BreadPlayer.SettingsViews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContactView : Page
    {
        ContactViewModel contactVM;
        public ContactView()
        {
            this.InitializeComponent();
            contactVM = new ContactViewModel();
            this.DataContext = contactVM;
        }

        private void RichEditBox_TextChanged(object sender, RoutedEventArgs e)
        {
            var richEditBox = sender as RichEditBox;
            richEditBox.Document.GetText(Windows.UI.Text.TextGetOptions.None, out string text);
            contactVM.Content = text;
        }
    }
}
