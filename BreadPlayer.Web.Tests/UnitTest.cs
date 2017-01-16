using Xunit;
using BreadPlayer.Web.Lastfm;
using System.Threading.Tasks;
using BreadPlayer.Models;

namespace MyFirstUWPTests
{
    public class UnitTest
    {
        InitializeLastfm init = new InitializeLastfm();
        [Theory]
        [InlineData("","")]
        [InlineData("", "")]
        [InlineData("", "")]
        public async void LoginTest(string user, string pass)
        {
            Assert.True(await IsLoggedIn(user, pass));
        }
        [Theory]
        [InlineData("Eminem","Southpaw","Phenomenal")]
        [InlineData("", "Southpaw", "Phenomenal")]
        [InlineData("", "Southpaw", "")]
        [InlineData("", "", "Phenomenal")]
        [InlineData("Eminem", "", "")]
        public async void ScrobbleTest(string artist, string album, string title)
        {
            Assert.True(await HasScrobbled(artist,album,title));
        }
        [Theory]
        [InlineData("Eminem")]
        [InlineData("Southpaw")]
        [InlineData("Hello")]
        [InlineData("lakjcsa")]
        [InlineData("Hello Adele")]
        [InlineData("Adele Hello")]
        [InlineData("Collage")]
        public async void _123MusicAPISearchTest(string term)
        {
            Assert.True(await new BreadPlayer.Web._123music.API().SearchSongs(term));
        }
        async Task<bool> HasScrobbled(params string[] mediaFile)
        {
            await init.Login("", "");
            Lastfm last = new Lastfm();
            return await last.Scrobble(init.Auth.Auth, mediaFile);
        }
        async Task<bool> IsLoggedIn(string user, string pass)
        {
            return await init.Login(user, pass);
        }
    }
}