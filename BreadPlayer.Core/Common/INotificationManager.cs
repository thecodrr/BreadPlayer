using System.Threading.Tasks;
using BreadPlayer.Core.Models;

namespace BreadPlayer.Core.Common
{
    public interface INotificationManager
    {
        Task ShowMessageBoxAsync(string message, string title);
        Task ShowMessageAsync(string message, int duration = 10);
        void SendUpcomingSongNotification(Mediafile mediaFile);
    }
}
