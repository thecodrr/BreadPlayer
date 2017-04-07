using Xunit;
using BreadPlayer.Web.Lastfm;
using System.Threading.Tasks;
using BreadPlayer.Web._123music;
using BreadPlayer.Web.BaiduLyricsAPI;
using BreadPlayer.Web.TagParser;
using IF.Lastfm.Core.Api.Helpers;

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
            Assert.True((await HasScrobbled(artist,album,title)).Success);
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
        [Theory]
        [InlineData(DataType._new)]
        [InlineData(DataType._hot)]
        public async void SongsListTest(DataType term)
        {
            Assert.True(await new API().GetSongsList(term));
        }
        [Theory]
        [InlineData(DataType._new)]
        [InlineData(DataType._hot)]
        public async void ArtistsListTest(DataType term)
        {
            Assert.True(await new API().GetArtistsList(term));
        }
        [Theory]
        [InlineData(DataType._new)]
        [InlineData(DataType._hot)]
        public async void AlbumsListTest(DataType term)
        {
            Assert.True(await new API().GetAlbumsList(term));
        }
        [Theory]
        [InlineData("172072")]
        public async void RequestSongInfoTest(string id)
        {
            Assert.NotNull(await new ApiMethods().RequestAlbumByArtist(id));
        }
        [Theory]
        [InlineData("172072")]
        public async void RequestArtistInfoTest(string id)
        {
            Assert.NotNull(await new ApiMethods().RequestArtistInfo(id));
        }
        [Theory]
        [InlineData("172072")]
        public async void RequestMusicInfoTest(string id)
        {
            Assert.NotNull(await new ApiMethods().RequestMusicInfo(id));
        }
        [Theory]
        [InlineData("eminem", "justin bieber")]
        [InlineData("The Way I Am", "Nothing Like Us")]
        [InlineData("The Way I Am", "eminem")]
        [InlineData("justin bieber", "Nothing Like Us")]
        public void ComparisonTest(string a, string b)
        {
            Assert.NotNull(new BreadParser().Compare2Strings(a, b));
        }
        async Task<LastResponse> HasScrobbled(params string[] mediaFile)
        {
            await init.Login("thecodrr", "Allatonce1.1");
            Lastfm last = new Lastfm(init.Auth.Auth);
            return await last.Scrobble(mediaFile);
        }
        async Task<bool> IsLoggedIn(string user, string pass)
        {
            return await init.Login(user, pass);
        }
    }
}
