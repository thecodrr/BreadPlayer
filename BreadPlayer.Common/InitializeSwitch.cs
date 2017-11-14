using BreadPlayer.Interfaces;

public class InitializeSwitch
{
    public InitializeSwitch(INotificationManager notificationManager, IEqualizerSettingsHelper equalizerSettingsHelper, IDispatcher dispatcher, bool isMobile)
    {
        NotificationManager = notificationManager;
        EqualizerSettingsHelper = equalizerSettingsHelper;
        Dispatcher = dispatcher;
        IsMobile = isMobile;
    }
    public static bool IsMobile { get; set; }
    public static INotificationManager NotificationManager { get; private set; }
    public static IEqualizerSettingsHelper EqualizerSettingsHelper { get; private set; }
    public static IDispatcher Dispatcher { get; private set; }
}
