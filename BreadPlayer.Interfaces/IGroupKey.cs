using System;

namespace BreadPlayer.Interfaces
{
    public interface IGroupKey : IComparable<IGroupKey>
    {
        string Key { get; set; }
    }
}