using BreadPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Core.Common
{
    public interface INotificationManager
    {
        Task ShowMessageBoxAsync(string message, string title);
        Task ShowMessageAsync(string message, int duration = 10);
        void SendUpcomingSongNotification(Mediafile mediaFile);
    }
}
