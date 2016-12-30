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
    static BreadNotificationManager notificationManager;
    public static BreadNotificationManager NotificationManager
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
    public InitializeCore(IDispatcher dispatcher)
    {
        Dispatcher = dispatcher;
        InitializeFramework.Dispatcher = dispatcher;
    }
}

