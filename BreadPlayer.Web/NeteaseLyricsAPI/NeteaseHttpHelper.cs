using BreadPlayer.Web.NeteaseLyricsAPI.Responses;
using Newtonsoft.Json;
using RestSharp.Portable;
using RestSharp.Portable.HttpClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Web.NeteaseLyricsAPI
{
    public class NeteaseHttpHelper
    {
        public static async Task<string> PostAsync(string url, string body)
        {
            HttpClientHandler handler = new HttpClientHandler();
            CookieContainer cookieJar = new CookieContainer();
            cookieJar.Add(new Uri("http://music.163.com/"), new Cookie("appver", "1.5.2"));
            handler.CookieContainer = cookieJar;
            handler.UseCookies = true;
            handler.AutomaticDecompression = DecompressionMethods.GZip;
            HttpClient client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_9_2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/33.0.1750.152 Safari/537.36");
            client.DefaultRequestHeaders.Referrer = new Uri("http://music.163.com/search");
            client.DefaultRequestHeaders.Host = "music.163.com";
            var response = await client.PostAsync(url, new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded"));
            return await response.Content.ReadAsStringAsync();
        }
        public static Task<string> GetAsync(string url)
        {
            HttpClient client = new HttpClient();
            return client.GetStringAsync(url);
        }
    }
}
