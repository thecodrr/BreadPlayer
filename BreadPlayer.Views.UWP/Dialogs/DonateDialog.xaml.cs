using BreadPlayer.Core;
using BreadPlayer.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Store;
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
    public sealed partial class DonateDialog : ContentDialog
    {
        public DonateDialog()
        {
            this.InitializeComponent();
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            string productID = null;
            var shellVM = App.Current.Resources["ShellVM"] as ShellViewModel;
            if (watchAnAdRadioBtn.IsChecked == true)
                shellVM.WatchAnAdCommand.Execute(null);
            else if (donate1.IsChecked == true)
            {
                productID = "Donate2Dollars";
            }
            else if (donate2.IsChecked == true)
            {
                productID = "Donate5Dollars";
            }
            else if (donate3.IsChecked == true)
            {
                productID = "Donate10Dollars";
            }
            if (productID != null)
            {
                PurchaseResults purchaseResults = await CurrentAppSimulator.RequestProductPurchaseAsync(productID);
                switch (purchaseResults.Status)
                {
                    case ProductPurchaseStatus.Succeeded:
                        var transId = purchaseResults.TransactionId;
                        await SharedLogic.Instance.NotificationManager.ShowMessageAsync("Thanks for the donation! Love you!");
                        await CurrentAppSimulator.ReportConsumableFulfillmentAsync(productID, transId);
                        break;
                }
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Hide();
        }
    }
}
