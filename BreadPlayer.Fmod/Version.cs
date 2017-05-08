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
    public class FMODVersion
    {
        public const int    number = 0x00010807;
#if WIN64
        public const string DLL    = "fmod_X64";
#else
        public const string DLL    = "fmod_X86";
#endif
    }
}
