/* ========================================================================================== */
/*                                                                                            */
/* BreadPlayer.Fmod Studio - C# Wrapper . Copyright (c), Firelight Technologies Pty, Ltd. 2004-2016.      */
/*                                                                                            */
/* ========================================================================================== */

using BreadPlayer.Fmod.Enums;
using System;
using System.Runtime.InteropServices;
using static BreadPlayer.Fmod.Callbacks;

namespace BreadPlayer.Fmod
{
    public class Memory
    {
        public static Result Initialize(IntPtr poolmem, int poollen, MemoryAllocCallback useralloc, MemoryReallocCallback userrealloc, MemoryFreeCallback userfree, MemoryType memtypeflags)
        {
            return FMOD_Memory_Initialize(poolmem, poollen, useralloc, userrealloc, userfree, memtypeflags);
        }

        public static Result GetStats(out int currentalloced, out int maxalloced)
        {
            return GetStats(out currentalloced, out maxalloced, false);
        }

        public static Result GetStats(out int currentalloced, out int maxalloced, bool blocking)
        {
            return FMOD_Memory_GetStats(out currentalloced, out maxalloced, blocking);
        }


        #region importfunctions

        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Memory_Initialize(IntPtr poolmem, int poollen, MemoryAllocCallback useralloc, MemoryReallocCallback userrealloc, MemoryFreeCallback userfree, MemoryType memtypeflags);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Memory_GetStats(out int currentalloced, out int maxalloced, bool blocking);

        #endregion
    }
}
