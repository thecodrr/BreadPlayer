using BreadPlayer.Core.Common;
using BreadPlayer.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.UI.Xaml;

namespace BreadPlayer.NotificationManager
{
	public class BreadNotificationManager : ObservableObject, INotificationManager  
    {
        DispatcherTimer hideTimer;
        string status = "Nothing Baking";
        public string Status
        {
            get { return status; }
            set { Set(ref status, value);}
        }       
      
        public async Task ShowMessageAsync(string status)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Status = status;
                if (hideTimer == null)
                    hideTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(10) };
                hideTimer.Start();
                hideTimer.Tick += HideTimer_Tick;
            });
        }
        public async Task ShowMessageBoxAsync(string message, string title)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                var dialog = new Windows.UI.Popups.MessageDialog(message, title);
                await dialog.ShowAsync();
            });
        }
        public void SendUpcomingSongNotification(Mediafile mediaFile)
        {
            if (mediaFile != null)
            {
                ToastNotificationManager.History.Clear();
                var notificationXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText04);
                var toeastElement = notificationXml.GetElementsByTagName("text");
                toeastElement[0].AppendChild(notificationXml.CreateTextNode("Upcoming Song"));
                toeastElement[1].AppendChild(notificationXml.CreateTextNode(mediaFile.Title));
                toeastElement[2].AppendChild(notificationXml.CreateTextNode(mediaFile.LeadArtist));
                var imageElement = notificationXml.GetElementsByTagName("image");
                imageElement[0].Attributes[1].NodeValue = mediaFile.AttachedPicture ?? "ms-appx:///Assets/albumart.png";
                var toastNotification = new ToastNotification(notificationXml);
                toastNotification.Group = "upcoming-song";
                ToastNotificationManager.CreateToastNotifier().Show(toastNotification);
            }
        }
        private void HideTimer_Tick(object sender, object e)
        {
            Status = "Nothing Baking!";
            hideTimer.Stop();
        }
        
    }
}
