using SharpRaven;
using SharpRaven.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadPlayer.Core;

namespace BreadPlayer.SentryAPI
{
    public class SentryMessageSender : ISentryMessageSender
    {
        private RavenClient ravenClient;
        public void InitSentry()
        {
            ravenClient = new RavenClient(
            "https://0517ff9dd4c84fe7a1922377ae0568c8:9fe0a9ffb8e84118881c26b42919ca56@sentry.io/226984",
            null, null, new SentryUserFactory())
            {
                Logger = "user",
                Release = "v2.7.7.0"
            };
        }
        public async Task SendMessageAsync(string message, Exception ex, string errorLevel)
        {
            var sentryMessage = new SentryMessage(string.Format(
                       "Message: {0} \r\n\r\nException:{1}",
                       message,
                       ex.ToString()));
            var sentryEvent = new SentryEvent(sentryMessage)
            {
                Level = ErrorLevel.Error,
                Tags = new Dictionary<string, string>()
            {
                { "device.model", DeviceInfoHelper.DeviceModel},
                { "device.arch", DeviceInfoHelper.SystemArchitecture},
                { "app.version", DeviceInfoHelper.ApplicationVersion},
                { "system.version", DeviceInfoHelper.SystemVersion},
                { "system.family", DeviceInfoHelper.SystemFamily},
            }
            };

            var result = await ravenClient.CaptureAsync(sentryEvent).ConfigureAwait(false);
        }
    }
}
