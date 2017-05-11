/* ========================================================================================== */
/*                                                                                            */
/* BreadPlayer.Fmod Studio - C# Wrapper . Copyright (c), Firelight Technologies Pty, Ltd. 2004-2016.      */
/*                                                                                            */
/* ========================================================================================== */


namespace BreadPlayer.Fmod
{
    /*
            BreadPlayer.Fmod version number.  Check this against BreadPlayer.Fmod::FMODSystem::getVersion / System_GetVersion
            0xaaaabbcc -> aaaa = major version number.  bb = minor version number.  cc = development version number.
        */
    public class FmodVersion
    {
        public const int    Number = 0x00010807;
#if CPU_X64
        public const string DLL    = "fmod_X64";
#elif CPU_X86
        public const string Dll    = "fmod_X86";
#else
        public const string DLL = "fmod_ARM";
#endif
    }
}
