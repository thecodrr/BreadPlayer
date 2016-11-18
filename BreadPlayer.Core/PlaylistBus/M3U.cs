using BreadPlayer.Models;
using BreadPlayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;

namespace BreadPlayer.PlaylistBus
{
   public class M3U : IPlaylist
    {
        public async Task LoadPlaylist(StorageFile file)
        {
            Core.CoreMethods.LibVM.Database.CreatePlaylistDB(file.DisplayName);
            // List<Mediafile> PlaylistSongs = new List<Mediafile>();
            // Dictionary<Playlist, IEnumerable<Mediafile>> PlaylistDict = new Dictionary<Models.Playlist, IEnumerable<Mediafile>>();
            Playlist Playlist = new Playlist() { Name = file.DisplayName };
            using (var streamReader = new StreamReader(await file.OpenStreamForReadAsync()))
            {
                string line;
                int index = 0;
                int failedFiles = 0;
                bool ext = false;
                while ((line = streamReader.ReadLine()) != null)
                {

                    if (line.ToLower() == "#extm3u") //m3u header
                        ext = true;
                    else if (ext && line.ToLower().StartsWith("#extinf:")) //extinfo of each song
                        continue;
                    else if (line.StartsWith("#") || line == "") //pass blank lines
                        continue;
                    else
                    {
                        await Task.Run(async () =>
                        {
                            try
                            {
                                index++;
                                FileInfo info = new FileInfo(file.Path);//get playlist file info to get directory path
                                string path = line;
                                if (!File.Exists(line) && line[1] != ':') // if file doesn't exist then perhaps the path is relative
                                {
                                    path = info.DirectoryName + line; //add directory path to song path.
                                }

                                var accessFile = await StorageFile.GetFileFromPathAsync(path);
                                var token = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(accessFile);

                                Mediafile mp3File = await Core.CoreMethods.CreateMediafile(accessFile); //prepare Mediafile
                                await SettingsViewModel.SaveSingleFileAlbumArtAsync(mp3File, accessFile);

                                await Core.CoreMethods.NotificationManager.ShowAsync(index.ToString() + " songs sucessfully added into playlist: " + file.DisplayName);

                                if (!Core.CoreMethods.LibVM.Database.tracks.Exists(t => t._id == mp3File._id))
                                    Core.CoreMethods.LibVM.Database.Insert(mp3File);

                                StorageApplicationPermissions.FutureAccessList.Remove(token);
                            }
                            catch
                            {
                                failedFiles++;
                            }
                        });
                    }

                }
                string message = string.Format("Playlist \"{3}\" successfully imported! Total Songs: {0} Failed: {1} Succeeded: {2}", index, failedFiles, index - failedFiles, file.DisplayName);
                await Core.CoreMethods.NotificationManager.ShowAsync(message);
                Core.CoreMethods.LibVM.Database.CreateDB();
            }
        }
        public async Task<bool> SavePlaylist(IEnumerable<Mediafile> Songs)
        {
            FileSavePicker picker = new FileSavePicker();
            picker.FileTypeChoices.Add("M3U Playlists", new List<string>() { ".m3u" });
            picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            var file = await picker.PickSaveFileAsync();
            using (StreamWriter writer = new StreamWriter(await file.OpenStreamForWriteAsync()))
            {
                writer.WriteLine("#EXTM3U");
                writer.WriteLine("");
                foreach (var track in Songs)
                {
                    writer.WriteLine(String.Format("#EXTINF:{0},{1} - {2}", track.Length, track.LeadArtist, track.Title));
                    writer.WriteLine(track.Path);
                    writer.WriteLine("");
                }
            }
            return false;
        }
    }
}
