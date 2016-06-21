using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace PicRate
{
    class ImageCache
    {
        private Dictionary<string, string> cache = new Dictionary<string, string>();

        public readonly string FolderPath;

        public ImageCache(string folderPath)
        {
            FolderPath = folderPath;

            Directory.CreateDirectory(FolderPath);

            foreach (var filePath in Directory.GetFiles(FolderPath))
            {
                string url = CacheHelper.FromBase64(Path.GetFileName(filePath));
                cache.Add(url, filePath);
            }
        }

        public void Add(string url, Image image)
        {
            var path = Path.Combine(FolderPath, CacheHelper.ToBase64(url));
            image.Save(path);
            cache.Add(url, path);
        }

        public bool ContainsUrl(string url) => cache.ContainsKey(url);

        public Image this[string url] => Image.FromFile(cache[url]);
    }
}
