using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Portable;
using Windows.Storage;

namespace BreadPlayer.Services
{
    public class MTPDeviceService
    {
        public ThreadSafeObservableCollection<DeviceInformation> Devices { get; set; }
        public MTPDeviceService()
        {
            Devices = new ThreadSafeObservableCollection<DeviceInformation>();
            Initiate();
        }
        private void Initiate()
        {

            var deviceWatcher = DeviceInformation.CreateWatcher(StorageDevice.GetDeviceSelector());
            deviceWatcher.Added += DeviceWatcher_Added;
         
            deviceWatcher.Start();
            
        }
        
        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            Devices.AddRange(await GetDevices());
        }
        public StorageFolder GetMusicFolderFromDevice(DeviceInformation deviceInfo)
        {
            var device = StorageDevice.FromId(deviceInfo.Id);
            Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(device);
            return device;
        }
        private async Task<IEnumerable<DeviceInformation>> GetDevices()
        {
            return await DeviceInformation.FindAllAsync(StorageDevice.GetDeviceSelector());
        }
    }
}
