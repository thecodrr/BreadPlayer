using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using BreadPlayer.Core;
using BreadPlayer.Core.Models;
using BreadPlayer.Database;
using BreadPlayer.ViewModels;
using BreadPlayer.Helpers;

namespace BreadPlayer.PlaylistBus
{
    internal class Pls : IPlaylist
    {
        public async Task<IEnumerable<Mediafile>> LoadPlaylist(StorageFile file)
        {
            using (var reader = new StreamReader(await file.OpenStreamForReadAsync()))
            {
                bool hdr = false; //[playlist] header
                string version = ""; //pls version at the end
                int noe = 0; //numberofentries at the end.
                int nr = 0;
                int failedFiles = 0;
                //int count = 0;
                string line; //a single line in stream
                List<string> lines = new List<string>();
                List<Mediafile> playlistSongs = new List<Mediafile>();

                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                    nr++;
                    if (line == "[playlist]")
                    {
                        hdr = true;
                    }
                    else if (!hdr)
                    {
                        return null;
                    }
                    else if (line.ToLower().StartsWith("numberofentries="))
                    {
                        noe = Convert.ToInt32(line.Split('=')[1]);
                    }
                    else if (line.ToLower().StartsWith("version="))
                    {
                        version = line.Split('=')[1];
                    }
                }
                string[,] tracks = new string[noe, 3];
                nr = 0;
                foreach (string l in lines)
                {
                    var _l = l.ToLower();
                    if (_l.StartsWith("file") || _l.StartsWith("title") || _l.StartsWith("length"))
                    {
                        int tmp = 4;
                        int index = 0;
                        if (_l.StartsWith("title")) { tmp = 5; index = 1; }
                        else if (_l.StartsWith("length")) { tmp = 6; index = 2; }

                        string[] split = l.Split('=');
                        int number = Convert.ToInt32(split[0].Substring(tmp));

                        if (number > noe)
                        {
                            continue;
                        }

                        tracks[number - 1, index] = split[1];
                    }
                    //else if (!_l.StartsWith("numberofentries") && _l != "[playlist]" && !_l.StartsWith("version="))
                    //{
                    //}
                }
               
                for (int i = 0; i < noe; i++)
                {
                    await Task.Run(async () =>
                    {
                        try
                        {
                            string trackPath = tracks[i, 0];
                            FileInfo info = new FileInfo(file.Path);//get playlist file info to get directory path
                            string path = trackPath;
                            if (!File.Exists(trackPath) && line[1] != ':') // if file doesn't exist then perhaps the path is relative
                            {
                                path = info.DirectoryName + line; //add directory path to song path.
                            }
                            var accessFile = await StorageFile.GetFileFromPathAsync(path);
                            var token = StorageApplicationPermissions.FutureAccessList.Add(accessFile);

                            Mediafile mp3File = await TagReaderHelper.CreateMediafile(accessFile); //prepare Mediafile
                            await SettingsViewModel.SaveSingleFileAlbumArtAsync(mp3File, accessFile);

                            playlistSongs.Add(mp3File);
                            StorageApplicationPermissions.FutureAccessList.Remove(token);
                        }
                        catch
                        {
                            failedFiles++;
                        }
                    });
                   
                }
                return playlistSongs;
            }
        }

        public async Task<bool> SavePlaylist(IEnumerable<Mediafile> songs)
        {
            FileSavePicker picker = new FileSavePicker();
            picker.FileTypeChoices.Add("PLS Playlists", new List<string> { ".pls" });
            picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            var file = await picker.PickSaveFileAsync();
            using (StreamWriter writer = new StreamWriter(await file.OpenStreamForWriteAsync()))
            {
                writer.WriteLine("[playlist]");
                writer.WriteLine("");
                int i = 0;
                foreach (var track in songs)
                {
                    i++;
                    writer.WriteLine(string.Format("File{0}={1}", i, track.Path));
                    writer.WriteLine(string.Format("Title{0}={1}", i, track.Title));
                    writer.WriteLine(string.Format("Length{0}={1}", i, track.Length));
                    writer.WriteLine("");
                }
                writer.WriteLine("NumberOfEntries=" + i);
                writer.WriteLine("Version=2");
            }
            return false;
        }
    }
}
