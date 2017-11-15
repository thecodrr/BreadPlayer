using BreadPlayer.Core.Common;
using BreadPlayer.Core.Models;
using BreadPlayer.Dispatcher;
using BreadPlayer.Interfaces;
using BreadPlayer.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Data.Xml.Dom;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace BreadPlayer.NotificationManager
{
    public class BreadNotificationManager : ObservableObject, INotificationManager
    {
        private Queue<string> NotificationQueue => GSingleton<Queue<string>>.Instance.Singleton;
        private ICommand _closeCommand;
        private DispatcherTimer _hideTimer;
        private string _status = string.Empty;
        private StatusBar statusBar;
        public ICommand CloseCommand
        {
            get => _closeCommand ?? (_closeCommand = new DelegateCommand(HideStaticMessage));
        }
        public string Status
        {
            get => _status;
            set => Set(ref _status, value);
        }

        private bool _show;

        public bool Show
        {
            get => _show;
            set => Set(ref _show, value);
        }
        public void HideStaticMessage()
        {
            HideTimer_Tick(_hideTimer, null);
        }
        public void ShowStaticMessage(string message)
        {
            Status = message;
            Show = true;
        }
        public async Task ShowMessageAsync(string status, int duration = 10)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (!Show)
                {
                    Status = status;
                    Show = true;
                    if (duration > 0)
                    {
                        _hideTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(duration) };
                        _hideTimer.Start();
                        _hideTimer.Tick += HideTimer_Tick;
                    }
                }
                else
                {
                    NotificationQueue.Enqueue(status);
                }
            });
        }

        public async Task ShowMessageBoxAsync(string message, string title)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var dialog = new MessageDialog(message, title);
                await dialog.ShowAsync();
            });
        }

        public void SendUpcomingSongNotification(IMediafile mediaFile)
        {
            if (mediaFile != null)
            {
                ToastNotificationManager.History.Clear();
                var notificationXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText04);
                IXmlNode toastNode = notificationXml.SelectSingleNode("/toast");
                ((XmlElement)toastNode).SetAttribute("launch", "NowPlaying.xaml");

                XmlElement audio = notificationXml.CreateElement("audio");
                audio.SetAttribute("silent", "True");  //Here
                toastNode.AppendChild(audio);
                var toeastElement = notificationXml.GetElementsByTagName("text");
                toeastElement[0].AppendChild(notificationXml.CreateTextNode("Upcoming Song"));
                toeastElement[1].AppendChild(notificationXml.CreateTextNode(mediaFile.Title));
                toeastElement[2].AppendChild(notificationXml.CreateTextNode(mediaFile.LeadArtist));
                var imageElement = notificationXml.GetElementsByTagName("image");
                imageElement[0].Attributes[1].NodeValue = mediaFile.AttachedPicture ?? "ms-appx:///Assets/albumart.png";
                var toastNotification = new ToastNotification(notificationXml)
                {
                    Group = "upcoming-song"
                };

                ToastNotificationManager.CreateToastNotifier().Show(toastNotification);
            }
        }

        private async void HideTimer_Tick(object sender, object e)
        {
            await BreadDispatcher.InvokeAsync(() =>
            {
                Status = string.Empty;
                Show = false;
                _hideTimer?.Stop();
            });
            if (NotificationQueue.Count > 0)
            {
                await ShowMessageAsync(NotificationQueue.Dequeue()).ConfigureAwait(false);
            }
        }

        public async Task ShowStatusBarMessageAsync(string message)
        {
            if (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1))
            {
                await BreadDispatcher.InvokeAsync(async () =>
                {
                    if(statusBar == null)
                        statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                    await statusBar.ShowAsync();
                    statusBar.ProgressIndicator.Text = message;
                    await statusBar.ProgressIndicator.ShowAsync();
                    await Task.Delay(3000);
                    await statusBar.ProgressIndicator.HideAsync();
                    await statusBar.HideAsync();
                });
            }
        }
    }
}