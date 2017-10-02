using BreadPlayer.Core.Common;
using BreadPlayer.Core.Interfaces;

namespace BreadPlayer.Core
{
    public class InitializeCore
    {
        public static bool IsMobile { get; set; }
        private static INotificationManager _notificationManager;

        public static INotificationManager NotificationManager
        {
            get => _notificationManager;
            set => _notificationManager = value;
        }

        private static IEqualizerSettingsHelper _equalizerSettingsHelper;

        public static IEqualizerSettingsHelper EqualizerSettingsHelper
        {
            get => _equalizerSettingsHelper;
            set => _equalizerSettingsHelper = value;
        }

        private static IDispatcher _dispatcher;

        public static IDispatcher Dispatcher
        {
            get => _dispatcher;
            set
            {
                _dispatcher = value;
                InitializeFramework.Dispatcher = _dispatcher;
            }
        }
    }
}