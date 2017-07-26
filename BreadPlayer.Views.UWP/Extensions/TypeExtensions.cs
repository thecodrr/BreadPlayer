using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Extensions
{
    public static class TypeExtensions
    {
        public static byte ToByte(this int integer)
        {
            return Convert.ToByte(integer);
        }
    }
}
