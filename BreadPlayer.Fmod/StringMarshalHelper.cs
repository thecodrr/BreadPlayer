/* ========================================================================================== */
/*                                                                                            */
/* BreadPlayer.Fmod Studio - C# Wrapper . Copyright (c), Firelight Technologies Pty, Ltd. 2004-2016.      */
/*                                                                                            */
/* ========================================================================================== */

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BreadPlayer.Fmod
{
    internal class StringMarshalHelper
    {
        static internal void NativeToBuilder(StringBuilder builder, IntPtr nativeMem)
        {
            byte[] bytes = new byte[builder.Capacity];
            Marshal.Copy(nativeMem, bytes, 0, builder.Capacity);
			int strlen = Array.IndexOf(bytes, (byte)0);
			if (strlen > 0)
			{
				String str = Encoding.UTF8.GetString(bytes, 0, strlen);
				builder.Append(str);
			}
        }
    }
}
