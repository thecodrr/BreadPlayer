using IF.Lastfm.Core.Scrobblers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;
using IF.Lastfm.Core.Api;

namespace BreadPlayer.Web.Lastfm
{
    public class Lastfm
    {
        public async Task<bool> Scrobble(ILastAuth Auth, params string[] data)
        {
          
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync("db",
                CreationCollisionOption.OpenIfExists);
            IFile file = await folder.CreateFileAsync("scrobbles.db",
                CreationCollisionOption.ReplaceExisting);
            IScrobbler _scrobbler = new BreadScrobbler(Auth, file.Path);
           return (await _scrobbler.ScrobbleAsync(new IF.Lastfm.Core.Objects.Scrobble(data[0], data[1], data[2], DateTimeOffset.Now))).Success;
        }
    }
}
