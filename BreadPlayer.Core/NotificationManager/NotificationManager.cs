using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadPlayer.Core;
using Windows.UI.Xaml;

namespace BreadPlayer.BreadNotificationManager
{
    public class NotificationManager : ViewModelBase
    {
        DispatcherTimer hideTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(5) };
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
            set { Set(ref _show, value); if (_show == true); }
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
