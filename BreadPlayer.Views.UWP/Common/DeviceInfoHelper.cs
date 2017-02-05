using Windows.ApplicationModel;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;

public static class Info
{
    public static string SystemFamily { get; }
    public static string SystemVersion { get; }
    public static string SystemArchitecture { get; }
    public static string ApplicationName { get; }
    public static string ApplicationVersion { get; }
    public static string DeviceManufacturer { get; }
    public static string DeviceModel { get; }

    static Info()
    {
        // get the system family name
        AnalyticsVersionInfo ai = AnalyticsInfo.VersionInfo;
        SystemFamily = ai.DeviceFamily;

        // get the system version number
        string sv = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
        ulong v = ulong.Parse(sv);
        ulong v1 = (v & 0xFFFF000000000000L) >> 48;
        ulong v2 = (v & 0x0000FFFF00000000L) >> 32;
        ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
        ulong v4 = (v & 0x000000000000FFFFL);
        SystemVersion = $"{v1}.{v2}.{v3}.{v4}";

        // get the package architecure
        Package package = Package.Current;
        SystemArchitecture = package.Id.Architecture.ToString();

        // get the user friendly app name
        ApplicationName = package.DisplayName;

        // get the app version
        PackageVersion pv = package.Id.Version;
        ApplicationVersion = $"{pv.Major}.{pv.Minor}.{pv.Build}.{pv.Revision}";

        // get the device manufacturer and model name
        EasClientDeviceInformation eas = new EasClientDeviceInformation();
        DeviceManufacturer = eas.SystemManufacturer;
        DeviceModel = eas.SystemProductName;
    }
}