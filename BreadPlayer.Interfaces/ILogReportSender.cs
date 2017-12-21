using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Core
{
    public interface ILogReportSender
    {
        Task SendReportAsync(string message, Exception ex, string errorLevel);
        void Init();
    }
}
