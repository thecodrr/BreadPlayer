using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using System.Net.Http;
using AngleSharp.Parser.Html;
using AngleSharp.Dom.Html;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace BreadPlayer.Web._123music
{
    public class API
    {
        public async Task<bool> SearchSongs(string term)
        {
            var htmlDoc = await GetHtmlResponseDocumentAsync(string.Format(Endpoints.SearchEndpoint, "songs", term, ""));
            string results = htmlDoc.QuerySelector("a[title=\"Songs\"]").TextContent;
            int songCount = Convert.ToInt32(Regex.Match(results, @"\d+").Value);
            if (songCount <= 0)
                return false;
            int totalPages = Convert.ToInt32(htmlDoc.QuerySelector("ul.pagination")?.Children.Last().FirstElementChild.GetAttribute("data-ci-pagination-page"));
            List<Track> Songs = new List<Track>(GetSongs(htmlDoc));
            if (totalPages > 0)
            {
                for (int i = 2; i <= 3; i++)
                {
                    var page = await GetHtmlResponseDocumentAsync(string.Format(Endpoints.SearchEndpoint, "songs", term, i.ToString() + ".html"));
                    Songs.AddRange(GetSongs(page));
                }
            }
           
            return true;
        }
        public async Task<bool> GetSongsList(DataType type)
        {
            return await GetItemLists<Track>(string.Format(Endpoints.SongsListEndpoint, type.ToString().Remove(0, 1)), type == DataType._hot ? true : false);
        }
        public async Task<bool> GetArtistsList(DataType type)
        {
            return await GetItemLists<Artist>(string.Format(Endpoints.ArtistsListEndpoint, type.ToString().Remove(0, 1)), type == DataType._hot ? true : false);
        }
        private async Task<bool> GetItemLists<T>(string url, bool isJson)
        {           
            var htmlDoc = isJson ? await GetJsonResponseDocumentAsync(url) : await GetHtmlResponseDocumentAsync(Endpoints.BaseURL);
            if (htmlDoc != null)
            {
                List<T> Items = new List<T>(GetItems<T>(htmlDoc, isJson));
                if (Items.Count > 0)
                    return true;
            }
            return false;
        }
        private IEnumerable<T> GetItems<T>(IHtmlDocument htmlDoc, bool directList = false)
        {
            if (typeof(T) == typeof(Album))
                return (IEnumerable<T>)GetAlbums(htmlDoc, directList);
            else if (typeof(T) == typeof(Artist)) 
                return (IEnumerable<T>)GetArtists(htmlDoc, directList);
            else if (typeof(T) == typeof(Track))
                return (IEnumerable<T>)GetSongs(htmlDoc, directList);
            else
                return null;
        }
        public async Task<bool> GetAlbumsList(DataType type)
        {
            return await GetItemLists<Album>(string.Format(Endpoints.AlbumsListEndpoint, type.ToString().Remove(0,1)), type == DataType._new ? true : false);
        }       
        private async Task<IHtmlDocument> GetJsonResponseDocumentAsync(string url)
        {
            HttpClient httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(url);
            var tmp = JsonConvert.DeserializeObject<ResponseMessage>(response);
            var parser = new HtmlParser();
            var htmlDoc = await new HtmlParser().ParseAsync(tmp.html);
            return htmlDoc;
        }
        private async Task<IHtmlDocument> GetHtmlResponseDocumentAsync(string url)
        {
            HttpClient httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            var parser = new HtmlParser();
            var htmlDoc = await parser.ParseAsync(html);
            return htmlDoc;
        }
        private IEnumerable<Track> GetSongs(IHtmlDocument htmlDoc, bool directList = false)
        {
            var items = directList ? htmlDoc.QuerySelectorAll("div.item") : htmlDoc.QuerySelector("div.song-list").QuerySelectorAll("div.item");
            if (items.Any())
            {
                foreach (var item in items)
                {
                    string songID = item.QuerySelector("span.playlist-tool").GetAttribute("data-song-id");
                    string title = item.QuerySelector("h3").TextContent.Trim();
                    var singerNode = item.QuerySelector(".singer");
                    string artist = singerNode.FirstElementChild.GetAttribute("title").Trim();
                    string artistID = Regex.Match(singerNode.FirstElementChild.GetAttribute("href").Replace(Endpoints.BaseURL + "artist/", ""), @"\d+").Value; //after replacing the artist endpoint only ID digits are left. 
                    int listenCount = Convert.ToInt32(item.QuerySelector("span.item-view").TextContent);
                    yield return new Track(songID, title, artist, artistID, listenCount);
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
                    string artistID = GetIDFromURL(artistLink.GetAttribute("href"));
                    string artistName = artistLink.GetAttribute("title");
                    string artistPhoto = artistLink.FirstElementChild.GetAttribute("src");
                    yield return new Artist(artistName, artistPhoto, artistID);
                }
            }
        }
        private string GetIDFromURL(string url)
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
                    string artistID = GetIDFromURL(artistLink.GetAttribute("href"));
                    string albumID = GetIDFromURL(albumLink.GetAttribute("href"));
                    string artistName = artistLink.GetAttribute("title");
                    string albumName = albumLink.GetAttribute("title");
                    string albumPhoto = item.FirstElementChild.FirstElementChild.GetAttribute("src");
                    yield return new Album(albumName, albumID, albumPhoto, artistName, artistID);
                }
            }
        }
    }
}
