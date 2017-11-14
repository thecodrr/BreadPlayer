using BreadPlayer.Interfaces;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace BreadPlayer.Dispatcher
{
    public class BreadDispatcher : IDispatcher
    {
        private static CoreApplicationView ApplicationView = CoreApplication.GetCurrentView();
        public static CoreDispatcher ParentDispatcher {
            get
            {
                return ApplicationView.Dispatcher;
            }
        }

        public async Task RunAsync(Action action)
        {
            if (action == null)
                return;
            if (ParentDispatcher.HasThreadAccess)
                action();
            else
                await ParentDispatcher.TryRunAsync(CoreDispatcherPriority.Normal, () => action());
        }

        public static async Task InvokeAsync(Action action)
        {
            if (action == null)
                return;
            if (ParentDispatcher.HasThreadAccess)
                action();
            else
                await ParentDispatcher.TryRunAsync(CoreDispatcherPriority.Normal, () => action());
        }

        public bool HasThreadAccess => ParentDispatcher.HasThreadAccess;
    }
}