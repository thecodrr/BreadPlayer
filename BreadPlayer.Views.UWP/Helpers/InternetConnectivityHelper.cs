using Windows.Networking.Connectivity;

namespace BreadPlayer.Helpers
{
    public class InternetConnectivityHelper
    {
        //public static event EventHandler InternetConnectivityChanged;
        static bool isInternetConnected;
        public static bool IsInternetConnected
        {
            get { return isInternetConnected; }
            set { isInternetConnected = value; }
        }

        private InternetConnectivityHelper()
        {
            ConnectionProfile internetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();
            NetworkInformation.NetworkStatusChanged += OnInternetStatusChanged;
            IsInternetConnected = internetConnectionProfile != null && internetConnectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
        }

        private static void OnInternetStatusChanged(object sender)
        {
            ConnectionProfile internetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();
            IsInternetConnected = internetConnectionProfile != null && internetConnectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
        }
    }
}