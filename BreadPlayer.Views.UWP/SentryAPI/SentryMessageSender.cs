using SharpRaven;
using SharpRaven.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.SentryAPI
{
    public class SentryMessageSender
    {
        public static async Task SendMessageAsync(RavenClient ravenClient, string message, Exception ex, ErrorLevel errorLevel)
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
