using BreadPlayer.Common;
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
        ravenClient = new RavenClient("https://0517ff9dd4c84fe7a1922377ae0568c8:9fe0a9ffb8e84118881c26b42919ca56@sentry.io/226984");
        var formatter = new LineFormatter();
        var logConfig = new LogConfig(formatter);
        logConfig.AddTarget(LogLevel.Trace, LogLevel.Fatal, new AsyncFileTarget(formatter, "Log.log"));
        LogManager.Init(logConfig);
        Logger = LogManager.Default.GetLogger("BLogger");      
    }   
    
    public static async void E(string message, Exception exception)
    {
        Logger.Error(message, exception);
        var sentryMessage = new SentryMessage(string.Format(
                        "Message: {0} \r\n\r\nException:{1}",
                        message,
                        exception.ToString()));
        var sentryEvent = new SentryEvent(sentryMessage)
        {
            Level = ErrorLevel.Error
        };
        var result = await ravenClient.CaptureAsync(sentryEvent).ConfigureAwait(false);
    }
    public static async void F(string message, Exception exception)
    {
        Logger.Fatal(message, exception);
        var sentryMessage = new SentryMessage(string.Format(
                        "Message: {0} \r\n\r\nException:{1}",
                        message,
                        exception.ToString()));
        var sentryEvent = new SentryEvent(sentryMessage)
        {
            Level = ErrorLevel.Fatal
        };
        var result = await ravenClient.CaptureAsync(sentryEvent).ConfigureAwait(false);
    }
    public static void I(string message)
    {
        Logger.Info(message);
    }
}