using BreadPlayer.SettingsViews.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BreadPlayer.SettingsViews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContactView : CachedPage
    {
        private ContactViewModel contactVM;

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