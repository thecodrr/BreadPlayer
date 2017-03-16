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
        Task ShowMessageAsync(string message);
        void SendUpcomingSongNotification(Mediafile mediaFile);
    }
}
