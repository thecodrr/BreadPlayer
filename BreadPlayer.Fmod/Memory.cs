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
        public static Result Initialize(IntPtr poolmem, int poollen, MEMORY_ALLOC_CALLBACK useralloc, MEMORY_REALLOC_CALLBACK userrealloc, MEMORY_FREE_CALLBACK userfree, MemoryType memtypeflags)
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

        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Memory_Initialize(IntPtr poolmem, int poollen, MEMORY_ALLOC_CALLBACK useralloc, MEMORY_REALLOC_CALLBACK userrealloc, MEMORY_FREE_CALLBACK userfree, MemoryType memtypeflags);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Memory_GetStats(out int currentalloced, out int maxalloced, bool blocking);

        #endregion
    }
}
