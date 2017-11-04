using System;

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