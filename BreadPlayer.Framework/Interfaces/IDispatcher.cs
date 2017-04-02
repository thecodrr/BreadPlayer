using System;
using System.Threading.Tasks;

namespace BreadPlayer.Core.Interfaces
{
    public interface IDispatcher
    {
        Task RunAsync(Action action);
        bool HasThreadAccess { get; }
    }
}
