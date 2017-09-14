using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Core.Models
{
    public interface IGroupKey : IComparable<IGroupKey>
    {
        string Key { get; set; }
    }
}
