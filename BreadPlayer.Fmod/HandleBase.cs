/* ========================================================================================== */
/*                                                                                            */
/* BreadPlayer.Fmod Studio - C# Wrapper . Copyright (c), Firelight Technologies Pty, Ltd. 2004-2016.      */
/*                                                                                            */
/* ========================================================================================== */

using System;

namespace BreadPlayer.Fmod
{
    public class HandleBase
    {
        public HandleBase(IntPtr newPtr)
        {
            RawPtr = newPtr;
        }

        public bool IsValid()
        {
            return RawPtr != IntPtr.Zero;
        }

        public IntPtr GetRaw()
        {
            return RawPtr;
        }

        protected IntPtr RawPtr;

        #region equality

        public override bool Equals(Object obj)
        {
            return Equals(obj as HandleBase);
        }
        public bool Equals(HandleBase p)
        {
            // Equals if p not null and handle is the same
            return ((object)p != null && RawPtr == p.RawPtr);
        }
        public override int GetHashCode()
        {
            return RawPtr.ToInt32();
        }
        public static bool operator ==(HandleBase a, HandleBase b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
            {
                return true;
            }
            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }
            // Return true if the handle matches
            return (a.RawPtr == b.RawPtr);
        }
        public static bool operator !=(HandleBase a, HandleBase b)
        {
            return !(a == b);
        }
        #endregion

    }
}
