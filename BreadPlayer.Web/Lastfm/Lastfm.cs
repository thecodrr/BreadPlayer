using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Objects;
using IF.Lastfm.Core.Scrobblers;
using PCLStorage;
using System;
using System.Threading.Tasks;

namespace BreadPlayer.Web.Lastfm
{
    public class Lastfm
    {
        public LastfmClient LastfmClient
        {
            get; set;
        }

        public Lastfm()
        {
            LastfmClient = new LastfmClient("c0e0693c36913c6f691e36d6d199aa79", "1d9298cdcca7a7488d92ab13803ceb7e");
        }

        public async Task<bool> Login(string username, string password)
        {
            await LastfmClient.Auth.GetSessionTokenAsync(username, password);
            return LastfmClient.Auth.Authenticated;
        }

        public async Task<ScrobbleResponse> Scrobble(params string[] data)
        {
            try
            {
                if (!LastfmClient.Auth.Authenticated)
                {
                    return new ScrobbleResponse(LastResponseStatus.BadAuth);
                }

                IFolder rootFolder = FileSystem.Current.LocalStorage;
                IFolder folder = await rootFolder.CreateFolderAsync("db",
                    CreationCollisionOption.OpenIfExists);
                IFile file = await folder.CreateFileAsync("scrobbles.db",
                    CreationCollisionOption.OpenIfExists);
                IScrobbler scrobbler = new BreadScrobbler(LastfmClient.Auth, file.Path);
                return (await scrobbler.ScrobbleAsync(new Scrobble(data[0], data[1], data[2], DateTimeOffset.Now)));
            }
            catch (NullReferenceException)
            {
                return new ScrobbleResponse(LastResponseStatus.Failure);
            }
        }
    }
}