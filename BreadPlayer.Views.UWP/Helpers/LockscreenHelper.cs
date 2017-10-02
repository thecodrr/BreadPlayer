using BreadPlayer.Common;
using BreadPlayer.Core.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System.UserProfile;
using Windows.UI.Core;
using Windows.UI.Popups;

namespace BreadPlayer.Helpers
{
    public class LockscreenHelper
    {
        public static StorageFile DefaultImage { get; set; }

        public static async Task<bool> SaveCurrentLockscreenImage()
        {
            if (SettingsHelper.GetLocalSetting<string>("DefaultImagePath", "") != "")
            {
                DefaultImage = await StorageFile.GetFileFromPathAsync(SettingsHelper.GetLocalSetting<string>("DefaultImagePath", ""));
                return true;
            }
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                if (DefaultImage == null)
                {
                    bool success = false;
                    await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        MessageDialog dialog = new MessageDialog("To enable this feature you must set a default lockscreen wallpaper.\rPress OK to continue or cancel to cancel.", "Choose a default lockscreen wallpaper");
                        dialog.Commands.Add(new UICommand("OK"));
                        dialog.Commands.Add(new UICommand("Cancel"));
                        var response = await dialog.ShowAsync();
                        if (response.Label == "OK")
                        {
                            FileOpenPicker defaultLockScreenImageDialog = new FileOpenPicker()
                            {
                                CommitButtonText = "Set default lockscreen image."
                            };
                            defaultLockScreenImageDialog.FileTypeFilter.Add(".jpg");
                            defaultLockScreenImageDialog.FileTypeFilter.Add(".png");
                            var image = await defaultLockScreenImageDialog.PickSingleFileAsync();
                            if (image != null)
                            {
                                DefaultImage = await image.CopyAsync(ApplicationData.Current.LocalFolder, "lockscreen.jpg", NameCollisionOption.ReplaceExisting);
                                SettingsHelper.SaveLocalSetting("DefaultImagePath", DefaultImage.Path);
                                success = true;
                            }
                        }
                    });

                    return success;
                }
            }
            else if (!File.Exists(ApplicationData.Current.TemporaryFolder.Path + "\\lockscreen.jpg"))
            {
                using (IRandomAccessStream imageStream = LockScreen.GetImageStream())
                using (var reader = new DataReader(imageStream))
                {
                    var lockscreenFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("lockscreen.jpg", CreationCollisionOption.FailIfExists);
                    await reader.LoadAsync((uint)imageStream.Size);
                    var buffer = new byte[(int)imageStream.Size];
                    reader.ReadBytes(buffer);
                    await FileIO.WriteBytesAsync(lockscreenFile, buffer);
                    DefaultImage = lockscreenFile;
                    StorageApplicationPermissions.FutureAccessList.Add(DefaultImage);
                }
                return true;
            }
            return false;
        }

        public static async Task ChangeLockscreenImage(Mediafile mediaFile)
        {
            if (!string.IsNullOrEmpty(mediaFile.AttachedPicture))
            {
                await SaveCurrentLockscreenImage();
                var albumart = await StorageFile.GetFileFromPathAsync(mediaFile.AttachedPicture);
                await UserProfilePersonalizationSettings.Current.TrySetLockScreenImageAsync(albumart);
            }
            else
            {
                await ResetLockscreenImage();
            }
        }

        public static async Task ResetLockscreenImage()
        {
            if (DefaultImage != null)
            {
                if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                {
                    await UserProfilePersonalizationSettings.Current.TrySetLockScreenImageAsync(DefaultImage);
                }
                else
                {
                    await LockScreen.SetImageFileAsync(DefaultImage);
                }
            }
        }
    }
}