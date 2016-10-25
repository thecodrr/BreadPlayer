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

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BreadPlayer.Dialogs
{
    public sealed partial class TagDialog : ContentDialog
    {
        public static readonly DependencyProperty MediafileProperty = DependencyProperty.Register(
             "Mediafile", typeof(Mediafile), typeof(TagDialog), new PropertyMetadata(null));
        public Mediafile Mediafile
        {
            get { return (Mediafile)GetValue(MediafileProperty); }
            set { SetValue(MediafileProperty, value); }
        }
        public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register(
            "ItemWidth", typeof(double), typeof(TagDialog), new PropertyMetadata(null));
        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }
        public TagDialog()
        {
            this.InitializeComponent();
        }
        public TagDialog(Mediafile file)
        {
            InitializeComponent();
            Mediafile = file;
        }
        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
