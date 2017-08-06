using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BreadPlayer.Core.Models;
using BreadPlayer.ViewModels;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BreadPlayer.Dialogs
{
    public sealed partial class TagDialog : ContentDialog
    {
        public static readonly DependencyProperty MediafileProperty = DependencyProperty.Register(
             "Mediafile", typeof(Mediafile), typeof(TagDialog), new PropertyMetadata(null));
        public Mediafile Mediafile
        {
            get => (Mediafile)GetValue(MediafileProperty);
            set => SetValue(MediafileProperty, value);
        }
        public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register(
            "ItemWidth", typeof(double), typeof(TagDialog), new PropertyMetadata(null));
        public double ItemWidth
        {
            get => (double)GetValue(ItemWidthProperty);
            set => SetValue(ItemWidthProperty, value);
        }
        public TagDialog()
        {
           InitializeComponent();
        }
        public TagDialog(Mediafile file)
        {
            InitializeComponent();
            Mediafile = file;
            ToggleShuffle.IsOn = Mediafile.SkipOnShuffle;
        }
        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ToggleShuffle_OnToggled(object sender, RoutedEventArgs e)
        {
            Mediafile.SkipOnShuffle = ((ToggleSwitch) sender).IsOn;
            ShellViewModel.SaveSettings(Mediafile);
        }
    }
}
