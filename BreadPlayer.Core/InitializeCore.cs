using BreadPlayer.Core.Common;
using BreadPlayer.Core.Interfaces;

public class InitializeCore
{
    static INotificationManager notificationManager;
    public static INotificationManager NotificationManager
    {
        get { return notificationManager; }
        set { notificationManager = value; }
    }

    static IDispatcher dispatcher;
    public static IDispatcher Dispatcher
    {
        get { return dispatcher; }
        set
        {
            dispatcher = value;
            InitializeFramework.Dispatcher = dispatcher;
        }
    }
}

