using BreadPlayer.Core.Interfaces;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace BreadPlayer.Dispatcher
{
    public class BreadDispatcher : IDispatcher
    {
        public static CoreDispatcher ParentDispatcher { get => CoreApplication.MainView.CoreWindow.Dispatcher; }

        public async Task RunAsync(Action action)
        {
            await ParentDispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
        }

        public static async Task InvokeAsync(Action action)
        {
            await ParentDispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
        }

        public bool HasThreadAccess => ParentDispatcher.HasThreadAccess;
    }
}