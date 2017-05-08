/* ========================================================================================== */
/*                                                                                            */
/* BreadPlayer.Fmod Studio - C# Wrapper . Copyright (c), Firelight Technologies Pty, Ltd. 2004-2016.      */
/*                                                                                            */
/* ========================================================================================== */


using BreadPlayer.Fmod.Structs;

namespace BreadPlayer.Fmod
{
#pragma warning restore 414

    /*
    [DEFINE]
    [
    [NAME]
    FMOD_REVERB_PRESETS

    [DESCRIPTION]
    A set of predefined environment PARAMETERS, created by Creative Labs
    These are used to initialize an FMOD_REVERB_PROPERTIES structure statically.
    ie
    FMOD_REVERB_PROPERTIES prop = FMOD_PRESET_GENERIC;

    [SEE_ALSO]
    System::setReverbProperties
    ]
    */
    public class Preset
    {
        /*                                                                                  Instance  Env   Diffus  Room   RoomHF  RmLF DecTm   DecHF  DecLF   Refl  RefDel   Revb  RevDel  ModTm  ModDp   HFRef    LFRef   Diffus  Densty  FLAGS */
        public static ReverbProperties OFF()                 { return new ReverbProperties(  1000,    7,  11, 5000, 100, 100, 100, 250, 0,    20,  96, -80.0f );}
        public static ReverbProperties GENERIC()             { return new ReverbProperties(  1500,    7,  11, 5000,  83, 100, 100, 250, 0, 14500,  96,  -8.0f );}
        public static ReverbProperties PADDEDCELL()          { return new ReverbProperties(   170,    1,   2, 5000,  10, 100, 100, 250, 0,   160,  84,  -7.8f );}
        public static ReverbProperties ROOM()                { return new ReverbProperties(   400,    2,   3, 5000,  83, 100, 100, 250, 0,  6050,  88,  -9.4f );}
        public static ReverbProperties BATHROOM()            { return new ReverbProperties(  1500,    7,  11, 5000,  54, 100,  60, 250, 0,  2900,  83,   0.5f );}
        public static ReverbProperties LIVINGROOM()          { return new ReverbProperties(   500,    3,   4, 5000,  10, 100, 100, 250, 0,   160,  58, -19.0f );}
        public static ReverbProperties STONEROOM()           { return new ReverbProperties(  2300,   12,  17, 5000,  64, 100, 100, 250, 0,  7800,  71,  -8.5f );}
        public static ReverbProperties AUDITORIUM()          { return new ReverbProperties(  4300,   20,  30, 5000,  59, 100, 100, 250, 0,  5850,  64, -11.7f );}
        public static ReverbProperties CONCERTHALL()         { return new ReverbProperties(  3900,   20,  29, 5000,  70, 100, 100, 250, 0,  5650,  80,  -9.8f );}
        public static ReverbProperties CAVE()                { return new ReverbProperties(  2900,   15,  22, 5000, 100, 100, 100, 250, 0, 20000,  59, -11.3f );}
        public static ReverbProperties ARENA()               { return new ReverbProperties(  7200,   20,  30, 5000,  33, 100, 100, 250, 0,  4500,  80,  -9.6f );}
        public static ReverbProperties HANGAR()              { return new ReverbProperties( 10000,   20,  30, 5000,  23, 100, 100, 250, 0,  3400,  72,  -7.4f );}
        public static ReverbProperties CARPETTEDHALLWAY()    { return new ReverbProperties(   300,    2,  30, 5000,  10, 100, 100, 250, 0,   500,  56, -24.0f );}
        public static ReverbProperties HALLWAY()             { return new ReverbProperties(  1500,    7,  11, 5000,  59, 100, 100, 250, 0,  7800,  87,  -5.5f );}
        public static ReverbProperties STONECORRIDOR()       { return new ReverbProperties(   270,   13,  20, 5000,  79, 100, 100, 250, 0,  9000,  86,  -6.0f );}
        public static ReverbProperties ALLEY()               { return new ReverbProperties(  1500,    7,  11, 5000,  86, 100, 100, 250, 0,  8300,  80,  -9.8f );}
        public static ReverbProperties FOREST()              { return new ReverbProperties(  1500,  162,  88, 5000,  54,  79, 100, 250, 0,   760,  94, -12.3f );}
        public static ReverbProperties CITY()                { return new ReverbProperties(  1500,    7,  11, 5000,  67,  50, 100, 250, 0,  4050,  66, -26.0f );}
        public static ReverbProperties MOUNTAINS()           { return new ReverbProperties(  1500,  300, 100, 5000,  21,  27, 100, 250, 0,  1220,  82, -24.0f );}
        public static ReverbProperties QUARRY()              { return new ReverbProperties(  1500,   61,  25, 5000,  83, 100, 100, 250, 0,  3400, 100,  -5.0f );}
        public static ReverbProperties PLAIN()               { return new ReverbProperties(  1500,  179, 100, 5000,  50,  21, 100, 250, 0,  1670,  65, -28.0f );}
        public static ReverbProperties PARKINGLOT()          { return new ReverbProperties(  1700,    8,  12, 5000, 100, 100, 100, 250, 0, 20000,  56, -19.5f );}
        public static ReverbProperties SEWERPIPE()           { return new ReverbProperties(  2800,   14,  21, 5000,  14,  80,  60, 250, 0,  3400,  66,   1.2f );}
        public static ReverbProperties UNDERWATER()          { return new ReverbProperties(  1500,    7,  11, 5000,  10, 100, 100, 250, 0,   500,  92,   7.0f );}
    }
}
