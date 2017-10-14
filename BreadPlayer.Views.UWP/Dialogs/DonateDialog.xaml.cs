using BreadPlayer.Core;
using BreadPlayer.Services;
using BreadPlayer.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Services.Store;
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

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            string productID = null;
            var shellVM = App.Current.Resources["ShellVM"] as ShellViewModel;
            if (watchAnAdRadioBtn.IsChecked == true)
                shellVM.WatchAnAdCommand.Execute(null);
            else if (donate1.IsChecked == true)
            {
                productID = "9p85s7w23ghc";
            }
            else if (donate2.IsChecked == true)
            {
                productID = "9p7gkmjj249c";
            }
            else if (donate3.IsChecked == true)
            {
                productID = "9pddqfm4rqtq";
            }
            if (productID != null)
            {
                ConsumeAddOn(productID, 1);
            }
        }
        private StoreContext context = null;

        public async void ConsumeAddOn(string addOnStoreId, uint quantity)
        {
            if (context == null)
            {
                context = StoreContext.GetDefault();
            }

            Guid trackingId = Guid.NewGuid();
            
            StoreConsumableResult result = await context.ReportConsumableFulfillmentAsync(
                addOnStoreId.ToUpperInvariant(), quantity, trackingId);

            // Capture the error message for the operation, if any.
            string extendedError = string.Empty;
            if (result.ExtendedError != null)
            {
                extendedError = result.ExtendedError.Message;
            }

            switch (result.Status)
            {
                case StoreConsumableStatus.Succeeded:
                    await SharedLogic.Instance.NotificationManager.ShowMessageAsync("Thanks for the donation! Love you!");
                    this.Hide();
                    SplitViewMenu.SplitViewMenu.SelectPrevious();
                    break;
                case StoreConsumableStatus.NetworkError:
                    await SharedLogic.Instance.NotificationManager.ShowMessageAsync("The fulfillment was unsuccessful due to a network error. " +
                        "ExtendedError: " + extendedError);
                    break;
                case StoreConsumableStatus.ServerError:
                    await SharedLogic.Instance.NotificationManager.ShowMessageAsync("The fulfillment was unsuccessful due to a server error. " +
                        "ExtendedError: " + extendedError);
                    break;
                default:
                    await SharedLogic.Instance.NotificationManager.ShowMessageAsync("The fulfillment was unsuccessful due to an unknown error. " +
                         "ExtendedError: " + extendedError);
                    break;
            }
        }
        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Hide();
            SplitViewMenu.SplitViewMenu.SelectPrevious();
        }
    }
}
