using IF.Lastfm.Core.Scrobblers;
using System;
using System.Threading.Tasks;
using PCLStorage;
using IF.Lastfm.Core.Api;

namespace BreadPlayer.Web.Lastfm
{
    public class Lastfm
    {
        ILastAuth Auth { get; set; }
        public Lastfm(ILastAuth auth)
        {
            Auth = auth;
        }
        public async Task<ScrobbleResponse> Scrobble(params string[] data)
        {
            try
            {
                if (!Auth.Authenticated)
                    return new ScrobbleResponse(IF.Lastfm.Core.Api.Enums.LastResponseStatus.BadAuth);
                IFolder rootFolder = FileSystem.Current.LocalStorage;
                IFolder folder = await rootFolder.CreateFolderAsync("db",
                    CreationCollisionOption.OpenIfExists);
                IFile file = await folder.CreateFileAsync("scrobbles.db",
                    CreationCollisionOption.OpenIfExists);
                IScrobbler _scrobbler = new BreadScrobbler(Auth, file.Path);
                return (await _scrobbler.ScrobbleAsync(new IF.Lastfm.Core.Objects.Scrobble(data[0], data[1], data[2], DateTimeOffset.Now)));
            }
            catch (NullReferenceException)
            {
                return new ScrobbleResponse(IF.Lastfm.Core.Api.Enums.LastResponseStatus.Failure);
            }
        }
    }
}
