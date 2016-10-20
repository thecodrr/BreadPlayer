using BreadPlayer.Models;
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
        public async Task<Dictionary<Playlist, IEnumerable<Mediafile>>> LoadPlaylist(StorageFile file)
        {
            Dictionary<Playlist, IEnumerable<Mediafile>> PlaylistDict = new Dictionary<Models.Playlist, IEnumerable<Mediafile>>();
            Playlist Playlist = new Playlist() { Name = file.DisplayName };
            using (var streamReader = new StreamReader(await file.OpenStreamForReadAsync()))
            {
                string line;
                bool ext = false;
                List<Mediafile> PlaylistSongs = new List<Mediafile>();
                while ((line = streamReader.ReadLine()) != null)
                {
                    try
                    {
                        if (line.ToLower() == "#extm3u") //m3u header
                            ext = true;
                        else if (ext && line.ToLower().StartsWith("#extinf:")) //extinfo of each song
                            continue;
                        else if (line.StartsWith("#") || line == "") //pass blank lines
                            continue;
                        else
                        {
                            await Task.Run( async () => 
                            {
                                FileInfo info = new FileInfo(file.Path);//get playlist file info to get directory path
                            var accessFile = await StorageFile.GetFileFromPathAsync(line);
                                var token = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(accessFile);
                                string path = line;
                                if (!File.Exists(line)) // if file doesn't exist then perhaps the path is relative
                                {                                    
                                    path = info.DirectoryName + line; //add directory path to song path.
                                }
                                Mediafile mp3File = await Core.CoreMethods.CreateMediafile(await StorageFile.GetFileFromPathAsync(path)); //prepare Mediafile
                                PlaylistSongs.Add(mp3File); //add Mediafile to list.

                                Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Remove(token);
                            });                           
                        }
                    }
                    catch (FileNotFoundException)
                    {

                    }
                }
                PlaylistDict.Add(Playlist, PlaylistSongs);
                return PlaylistDict;
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
