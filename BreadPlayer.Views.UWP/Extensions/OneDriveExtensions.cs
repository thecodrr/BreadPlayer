using Microsoft.Toolkit.Uwp.Services.OneDrive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Extensions
{
    public static class OneDriveExtensions
    {
        public static async Task<string> GetDownloadURL(this OneDriveStorageFile oneDriveFile)
        {
            var requestMessage = OneDriveService.Instance.Provider.Drive.Items[oneDriveFile.OneDriveItem.Id].Content.Request().GetHttpRequestMessage();
            await OneDriveService.Instance.Provider.AuthenticationProvider.AuthenticateRequestAsync(requestMessage).AsAsyncAction().AsTask();
            string headerStr = "\r\n";
            foreach (var header in requestMessage.Headers)
            {
                headerStr += header.Key + ": " + header.Value.First() + "\r\n";
            }
            return requestMessage.RequestUri.AbsoluteUri.Replace("https", "http") + headerStr;
        }
    }
}
