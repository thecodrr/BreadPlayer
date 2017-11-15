using BreadPlayer.Interfaces;
using System.Threading.Tasks;

namespace BreadPlayer.Interfaces
{
    public interface INotificationManager
    {
        Task ShowMessageBoxAsync(string message, string title);

        Task ShowMessageAsync(string message, int duration = 10);
        Task ShowStatusBarMessageAsync(string message);
        void SendUpcomingSongNotification(IMediafile mediaFile);
    }
}