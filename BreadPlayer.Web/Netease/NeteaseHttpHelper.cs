using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Web.NeteaseLyricsAPI
{
    public class NeteaseHttpHelper
    {
        static HttpClient neteaseHttpPostClient;
        private static HttpClient NeteaseHttpPostClient
        {
            get
            {
                if (neteaseHttpPostClient == null)
                {
                    var newClient = new HttpClient(NeteaseHttpClientHandler);
                    newClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_9_2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/33.0.1750.152 Safari/537.36");
                    newClient.DefaultRequestHeaders.Referrer = new Uri("http://music.163.com/search");
                    newClient.DefaultRequestHeaders.Host = "music.163.com";
                    return newClient;
                }
                return neteaseHttpPostClient;
            }
        }
        static HttpClientHandler neteaseHttpClientHandler;
        private static HttpClientHandler NeteaseHttpClientHandler
        {
            get
            {
                if (neteaseHttpClientHandler == null)
                {
                    CookieContainer cookieJar = new CookieContainer();
                    cookieJar.Add(new Uri("http://music.163.com/"), new Cookie("appver", "1.5.2"));
                    neteaseHttpClientHandler = new HttpClientHandler()
                    {
                        CookieContainer = cookieJar,
                        UseCookies = true,
                        AutomaticDecompression = DecompressionMethods.GZip,
                    };
                }
                return neteaseHttpClientHandler;
            }
        }
        public static async Task<string> PostAsync(string url, string body)
        {
            NeteaseHttpPostClient.CancelPendingRequests();
            var response = await NeteaseHttpPostClient.PostAsync(url, new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded"));
            return await response.Content.ReadAsStringAsync();
        }

        static HttpClient NeteaseHttpClient = new HttpClient();
        public static Task<string> GetAsync(string url)
        {
            NeteaseHttpClient.CancelPendingRequests();
            return NeteaseHttpClient.GetStringAsync(url);
        }
    }
}