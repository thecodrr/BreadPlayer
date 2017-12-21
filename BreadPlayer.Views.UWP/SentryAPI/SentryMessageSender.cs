using SharpRaven;
using SharpRaven.Data;
using System;
using System.Collections.Generic;
using BreadPlayer.Extensions;
using System.Threading.Tasks;
using BreadPlayer.Core;

namespace BreadPlayer.SentryAPI
{
    public class SentryMessageSender : ILogReportSender
    {
        private RavenClient ravenClient;
        public void InitSentry()
        {
            ravenClient = new RavenClient(
            "https://0517ff9dd4c84fe7a1922377ae0568c8:9fe0a9ffb8e84118881c26b42919ca56@sentry.io/226984",
            null, null, new SentryUserFactory())
            {
                Logger = "user",
                Release = "v2.7.9.0"
            };
        }
        public async Task SendMessageAsync(string message, Exception ex, string errorLevel)
        {
            if (string.IsNullOrEmpty(message) || ex == null || string.IsNullOrEmpty(errorLevel))
                return;
            var sentryMessage = new SentryMessage(string.Format(
                       "Message: {0} \r\n\r\nException:{1}",
                       message,
                       ex.ToString()));
            var sentryEvent = new SentryEvent(sentryMessage)
            {
                Level = ErrorLevel.Error,
                Tags = new Dictionary<string, string>()
                {
                    { "device.model", DeviceInfoHelper.DeviceModel.GetStringForNullOrEmptyProperty("0")},
                    { "device.arch", DeviceInfoHelper.SystemArchitecture.GetStringForNullOrEmptyProperty("unknown")},
                    { "app.version", DeviceInfoHelper.ApplicationVersion.GetStringForNullOrEmptyProperty("2.7.9.0")},
                    { "system.version", DeviceInfoHelper.SystemVersion.GetStringForNullOrEmptyProperty("0")},
                    { "system.family", DeviceInfoHelper.SystemFamily.GetStringForNullOrEmptyProperty("unknown")},
                }
            };

            var result = await ravenClient.CaptureAsync(sentryEvent).ConfigureAwait(false);
        }
    }
}
