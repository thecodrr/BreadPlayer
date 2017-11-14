using System;
using System.Threading.Tasks;

namespace BreadPlayer.Interfaces
{
    public interface IDispatcher
    {
        Task RunAsync(Action action);

        bool HasThreadAccess { get; }
    }
}