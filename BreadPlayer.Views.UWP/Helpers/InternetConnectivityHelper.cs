using BreadPlayer.Core;
using System.Linq;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;

namespace BreadPlayer.Helpers
{
    public class InternetConnectivityHelper
    { 
        public static string LocalIp { get; set; }
        public static bool IsInternetConnected { get; set; }
        public static bool IsConnectedToNetwork { get; set; }
        public static async Task<bool> CheckAndNotifyAsync()
        {
            if (!IsInternetConnected)
            {
                await SharedLogic.Instance.NotificationManager.ShowMessageAsync("You have no internet, sir. Please try again with a working internet connection.");
                return false;
            }
            else if (!IsConnectedToNetwork)
            {
                await SharedLogic.Instance.NotificationManager.ShowMessageAsync("You are not connected to any network. Please try again when you are connected to a Local Area Network.");
                return false;
            }
            return true;
        }
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
                var hostnames = NetworkInformation.GetHostNames();
                if (hostnames != null || hostnames.Any())
                {
                    var hostname =
                        hostnames.FirstOrDefault(
                            hn =>
                                hn.IPInformation?.NetworkAdapter != null && hn.IPInformation.NetworkAdapter.NetworkAdapterId
                                == internetConnectionProfile.NetworkAdapter.NetworkAdapterId);
                    LocalIp = hostname?.CanonicalName;
                }
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
