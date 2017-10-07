using BreadPlayer.Common;
using BreadPlayer.Targets;
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

    public static void InitLogger()
    {
        var formatter = new LineFormatter();
        var logConfig = new LogConfig(formatter);
        logConfig.AddTarget(LogLevel.Trace, LogLevel.Fatal, new AsyncFileTarget(formatter, "Log.log"));
        LogManager.Init(logConfig);
        Logger = LogManager.Default.GetLogger("BLogger");
    }    
}