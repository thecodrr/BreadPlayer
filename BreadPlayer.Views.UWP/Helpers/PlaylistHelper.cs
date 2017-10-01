using BreadPlayer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using System.IO;
using BreadPlayer.Dispatcher;
using Windows.Storage;

namespace BreadPlayer.Helper
{
    public class PlaylistHelper
    {
        public async Task<bool> SavePlaylist(Playlist playlist, IEnumerable<Mediafile> songs)
        {
            bool saved = false;
            FileSavePicker picker = new FileSavePicker();
            picker.FileTypeChoices.Add("PLS Playlists", new List<string> { ".pls" });
            picker.FileTypeChoices.Add("M3U Playlists", new List<string> { ".m3u" });
            picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            picker.SuggestedFileName = playlist.Name;

            await BreadDispatcher.InvokeAsync(async () =>
            {
                var file = await picker.PickSaveFileAsync();
                if (file != null)
                {
                    IPlaylist sPlaylist = null;
                    switch (file.FileType.ToLower())
                    {
                        case ".m3u":
                            sPlaylist = new M3U();
                            break;
                        case ".pls":
                            sPlaylist = new Pls();
                            break;
                    }
                    saved = await sPlaylist.SavePlaylist(songs, await file.OpenStreamForWriteAsync().ConfigureAwait(false)).ConfigureAwait(false);
                }
            });
            return saved;
        }
    }
}
