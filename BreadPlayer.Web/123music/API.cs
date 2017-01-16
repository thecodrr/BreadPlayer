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

namespace BreadPlayer.Web._123music
{
    public class API
    {

        public async Task<bool> SearchSongs(string term)
        {
            var htmlDoc = await GetDocumentAsync(string.Format(Endpoints.SearchEndpoint, "songs", term, ""));
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
                    var page = await GetDocumentAsync(string.Format(Endpoints.SearchEndpoint, "songs", term, i.ToString() + ".html"));
                    Songs.AddRange(GetSongs(page));
                }
            }
           
            return true;
        }
        public async Task<IHtmlDocument> GetDocumentAsync(string url)
        {
            HttpClient httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            var parser = new HtmlParser();
            var htmlDoc = await parser.ParseAsync(html);
            return htmlDoc;
        }
        public IEnumerable<Track> GetSongs(IHtmlDocument htmlDoc)
        {
            var items = htmlDoc.QuerySelector("div.song-list").QuerySelectorAll("div.item");
            foreach(var item in items)
            {
                string songID = item.QuerySelector("span.playlist-tool").GetAttribute("data-song-id");
                string title = item.QuerySelector("h3").TextContent.Trim();
                var singerNode = item.QuerySelector("span[class=\"singer\"]");
                string artist = singerNode.FirstElementChild.GetAttribute("title").Trim();
                string artistID = Regex.Match(singerNode.FirstElementChild.GetAttribute("href").Replace(Endpoints.BaseURL + "artist/", ""), @"\d+").Value; //after replacing the artist endpoint only ID digits are left. 
                int listenCount = Convert.ToInt32(item.QuerySelector("span.item-view").TextContent);
                yield return new Track(songID, title, artist, artistID, listenCount);
            }
        }
    }
}
