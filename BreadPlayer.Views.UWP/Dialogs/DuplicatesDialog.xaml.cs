using BreadPlayer.Models;
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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BreadPlayer.Dialogs
{
    public sealed partial class DuplicatesDialog : ContentDialog
    {
        public DuplicatesDialog()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
        public static readonly DependencyProperty DuplicatesProperty = DependencyProperty.Register(
         "Duplicates", typeof(IEnumerable<Mediafile>), typeof(DuplicatesDialog), new PropertyMetadata(null));
        public IEnumerable<Mediafile> Duplicates
        {
            get { return (IEnumerable<Mediafile>)GetValue(DuplicatesProperty); }
            set { SetValue(DuplicatesProperty, value); }
        }
        public static readonly DependencyProperty SelectedDuplicatesProperty = DependencyProperty.Register(
         "SelectedDuplicates", typeof(List<Mediafile>), typeof(DuplicatesDialog), new PropertyMetadata(null));
        public List<Mediafile> SelectedDuplicates
        {
            get { return (List<Mediafile>)GetValue(SelectedDuplicatesProperty); }
            set { SetValue(SelectedDuplicatesProperty, value); }
        }
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            SelectedDuplicates = new List<Mediafile>(this.listView.SelectedItems.Cast<Mediafile>());
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
