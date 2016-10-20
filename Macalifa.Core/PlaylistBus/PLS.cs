using BreadPlayer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace BreadPlayer.PlaylistBus
{
    class PLS : IPlaylist
    {
        public async Task<Dictionary<Playlist, IEnumerable<Mediafile>>> LoadPlaylist(StorageFile file)
        {
            Dictionary<Playlist, IEnumerable<Mediafile>> PlaylistDict = new Dictionary<Models.Playlist, IEnumerable<Mediafile>>();
            Playlist Playlist = new Playlist() { Name = file.DisplayName };
            using (var reader = new StreamReader(await file.OpenStreamForReadAsync()))
            {
                bool hdr = false; //[playlist] header
                string version = ""; //pls version at the end
                int noe = 0; //numberofentries at the end.
                int nr = 0;
                string line; //a single line in stream
                List<string> lines = new List<string>();
                List<Mediafile> PlaylistSongs = new List<Mediafile>();
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                    nr++;
                    if (line == "[playlist]")
                        hdr = true;
                    else if (!hdr)
                        return null;
                    else if (line.ToLower().StartsWith("numberofentries="))
                        noe = Convert.ToInt32(line.Split('=')[1]);
                    else if (line.ToLower().StartsWith("version="))
                        version = line.Split('=')[1];
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
                            continue;
                        else
                            tracks[number - 1, index] = split[1];
                    }
                    else if (!_l.StartsWith("numberofentries") && _l != "[playlist]" && !_l.StartsWith("version="))
                    {
                        continue;
                    }
                }
                for (int i = 0; i < noe; i++)
                {
                    string trackPath = tracks[i, 0];
                    Mediafile mp3file = await Core.CoreMethods.CreateMediafile(await StorageFile.GetFileFromPathAsync(trackPath));
                    PlaylistSongs.Add(mp3file);
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
                writer.WriteLine("[playlist]");
                writer.WriteLine("");
                int i = 0;
                foreach (var track in Songs)
                {
                    i++;
                    writer.WriteLine(String.Format("File{0}={1}", i, track.Path));
                    writer.WriteLine(String.Format("Title{0}={1}", i, track.Title));
                    writer.WriteLine(String.Format("Length{0}={1}", i, track.Length));
                    writer.WriteLine("");
                }
                writer.WriteLine("NumberOfEntries=" + i);
                writer.WriteLine("Version=2");
            }
            return false;
        }
    }
}

