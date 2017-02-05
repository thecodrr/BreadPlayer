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

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BreadPlayer.Dialogs
{
    public sealed partial class InputDialog : ContentDialog
    {
        public InputDialog()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
           "Text", typeof(string), typeof(InputDialog), new PropertyMetadata(null));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
           "Description", typeof(string), typeof(InputDialog), new PropertyMetadata(null));
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register(
          "Password", typeof(string), typeof(InputDialog), new PropertyMetadata(null));
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }
        public static readonly DependencyProperty IsPrivateProperty = DependencyProperty.Register(
        "IsPrivate", typeof(bool), typeof(InputDialog), new PropertyMetadata(null));
        public bool IsPrivate
        {
            get { return (bool)GetValue(IsPrivateProperty); }
            set { SetValue(IsPrivateProperty, value); }
        }
        public static readonly DependencyProperty DialogWidthProperty = DependencyProperty.Register(
         "DialogWidth", typeof(double), typeof(InputDialog), new PropertyMetadata(null));
        public double DialogWidth
        {
            get { return (double)GetValue(DialogWidthProperty); }
            set { SetValue(DialogWidthProperty, value); }
        }
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
