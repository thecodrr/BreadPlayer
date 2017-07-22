using BreadPlayer.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class DevicesView : Page
    {
        public DevicesView()
        {
            this.InitializeComponent();
            get();
        }
        MTPDeviceService mptService;
        void get()
        {
            mptService = new MTPDeviceService();
            mptService.Devices.CollectionChanged += Devices_CollectionChanged;
        }

        private async void Devices_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var folder = mptService.GetMusicFolderFromDevice(mptService.Devices[0]);
            var folders = (await folder.GetFoldersAsync(Windows.Storage.Search.CommonFolderQuery.DefaultQuery)).ToList();
            var musicFolderquery = folder.CreateFolderQueryWithOptions(new Windows.Storage.Search.QueryOptions() { FolderDepth = Windows.Storage.Search.FolderDepth.Deep });
            folders = (await musicFolderquery.GetFoldersAsync()).ToList();
        }
    }
}
