using BreadPlayer.Core.Common;
using BreadPlayer.Core.Interfaces;

public class InitializeCore
{
    public static bool IsMobile { get; set; }
    static INotificationManager notificationManager;
    public static INotificationManager NotificationManager
    {
        get { return notificationManager; }
        set { notificationManager = value; }
    }
    static IEqualizerSettingsHelper equalizerSettingsHelper;
    public static IEqualizerSettingsHelper EqualizerSettingsHelper
    {
        get => equalizerSettingsHelper;
        set => equalizerSettingsHelper = value;
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
