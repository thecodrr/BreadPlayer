using Microsoft.Graph;
using Microsoft.Toolkit.Services.OneDrive;
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
            var _service = OneDriveService.Instance;
            var requestMessage = ((IGraphServiceClient)_service.Provider.GraphProvider).Drive.Items[oneDriveFile.OneDriveItem.Id].Content.Request().GetHttpRequestMessage();
            await _service.Provider.GraphProvider.AuthenticationProvider.AuthenticateRequestAsync(requestMessage).AsAsyncAction().AsTask();
            string headerStr = "\r\n";
            foreach (var header in requestMessage.Headers)
            {
                headerStr += header.Key + ": " + header.Value.First() + "\r\n";
            }
            return requestMessage.RequestUri.AbsoluteUri.Replace("https", "http") + headerStr;
        }
    }
}
