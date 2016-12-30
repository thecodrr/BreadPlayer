using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Core.Interfaces
{
    public interface IDispatcher
    {
        Task RunAsync(Action action);
        bool HasThreadAccess { get; }
    }
}
