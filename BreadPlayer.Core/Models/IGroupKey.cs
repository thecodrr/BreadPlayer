using System;

namespace BreadPlayer.Core.Models
{
    public interface IGroupKey : IComparable<IGroupKey>
    {
        string Key { get; set; }
    }
}