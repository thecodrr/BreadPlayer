using BreadPlayer.Core.Interfaces;
using System;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace BreadPlayer.Dispatcher
{
    public class BreadDispatcher : IDispatcher
    {
        CoreDispatcher _dispatcher;
        public BreadDispatcher(CoreDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }
        public async Task RunAsync(Action action)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
        }
        public bool HasThreadAccess
        {
            get { return _dispatcher.HasThreadAccess; }
        }
    }
}
