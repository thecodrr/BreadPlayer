using Macalifa;
using System;
using System.Windows.Input;

namespace SplitViewMenu
{
    public interface INavigationMenuItem
    {
        Type DestinationPage { get; }
        object Arguments { get; }
        
        string Label { get; }
    }
}