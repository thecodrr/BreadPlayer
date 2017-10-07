using BreadPlayer.Common;
using BreadPlayer.SentryAPI;
using BreadPlayer.Targets;
using SharpRaven;
using SharpRaven.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using XLog;
using XLog.Formatters;

public class BLogger
{
    private static Logger _logger;

    public static Logger Logger
    {
        get => _logger;
        set => _logger = value;
    }
    private static RavenClient ravenClient;
    public static void InitLogger()
    {
        ravenClient = new RavenClient(
            "https://0517ff9dd4c84fe7a1922377ae0568c8:9fe0a9ffb8e84118881c26b42919ca56@sentry.io/226984",
            null, null, new BPSentryUserFactory());
        ravenClient.Logger = "user";

        var formatter = new LineFormatter();
        var logConfig = new LogConfig(formatter);
        logConfig.AddTarget(LogLevel.Trace, LogLevel.Fatal, new AsyncFileTarget(formatter, "Log.log"));
        LogManager.Init(logConfig);
        Logger = LogManager.Default.GetLogger("BLogger");      
    }

    public static async void E(string message, Exception exception)
    {
        Logger.Error(message, exception);
        await SentryMessageSender.SendMessageAsync(ravenClient, message, exception, ErrorLevel.Error).ConfigureAwait(false);
    }
    public static async void F(string message, Exception exception)
    {
        Logger.Fatal(message, exception);
        await SentryMessageSender.SendMessageAsync(ravenClient, message, exception, ErrorLevel.Fatal).ConfigureAwait(false);
    }
    public static void I(string message)
    {
        Logger.Info(message);
    }
}