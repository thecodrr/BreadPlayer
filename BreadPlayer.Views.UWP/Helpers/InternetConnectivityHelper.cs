using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;

namespace BreadPlayer.Helpers
{
    public class InternetConnectivityHelper
    {
        //public static event EventHandler InternetConnectivityChanged;
        public static bool IsInternetConnected
        {
            get { return IsInternetAvailable(); }
        }
        private static bool IsInternetAvailable()
        {
            ConnectionProfile internetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();
            // NetworkInformation.NetworkStatusChanged += NetworkInformation_NetworkStatusChanged;
            return internetConnectionProfile != null && internetConnectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
        }

        //private static void NetworkInformation_NetworkStatusChanged(object sender)
        //{
        //    InternetConnectivityChanged?.Invoke(sender, new EventArgs());
        //}
    }
}
