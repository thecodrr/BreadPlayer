using BreadPlayer.Core.Common;
using BreadPlayer.Core.Interfaces;
using BreadPlayer.NotificationManager;
using BreadPlayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
        set { dispatcher = value; }
    }
    public InitializeCore(IDispatcher dispatcher, INotificationManager notificationManager)
    {
        Dispatcher = dispatcher;
        NotificationManager = notificationManager;
        InitializeFramework.Dispatcher = dispatcher;
    }
}

