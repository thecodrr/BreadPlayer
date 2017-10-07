using BreadPlayer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace BreadPlayer.Helpers
{
    public class WhatsNewDialogHelper
    {
        private static async Task<string> GetWhatsNewStringAsync()
        {
            var whatsNewFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/whatsnew.txt"));
            return await FileIO.ReadTextAsync(whatsNewFile);
        }

        public static async Task ShowWhatsNewDialogAsync()
        {
            await SharedLogic.Instance.NotificationManager.ShowMessageBoxAsync(await GetWhatsNewStringAsync(), "What's new in v" + ApplicationHelper.GetAppVersion());
        }
    }
}
