using IF.Lastfm.Core.Api;
using System.Threading.Tasks;

namespace BreadPlayer.Web.Lastfm
{
    public class InitializeLastfm
    {
        public LastfmClient Auth
        {
            get;set;
        }
        public InitializeLastfm(string username, string password) : this()
        {
            Login(username, password);
        }
        public InitializeLastfm()
        {
            Auth = new LastfmClient("c0e0693c36913c6f691e36d6d199aa79", "1d9298cdcca7a7488d92ab13803ceb7e");
        }
        public async Task<bool> Login(string username, string password)
        {
            var response = await Auth.Auth.GetSessionTokenAsync(username, password);

            return Auth.Auth.Authenticated;
        }
    }
}
