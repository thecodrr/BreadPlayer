using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace BreadPlayer.Helpers
{
    public class ApplicationHelper
    {
        public static string GetAppVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;
            return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }
        public static void SaveAppSession(Type pageType, object parameter)
        {
            SettingsHelper.SaveLocalSetting("AppSession", new AppSession() { PageType = pageType, NavigationParameter = parameter }.ToString());
        }
        public static AppSession GetAppSession()
        {
            var appSession = SettingsHelper.GetLocalSetting<string>("AppSession", null);
            if (appSession == null)
                return new AppSession(true);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<AppSession>(appSession);
        }
    }
    public class AppSession
    {
        public Type PageType { get; set; }
        public object NavigationParameter { get; set; }
        public AppSession() { }
        public AppSession(bool def)
        {
            if (def)
            {
                PageType = typeof(LibraryView);
                NavigationParameter = "Toasts";
            }
        }
        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
