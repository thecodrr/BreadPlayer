using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Web.Musixmatch
{
    class HttpHelper
    {
        static HttpClient musixmatchHttpClient;
        public static HttpClient MusixmatchHttpClient
        {
            get
            {
                if (musixmatchHttpClient == null)
                {
                    musixmatchHttpClient = new HttpClient(new HttpClientHandler() { AllowAutoRedirect = false});
                    musixmatchHttpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Musixmatch/0.19.4 Chrome/56.0.2924.87 Electron/1.6.10 Safari/537.36");
                    musixmatchHttpClient.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));
                }
                return musixmatchHttpClient;
            }
        }

    }
}
