using BreadPlayer.Common;
using BreadPlayer.Core;
using BreadPlayer.Core.Common;
using BreadPlayer.Core.Models;
using BreadPlayer.Dialogs;
using BreadPlayer.Extensions;
using BreadPlayer.Helpers;
using BreadPlayer.Messengers;
using Microsoft.Toolkit.Uwp.Services.OneDrive;
using SharpCifs.Smb;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Search;
using Windows.UI.Xaml.Controls;
using static Microsoft.Toolkit.Uwp.Services.OneDrive.OneDriveEnums;

namespace BreadPlayer.ViewModels
{
    public class FoldersViewModel : ObservableObject
    {
        DiskItem currentDiskItem;
        Stack<DiskItem> NavigationStack { get; set; }
        ThreadSafeObservableCollection<DiskItem> storageItems;
        public ThreadSafeObservableCollection<DiskItem> StorageItems
        {
            get => storageItems;
            set => Set(ref storageItems, value);
        }        
        bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set => Set(ref isBusy, value);
        }
        public ICommand OpenItemCommand { get; set; }
        public ICommand GoBackCommand { get; set; }
        public ICommand GoHomeCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public FoldersViewModel()
        {
            OpenItemCommand = new RelayCommand(OpenItem);
            GoBackCommand = new DelegateCommand(GoBack);
            GoHomeCommand = new DelegateCommand(GoHome);
            RefreshCommand = new DelegateCommand(Refresh);

            NavigationStack = new Stack<DiskItem>();
            GoHome();           
        }
        private void Clear()
        {
            StorageItems = null;
            StorageItems = new ThreadSafeObservableCollection<DiskItem>();
        }
        private async void Refresh()
        {
            //we use this instead of OpenItem(object para) because we don't want to
            //put duplicate entries in the navigation stack.
            await BrowseItemAsync(currentDiskItem);
        }
        private void GoHome()
        {
            currentDiskItem = null;
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
                    Title = "Browse my OneDrive",
                    Icon = "\uE753",
                    Path = "OneDrive",
                },
                new DiskItem
                {
                    Title = "Browse my devices",
                    Icon = "\uE975",
                    Path = "Devices",
                },
            });
        }
        private async void OpenItem(object para)
        {            
            var item = (DiskItem)para;
            if (item == null)
                return;
            if (!item.IsFile) //i.e. it is a folder
            {
                IsBusy = true;
                NavigationStack.Push(currentDiskItem);
                await BrowseItemAsync(item);
                IsBusy = false;
            }
            else
            {
                await OpenFileAsync(item);
            }
        }
        private async void GoBack()
        {
            if (NavigationStack.Any())
            {
                var item = NavigationStack.Pop();
                if (item != null)
                {
                    await BrowseItemAsync(item);                   
                }
                else
                {
                    GoHome();
                }
            }
        }
        private async Task BrowseItemAsync(DiskItem item)
        {
            currentDiskItem = item;
            switch (item.Path)
            {
                case "Music Library":
                    await GetLibraryItemsAsync();
                    break;
                case "Other":
                    await BrowseForFolder(item);
                    break;
                case "Network":
                    await BrowseNetworkAsync(item);
                    break;
                case "OneDrive":
                    await BrowseOneDriveAsync(item);
                    break;
                case "Devices":
                    await GetDevicesAsync();
                    break;
                default:
                    await OpenFolderAsync(item);
                    break;
            }
        }
        ThreadSafeObservableCollection<DiskItem> _nowPlayingItems;
        private async Task OpenFileAsync(DiskItem item)
        {
            if (StorageItems.Any(t => t.IsPlaying))
            {
                StorageItems.FirstOrDefault(t => t.IsPlaying).IsPlaying = false;
            }
            if (item.DiskItemLocation == DiskItemLocationType.Local)
            {
                Messenger.Instance.NotifyColleagues(MessageTypes.MsgPlaySong, string.IsNullOrEmpty(item.Path) ? item.Cache : item.Path);
                if (_nowPlayingItems != storageItems)
                {
                    _nowPlayingItems = StorageItems;
                    Messenger.Instance.NotifyColleagues(MessageTypes.MsgLibraryLoaded, new object[] { _nowPlayingItems, MediaLocationType.Local });
                }
            }
            else if (item.DiskItemLocation == DiskItemLocationType.OneDrive)
            {
                var oneDriveFile = (OneDriveStorageFile)item.Cache;
                var mp3File = new Mediafile
                {
                    Path = item.Path,
                    Title = oneDriveFile.DisplayName,
                    MediaLocation = MediaLocationType.Internet,
                    Size = item.Size
                };
                Messenger.Instance.NotifyColleagues(MessageTypes.MsgPlaySong, new List<object> { mp3File, true, false });
                if (_nowPlayingItems != storageItems)
                {
                    _nowPlayingItems = StorageItems;
                    Messenger.Instance.NotifyColleagues(MessageTypes.MsgLibraryLoaded, new object[] { _nowPlayingItems, MediaLocationType.Internet });
                }
            }
            else if (item.DiskItemLocation == DiskItemLocationType.Network)
            {
                var networkFile = (SmbFile)item.Cache;
                if (networkFile.Exists() && networkFile.CanRead())
                {
                    var buffer = await ((Stream)(await networkFile.GetInputStreamAsync().ConfigureAwait(false))).ToByteArray();
                    if (buffer != null)
                    {
                        var mp3File = new Mediafile
                        {
                            Title = item.Title,
                            ByteArray = buffer,
                            MediaLocation = MediaLocationType.Network,
                        };
                        Messenger.Instance.NotifyColleagues(MessageTypes.MsgPlaySong, new List<object> { mp3File, true, false });
                    }
                }
            }
            item.IsPlaying = true;
        }
        private async Task OpenFolderAsync(DiskItem item)
        {
            Clear();
            IEnumerable<DiskItem> items = null;
            switch (item.DiskItemLocation)
            {
                case DiskItemLocationType.Network:
                    items = await BrowseNetworkFolderAsync((SmbFile)item.Cache);
                    break;
                case DiskItemLocationType.OneDrive:
                    items = await GetOneDriveFolderItemsAsync((OneDriveStorageFolder)item.Cache);
                    break;
                default:
                case DiskItemLocationType.Local:
                    items = await GetStorageItemsInFolderAsync((StorageFolder)item.Cache);
                    break;
            }
            if (items != null)
            {
                StorageItems.AddRange(items);
            }
        }
        private async Task GetLibraryItemsAsync()
        {
            Clear();
            var musicLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);
            foreach (var item in musicLibrary.Folders)
            {
                StorageItems.Add(new DiskItem
                {
                    Title = item.DisplayName,
                    Icon = "\uE8B7",
                    Path = item.Path,
                    Cache = item,
                    DiskItemLocation = DiskItemLocationType.Local,
                });
            }
        }

        public Task<IReadOnlyList<IStorageItem>> GetQueryResultItemsAsync(StorageFolder folder, StorageItemTypes filter, QueryOptions queryOptions, uint index, uint stepSize)
        {
            IStorageQueryResultBase queryResult = null;
            switch (filter)
            {
                case StorageItemTypes.File:
                    queryResult = folder.CreateFileQueryWithOptions(queryOptions);
                    return ((IAsyncOperation<IReadOnlyList<IStorageItem>>)(queryResult as StorageFileQueryResult).GetFilesAsync(index, stepSize)).AsTask();
                case StorageItemTypes.Folder:
                    queryResult = folder.CreateFolderQueryWithOptions(queryOptions);
                    return ((IAsyncOperation<IReadOnlyList<IStorageItem>>)(queryResult as StorageFolderQueryResult).GetFoldersAsync(index, stepSize)).AsTask();
                default:
                case StorageItemTypes.None:
                    queryResult = folder.CreateItemQueryWithOptions(queryOptions);
                    return (queryResult as StorageItemQueryResult).GetItemsAsync(index, stepSize).AsTask();
            }
        }
        public async Task GetStorageItemsInFolderAsync(StorageFolder folder, QueryOptions queryOptions, uint stepSize,
                StorageItemTypes filter, Action<IStorageItem> action)
        {
            IReadOnlyList<IStorageItem> files = null;
            uint index = 0;
            files = await GetQueryResultItemsAsync(folder, filter, queryOptions, index, stepSize);
            index += stepSize;
            while (files.Count != 0)
            {
                var fileTask = GetQueryResultItemsAsync(folder, filter, queryOptions, index, stepSize);

                foreach (var item in files)
                {
                    var copyItem = item;
                    //closure
                    action.Invoke(copyItem);
                }

                files = await fileTask;
                index += stepSize;
            }
        }
        private async Task<IEnumerable<DiskItem>> GetStorageItemsInFolderAsync(StorageFolder folder)
        {
            List<DiskItem> storageItems = new List<DiskItem>();
            await GetStorageItemsInFolderAsync(folder, DirectoryWalker.GetQueryOptions(folderDepth: FolderDepth.Shallow), 40, StorageItemTypes.None,
                new Action<IStorageItem>(async (item) =>
                {
                    if (item is StorageFolder folderInformation && !folder.DisplayName.StartsWith("."))
                    {
                        storageItems.Add(new DiskItem
                        {
                            Title = folderInformation.DisplayName,
                            Icon = "\uE8B7",
                            Path = folderInformation.Path,
                            Cache = item,
                            DiskItemLocation = DiskItemLocationType.Local,
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
                            Cache = item,
                            DiskItemLocation = DiskItemLocationType.Local,
                        });
                    }
                }));

            return storageItems.OrderBy(t => t.IsFile);
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
            if (!await InternetConnectivityHelper.CheckAndNotifyAsync())
                return;
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
                else
                {
                    currentDiskItem = null;
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
            if (folder != null)
            {
                StorageItems.AddRange(await GetStorageItemsInFolderAsync(folder));
                item.Cache = folder;
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
                            DiskItemLocation = DiskItemLocationType.Network
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
                            DiskItemLocation = DiskItemLocationType.Network
                        });
                    }
                }
            }
            return networkStorageItems.OrderBy(t => t.IsFile);
        }
        private async Task<IEnumerable<DiskItem>> GetItemsOnNetworkPCAsync(string hostname, string username, string password)
        {
            SharpCifs.Config.SetProperty("jcifs.smb.client.lport", "8137");
            SharpCifs.Config.SetProperty("jcifs.smb.client.laddr", InternetConnectivityHelper.LocalIp);
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
        
        private async Task BrowseOneDriveAsync(DiskItem item)
        {
            if (!await InternetConnectivityHelper.CheckAndNotifyAsync())
                return;
            try
            {
                Clear();
                if (item.Cache == null)
                {
                    if (OneDriveService.Instance.Initialize("000000004C1B185C", AccountProviderType.Msa, OneDriveScopes.OfflineAccess | OneDriveScopes.ReadWrite))
                    {
                        if (!await OneDriveService.Instance.LoginAsync())
                        {
                            throw new Exception("Unable to sign in");
                        }
                        var folder = await OneDriveService.Instance.RootFolderAsync();
                        StorageItems.AddRange(await GetOneDriveFolderItemsAsync(folder));
                    }
                }
                else
                {
                    StorageItems.AddRange(await GetOneDriveFolderItemsAsync((OneDriveStorageFolder)item.Cache));
                }
            }
            catch (Exception ex)
            {
                BLogger.E("Error while browsing OneDrive folder.", ex);
            }
        }
        private async Task<IEnumerable<DiskItem>> GetOneDriveFolderItemsAsync(OneDriveStorageFolder folder)
        {
            List<DiskItem> oneDriveStorageItems = new List<DiskItem>();
            foreach (var item in await folder.GetItemsAsync(folder.OneDriveItem.Folder.ChildCount ?? short.MaxValue))
            {
                if (!item.IsOneNote())
                {
                    if (item.IsFolder())
                    {
                        oneDriveStorageItems.Add(new DiskItem
                        {
                            Path = item.Path,
                            Title = item.DisplayName,
                            Cache = item,
                            DiskItemLocation = DiskItemLocationType.OneDrive
                        });
                    }
                    else if (item.IsFile())
                    {
                        oneDriveStorageItems.Add(new DiskItem
                        {
                            Path = await ((OneDriveStorageFile)item).GetDownloadURL(),
                            Title = item.DisplayName,
                            Cache = item,
                            IsFile = true,
                            Size = item.OneDriveItem.Size.Value.ToString(),
                            DiskItemLocation = DiskItemLocationType.OneDrive
                        });
                    }
                }
            }
            return oneDriveStorageItems.OrderBy(t => t.IsFile);
        }

    }
}
