using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLog.Formatters;
using XLog;
using System.IO;
using Windows.Storage;
using BreadPlayer.Targets;
using LightBuzz.SMTP;
using Windows.ApplicationModel.Email;

public class BLogger
{
    static Logger logger;
    public static Logger Logger
    {
        get
        {
            return logger;
        }
        set { logger = value; }
    }
    public static void InitLogger()
    {       
        var formatter = new LineFormatter();
        var logConfig = new LogConfig(formatter);
        logConfig.AddTarget(LogLevel.Trace, LogLevel.Fatal, new AsyncFileTarget(formatter, "Log.log"));
        LogManager.Init(logConfig);
        Logger = LogManager.Default.GetLogger("BLogger");      
    }
    public async static Task CopyLogAndMailAsync()
    {
        var logFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("Log.log", CreationCollisionOption.OpenIfExists);

        if (logFile != null)
        {
            await logFile.CopyAsync(ApplicationData.Current.TemporaryFolder, "breadplayer.log", NameCollisionOption.GenerateUniqueName);
        }
        if (BreadPlayer.Common.RoamingSettingsHelper.GetSetting<bool>("SendReportOnEveryStartup", true))
        {
            int totalErrorCount = 0;
            var logfiles = new List<StorageFile>();

            foreach (var lf in await ApplicationData.Current.TemporaryFolder.GetFilesAsync(Windows.Storage.Search.CommonFileQuery.DefaultQuery))
            {
                int exceptionCount = GetExceptionCount(await new StreamReader(await lf.OpenStreamForReadAsync()).ReadToEndAsync());
                totalErrorCount += exceptionCount;
                if (exceptionCount > 0)
                    logfiles.Add(lf);
            }
            if (logfiles.Count > 0 && totalErrorCount > 0)
            {
                await MailLogFileAsync(logfiles, totalErrorCount);
            }
        }
    }
    static int GetExceptionCount(string text)
    {
        char[] delimiters = new char[] { ' ', '\r', '\n', '|', ',', '.' };
        Dictionary<string, int> count =
     text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries)
     .GroupBy(s => s)
     .ToDictionary(g => g.Key, g => g.Count());
        int errorCount = 0;
        if (count.ContainsKey("Error".ToUpper()))
            errorCount += count["Error".ToUpper()];
        if (count.ContainsKey("Fatal".ToUpper()))
            errorCount += count["Fatal".ToUpper()];
        return errorCount;
    }
    static async Task MailLogFileAsync(IReadOnlyList<StorageFile> logFiles, int exceptionCount)
    {
        using (SmtpClient client = new SmtpClient("smtp.1and1.com", 587, false, "support@breadplayer.com", "Allatonce1.1"))
        {
            EmailMessage emailMessage = new EmailMessage();
            emailMessage.To.Add(new EmailRecipient("support@breadplayer.com"));
            emailMessage.Subject = "Log Report from BreadPlayer";
            string body = "Device Family: {0}" + "\r\n" +
                "OS Version: {1}" + "\r\n" +
                "OS Architecture: {2}" + "\r\n" +
                "Device Model: {3}" + "\r\n" +
                "Device Manufacturer: {4}" + "\r\n" +
                "App Version: {5}" + "\r\n" +
                "Date Reported: {6}" + "\r\n" +
                "Exception Count: {7}";
            emailMessage.Body = string.Format(body, Info.SystemFamily, Info.SystemVersion, Info.SystemArchitecture, Info.DeviceModel, Info.DeviceManufacturer, Info.ApplicationVersion, DateTime.Now, exceptionCount) ;
            foreach (var logFile in logFiles)
            {
                var stream = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromFile(logFile);

                var attachment = new EmailAttachment(
                    logFile.Name,
                    stream);

                emailMessage.Attachments.Add(attachment);
            }          
            var x = await client.SendMailAsync(emailMessage);
            if(x == SmtpResult.OK)
                await ApplicationData.Current.TemporaryFolder.DeleteAsync(StorageDeleteOption.PermanentDelete);
        }
    }
}
