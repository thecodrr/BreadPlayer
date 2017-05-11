using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BreadPlayer.Core.Models;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BreadPlayer.Dialogs
{
    public sealed partial class DuplicatesDialog : ContentDialog
    {
        public DuplicatesDialog()
        {
            InitializeComponent();
            DataContext = this;
        }
        public static readonly DependencyProperty DuplicatesProperty = DependencyProperty.Register(
         "Duplicates", typeof(IEnumerable<Mediafile>), typeof(DuplicatesDialog), new PropertyMetadata(null));
        public IEnumerable<Mediafile> Duplicates
        {
            get => (IEnumerable<Mediafile>)GetValue(DuplicatesProperty);
            set => SetValue(DuplicatesProperty, value);
        }
        public static readonly DependencyProperty SelectedDuplicatesProperty = DependencyProperty.Register(
         "SelectedDuplicates", typeof(List<Mediafile>), typeof(DuplicatesDialog), new PropertyMetadata(null));
        public List<Mediafile> SelectedDuplicates
        {
            get => (List<Mediafile>)GetValue(SelectedDuplicatesProperty);
            set => SetValue(SelectedDuplicatesProperty, value);
        }
        public static readonly DependencyProperty DialogWidthProperty = DependencyProperty.Register(
        "DialogWidth", typeof(double), typeof(DuplicatesDialog), new PropertyMetadata(null, (sender, e) => 
        {
            var dialog = sender as DuplicatesDialog;
            var width = (double)e.NewValue;
            if (width < 501)
            {
                dialog.listView.ItemContainerStyle = dialog.Resources["CenterAlignedStyle"] as Style;
                dialog.listView.ItemTemplate = Application.Current.Resources["MediafileUnselectedMobileTemplate"] as DataTemplate;
            }
            else
            {
                dialog.listView.ItemTemplate = Application.Current.Resources["MediafileUnselectedNarrowTemplate"] as DataTemplate;
            }
        }));
        public double DialogWidth
        {
            get => (double)GetValue(DialogWidthProperty);
            set => SetValue(DialogWidthProperty, value);
        }
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            SelectedDuplicates = new List<Mediafile>(listView.SelectedItems.Cast<Mediafile>());
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
            {
                listView.SelectAll();
            }
            else
            {
                listView.SelectedIndex = -1;
            }
        }
    }
}
