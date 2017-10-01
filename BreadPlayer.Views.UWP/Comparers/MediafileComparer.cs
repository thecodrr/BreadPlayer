using BreadPlayer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Comparers
{
    public class MediafileComparer : IComparer<Mediafile>
    {
        public int Compare(Mediafile x, Mediafile y)
        {
            return Convert.ToInt32(x.TrackNumber).CompareTo(Convert.ToInt32(y.TrackNumber));
        }
    }
}
