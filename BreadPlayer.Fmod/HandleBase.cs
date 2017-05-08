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
            rawPtr = newPtr;
        }

        public bool isValid()
        {
            return rawPtr != IntPtr.Zero;
        }

        public IntPtr getRaw()
        {
            return rawPtr;
        }

        protected IntPtr rawPtr;

        #region equality

        public override bool Equals(Object obj)
        {
            return Equals(obj as HandleBase);
        }
        public bool Equals(HandleBase p)
        {
            // Equals if p not null and handle is the same
            return ((object)p != null && rawPtr == p.rawPtr);
        }
        public override int GetHashCode()
        {
            return rawPtr.ToInt32();
        }
        public static bool operator ==(HandleBase a, HandleBase b)
        {
            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(a, b))
            {
                return true;
            }
            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }
            // Return true if the handle matches
            return (a.rawPtr == b.rawPtr);
        }
        public static bool operator !=(HandleBase a, HandleBase b)
        {
            return !(a == b);
        }
        #endregion

    }
}
