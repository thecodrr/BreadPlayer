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
    static ILogReportSender LogReportSender;
    public static void InitLogger(string path, ILogReportSender logReportSender)
    {
        LogReportSender = logReportSender;
        const string fileOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}";

        Log.Logger = new LoggerConfiguration()
                                .MinimumLevel.Verbose()
                                .WriteTo.File(path, outputTemplate: fileOutputTemplate, shared: true)
                                .CreateLogger();
        Logger = Log.Logger;
        I("Logger initialized.");
        LogReportSender.Init();
        I("Raven initialized.");
    }

    public static void E(string message, Exception exception, params object[] propertyValues)
    {
        Logger?.Error(exception, message, propertyValues);
        //await LogReportSender.SendReportAsync(message, exception, "Error").ConfigureAwait(false);
    }
    public static void F(string message, Exception exception, params object[] propertyValues)
    {
        Logger?.Fatal(exception, message, propertyValues);
        //await LogReportSender.SendReportAsync(message, exception, "Fatal").ConfigureAwait(false);
    }
    public static void I(string message, params object[] propertyValues)
    {
        Logger?.Information(message, propertyValues);
    }
}