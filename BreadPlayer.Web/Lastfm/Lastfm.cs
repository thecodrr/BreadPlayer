using IF.Lastfm.Core.Scrobblers;
using System;
using System.Threading.Tasks;
using PCLStorage;
using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Objects;

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
            var response = await LastfmClient.Auth.GetSessionTokenAsync(username, password);            
            Auth = LastfmClient.Auth;
            return LastfmClient.Auth.Authenticated;
        }
        ILastAuth Auth { get; set; }       
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
