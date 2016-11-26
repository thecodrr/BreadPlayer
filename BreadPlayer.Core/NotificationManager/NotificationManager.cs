using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace BreadPlayer.BreadNotificationManager
{
	public class NotificationManager : ViewModelBase
    {
        DispatcherTimer hideTimer;
        string status = "Nothing Baking";
        public string Status
        {
            get { return status; }
            set { Set(ref status, value);}
        }
        string header = "Nothing Baking";
        public string Heading
        {
            get { return header; }
            set { Set(ref header, value); }
        }
        bool _show = false;
        public bool Show
        {
            get { return _show; }
            set { Set(ref _show, value); }
        }
        public async Task ShowAsync(string status, string heading = "Oops! Burnt!")
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Status = status;
                Heading = heading;
                //if (!Show)
                //{
                //    Show = true;
               if(hideTimer == null) hideTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(10) };
                hideTimer.Start();
                    hideTimer.Tick += HideTimer_Tick;
                //}
            });          
        }
        
        private void HideTimer_Tick(object sender, object e)
        {
            Status = "Nothing Baking!";
            //Show = false;
            hideTimer.Stop();
        }
        
    }
}
