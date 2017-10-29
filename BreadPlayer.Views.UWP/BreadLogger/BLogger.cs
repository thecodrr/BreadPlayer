using BreadPlayer.SentryAPI;
using Serilog;
using SharpRaven;
using SharpRaven.Data;
using System;
using System.IO;
using Windows.Storage;

public class BLogger
{
    public static ILogger Logger
    {
        get => _logger;
        set => _logger = value;
    }
    private static RavenClient ravenClient;
    private static ILogger _logger;
    public static void InitLogger()
    {
        const string fileOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}{NewLine}{Properties}";
        var logPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Logs", "BreadPlayer.log");

        Log.Logger = new LoggerConfiguration()
                                .MinimumLevel.Verbose()
                                .WriteTo.File(logPath, outputTemplate: fileOutputTemplate, shared: true)
                                .CreateLogger();
        Logger = Log.Logger;
        I("Logger initialized.");
        ravenClient = new RavenClient(
            "https://0517ff9dd4c84fe7a1922377ae0568c8:9fe0a9ffb8e84118881c26b42919ca56@sentry.io/226984",
            null, null, new BPSentryUserFactory())
        {
            Logger = "user"
        };
        I("Raven initialized.");
    }

    public static async void E(string message, Exception exception, params object[] propertyValues)
    {
        Logger.Error(exception, message, propertyValues);
        await SentryMessageSender.SendMessageAsync(ravenClient, message, exception, ErrorLevel.Error).ConfigureAwait(false);
    }
    public static async void F(string message, Exception exception, params object[] propertyValues)
    {
        Logger.Fatal(exception, message, propertyValues);
        await SentryMessageSender.SendMessageAsync(ravenClient, message, exception, ErrorLevel.Fatal).ConfigureAwait(false);
    }
    public static void I(string message, params object[] propertyValues)
    {
        Logger.Information(message, propertyValues);
    }
}