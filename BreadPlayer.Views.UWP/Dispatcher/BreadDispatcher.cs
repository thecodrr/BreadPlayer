using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using BreadPlayer.Core.Interfaces;
using Windows.ApplicationModel.Core;

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
