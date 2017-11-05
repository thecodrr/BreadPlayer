using System.Linq;
using Windows.Networking.Connectivity;

namespace BreadPlayer.Helpers
{
    public class InternetConnectivityHelper
    { 
        public static string LocalIp { get; set; }
        public static bool IsInternetConnected { get; set; }
        public static bool IsConnectedToNetwork { get; set; }
        public InternetConnectivityHelper()
        {
            RefreshConnection();
            NetworkInformation.NetworkStatusChanged += OnInternetStatusChanged;
        }
        private static void RefreshConnection()
        {
            ConnectionProfile internetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();
            if (internetConnectionProfile?.NetworkAdapter != null)
            {
                var hostname =
                  NetworkInformation.GetHostNames()
                      .SingleOrDefault(
                          hn =>
                              hn.IPInformation?.NetworkAdapter != null && hn.IPInformation.NetworkAdapter.NetworkAdapterId
                              == internetConnectionProfile.NetworkAdapter.NetworkAdapterId);
                LocalIp = hostname?.CanonicalName;
            }
            IsInternetConnected = internetConnectionProfile != null && internetConnectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            IsConnectedToNetwork = internetConnectionProfile != null;
        }
        private static void OnInternetStatusChanged(object sender)
        {
            RefreshConnection();
        }
    }
}