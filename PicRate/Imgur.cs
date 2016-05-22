using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace PicRate
{
    class Imgur
    {
        private const string apiEndpoint = @"https://api.imgur.com/3/";
        private const string clientId = "75a7b93a575fde4";

        public ImageCache ImageCache = new ImageCache(@"C:\Users\foxneSs\Desktop\Images.cache");
        private HttpClient client = new HttpClient();
        private Regex directImageRegex = new Regex(@"^https?://i\.imgur\.com/\w+\.\w+(\?\d+)?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        private Regex embeddedAlbumImageRegex = new Regex(@"^https?://imgur\.com/a/(?<album>\w+)/embed#(?<id>\d+)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        public Imgur()
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Client-ID", clientId);
        }

        public Image GetImage(string url)
        {
            var sw = new Stopwatch();
            sw.Start();
            
            var imageUrl = GetImageUrl(url);
            if (imageUrl == null)
                return null;

            sw.Stop();
            Debug.WriteLine(sw.Elapsed);

            if (!ImageCache.ContainsKey(imageUrl))
                ImageCache.Add(imageUrl, Image.FromStream(client.GetStreamAsync(imageUrl).Result));

            return ImageCache[imageUrl];
        } 

        private string GetImageUrl(string url)
        {
            if (directImageRegex.IsMatch(url))
                return url;

            var match = embeddedAlbumImageRegex.Match(url);
            if (!match.Success)
                return null;

            return GetImageUrl(match.Groups["album"].Value, Int32.Parse(match.Groups["id"].Value));
        }

        private string GetImageUrl(string album, int id)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(client.GetStringAsync($"{apiEndpoint}album/{album}").Result);
            return parsedJson.data.images[id].link;
        }

        /*public void Test(List<string> urls)
        {
            var directImageUrls = urls.Where(url => directImageRegex.IsMatch(url)).ToList();
            var rest = urls.Where(url => !directImageRegex.IsMatch(url)).ToList();

            var embeddedAlbumImageUrls = rest.Where(url => embeddedAlbumImageRegex.IsMatch(url)).ToList();
            rest = rest.Where(url => !embeddedAlbumImageRegex.IsMatch(url)).ToList();

            var asd = rest.Aggregate((a, b) => $"{a}\n{b}");
            double directImagePercentage = 100d * directImageUrls.Count / urls.Count;
            double embeddedAlbumImagePercentage = 100d * embeddedAlbumImageUrls.Count / urls.Count;
        }*/
    }
}
