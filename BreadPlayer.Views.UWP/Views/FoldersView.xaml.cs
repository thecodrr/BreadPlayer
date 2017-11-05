using BreadPlayer.Common;
using BreadPlayer.Core.Models;
using BreadPlayer.Dialogs;
using BreadPlayer.Extensions;
using BreadPlayer.Helpers;
using BreadPlayer.Messengers;
using BreadPlayer.ViewModels;
using SharpCifs.Smb;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.BulkAccess;
using Windows.Storage.Pickers;
using Windows.Storage.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BreadPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FoldersView : Page
    {
        public FoldersView()
        {
            this.InitializeComponent();
        }

        private void OnItemClicked(object sender, ItemClickEventArgs e)
        {
            (Resources["FoldersVM"] as FoldersViewModel).OpenItemCommand.Execute(e.ClickedItem);
        }
    }    
}
