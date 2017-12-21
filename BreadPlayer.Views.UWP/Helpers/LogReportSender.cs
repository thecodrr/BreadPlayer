using BreadPlayer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace BreadPlayer.Helpers
{
    public class LogReportSender : ILogReportSender
    {
        public async void Init()
        {
            AppCenter.Start("3a3c519a-67b0-487d-bb6b-f2a94c2e2303", typeof(Analytics), typeof(Crashes));
            await AppCenter.SetEnabledAsync(true);
        }

        public Task SendReportAsync(string message, Exception ex, string errorLevel)
        {
            return null;
            //throw new NotImplementedException();
        }
    }
}
