using IF.Lastfm.Core.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Web.Lastfm
{
    public class InitializeLastfm
    {
        public LastfmClient Auth
        {
            get;set;
        }
        public InitializeLastfm()
        {
            Initialize();
        }
        public void Initialize()
        {
            //Auth = new LastAuth("2ce75ee035ba6ba994494ea5dee7f9c0", "fd2adb9f756533d8af70bfd680ddc2e6");
        }
        public async Task<bool> Login(string username, string password)
        {
            Auth = new LastfmClient("c0e0693c36913c6f691e36d6d199aa79", "1d9298cdcca7a7488d92ab13803ceb7e");
            var response = await Auth.Auth.GetSessionTokenAsync(username, password);
            if (Auth.Auth.Authenticated)
            {
                response = await Auth.Track.LoveAsync("Ibi Dreams of Pavement (A Better Day)", "Broken Social Scene");
            }
            if (response.Success)
                return true;
            return false;
        }
    }
}
