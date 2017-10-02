using BreadPlayer.Web._123music;
using BreadPlayer.Web.BaiduLyricsAPI;
using Xunit;

namespace MyFirstUWPTests
{
    public class UnitTest
    {
        //private InitializeLastfm _init = new InitializeLastfm();
        //[Theory]
        //[InlineData("", "")]
        //[InlineData("", "")]
        //[InlineData("", "")]
        //public async void LoginTest(string user, string pass)
        //{
        //    Assert.True(await IsLoggedIn(user, pass));
        //}
        //[Theory]
        //[InlineData("Eminem", "Southpaw", "Phenomenal")]
        //[InlineData("", "Southpaw", "Phenomenal")]
        //[InlineData("", "Southpaw", "")]
        //[InlineData("", "", "Phenomenal")]
        //[InlineData("Eminem", "", "")]
        //public async void ScrobbleTest(string artist, string album, string title)
        //{
        //    Assert.True((await HasScrobbled(artist, album, title)).Success);
        //}
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
            Assert.True(await new Api().SearchSongs(term));
        }

        [Theory]
        [InlineData(DataType.New)]
        [InlineData(DataType.Hot)]
        public async void SongsListTest(DataType term)
        {
            Assert.True(await new Api().GetSongsList(term));
        }

        [Theory]
        [InlineData(DataType.New)]
        [InlineData(DataType.Hot)]
        public async void ArtistsListTest(DataType term)
        {
            Assert.True(await new Api().GetArtistsList(term));
        }

        [Theory]
        [InlineData(DataType.New)]
        [InlineData(DataType.Hot)]
        public async void AlbumsListTest(DataType term)
        {
            Assert.True(await new Api().GetAlbumsList(term));
        }

        //[Theory]
        //[InlineData("172072")]
        //public async void RequestSongInfoTest(string id)
        //{
        //    Assert.NotNull(await new ApiMethods().RequestAlbumByArtist(id));
        //}
        [Theory]
        [InlineData("269398624")]
        public async void RequestArtistInfoTest(string id)
        {
            Assert.NotNull(await new BaiduClient().RequestSongLrc(id));
        }

        [Theory]
        [InlineData("eminem")]
        public async void RequestMusicInfoTest(string id)
        {
            Assert.NotNull(await new BaiduClient().Search(id));
        }

        [Theory]
        [InlineData("eminem")]
        public async void SearchSongsTest(string id)
        {
            Assert.NotNull(await new BreadPlayer.Web.NeteaseLyricsAPI.NeteaseClient().SearchSongs(id));
        }

        [Theory]
        [InlineData("17572546")]
        public async void GetLyricsTest(string id)
        {
            Assert.NotNull(await new BreadPlayer.Web.NeteaseLyricsAPI.NeteaseClient().GetLyrics(id));
        }

        [Theory]
        [InlineData("stan")]
        public async void Xiami_SearchSongTest(string id)
        {
            Assert.NotNull(await new BreadPlayer.Web.XiamiLyricsAPI.XiamiClient().SearchAsync(id));
        }

        //[Theory]
        //[InlineData("eminem", "justin bieber")]
        //[InlineData("The Way I Am", "Nothing Like Us")]
        //[InlineData("The Way I Am", "eminem")]
        //[InlineData("justin bieber", "Nothing Like Us")]
        //public void ComparisonTest(string a, string b)
        //{
        //    Assert.NotNull(new BreadParser().Compare2Strings(a, b));
        //}

        //private async Task<LastResponse> HasScrobbled(params string[] mediaFile)
        //{
        //    await _init.Login("thecodrr", "Allatonce1.1");
        //    Lastfm last = new Lastfm(_init.Auth.Auth);
        //    return await last.Scrobble(mediaFile);
        //}

        //private async Task<bool> IsLoggedIn(string user, string pass)
        //{
        //    return await _init.Login(user, pass);
        //}
    }
}