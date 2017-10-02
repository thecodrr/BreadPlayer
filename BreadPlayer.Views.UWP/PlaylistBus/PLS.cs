using BreadPlayer.Core.Models;
using BreadPlayer.Helpers;
using BreadPlayer.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;

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
                List<string> playlistSongs = new List<string>();

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
                    await Task.Run(() =>
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
                            playlistSongs.Add(path);
                        }
                        catch
                        {
                            failedFiles++;
                        }
                    });
                }
                return await PlaylistHelper.GetSongsInAllFoldersAsync(playlistSongs).ConfigureAwait(false);
            }
        }

        public async Task<bool> SavePlaylist(IEnumerable<Mediafile> songs, Stream fileStream)
        {           
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                await writer.WriteLineAsync("[playlist]").ConfigureAwait(false);
                await writer.WriteLineAsync("").ConfigureAwait(false);
                int i = 0;
                foreach (var track in songs)
                {
                    i++;
                    await writer.WriteLineAsync(string.Format("File{0}={1}", i, track.Path)).ConfigureAwait(false);
                    await writer.WriteLineAsync(string.Format("Title{0}={1}", i, track.Title)).ConfigureAwait(false);
                    await writer.WriteLineAsync(string.Format("Length{0}={1}", i, track.Length)).ConfigureAwait(false);
                    await writer.WriteLineAsync("");
                }
                await writer.WriteLineAsync("NumberOfEntries=" + i).ConfigureAwait(false);
                await writer.WriteLineAsync("Version=2").ConfigureAwait(false);
            }
            return false;
        }
    }
}