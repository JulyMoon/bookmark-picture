using System;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace PicRate
{
    class Imgur
    {
        private const string apiEndpoint = @"https://api.imgur.com/3/";
        private const string clientId = "75a7b93a575fde4";

        public ImageCache ImageCache = new ImageCache(@"C:\Users\foxneSs\Desktop\Images.cache");
        public ImageUrlCache ImageUrlCache = new ImageUrlCache(@"C:\Users\foxneSs\Desktop\Url.cache");
        private HttpClient client = new HttpClient();
        private Regex directImageRegex = new Regex(@"^https?://i\.imgur\.com/\w+\.\w+(\?\d+)?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        private Regex embeddedAlbumImageRegex = new Regex(@"^https?://imgur\.com/a/(?<album>\w+)/embed#(?<id>\d+)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        public Imgur()
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Client-ID", clientId);
        }

        public Image GetImage(string url)
        {
            var imageUrl = GetImageUrl(url);

            if (imageUrl == null)
                return null;

            if (!ImageCache.ContainsKey(imageUrl))
                ImageCache.Add(imageUrl, Image.FromStream(client.GetStreamAsync(imageUrl).Result));

            return ImageCache[imageUrl];
        } 

        private string GetImageUrl(string url)
        {
            if (ImageUrlCache.ContainsKey(url))
                return ImageUrlCache[url];

            string result;
            if (directImageRegex.IsMatch(url))
                result = url;
            else
            {
                var match = embeddedAlbumImageRegex.Match(url);
                result = match.Success ? GetImageUrl(match.Groups["album"].Value, Int32.Parse(match.Groups["id"].Value)) : null;
            }

            ImageUrlCache.Add(url, result);
            return result;
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
