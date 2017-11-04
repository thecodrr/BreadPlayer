using BreadPlayer.Common;
using BreadPlayer.Core.Models;
using BreadPlayer.Dialogs;
using BreadPlayer.Extensions;
using BreadPlayer.Helpers;
using BreadPlayer.Messengers;
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
        DiskItem currentDiskItem;
        Stack<DiskItem> NavigationStack { get; set; }
        ThreadSafeObservableCollection<DiskItem> StorageItems { get; set; } = new ThreadSafeObservableCollection<DiskItem>();
        public FoldersView()
        {
            this.InitializeComponent();
            DataContext = this;
            NavigationStack = new Stack<DiskItem>();
            this.Loaded += FoldersView_Loaded;
        }
        private void GoHome()
        {
            Clear();
            StorageItems.AddRange(new DiskItem[]
           {
                new DiskItem
                {
                    Title = "Browse my music library",
                    Icon = "\uE93C",
                    Path = "Music Library",
                },
                new DiskItem
                {
                    Title = "Browse other locations",
                    Icon = "\uE8DA",
                    Path = "Other",
                },
                new DiskItem
                {
                    Title = "Browse my network",
                    Icon = "\uEC27",
                    Path = "Network",
                },
                new DiskItem
                {
                    Title = "Browse my devices",
                    Icon = "\uE975",
                    Path = "Devices",
                },
           });
        }
        private void FoldersView_Loaded(object sender, RoutedEventArgs e)
        {
            GoHome();
        }

        private async Task GetLibraryItemsAsync()
        {
            Clear();
            var musicLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);
            foreach(var item in musicLibrary.Folders)
            {
                StorageItems.Add(new DiskItem
                {
                    Title = item.DisplayName,
                    Icon = "\uE8B7", 
                    Path = item.Path,
                });
            }
        }
        private FileInformationFactory GetFileInformationFactory(StorageFolder folder)
        {
            return new FileInformationFactory(
                folder.CreateItemQueryWithOptions(
                    DirectoryWalker.GetQueryOptions(
                        folderDepth: Windows.Storage.Search.FolderDepth.Shallow)),
                Windows.Storage.FileProperties.ThumbnailMode.MusicView);
        }
        private async Task<IEnumerable<DiskItem>> GetStorageItemsInFolderAsync(StorageFolder folder)
        {
            var queryResult = folder.CreateItemQueryWithOptions(DirectoryWalker.GetQueryOptions(folderDepth: Windows.Storage.Search.FolderDepth.Shallow));
            List<DiskItem> storageItems = new List<DiskItem>();

            uint index = 0, stepSize = 40;
            var files = await queryResult.GetItemsAsync(index, stepSize);
            index += stepSize;

            while (files.Count != 0)
            {
                var fileTask = queryResult.GetItemsAsync(index, stepSize).AsTask();

                foreach (var item in files)
                {
                    if (item is StorageFolder folderInformation && !folder.DisplayName.StartsWith("."))
                    {
                        storageItems.Add(new DiskItem
                        {
                            Title = folderInformation.DisplayName,
                            Icon = "\uE8B7",
                            Path = folderInformation.Path,
                            Cache = item,
                        });
                    }
                    else if (item is StorageFile fileInformation)
                    {
                        var musicProperties = await fileInformation.Properties.GetMusicPropertiesAsync();
                        storageItems.Add(new DiskItem
                        {
                            Icon = "\uEC4F",
                            Path = fileInformation.Path,
                            Title = musicProperties?.Title.GetStringForNullOrEmptyProperty(fileInformation.DisplayName) ?? fileInformation.DisplayName,
                            Artist = musicProperties?.Artist.GetStringForNullOrEmptyProperty("Unknown Artist"),
                            Album = musicProperties?.Album.GetStringForNullOrEmptyProperty("Unkonwn Album"),
                            IsFile = true,
                            Cache = item
                        });
                    }
                }
                files = await fileTask;
                index += stepSize;
            }
            return storageItems.OrderBy(t => t.IsFile);
        }
        private void Clear()
        {
            StorageItems = null;
            foldersList.ItemsSource = null;            
            foldersList.ItemsSource = StorageItems = new ThreadSafeObservableCollection<DiskItem>();
        }
        private async void OnItemTapped(object sender, ItemClickEventArgs e)
        {
            NavigationStack.Push(currentDiskItem);
            var item = (e.ClickedItem as DiskItem);
            if (!item.IsNetworkItem)
            {
                if (!item.IsFile)
                {
                    await OpenItemAsync(item);
                    currentDiskItem = item;
                }
                else
                {
                    if (StorageItems.Any(t => t.IsPlaying))
                    {
                        StorageItems.FirstOrDefault(t => t.IsPlaying).IsPlaying = false;
                    }
                    Messenger.Instance.NotifyColleagues(MessageTypes.MsgPlaySong, string.IsNullOrEmpty(item.Path) ? item.Cache : item.Path);
                    item.IsPlaying = true;
                }
            }
            else
            {
                if (!item.IsFile)
                {
                    var items = await BrowseNetworkFolderAsync((SmbFile)item.Cache);
                    if (items != null)
                    {
                        Clear();
                        StorageItems.AddRange(items);
                    }
                }
                else if (item.IsFile)
                {
                    var networkFile = (SmbFile)item.Cache;
                    if (networkFile.Exists() && networkFile.CanRead())
                    {
                        byte[] buffer = null;
                        long length = 0;
                        using (Stream networkFileStream = await networkFile.GetInputStreamAsync().ConfigureAwait(false))
                        {
                            using(MemoryStream byteStream = new MemoryStream())
                            {
                                await networkFileStream.CopyToAsync(byteStream);
                                buffer = byteStream.ToArray();
                                length = byteStream.Length;
                            }
                        }
                        if (buffer != null)
                        {
                            var mp3File = new Mediafile
                            {
                                Title = item.Title,
                                ByteArray = buffer,
                                FileLength = length,
                            };
                            Messenger.Instance.NotifyColleagues(MessageTypes.MsgPlaySong, new List<object> { mp3File, true, false });
                            item.IsPlaying = true;
                        }
                    }
                }
            }
        }

        private async Task OpenItemAsync(DiskItem item)
        {
            switch (item.Path)
            {
                case "Music Library":
                    await GetLibraryItemsAsync();
                    return;
                case "Other":
                    await BrowseForFolder(item);
                    return;
                case "Network":
                    await BrowseNetworkAsync(item);
                    return;
                case "Devices":
                    await GetDevicesAsync();
                    return;
                default:
                    await OpenFolderAsync((StorageFolder)item.Cache);
                    break;
            }
        }
        private async Task GetDevicesAsync()
        {
            Clear();
            var devices = await GetStorageItemsInFolderAsync(KnownFolders.MediaServerDevices);
            devices.Concat(await GetStorageItemsInFolderAsync(KnownFolders.RemovableDevices));
            StorageItems.AddRange(devices);
        }
        private async Task BrowseNetworkAsync(DiskItem item)
        {
            IEnumerable<DiskItem> items = null;
            if (item.Cache is IEnumerable<DiskItem> cachedItems)
            {
                items = cachedItems;
            }
            else
            {
                NetworkAccessDialog accessDialog = new NetworkAccessDialog();
                var result = await accessDialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    items = await GetItemsOnNetworkPCAsync(accessDialog.Hostname, accessDialog.Username, accessDialog.Password);
                }
            }
            if (items != null)
            {
                Clear();
                item.Cache = items;
                StorageItems.AddRange(items);
            }
        }
        private async Task BrowseForFolder(DiskItem item)
        {
            Clear();
            if (item.Cache is StorageFolder cachedFolder)
            {
                StorageItems.AddRange(await GetStorageItemsInFolderAsync(cachedFolder));
                return;
            }
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add("*");
            folderPicker.CommitButtonText = "Browse";
            folderPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            var folder = await folderPicker.PickSingleFolderAsync();
            if(folder != null)
            {
                StorageItems.AddRange(await GetStorageItemsInFolderAsync(folder));
                item.Cache = folder;
            }
        }
        private async Task OpenFolderAsync(StorageFolder folder)
        {
            Clear();
            StorageItems.AddRange(await GetStorageItemsInFolderAsync(folder));
        }
        private async void OnGoBack(object sender, RoutedEventArgs e)
        {
            if (NavigationStack.Any())
            {
                var item = NavigationStack.Pop();
                if(item != null)
                    await OpenItemAsync(item);
                else
                {
                    GoHome();
                }
            }
        }
        private async Task<IEnumerable<DiskItem>> BrowseNetworkFolderAsync(SmbFile networkFolder)
        {
            if (!networkFolder.Exists())
                return null;
            List<DiskItem> networkStorageItems = new List<DiskItem>();
            foreach (var item in await networkFolder.ListFilesAsync())
            {
                if (!item.IsHidden())
                {
                    if (item.IsDirectory())
                    {
                        networkStorageItems.Add(new DiskItem
                        {
                            Path = item.GetPath(),
                            Title = item.GetName(),
                            Cache = item,
                            IsNetworkItem = true,
                        });
                    }
                    else if (item.IsFile())
                    {
                        networkStorageItems.Add(new DiskItem
                        {
                            Path = item.GetPath(),
                            Title = item.GetName(),
                            Cache = item,
                            IsFile = true,
                            IsNetworkItem = true,
                        });
                    }
                }
            }
            return networkStorageItems.OrderBy(t => t.IsFile);
        }
        private async Task<IEnumerable<DiskItem>> GetItemsOnNetworkPCAsync(string hostname, string username, string password)
        {
            SharpCifs.Config.SetProperty("jcifs.smb.client.lport", "8137");
            SharpCifs.Config.SetProperty("jcifs.smb.client.laddr", GetLocalIp());
            SmbFile lanStorage = null;
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                var authentication = new NtlmPasswordAuthentication(null, username, password);
                lanStorage = new SharpCifs.Smb.SmbFile($"smb://{hostname}/", authentication);
            }
            else
                lanStorage = new SmbFile($"smb://{hostname}/");
            if (!lanStorage.Exists())
                return null;
            return await BrowseNetworkFolderAsync(lanStorage);
        }
        private string GetLocalIp()
        {
            var icp = NetworkInformation.GetInternetConnectionProfile();

            if (icp?.NetworkAdapter == null) return null;
            var hostname =
                NetworkInformation.GetHostNames()
                    .SingleOrDefault(
                        hn =>
                            hn.IPInformation?.NetworkAdapter != null && hn.IPInformation.NetworkAdapter.NetworkAdapterId
                            == icp.NetworkAdapter.NetworkAdapterId);

            // the ip address
            return hostname?.CanonicalName;
        }
    }
    public class DiskItem : ObservableObject
    {
        public double Size { get; set; }
        public string Icon { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public object Cache { get; set; }
        public bool IsFile { get; set; }
        public bool IsNetworkItem { get; set; }
        bool isPlaying;
        public bool IsPlaying
        {
            get => isPlaying;
            set => Set(ref isPlaying, value);
        }
    }
}
