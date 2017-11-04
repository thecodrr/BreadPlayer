using BreadPlayer.Core;
using Serilog;
using System;

public class BLogger
{
    public static ILogger Logger
    {
        get => _logger;
        set => _logger = value;
    }
   
    private static ILogger _logger;
    static ISentryMessageSender SentryMessageSender;
    public static void InitLogger(string path, ISentryMessageSender sentryMessageSender)
    {
        SentryMessageSender = sentryMessageSender;
        const string fileOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}";

        Log.Logger = new LoggerConfiguration()
                                .MinimumLevel.Verbose()
                                .WriteTo.File(path, outputTemplate: fileOutputTemplate, shared: true)
                                .CreateLogger();
        Logger = Log.Logger;
        I("Logger initialized.");
        SentryMessageSender.InitSentry();
        I("Raven initialized.");
    }

    public static async void E(string message, Exception exception, params object[] propertyValues)
    {
        Logger?.Error(exception, message, propertyValues);
        await SentryMessageSender.SendMessageAsync(message, exception, "Error").ConfigureAwait(false);
    }
    public static async void F(string message, Exception exception, params object[] propertyValues)
    {
        Logger?.Fatal(exception, message, propertyValues);
        await SentryMessageSender.SendMessageAsync(message, exception, "Fatal").ConfigureAwait(false);
    }
    public static void I(string message, params object[] propertyValues)
    {
        Logger?.Information(message, propertyValues);
    }
}