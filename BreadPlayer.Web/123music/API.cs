using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BreadPlayer.Web._123music
{
    public class Api
    {
        public async Task<bool> SearchSongs(string term)
        {
            var htmlDoc = await GetHtmlResponseDocumentAsync(string.Format(Endpoints.SearchEndpoint, "songs", term, ""));
            string results = htmlDoc.QuerySelector("a[title=\"Songs\"]").TextContent;
            int songCount = Convert.ToInt32(Regex.Match(results, @"\d+").Value);
            if (songCount <= 0)
            {
                return false;
            }

            int totalPages = Convert.ToInt32(htmlDoc.QuerySelector("ul.pagination")?.Children.Last().FirstElementChild.GetAttribute("data-ci-pagination-page"));
            if (totalPages > 0)
            {
                List<Track> songs = new List<Track>(GetSongs(htmlDoc));
                for (int i = 2; i <= 3; i++)
                {
                    var page = await GetHtmlResponseDocumentAsync(string.Format(Endpoints.SearchEndpoint, "songs", term, i + ".html"));
                    songs.AddRange(GetSongs(page));
                }
            }
            return true;
        }

        public Task<bool> GetSongsList(DataType type)
        {
            return GetItemLists<Track>(string.Format(Endpoints.SongsListEndpoint, type.ToString().Remove(0, 1)), type == DataType.Hot ? true : false);
        }

        public Task<bool> GetArtistsList(DataType type)
        {
            return GetItemLists<Artist>(string.Format(Endpoints.ArtistsListEndpoint, type.ToString().Remove(0, 1)), type == DataType.Hot ? true : false);
        }

        private async Task<bool> GetItemLists<T>(string url, bool isJson)
        {
            var htmlDoc = isJson ? await GetJsonResponseDocumentAsync(url) : await GetHtmlResponseDocumentAsync(Endpoints.BaseUrl);
            if (htmlDoc != null)
            {
                List<T> items = new List<T>(GetItems<T>(htmlDoc, isJson));
                return items.Count > 0;
            }
            return false;
        }

        private IEnumerable<T> GetItems<T>(IHtmlDocument htmlDoc, bool directList = false)
        {
            if (typeof(T) == typeof(Album))
            {
                return (IEnumerable<T>)GetAlbums(htmlDoc, directList);
            }

            if (typeof(T) == typeof(Artist))
            {
                return (IEnumerable<T>)GetArtists(htmlDoc, directList);
            }

            if (typeof(T) == typeof(Track))
            {
                return (IEnumerable<T>)GetSongs(htmlDoc, directList);
            }

            return null;
        }

        public Task<bool> GetAlbumsList(DataType type)
        {
            return GetItemLists<Album>(string.Format(Endpoints.AlbumsListEndpoint, type.ToString().Remove(0, 1)), type == DataType.New ? true : false);
        }

        private async Task<IHtmlDocument> GetJsonResponseDocumentAsync(string url)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.GetStringAsync(url);
                var tmp = JsonConvert.DeserializeObject<ResponseMessage>(response);
                return await new HtmlParser().ParseAsync(tmp.Html);
            }
        }

        private async Task<IHtmlDocument> GetHtmlResponseDocumentAsync(string url)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var html = await httpClient.GetStringAsync(url);
                return await new HtmlParser().ParseAsync(html);
            }
        }

        private IEnumerable<Track> GetSongs(IHtmlDocument htmlDoc, bool directList = false)
        {
            var items = directList ? htmlDoc.QuerySelectorAll("div.item") : htmlDoc.QuerySelector("div.song-list").QuerySelectorAll("div.item");
            if (items.Any())
            {
                foreach (var item in items)
                {
                    string songId = item.QuerySelector("span.playlist-tool").GetAttribute("data-song-id");
                    string title = item.QuerySelector("h3").TextContent.Trim();
                    var singerNode = item.QuerySelector(".singer");
                    string artist = singerNode.FirstElementChild.GetAttribute("title").Trim();
                    string artistId = Regex.Match(singerNode.FirstElementChild.GetAttribute("href").Replace(Endpoints.BaseUrl + "artist/", ""), @"\d+").Value; //after replacing the artist endpoint only ID digits are left.
                    int listenCount = Convert.ToInt32(item.QuerySelector("span.item-view").TextContent);
                    yield return new Track(songId, title, artist, artistId, listenCount);
                }
            }
        }

        private IEnumerable<Artist> GetArtists(IHtmlDocument htmlDoc, bool directList = false)
        {
            var items = directList ? htmlDoc.QuerySelectorAll("div.item") : htmlDoc.QuerySelector(".artist-list").QuerySelectorAll("div.item");
            if (items.Any())
            {
                foreach (var item in items)
                {
                    var artistLink = item.QuerySelector(".thumb");
                    string artistId = GetIdFromUrl(artistLink.GetAttribute("href"));
                    string artistName = artistLink.GetAttribute("title");
                    string artistPhoto = artistLink.FirstElementChild.GetAttribute("src");
                    yield return new Artist(artistName, artistPhoto, artistId);
                }
            }
        }

        private string GetIdFromUrl(string url)
        {
            return Regex.Match(url.Substring(url.IndexOf('-') + 1), @"\d+").Value;
        }

        private IEnumerable<Album> GetAlbums(IHtmlDocument htmlDoc, bool directList = false)
        {
            var items = directList ? htmlDoc.QuerySelectorAll("div.item") : htmlDoc.QuerySelector("div.album-list").QuerySelectorAll("div.item");
            if (items.Any())
            {
                foreach (var item in items)
                {
                    var subChild = item.QuerySelector(".item-caption");
                    var albumLink = subChild.QuerySelector("h3 > a");
                    var artistLink = subChild.QuerySelector(".singer > a");
                    string artistId = GetIdFromUrl(artistLink.GetAttribute("href"));
                    string albumId = GetIdFromUrl(albumLink.GetAttribute("href"));
                    string artistName = artistLink.GetAttribute("title");
                    string albumName = albumLink.GetAttribute("title");
                    string albumPhoto = item.FirstElementChild.FirstElementChild.GetAttribute("src");
                    yield return new Album(albumName, albumId, albumPhoto, artistName, artistId);
                }
            }
        }
    }
}