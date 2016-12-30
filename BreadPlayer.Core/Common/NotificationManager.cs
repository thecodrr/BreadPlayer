using BreadPlayer.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BreadPlayer.NotificationManager
{
	public class BreadNotificationManager : ObservableObject
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
            await InitializeCore.Dispatcher.RunAsync(() =>
            {
                Status = status;
                Heading = heading;
                //if (!Show)
                //{
                //    Show = true;
               if(hideTimer == null) hideTimer = new DispatcherTimer(InitializeCore.Dispatcher) { Interval = TimeSpan.FromSeconds(10) };
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
